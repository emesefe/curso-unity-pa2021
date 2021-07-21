using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon / Nuevo Pokemon")]

public class PokemonBase : ScriptableObject
{
    public int ID { get => id; }
    public string Name { get => name; }
    public string Description { get => description; }
    public PokemonType Type1 { get => type1; }
    public PokemonType Type2 { get => type2; }
    public int MaxHP { get => maxHP; }
    public int Attack { get => attack; }
    public int Defense { get => defense; }
    public int SPAttack { get => spAttack; }
    public int SPDefense { get => spDefense; }
    public int Speed { get => speed; }
    public List<LearnableMove> LearnableMoves { get => learnableMoves; }

    
    [SerializeField] private int id;
    [SerializeField] private string name;
    [TextArea][SerializeField] private string description;
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;
    [SerializeField] private List<LearnableMove> learnableMoves;

    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;
}

public enum PokemonType
{
    None,
    Bug,
    Dark,
    Dragon,
    Electric,
    Fairy,
    Fight,
    Fire,
    Flying,
    Ghost,
    Grass,
    Ground,
    Ice,
    Normal,
    Poison,
    Psychic,
    Rock,
    Steel,
    Water
}

[Serializable]
public class LearnableMove
{
    public MoveBase Move { get => move; }
    public int Level  { get => level; }
    
    [SerializeField] private MoveBase move;
    [SerializeField] private int level;
}
