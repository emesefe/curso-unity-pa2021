using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusConditionFactory
{
    public static Dictionary<StatusConditionID, StatusCondition> StatusConditions { get; set; } = 
        new Dictionary<StatusConditionID, StatusCondition>()
        {
            {
                StatusConditionID.Psn,
                new StatusCondition()
                {
                    Name = "Poison",
                    Description = "Hace que el Pokemon sufra daño en cada turno",
                    StartMessage = "ha sido envenado.",
                    OnFinishTurn = PoisonEffect
                }
            },
            {
                StatusConditionID.Brn,
                new StatusCondition()
                {
                    Name = "Burn",
                    Description = "Hace que el Pokemon sufra daño en cada turno",
                    StartMessage = "ha sido quemado.",
                    OnFinishTurn = BurnEffect
                }
            },
            {
                StatusConditionID.Par,
                new StatusCondition()
                {
                    Name = "Paralysis",
                    Description = "Hace que el Pokemon sufra parálisis",
                    StartMessage = "ha sido paralizado.",
                    OnStartTurn = ParalysisEffect
                }
            }
        };

    private static void PoisonEffect(Pokemon pokemon)
    {
        pokemon.UpdateHP(Mathf.CeilToInt((float) pokemon.MaxHP / 8));
        pokemon.StatsChangeMessages.Enqueue($"{pokemon.Base.Name} sufre los efectos del veneno.");
    }
    
    private static void BurnEffect(Pokemon pokemon)
    {
        pokemon.UpdateHP(Mathf.CeilToInt((float) pokemon.MaxHP / 15));
        pokemon.StatsChangeMessages.Enqueue($"{pokemon.Base.Name} sufre los efectos de la quemadura.");
    }

    private static bool ParalysisEffect(Pokemon pokemon)
    {
        // ¿Podemos atacar? Si true, es que sí. Si false, es que no porque estamos paralizados
        if (Random.Range(0, 100) < 25)
        {
            // Hay un 25% de probabilidad de sufrir parálisis
            pokemon.StatsChangeMessages.Enqueue($"{pokemon.Base.Name} está paralizado y no puede moverse.");
            return false;
        }
        return true;
    }
}

public enum StatusConditionID
{
    None, // Ningún estado alterado
    Brn, // Quemado
    Frz, // Congelado
    Par, // Paralizado
    Psn, // Envenenado
    Slp // Dormido
}