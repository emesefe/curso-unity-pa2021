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
                    Description = "Hace que el Pokemon sufra parálisis en el turno",
                    StartMessage = "ha sido paralizado.",
                    OnStartTurn = ParalysisEffect
                }
            },
            {
                StatusConditionID.Frz,
                new StatusCondition()
                {
                    Name = "Freezing",
                    Description = "Hace que el Pokemon esté congelado, pero se puede curar aleatoriamente durante un turno",
                    StartMessage = "ha sido congelado.",
                    OnStartTurn = FreezingEffect
                }
            },
            {
                StatusConditionID.Slp,
                new StatusCondition()
                {
                    Name = "Sleep",
                    Description = "Hace que el Pokemon esté dormido un número fijo de turnos",
                    StartMessage = "se ha dormido.",
                    OnApplyStatusCondition = (Pokemon pokemon) =>
                    {
                        pokemon.StatusNumberTurns = Random.Range(1, 4);
                    },
                    OnStartTurn = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusNumberTurns <= 0)
                        {
                            pokemon.CureStatusCondition();
                            pokemon.StatusChangeMessages.Enqueue($"¡{pokemon.Base.Name} ha despertado!");
                            return true;
                        }
                        pokemon.StatusNumberTurns--;
                        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sigue dormido.");
                        return false;
                    }
                }
            }
        };

    private static void PoisonEffect(Pokemon pokemon)
    {
        pokemon.UpdateHP(Mathf.CeilToInt((float) pokemon.MaxHP / 8));
        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sufre los efectos del veneno.");
    }
    
    private static void BurnEffect(Pokemon pokemon)
    {
        pokemon.UpdateHP(Mathf.CeilToInt((float) pokemon.MaxHP / 15));
        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sufre los efectos de la quemadura.");
    }

    private static bool ParalysisEffect(Pokemon pokemon)
    {
        // ¿Podemos atacar? Si true, es que sí. Si false, es que no porque estamos paralizados
        if (Random.Range(0, 100) < 25)
        {
            // Hay un 25% de probabilidad de sufrir parálisis
            pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} está paralizado y no puede moverse.");
            return false;
        }
        return true;
    }
    
    private static bool FreezingEffect(Pokemon pokemon)
    {
        // ¿Podemos atacar? Si true, es que sí. Si false, es que no porque estamos paralizados
        if (Random.Range(0, 100) < 25)
        {
            // Hay un 25% de probabilidad de dejar de estar congelado
            pokemon.CureStatusCondition();
            pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} ya no está congelado.");
            return true;
        }
        
        pokemon.StatusChangeMessages.Enqueue($"{pokemon.Base.Name} sigue congelado. No puede atacar.");
        return false;
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