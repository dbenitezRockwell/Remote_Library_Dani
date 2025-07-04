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
using FTOptix.ODBCStore;
using FTOptix.MQTTClient;
#endregion

//20250515  Copia una TAG (puede ser una estructura completa) y la copia en otra TAG

public class Cop_Tag2Tag_If_SF_OK : BaseNetLogic
{
    private PeriodicTask periodicTask;

    public override void Start()
    {
        LogicObject.GetVariable("O_DB_Status").Value = 55; // el 55 es para ver que luego se escribe un valor 0,1,2
        LogicObject.GetVariable("O_DB_SF_Ready").Value = 0;

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
            //OJO la siguiente asignacion no soy capaz de hacerla sin pasar por la variable temporal!!
            // 0 = Offline; 1 = OK; 2 Error devuelto por la DB
            var temp_Status = Project.Current.GetVariable("DataStores/EmbeddedDatabase/Status").Value;      //Lee el STATUS de la DB interna.  OJO se supone que siempre esta bien.....
            LogicObject.GetVariable("O_DB_Status").Value = temp_Status;

            //Si el c# consigue ver que la base de datos esta OK, el Store&Forware se inicializa bien y funcion
            //Este bit se pondra a 0 en el siguiente powerUp del OPTIX en la rutiena "Start()"

            //OJO con la DB interna no hay SF!!!!!!!!!!!!!!!!!!!

            if (temp_Status == 1) 
            {
                LogicObject.GetVariable("O_DB_SF_Ready").Value = 1;
            }


            bool temp_Enable = LogicObject.GetVariable("I_Enable").Value;
            bool temp_Messages_ON = LogicObject.GetVariable("I_Messages_ON").Value;        
            bool temp_O_DB_SF_Ready = LogicObject.GetVariable("O_DB_SF_Ready").Value;

            if (temp_Enable && temp_O_DB_SF_Ready)
                {
                if (temp_Messages_ON == true)
                {
                    Log.Info("Cop_Tag2Tag", "Entrando en el bulce de copia");
                }

                long tiks_Inicial = DateTime.Now.Ticks;


                //If using an alias (nicer approach than the entire path): 
                //Hay que probar con parametros de entrada, pero no lo he conseguido.
                var PLC_I = Project.Current.GetVariable("CommDrivers/RAEtherNet_IPDriver1/RAEtherNet_IPStation1/Tags/Program:PLC2Optix2DB/Dat2Log");
                var PLC_O = Project.Current.GetVariable("CommDrivers/RAEtherNet_IPDriver1/RAEtherNet_IPStation1/Tags/Program:PLC2Optix2DB/Dat_fdk");

                PLC_I.RemoteRead(); // to keep the variables synched even if not in use by any current page

                PLC_O.RemoteWrite(PLC_I.Value); ;


                long Resultado = (DateTime.Now.Ticks - tiks_Inicial) / 10000;

                //Activa los mesajes para tener dianostico del tiempo que se tarda en hacer la rutina

               if (temp_Messages_ON == true)
               {
                    Log.Info("Cop_Tag2Tag", "OK; Time = " + Resultado);
               }
            }
            else
            {
                if (temp_Messages_ON == true)
                {
                    Log.Info("Cop_Tag2Tag", "No entra en el bulce de copia");
                }
            }
        }
        catch (System.Exception)
        {
            Log.Error("NetLogix Cop_Tag2Tag", "!!Error!!");
        }
    }
}

