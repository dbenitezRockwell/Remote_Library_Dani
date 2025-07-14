#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.ODBCStore;
using FTOptix.Store;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.SQLiteStore;
using FTOptix.DataLogger;
using System.Linq;
using FTOptix.InfluxDBStore;
using FTOptix.System;
#endregion

public class CustomSFLogic : BaseNetLogic
{
    public override void Start()
    {
        ownerLogger = (DataLogger)Owner;
        localStore = InformationModel.Get<Store>(ownerLogger.Store);
        remoteStore = InformationModel.Get<Store>(LogicObject.GetVariable("TargetStore").Value);
        if (localStore == null || remoteStore == null)
        {
            Log.Error(LogicObject.BrowseName, $"Failed to retrieve stores for DataLogger: {ownerLogger.BrowseName}. Local store: {localStore?.BrowseName}, Remote store: {remoteStore?.BrowseName}");
            return;
        }
        tableSynced = AllignTables();
        Log.Info(LogicObject.BrowseName, $"Started Custom SF logic for DataLogger: {ownerLogger.BrowseName} with remote store: {remoteStore.BrowseName} and local store: {localStore.BrowseName}");
        syncTask = new PeriodicTask(SyncStores, (int)ownerLogger.SamplingPeriod + 100, LogicObject);
        syncTask.Start();
    }

    public override void Stop()
    {
        if (syncTask != null)
        {
            try
            {
                syncTask.Cancel();
            }
            catch
            {
                // Task is not running, ignore the exception
            }
            syncTask.Dispose();
            Log.Info(LogicObject.BrowseName, $"Stopped Custom SF logic for DataLogger: {ownerLogger.BrowseName}");
        }
    }

    private void SyncStores()
    {
        if (remoteStore.Status == StoreStatus.Online)
        {
            if (isFirtOffline)
            {
                Log.Info(LogicObject.BrowseName, $"Remote store {remoteStore.BrowseName} is now online, sync data is starting.");
            }
            isFirtOffline = false;
            // if the table is not synced, try to align the tables
            if (!tableSynced)
            {
                tableSynced = AllignTables();
                if (!tableSynced)
                {
                    Log.Error(LogicObject.BrowseName, $"Failed to align tables between {localStore.BrowseName} and {remoteStore.BrowseName}");
                    return;
                }
            }
            TrySyncStores();
        }
        else
        {
            if (!isFirtOffline)
            {
                // this boolean is used to avoid logging the same message multiple times
                isFirtOffline = true;
                Log.Warning(LogicObject.BrowseName, $"Remote store {remoteStore.BrowseName} is offline, will not sync data.");
            }
        }
    }

    private void TrySyncStores()
    {
        // get the table name from the owner logger TableName property, if not set use the BrowseName
        string tableName = string.IsNullOrEmpty(ownerLogger.TableName) ? ownerLogger.BrowseName : ownerLogger.TableName;
        var remoteTable = remoteStore.GetObject("Tables").Get<Table>(tableName);
        // Get all records from the local store for the specified table
        localStore.Query("SELECT * FROM " + tableName, out string[] columnNames, out object[,] rowValues);
        if (columnNames != null && rowValues != null)
        {
            try
            {
                // Try to insert the rows into the remote store
                remoteTable.Insert(columnNames, rowValues);
            }
            catch (Exception ex)
            {
                // if the store goes offline during the insert, we catch the exception and log it
                Log.Error(LogicObject.BrowseName, $"Error inserting rows into {remoteStore.BrowseName}: {ex.Message}");
                return;
            }
            // if the insert was successful, clean the local records
            CleanLocalRecords(tableName, columnNames, rowValues);
        }
    }

    private void CleanLocalRecords(string tableName, string[] columnNames, object[,] rowValues)
    {
        // Get the last timestamp from the local store
        int dateTimeColIndex = Array.IndexOf(columnNames, "Timestamp");
        int lastRowIndex = rowValues.GetLength(0) - 1;
        DateTime lastTimestamp = (DateTime)rowValues[lastRowIndex, dateTimeColIndex];
        // Fill the local variables in order to have the DeleteQuery ready (is a stringFormatter)
        LogicObject.GetVariable("TableName").Value = tableName;
        LogicObject.GetVariable("LastTimestamp").Value = lastTimestamp;
        // Run the delete query to remove the records from the local store
        localStore.Query(LogicObject.GetVariable("DeleteQuery").Value, out _, out _);
    }

    private bool AllignTables()
    {
        if (remoteStore.Status == StoreStatus.Online)
        {
            // get the table name from the owner logger TableName property, if not set use the BrowseName
            string tableName = string.IsNullOrEmpty(ownerLogger.TableName) ? ownerLogger.BrowseName : ownerLogger.TableName;
            if (!remoteStore.Tables.Any(table => table.BrowseName == tableName))
            {
                // if the table does not exist in the remote store, create it with the columns from the local store
                var columns = localStore.GetObject("Tables").Get<Table>(tableName).Columns;
                CreateTable(tableName, columns);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CreateTable(string tableName, PlaceholderChildNodeCollection<StoreColumn> columnsCollection)
    {
        remoteStore.AddTable(tableName);
        var newTable = remoteStore.GetObject("Tables").Get<Table>(tableName);
        foreach (var column in columnsCollection)
        {
            newTable.AddColumn(column.BrowseName, column.DataType);
        }
    }

    Store remoteStore;
    Store localStore;
    DataLogger ownerLogger;
    PeriodicTask syncTask;
    bool tableSynced = false;
    bool isFirtOffline = false;
}
