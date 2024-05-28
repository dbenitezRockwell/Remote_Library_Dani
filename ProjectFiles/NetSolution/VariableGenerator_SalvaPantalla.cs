#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NativeUI;
using FTOptix.NetLogic;
using FTOptix.HMIProject;
using FTOptix.DataLogger;
using FTOptix.UI;
using FTOptix.Alarm;
using FTOptix.EventLogger;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Modbus;
using FTOptix.CoreBase;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
#endregion

public class VariableGenerator_SalvaPantalla: BaseNetLogic
{


    private PeriodicTask taskPeriodico;
    public override void Start()
    {

        // Insert code to be executed when the user-defined logic is started
        taskPeriodico = new PeriodicTask(randomNum, 1500, LogicObject);
        taskPeriodico.Start();
    }

    public void randomNum()
    {
        //var Xmax = 400;
        var Xmax = Project.Current.GetVariable("UI/MainWindow/Salvapantalla/OriginalWidth").Value - Project.Current.GetVariable("UI/MainWindow/Salvapantalla/Imagen/Width").Value;
        var Ymax = Project.Current.GetVariable("UI/MainWindow/Salvapantalla/OriginalHeight").Value - Project.Current.GetVariable("UI/MainWindow/Salvapantalla/Imagen/Height").Value; 

        Random r = new Random();

        if ((bool)Project.Current.GetVariable("Model/SalvaPantalla_AuxTags/RandomEn").Value)
        {
            Project.Current.GetVariable("Model/SalvaPantalla_AuxTags/Pos_x").Value = r.Next(0, Xmax);
            Project.Current.GetVariable("Model/SalvaPantalla_AuxTags/Pos_y").Value = r.Next(0, Ymax);
        }
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        taskPeriodico.Dispose();
    }
}
