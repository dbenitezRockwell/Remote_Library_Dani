#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.System;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
#endregion

public class VariableGenerator_SalvaPantalla_V2 : BaseNetLogic
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
        //var Xmax = Project.Current.GetVariable("UI/MainWindow/Salvapantalla/OriginalWidth").Value - Project.Current.GetVariable("UI/MainWindow/Salvapantalla/Imagen/Width").Value;
        var Xmax = LogicObject.GetVariable("I_TamanoX").Value - LogicObject.GetVariable("I_TamanoImagenX").Value;

        //var Ymax = 400;
        //var Ymax = Project.Current.GetVariable("UI/MainWindow/Salvapantalla/OriginalHeight").Value - Project.Current.GetVariable("UI/MainWindow/Salvapantalla/Imagen/Height").Value;
        var Ymax = LogicObject.GetVariable("I_TamanoY").Value - LogicObject.GetVariable("I_TamanoImagenY").Value;


        Random r = new Random();

        if ((bool)Project.Current.GetVariable("Model/SalvaPantalla_AuxTags/RandomEn").Value)
        {
            //Project.Current.GetVariable("Model/SalvaPantalla_AuxTags/Pos_x").Value = r.Next(0, Xmax);
            //Project.Current.GetVariable("Model/SalvaPantalla_AuxTags/Pos_y").Value = r.Next(0, Ymax);

            LogicObject.GetVariable("O_PosX").Value = r.Next(0, Xmax);
            LogicObject.GetVariable("O_PosY").Value = r.Next(0, Ymax);

        }
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        taskPeriodico.Dispose();
    }
}
