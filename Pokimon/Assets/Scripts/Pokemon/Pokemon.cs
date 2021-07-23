using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pokemon
{
    #region VARIABLES PÚBLICAS
    // Vida base por nivel
    public int MaxHP { get => Mathf.FloorToInt((_base.MaxHP * _level) / 20f) + 10; }
    
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

    public List<Move> Moves
    {
        get => _moves;
        set => _moves = value;
    }

    // Vida actual
    public int HP
    {
        get => _hp;
        set => _hp = value;
    }
    
    public int Level { get => _level; }
    public PokemonBase Base { get => _base; }
    
    #endregion
    
    #region VARIABLES PRIVADAS

    private PokemonBase _base;
    private int _level;
    private List<Move> _moves;
    private int _hp;
    
    #endregion
        
    // Constructor
    public Pokemon(PokemonBase pokemonBase, int pokemonLevel)
    {
        _base = pokemonBase;
        _level = pokemonLevel;
        _moves = new List<Move>();

        _hp = MaxHP;

        foreach (LearnableMove learnableMove in _base.LearnableMoves)
        {
            if (learnableMove.Level <= _level)
            {
                _moves.Add(new Move(learnableMove.Move));
            }

            if (_moves.Count >= 4)
            {
                break;
            }
        }
    }

    public bool ReceiveDamage(Pokemon attacker, Move move)
    {
        float modifiers = Random.Range(0.85f, 1.0f);
        
        // Fórmula sacada de Bulbapedia para el daño infligido
        float baseDamage = ((2 * attacker.Level / 5.0f + 2) * move.Base.Power * (attacker.Attack / (float) Defense)) / 50.0f + 2;

        int totalDamage = Mathf.FloorToInt(baseDamage * modifiers);

        HP -= totalDamage;

        if (HP <= 0)
        {
            HP = 0;
            return true;
        }
        return false;
    }

    public Move RandomMove()
    {
        int randId = Random.Range(0, Moves.Count);
        return Moves[randId];
    }
}
