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

public class DB_VariableGenerator_SalvaPantalla_V1 : BaseNetLogic
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
        try
        {
            //If using an alias (nicer approach than the entire path):
            var Xmax = 0;
            var Ymax = 0;

             var temp_I_Tamano_X = LogicObject.GetVariable("I_TamanoX").Value; 
            var temp_I_Tamano_Y = LogicObject.GetVariable("I_TamanoY").Value;

            var temp_I_TamanoImagen_X = LogicObject.GetVariable("I_TamanoImagenX").Value;
            var temp_I_TamanoImagen_Y = LogicObject.GetVariable("I_TamanoImagenY").Value;



            if (temp_I_Tamano_X    > temp_I_TamanoImagen_X )
            {
                Xmax = temp_I_Tamano_X - temp_I_TamanoImagen_X;
            }
            else
            {
                Xmax = 0;    // Para que salga en la parte izquierda
            }


            //var Xmax = Project.Current.GetVariable("UI/MainWindow/Salvapantalla/OriginalWidth").Value - Project.Current.GetVariable("UI/MainWindow/Salvapantalla/Imagen/Width").Value;
            if (temp_I_Tamano_Y > temp_I_TamanoImagen_Y)
            {
                Ymax = temp_I_Tamano_Y - temp_I_TamanoImagen_Y;
            }
            else
            {
                Ymax = 0;    // Para que salga en la parte superior
            }

            Random r = new Random();

            LogicObject.GetVariable("O_PosX").Value = r.Next(0, Xmax);
            LogicObject.GetVariable("O_PosY").Value = r.Next(0, Ymax);

            //Log.Info("randomPosition", "OK");

        }
        catch (System.Exception)
        {
            Log.Error("randomPosition", "!!Error!!");
        }

    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        taskPeriodico.Dispose();
    }
}
