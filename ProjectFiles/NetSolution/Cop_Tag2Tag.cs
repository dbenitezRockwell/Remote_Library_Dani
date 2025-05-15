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

//20250515  Copia una TAG (puede ser una estructura completa) y la copia en otra TAG

public class Cop_Tag2Tag : BaseNetLogic
{
    private PeriodicTask periodicTask;

    public override void Start()
    {
        Int32 temp_UpdateTime = LogicObject.GetVariable("I_UpdateTime").Value;
        if (temp_UpdateTime < 50) //Proteccion de 50ms para evitar saturar la CPU del equipo
        { 
            temp_UpdateTime = 50;
            Log.Info("Cop_Tag2Tag", "Parametro I_UpdateTime inferior a 50ms. Se subira a 50ms");
        }
        periodicTask = new PeriodicTask(TareaPrincipal, temp_UpdateTime, LogicObject);
        periodicTask.Start();
    }

    public override void Stop()
    {
        periodicTask.Dispose();
        periodicTask = null;
    }


    [ExportMethod]
    public void TareaPrincipal()
    {
        try
        {
            bool temp_Enable = LogicObject.GetVariable("I_Enable").Value;

            if (temp_Enable == true)
            {

                long tiks_Inicial = DateTime.Now.Ticks;


                //If using an alias (nicer approach than the entire path): 
                //Hay que probar con parametros de entrada, pero no lo he conseguido.
                var PLC_I = Project.Current.GetVariable("CommDrivers/EtherNet_IP/RAEtherNet_IPStation1/Tags/Program:PLC2Optix2DB/Dat2Log");
                var PLC_O = Project.Current.GetVariable("CommDrivers/EtherNet_IP/RAEtherNet_IPStation1/Tags/Program:PLC2Optix2DB/Dat_fdk");

                PLC_I.RemoteRead(); // to keep the variables synched even if not in use by any current page

                PLC_O.RemoteWrite(PLC_I.Value); ;


                long Resultado = (DateTime.Now.Ticks - tiks_Inicial) / 10000;

                //Activa los mesajes para tener dianostico del tiempo que se tarda en hacer la rutina
               bool temp_Messages_ON = LogicObject.GetVariable("I_Messages_ON").Value;
               if (temp_Messages_ON == true)
               {
                    Log.Info("Cop_Tag2Tag", "OK; Time = " + Resultado);
               }
            }
        }
        catch (System.Exception)
        {
            Log.Error("NetLogix Cop_Tag2Tag", "!!Error!!");
        }
    }
}

