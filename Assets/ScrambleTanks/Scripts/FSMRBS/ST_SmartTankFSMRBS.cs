using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static AStar;

/// <summary>
/// Class <c>DumbTank</c> is an example class used to demonstrate how to use the functions available from the <c>AITank</c> base class. 
/// Copy this class when creating your smart tank class.
/// </summary>
public class ST_SmartTankFSMRBS : AITank
{
    public Dictionary<GameObject, float> enemyTanksFound = new Dictionary<GameObject, float>();     /*!< <c>enemyTanksFound</c> stores all tanks that are visible within the tanks sensor. */
    public Dictionary<GameObject, float> consumablesFound = new Dictionary<GameObject, float>();    /*!< <c>consumablesFound</c> stores all consumables that are visible within the tanks sensor. */
    public Dictionary<GameObject, float> enemyBasesFound = new Dictionary<GameObject, float>();     /*!< <c>enemyBasesFound</c> stores all enemybases that are visible within the tanks sensor. */

    public Transform calcTransform; //a transform used for more complex calculations
    public Transform enemyLastSeen; //last seen spot of the enemmy tank. updated inside of AITankUpdate

    public LayerMask raycastLayers; // layermask used to detect obstacles in the way of the tank's pathfinding via raycast

    public GameObject enemyTank;        /*!< <c>enemyTank</c> stores a reference to a target enemy tank. 
                                        * This should be taken from <c>enemyTanksFound</c>, only whilst within the tank sensor. 
                                        * Reference should be removed and refreshed every update. */

    public GameObject consumable;       /*!< <c>consumable</c> stores a reference to a target consumable. 
                                        * This should be taken from <c>consumablesFound</c>, only whilst within the tank sensor. 
                                        * Reference should be removed and refreshed every update. */

    public GameObject enemyBase;        /*!< <c>enemyBase</c> stores a reference to a target enemy base. 
                                         * This should be taken from <c>enemyBasesFound</c>, only whilst within the tank sensor. 
                                        * Reference should be removed and refreshed every update. */

    public Dictionary<string, bool> stats = new Dictionary<string, bool>(); //Dictionary for stats
    public ST_Rules rules = new ST_Rules(); //rules

    public bool lowHealth;
    public bool lowAmmo;
    public bool lowFuel;

    public float lastSeenTimer = 5000; // the time since the enemmy tank was last seen
    public bool attacked = false;
    public bool takenBackshot = false; // if the tank has been attacked from behind

    public bool hasKited = false; // if the tank has entered the kiting state before.
                                  // the kiting state should only happen when first seeing the enemmy to waste their ammo

    float lastFrameHealth; // used to check if the tank has taken damage inside the AITankUpdate function

    public HeuristicMode heuristicMode; /*!< <c>heuristicMode</c> Which heuristic used for find path. */

    /// <summary>
    ///WARNING, do not use void <c>Start()</c> function, use this <c>AITankStart()</c> function instead if you want to use Start method from Monobehaviour.
    ///Use this function to initialise your tank variables etc.
    /// </summary>
    public override void AITankStart()
    {
        InitialiseRules();
        InitialiseFacts();
        InitializeStateMachine();
        lastSeenTimer = 9999;
        calcTransform.parent = null;
        enemyLastSeen.parent = null;
        //controller.ControllerStart();
    }

    /// <summary>
    ///WARNING, do not use void <c>Update()</c> function, use this <c>AITankUpdate()</c> function instead if you want to use Start method from Monobehaviour.
    ///Function checks to see what is currently visible in the tank sensor and updates the Founds list. <code>First().Key</code> is used to get the first
    ///element found in that dictionary list and is set as the target, based on tank health, ammo and fuel. 
    /// </summary>
    public override void AITankUpdate()
    {
        if (VisibleEnemyTanks.Count > 0)
        {
            enemyLastSeen.position = VisibleEnemyTanks.Keys.First().transform.position;
            lastSeenTimer = 0;
        }
        else lastSeenTimer += Time.deltaTime;


        attacked = lastFrameHealth > TankCurrentHealth;
        takenBackshot = attacked && VisibleEnemyTanks.Count == 0;

        //controller.ControllerUpdate();

        lastFrameHealth = TankCurrentHealth;
    }

