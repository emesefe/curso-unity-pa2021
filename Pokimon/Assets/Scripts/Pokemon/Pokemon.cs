using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEditorInternal;


[Serializable]
public class Pokemon
{
    #region VARIABLES PÚBLICAS
    // Vida base por nivel
    public int MaxHP { get => Mathf.FloorToInt((_base.MaxHP * _level) / 20f) + 10; }
    
    // Ataque base por nivel
    public int Attack { get => GetStat(Stat.Attack); }
    
    // Defensa base por nivel
    public int Defense { get => GetStat(Stat.Defense); }
    
    // Ataque especial base por nivel
    public int SpAttack { get => GetStat(Stat.SpAttack); }
    
    // Defensa especial base por nivel
    public int SpDefense { get => GetStat(Stat.SpDefense); }
    
    // Velocidad base por nivel
    public int Speed { get => GetStat(Stat.Speed); }

    public List<Move> Moves
    {
        get => _moves;
        set => _moves = value;
    }

    // Vida actual
    public int HP
    {
        get => _hp;
        set
        {
            _hp = value;
            _hp = Mathf.FloorToInt(Mathf.Clamp(_hp, 0, MaxHP));
        } 
    }
    
    public int Level { get => _level; }
    public PokemonBase Base { get => _base; }
    
    public int Experience
    {
        get => _experience;
        set => _experience = value;
    }

    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatsBoosted { get; private set; }
    
    public Queue<string> StatsChangeMessages { get; private set; }

    #endregion
    
    #region VARIABLES PRIVADAS

    [SerializeField] private PokemonBase _base;
    [SerializeField] private int _level;
    private List<Move> _moves;
    private int _hp;

    private float criticalProbability = 8; // 8%

    private int _experience;

    #endregion
        
    // Constructor
    public Pokemon(PokemonBase pokemonBase, int pokemonLevel)
    {
        _base = pokemonBase;
        _level = pokemonLevel;
        
        InitPokemon();
    }

    public void InitPokemon()
    {
        _moves = new List<Move>();

        _hp = MaxHP;
        _experience = _base.GetNecessaryExperienceForLevel(_level);

        foreach (LearnableMove learnableMove in _base.LearnableMoves)
        {
            if (learnableMove.Level <= _level)
            {
                _moves.Add(new Move(learnableMove.Move));
            }

            if (_moves.Count >= PokemonBase.NUMBER_OF_LEARNABLE_MOVES)
            {
                break;
            }
        }
        
        CalculateStats();
        ResetBoostings();
    }

    public bool NeedsToLevelUp()
    {
        if (Experience > Base.GetNecessaryExperienceForLevel(_level + 1))
        {
            int currentMaxHP = MaxHP;
            _level++;
            HP += MaxHP - currentMaxHP;
            return true;
        }

        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return Base.LearnableMoves.Where(lm => lm.Level == _level).FirstOrDefault();
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

        float attack = (move.Base.IsSpecialMove ? attacker.SpAttack : attacker.Attack);
        float defense = (move.Base.IsSpecialMove ? SpDefense : Defense);

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

    public void LearnMove(LearnableMove learnableMove)
    {
        if (Moves.Count >= PokemonBase.NUMBER_OF_LEARNABLE_MOVES)
        {
            return;
        }
        
        // La cantidad de movimientos es inferior a 4
        Moves.Add(new Move(learnableMove.Move));
    }
    
    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((_base.Attack * _level) / 100f) + 1);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((_base.Defense * _level) / 100f) + 1);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((_base.SpAttack * _level) / 100f) + 1);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((_base.SpDefense * _level) / 100f) + 1);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((_base.Speed * _level) / 100f) + 1);
    }

    private int GetStat(Stat stat)
    {
        int statValue = Stats[stat];
        int boost = StatsBoosted[stat];

        // Niveles de mejora en Pokemon: 1 -> 1.5 -> 2 -> 2.5 -> ... -> 4
        float multiplier = 1.0f + Mathf.Abs(boost) / 2.0f;
        statValue = Mathf.FloorToInt(statValue * (boost >= 0 ? multiplier : (1 / multiplier)));
        
        return statValue;
    }

    public void ApplyBoost(StatBoosting boost)
    {
        Stat stat = boost.stat;
        int value = boost.boost;
            
        StatsBoosted[stat] = Mathf.Clamp(StatsBoosted[stat] + value, -6, 6);
        if (value > 0)
        {
            // La estadística del pokemon ha incrementado
            StatsChangeMessages.Enqueue($"{Base.Name} ha incrementado su {stat}.");
        }
        else if (value < 0)
        {
            // La estadística del pokemon ha decrementado
            StatsChangeMessages.Enqueue($"{Base.Name} ha reducido su {stat}.");
        }
        else
        {
            // value = 0
            StatsChangeMessages.Enqueue($"{Base.Name} no nota ningún efecto.");
        }
    }

    private void ResetBoostings()
    {
        StatsChangeMessages = new Queue<string>();
        StatsBoosted = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0}
        };
    }

    public void OnBattleFinish()
    {
        ResetBoostings();
    }
}

public class DamageDescription
{
    public float Critical { get; set; }
    public float Type { get; set; }
    public bool Weakened { get; set; }
}
