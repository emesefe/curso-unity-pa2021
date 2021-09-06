using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusCondition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    
    public Func<Pokemon, bool> OnStartTurn { get; set; }
    public Action<Pokemon> OnFinishTurn { get; set; }
}
