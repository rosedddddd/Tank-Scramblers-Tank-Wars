using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ST_Rules
{
    public void AddRule(Rule rule)
    {
        GetRules.Add(rule);
    }
    public List<Rule> GetRules {  get; } = new List<Rule>();
}
