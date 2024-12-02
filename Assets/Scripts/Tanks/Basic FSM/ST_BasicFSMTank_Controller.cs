using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class ST_BasicFSMTank_Controller : Controller
{


    
    public override void ControllerStart()
    {
        foreach (var state in states)
        {
            state.controller = this;
        }
    }

    public List<ST_BasicFSM_State> states;
    public AITank tank;
    
}




public class ST_BasicFSM_State : State
{
    public ST_BasicFSMTank_Controller controller;
}

