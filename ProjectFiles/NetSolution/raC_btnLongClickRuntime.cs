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

public class raC_btnLongClickRuntime : BaseNetLogic
{
    private bool _longClicked;
    System.Timers.Timer _clickTimer;

    private MethodInvocation _clickedEvent;
    private MethodInvocation _longClickedEvent;

    public override void Start()
    {
        _clickedEvent = (MethodInvocation)LogicObject.Get("Clicked");
        _longClickedEvent = (MethodInvocation)LogicObject.Get("LongClicked");
        if (_clickedEvent == null)
            throw new CoreConfigurationException("Unable to find clickedEvent method invocation");

        if (_longClickedEvent == null)
            throw new CoreConfigurationException("Unable to find longClickedEvent method invocation");

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
        if (e.EventType.BrowseName == "MouseDownEvent")
        {
            _longClicked = false;

            _clickTimer.Enabled = true;
        }
        else if (e.EventType.BrowseName == "MouseClickEvent")
        {
            _clickTimer.Stop();
            if (_longClicked)
            {
                return;
            }
            else
            {
                _clickedEvent.Invoke();
            }
        }
        else if (e.EventType.BrowseName == "MouseUpEvent")
        {
            _clickTimer.Stop();
        }
    }

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        _longClicked = true;
        _clickTimer.Stop();

        _longClickedEvent.Invoke();
    }
}
