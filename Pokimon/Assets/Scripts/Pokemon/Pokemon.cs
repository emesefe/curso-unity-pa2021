using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    // Vida base por nivel
    public int MaxHP { get => Mathf.FloorToInt((_base.MaxHP * _level) / 100f) + 10; }
    
    // Ataque base por nivel
    public int Attack { get => Mathf.FloorToInt((_base.Attack * _level) / 100f) + 1; }
    
    // Defensa base por nivel
    public int Defense { get => Mathf.FloorToInt((_base.Defense * _level) / 100f) + 1; }
    
    // Ataque especial base por nivel
    public int SPAttack { get => Mathf.FloorToInt((_base.SPAttack * _level) / 100f) + 1; }
    
    // Defensa especial base por nivel
    public int SPDefense { get => Mathf.FloorToInt((_base.SPDefense * _level) / 100f) + 1; }
    
    // Velocidad base por nivel
    public int Speed { get => Mathf.FloorToInt((_base.Speed * _level) / 100f) + 1; }
    
    private PokemonBase _base;
    private int _level;
    
    // Constructor
    public Pokemon(PokemonBase pokemonBase, int pokemonLevel)
    {
        _base = pokemonBase;
        _level = pokemonLevel;
    }
}