    /// <summary>
    ///WARNING, do not use void <c>OnCollisionEnter()</c> function, use this <c>AIOnCollisionEnter()</c> function instead if you want to use OnColiisionEnter function from Monobehaviour.
    ///Use this function to see if tank has collided with anything.
    /// </summary>
    public override void AIOnCollisionEnter(Collision collision)
    {
    }



    /*******************************************************************************************************       
    Below are a set of functions you can use. These reference the functions in the AITank Abstract class
    and are protected. These are simply to make access easier if you an not familiar with inheritance and modifiers
    when dealing with reference to this class. This does mean you will have two similar function names, one in this
    class and one from the AIClass. 
    *******************************************************************************************************/


    /// <summary>
    /// Generate a path from current position to pointInWorld (GameObject). If no heuristic mode is set, default is Euclidean,
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    public void GeneratePathToWorldPoint(GameObject pointInWorld)
    {
        a_FindPathToPoint(pointInWorld);
    }

    /// <summary>
    /// Generate a path from current position to pointInWorld (GameObject)
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void GeneratePathToWorldPoint(GameObject pointInWorld, HeuristicMode heuristic)
    {
        a_FindPathToPoint(pointInWorld, heuristic);
    }

    /// <summary>
    ///Generate and Follow path to pointInWorld (GameObject) at normalizedSpeed (0-1). If no heuristic mode is set, default is Euclidean,
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    public void FollowPathToWorldPoint(GameObject pointInWorld, float normalizedSpeed)
    {
        a_FollowPathToPoint(pointInWorld, normalizedSpeed);
    }

    /// <summary>
    ///Generate and Follow path to pointInWorld (GameObject) at normalizedSpeed (0-1). 
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void FollowPathToWorldPoint(GameObject pointInWorld, float normalizedSpeed, HeuristicMode heuristic)
    {
        a_FollowPathToPoint(pointInWorld, normalizedSpeed, heuristic);
    }

    /// <summary>
    ///Generate and Follow path to a randome point at normalizedSpeed (0-1). Go to a randon spot in the playfield. 
    ///If no heuristic mode is set, default is Euclidean,
    /// </summary>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    public void FollowPathToRandomWorldPoint(float normalizedSpeed)
    {
        a_FollowPathToRandomPoint(normalizedSpeed);
    }

    /// <summary>
    ///Generate and Follow path to a randome point at normalizedSpeed (0-1). Go to a randon spot in the playfield
    /// </summary>
    /// <param name="normalizedSpeed">This is speed the tank should go at. Normalised speed between 0f,1f.</param>
    /// <param name="heuristic">Chosen heuristic for path finding</param>
    public void FollowPathToRandomWorldPoint(float normalizedSpeed, HeuristicMode heuristic)
    {
        a_FollowPathToRandomPoint(normalizedSpeed, heuristic);
    }

    /// <summary>
    ///Generate new random point
    /// </summary>
    public void GenerateNewRandomWorldPoint()
    {
        a_GenerateRandomPoint();
    }

    /// <summary>
    /// Stop Tank at current position.
    /// </summary>
    public void TankStop()
    {
        a_StopTank();
    }

    /// <summary>
    /// Continue Tank movement at last know speed and pointInWorld path.
    /// </summary>
    public void TankGo()
    {
        a_StartTank();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject)
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    public void TurretFaceWorldPoint(GameObject pointInWorld)
    {
        a_FaceTurretToPoint(pointInWorld);
    }

    /// <summary>
    /// Reset turret to forward facing position
    /// </summary>
    public void TurretReset()
    {
        a_ResetTurret();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject) and fire (has delay).
    /// </summary>
    /// <param name="pointInWorld">This is a gameobject that is in the scene.</param>
    public void TurretFireAtPoint(GameObject pointInWorld)
    {
        a_FireAtPoint(pointInWorld);
    }

    /// <summary>
    /// Returns true if the tank is currently in the process of firing.
    /// </summary>
    public bool TankIsFiring()
    {
        return a_IsFiring;
    }

    void InitialiseFacts()
    {
        stats.Add("lowHealth_FSMRBS", lowHealth);
        stats.Add("lowFuel_FSMRBS", lowFuel);
        stats.Add("lowAmmo_FSMRBS", lowAmmo);

        stats.Add("attackState_FSMRBS", false);
        stats.Add("chaseState_FSMRBS", false);
        stats.Add("kitingState_FSMRBS", false);
        stats.Add("retreatState_FSMRBS", false);
        stats.Add("searchState_FSMRBS", false);

        stats.Add("enemySpotted", false);
        stats.Add("targetReached", false);
        stats.Add("hasKited", false);
    }

