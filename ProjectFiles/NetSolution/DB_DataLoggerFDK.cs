#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.DataLogger;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.Store;
using FTOptix.ODBCStore;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.SQLiteStore;
using FTOptix.RAEtherNetIP;
using FTOptix.CommunicationDriver;
using FTOptix.Report;
#endregion
// DB y EP 20240107  modificación de código para obtener la lectura del ULTIMO dato del datalog y pasarlo a variables del PLC
// Para que funcione, hay que añadir tantas variables como tenga el LOG (del mismo tipo) y añadirles el prefijo "FDK_". De esta forma, el código coge del DATALOG los nombres

public class DB_DataLoggerFDK : BaseNetLogic
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
    public void RestoreLastValues()
    {
        try
        {
            var dl = (DataLogger)Owner;
            var dbmStore = InformationModel.Get<Store>(dl.Store);
            foreach (var variableToLog in dl.VariablesToLog)
            {
                dbmStore.Query(
                    $"SELECT {variableToLog.BrowseName} FROM {dl.BrowseName} ORDER BY Timestamp DESC LIMIT 1"
                    , out string[] header
                    , out object[,] results
                    );
                var loggedVariableType = variableToLog.Value.Value.GetType();

                switch (Type.GetTypeCode(loggedVariableType))
                {

                    case TypeCode.Int32:
                        //Log.Error("Entrado en el tipo INT32");
                        LogicObject.GetVariable("FDK_" + variableToLog.BrowseName).Value = (long)results[0, 0];
                        break;
                    case TypeCode.Int64:
                        //Log.Error("Entrado en el tipo INT64");
                        LogicObject.GetVariable("FDK_" + variableToLog.BrowseName).Value = (long)results[0, 0];
                        break;
                    case TypeCode.String:
                        //Log.Error("Entrado en el tipo string");
                        LogicObject.GetVariable("FDK_" + variableToLog.BrowseName).Value = (string)results[0, 0];
                        break;
                    case TypeCode.Boolean:
                        //Log.Error("Entrado en el tipo BOOL");
                        LogicObject.GetVariable("FDK_" + variableToLog.BrowseName).Value = (long)results[0, 0];
                        break;
                    default:
                        Log.Error("Hola hay un tipo en el Log que no tengo definido, modifica el codigo de C# para que funcione.");
                        break;
                }


            }
        }
        catch (System.Exception ex)
        {
            Log.Error(ex.Message);
        }
    }
}
