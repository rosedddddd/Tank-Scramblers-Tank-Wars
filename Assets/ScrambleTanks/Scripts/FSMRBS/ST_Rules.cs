using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ST_Rules
{
    public void AddRule(ST_Rule rule)
    {
        GetRules.Add(rule);
    }
    public List<ST_Rule> GetRules {  get; } = new List<ST_Rule>();
}