    void InitialiseRules()
    {
        //if not seen and not in search, Search
        rules.AddRule(new ST_Rule("enemySpotted", "searchState_FSMRBS", typeof(ST_Search_FSMRBS), ST_Rule.Predicate.nAnd));
        //if target seen but not reachable, Chase
        rules.AddRule(new ST_Rule("enemySpotted", "searchState_FSMRBS", typeof(ST_Chase_FSMRBS), ST_Rule.Predicate.And));
        //if tank hasn't kited, enter kit state
        rules.AddRule(new ST_Rule("targetReachable", "chaseState_FSMRBS", typeof(ST_Kiting_FSMRBS), ST_Rule.Predicate.And));
        //if target reachable and chasing
        rules.AddRule(new ST_Rule("hasKited", "targetReachable", "chaseState_FSMRBS", typeof(ST_Attack_FSMRBS), ST_Rule.Predicate.And));
        //if low health, low fuel or low ammo, flee
        rules.AddRule(new ST_Rule("lowHealth_FSMRBS", "lowAmmo_FSMRBS","lowFuel_FSMRBS", typeof(ST_Retreat_FSMRBS), ST_Rule.Predicate.Or));

    }

    //Initialize FSM machine States
    void InitializeStateMachine()
    {
        Dictionary<Type, ST_Base_FSMRBS> states = new Dictionary<Type, ST_Base_FSMRBS>();

        states.Add(typeof(ST_Attack_FSMRBS), new ST_Attack_FSMRBS(this));
        states.Add(typeof(ST_Chase_FSMRBS), new ST_Chase_FSMRBS(this));
        states.Add(typeof(ST_Kiting_FSMRBS), new ST_Kiting_FSMRBS(this));
        states.Add(typeof(ST_Retreat_FSMRBS), new ST_Retreat_FSMRBS(this));
        states.Add(typeof(ST_Search_FSMRBS), new ST_Search_FSMRBS(this));

        GetComponent<ST_StateMachine_FSMRBS>().SetStates(states);
    }
    
    //check is there are enemy tanks withing FOV
    void CheckEnemySpotted()
    {
        if (enemyTanksFound.Count > 0) { stats["enemySpotted"] = true; }
        else { stats["enemySpotted"] = false; }
    }

    //Check if enTank is withing fire range
    void CheckTargetReached()
    {
        float dist = Vector3.Distance(transform.position, enemyTanksFound.Keys.First().transform.position);

        if (dist < 35f) { stats["targetReached"] = true; }
        else { stats["targetReached"] = false;}
    }

    #region extras
    /// <summary>
    /// Returns float value of remaining health.
    /// </summary>
    /// <returns>Current health.</returns>
    public float TankCurrentHealth
    {
        get
        {
            return a_GetHealthLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining ammo.
    /// </summary>
    /// <returns>Current ammo.</returns>
    public float TankCurrentAmmo
    {
        get
        {
            return a_GetAmmoLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining fuel.
    /// </summary>
    /// <returns>Current fuel level.</returns>
    public float TankCurrentFuel
    {
        get
        {
            return a_GetFuelLevel;
        }
    }

    /// <summary>
    /// Returns list of friendly bases. Does not include bases which have been destroyed.
    /// </summary>
    /// <returns>List of your own bases which are. </returns>
    public List<GameObject> MyBases
    {
        get
        {
            return a_GetMyBases;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject target, float distance) of visible targets (tanks in TankMain LayerMask).
    /// </summary>
    /// <returns>All enemy tanks currently visible.</returns>
    public Dictionary<GameObject, float> VisibleEnemyTanks
    {
        get
        {
            return a_TanksFound;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject consumable, float distance) of visible consumables (consumables in Consumable LayerMask).
    /// </summary>
    /// <returns>All consumables currently visible.</returns>
    public Dictionary<GameObject, float> VisibleConsumables
    {
        get
        {
            return a_ConsumablesFound;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject base, float distance) of visible enemy bases (bases in Base LayerMask).
    /// </summary>
    /// <returns>All enemy bases currently visible.</returns>
    public Dictionary<GameObject, float> VisibleEnemyBases
    {
        get
        {
            return a_BasesFound;
        }
    }
    #endregion extras
}