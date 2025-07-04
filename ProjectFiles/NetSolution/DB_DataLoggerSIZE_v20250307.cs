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
//DB 20250307 actualizado para detectar si el log tiene nombre de tabla o si no tiene.
// OJO Si el Log no tiene nombre de tabla, en el DataStore coge como nombre de la tabla el nombre del Log.
// OJO Si el log TIENE nombre de tabla, en el DataStore coge como nombre de la tabla el nombre puesto en la tabla.


public class DB_DataLoggerSIZE_v20250307 : BaseNetLogic
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
           
            //OJO si el Logger no tiene tabla definida, en el DataStore aparece una tabla con el nombre del propio Logger
            var table = dl.TableName; // Nombre de la tabla definida en el Logger
            if (dl.TableName == null) // Si no hay nombre definido coje el nombre del propio Logger
            {
                table = dl.BrowseName;
            }

            
            //Log.Info("DataLoger = " + dl.BrowseName);
            //Log.Info("Lugar de almacenamiento = " + dbmStore.BrowseName);
            //Log.Info("Tabla consultada = " + dl.TableName);

            dbmStore.Query($"SELECT COUNT(*) FROM {table}", out string[] header, out object[,] results);
            LogicObject.GetVariable("Size").Value = Convert.ToInt32(results[0, 0]);

        }
        catch (System.Exception ex)
        {
            Log.Error("El COUNT de la tabla del DataLog ha tenido problems y he entrado en el catch");
            Log.Error(ex.Message);
            Log.Error(ex.StackTrace);
            Log.Error(ex.HelpLink);
        }
    }


}
