#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.Core;
using System.Timers;
#endregion

public class raC_btnDoubleClickRuntime : BaseNetLogic
{
    private bool _Clicked;
    System.Timers.Timer _clickTimer;

    private MethodInvocation _clickedEvent;
    private MethodInvocation _doubleClickedEvent;

    public override void Start()
    {
        _clickedEvent = (MethodInvocation)LogicObject.Get("Clicked");
        _doubleClickedEvent = (MethodInvocation)LogicObject.Get("DoubleClicked");
        if (_clickedEvent == null)
            throw new CoreConfigurationException("Unable to find clickedEvent method invocation");

        if (_doubleClickedEvent == null)
            throw new CoreConfigurationException("Unable to find _doubleClickedEvent method invocation");

        ((IUAObject)Owner).UAEvent += btnClicked;
        
        _clickTimer = new System.Timers.Timer();
        _clickTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

        _clickTimer.Interval = Owner.GetVariable("cfgTimespan").Value;
    }

    public override void Stop()
    {
        ((IUAObject)Owner).UAEvent -= btnClicked;
    }

    private void btnClicked(object sender, UAEventArgs e)
    {
        if (e.EventType.BrowseName == "MouseClickEvent")
        {
            if (_Clicked)
            {
                _clickTimer.Stop();
                _doubleClickedEvent.Invoke();
                _Clicked = false;
            }
            else
            {
                _clickTimer.Start();
                _Clicked = true;
            }
        }

    }

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        _Clicked = false;
        _clickTimer.Stop();

        _clickedEvent.Invoke();
    }
}
