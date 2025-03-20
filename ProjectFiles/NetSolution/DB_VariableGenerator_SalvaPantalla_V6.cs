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

public class DB_VariableGenerator_SalvaPantalla_V6 : BaseNetLogic
{
    private PeriodicTask taskPeriodico;
    public override void Start()
    {

        // Insert code to be executed when the user-defined logic is started
        taskPeriodico = new PeriodicTask(randomPosition, 1500, LogicObject);
        taskPeriodico.Start();
    }

    public void randomPosition()
    {
        var Xmax = 0;
        if (LogicObject.GetVariable("I_TamanoX").Value > LogicObject.GetVariable("I_TamanoImagenX").Value)
        {
            Xmax = LogicObject.GetVariable("I_TamanoX").Value - LogicObject.GetVariable("I_TamanoImagenX").Value;
        }
        else
        {
            Xmax = 0;    // Para que salga en la parte izquierda
        }

        var Ymax = 0;
        //var Xmax = Project.Current.GetVariable("UI/MainWindow/Salvapantalla/OriginalWidth").Value - Project.Current.GetVariable("UI/MainWindow/Salvapantalla/Imagen/Width").Value;
        if (LogicObject.GetVariable("I_TamanoY").Value > LogicObject.GetVariable("I_TamanoImagenY").Value)
        {
            Ymax = LogicObject.GetVariable("I_TamanoY").Value - LogicObject.GetVariable("I_TamanoImagenY").Value;
        }
        else
        {
            Ymax = 0;    // Para que salga en la parte superior
        }

        Random r = new Random();

        LogicObject.GetVariable("O_PosX").Value = r.Next(0, Xmax);
        LogicObject.GetVariable("O_PosY").Value = r.Next(0, Ymax);
            
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        taskPeriodico.Dispose();
    }
}
