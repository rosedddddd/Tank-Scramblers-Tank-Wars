using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST_Search_FSMRBS : ST_Base_FSMRBS
{

    private ST_SmartTankFSMRBS smartTank;

    public ST_Search_FSMRBS(ST_SmartTankFSMRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    // setting up to enter the state
    public override Type EnterState()
    {
        //smartTank.stats["searchState_FSMRBS"] = true;
        return null;
    }

    // anything that needs to be reset or changed once the player leaves the stage
    public override Type LeaveState()
    {
        //smartTank.stats["searchState_FSMRBS"] = false;
        return null;
    }

    // logic that runs every physics update inside of the controller
    public override Type StateLogic()
    {
        //foreach (var item in smartTank.rules.GetRules)
        //{
        //    if (item.CheckRule(smartTank.stats) != null)
        //    {
        //        return item.CheckRule(smartTank.stats);
        //    }
        //}

        return null;
    }
}
