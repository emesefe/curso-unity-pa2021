using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon / Nuevo Movimiento")]
public class MoveBase : ScriptableObject
{
    #region VARIABLES PUBLICAS
    
    public string Name { get => name; }
    public string Description { get => description; }
    public PokemonType Type { get => type; }
    public int PP { get => pp; }
    public int Power { get => power; }
    public int Accuracy { get => accuracy; }
    public MoveType MoveType { get => moveType; }
    public MoveStatEffect Effects { get => effects; }
    public MoveTarget Target { get => target; }
    public bool IsSpecialMove =>  moveType == MoveType.Special;
    
    #endregion
    
    #region VARIABLES PRIVADAS
    
    [SerializeField] private string name;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private PokemonType type;
    [SerializeField] private int pp;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private MoveType moveType;
    [SerializeField] private MoveStatEffect effects;
    [SerializeField] private MoveTarget target;
    
    #endregion
}

public enum MoveType
{
    Physical,
    Special, 
    Stats
}

[Serializable]
public class MoveStatEffect
{
    public List<StatBoosting> Boostings => boostings;
    public StatusConditionID Status => status;
    
    [SerializeField] private List<StatBoosting> boostings;
    [SerializeField] private StatusConditionID status;
}

[Serializable]
public class StatBoosting
{
    public Stat stat;
    public int boost;
    public MoveTarget target;
}

public enum MoveTarget
{
    Self,
    Other
}
