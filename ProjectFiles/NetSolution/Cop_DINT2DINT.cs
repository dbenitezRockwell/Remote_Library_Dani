#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.RAEtherNetIP;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.WebUI;
using FTOptix.NativeUI;
using FTOptix.Modbus;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using FTOptix.EventLogger;
using FTOptix.Store;
using FTOptix.SQLiteStore;
#endregion

public class Cop_DINT2DINT : BaseNetLogic
{
    private PeriodicTask periodicTask;

    public override void Start()
    {
        periodicTask = new PeriodicTask(test, 500, LogicObject);
        periodicTask.Start();
    }

    public override void Stop()
    {
        periodicTask.Dispose();
        periodicTask = null;
    }


    [ExportMethod]
    public void test()
    {
        try
        {
            bool temp_Enable = LogicObject.GetVariable("Enable").Value;

            if (temp_Enable == true)
            {

                long tiks_Inicial = DateTime.Now.Ticks;


                //If using an alias (nicer approach than the entire path):
                var PLC_I = Project.Current.GetVariable("CommDrivers/EtherNet_IP/PLC/Tags/Program:Test_Datos/Datos_DINT_In"); // Daniel_test_Escritura_Vectores / CommDrivers / EtherNet_IP / PLC / Tags / Program:Test_Datos / Datos_DINT_In
                var PLC_O = Project.Current.GetVariable("CommDrivers/EtherNet_IP/PLC/Tags/Program:Test_Datos/Datos_DINT_Out");
                PLC_O.RemoteRead(); // to keep the variables synched even if not in use by any current page

                PLC_I.RemoteWrite(PLC_O.Value);


                long Resultado = (DateTime.Now.Ticks - tiks_Inicial) / 10000;
                LogicObject.GetVariable("UpdateTime").Value = (int)Resultado;


                Log.Info("Cop_DINT2DINT", "OK; Time = " + Resultado);
            }
        }
        catch (System.Exception)
        {
            Log.Error("Cop_DINT2DINT", "!!Error!!");

        }
    }
}

