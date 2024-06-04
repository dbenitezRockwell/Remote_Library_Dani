#region Using directives
using System;
using CoreBase = FTOptix.CoreBase;
using FTOptix.HMIProject;
using UAManagedCore;
using FTOptix.UI;
using FTOptix.NetLogic;
using FTOptix.WebUI;
using FTOptix.ODBCStore;
using FTOptix.OPCUAServer;
#endregion

public class DB_DateTime2PLC : BaseNetLogic
{
	public override void Start()
	{
		periodicTask = new PeriodicTask(UpdateTime, 1000, LogicObject);
		periodicTask.Start();
	}

	public override void Stop()
	{
		periodicTask.Dispose();
		periodicTask = null;
	}

	private void UpdateTime()
	{
		LogicObject.GetVariable("Time").Value = DateTime.Now;
		LogicObject.GetVariable("UTCTime").Value = DateTime.UtcNow;
        LogicObject.GetVariable("Ano").Value = DateTime.Now.Year;
        LogicObject.GetVariable("Mes").Value = DateTime.Now.Month;
        LogicObject.GetVariable("Dia").Value = DateTime.Now.Day;
        LogicObject.GetVariable("Hora").Value = DateTime.Now.Hour;
        LogicObject.GetVariable("Minuto").Value = DateTime.Now.Minute;
        LogicObject.GetVariable("Segundo").Value = DateTime.Now.Second;

	}

	private PeriodicTask periodicTask;
}
