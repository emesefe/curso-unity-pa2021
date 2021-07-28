using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;


[Serializable]
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

    [SerializeField] private PokemonBase _base;
    [SerializeField] private int _level;
    private List<Move> _moves;
    private int _hp;

    private float criticalProbability = 8; // 8%
    
    #endregion
        
    // Constructor
    public void InitPokemon()
    {
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

    public DamageDescription ReceiveDamage(Pokemon attacker, Move move)
    {
        float critical = 1f;
        if (Random.Range(0, 100) < criticalProbability)
        {
            critical = 2f;
        }

        float type1 = TypeMatrix.GetMultiplierEffectiveneess(move.Base.Type, Base.Type1);
        float type2 = TypeMatrix.GetMultiplierEffectiveneess(move.Base.Type, Base.Type2);

        DamageDescription damageDescription = new DamageDescription()
        {
            Critical = critical,
            Type = type1 * type2,
            Weakened = false
        };

        float attack = (move.Base.IsSpecialMove ? attacker.SPAttack : attacker.Attack);
        float defense = (move.Base.IsSpecialMove ? SPDefense : Defense);

        float modifiers = Random.Range(0.85f, 1.0f) * type1 * type2 * critical;
        
        // Fórmula sacada de Bulbapedia para el daño infligido
        float baseDamage = ((2 * attacker.Level / 5.0f + 2) * move.Base.Power * (attack / (float) defense)) / 50.0f + 2;

        int totalDamage = Mathf.FloorToInt(baseDamage * modifiers);

        HP -= totalDamage;

        if (HP <= 0)
        {
            HP = 0;
            damageDescription.Weakened = true;
        }
        
        return damageDescription;
    }

    public Move RandomMovement()
    {
        List<Move> movesWithPP = Moves.Where(m => m.PP > 0).ToList();
        if (movesWithPP.Count > 0)
        {
            int randId = Random.Range(0, Moves.Count);
            return movesWithPP[randId];
        }
        // No hay Pps en ningún ataque
        // TODO: Implementar combate, ataque que hace daño al target y a ti mismo
        return null;
    }
}

public class DamageDescription
{
    public float Critical { get; set; }
    public float Type { get; set; }
    public bool Weakened { get; set; }
}
