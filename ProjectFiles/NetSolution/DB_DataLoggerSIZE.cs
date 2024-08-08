#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.UI;
using FTOptix.DataLogger;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.WebUI;
using FTOptix.NetLogic;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Report;
using FTOptix.RAEtherNetIP;
using FTOptix.Retentivity;
using FTOptix.System;
using FTOptix.CommunicationDriver;
using FTOptix.UI;
using FTOptix.Core;
#endregion

// DB 20240808  Calcula la cantidad de filas en un DATALOG
// Para que funcione, hay que "colgar" este código del DATALOG!  De forma automatica lee el nombre del DATALOG y de la BASE DE DATOS!!


public class DB_DataLoggerSIZE : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void CalculateSize()
    {
        try
        {
            var dl = (DataLogger)Owner;
            var dbmStore = InformationModel.Get<Store>(dl.Store);

                dbmStore.Query($"SELECT COUNT(*) FROM {dl.BrowseName}", out string[] header, out object[,] results);
                LogicObject.GetVariable("Size").Value = Convert.ToInt32(results[0, 0]);
                //Log.Error("Query realizada!");

        }
        catch (System.Exception ex)
        {
            Log.Error(ex.Message);
        }
    }


}
