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
            }
        };

    private static void PoisonEffect(Pokemon pokemon)
    {
        pokemon.UpdateHP(pokemon.MaxHP / 8);
        pokemon.StatsChangeMessages.Enqueue($"{pokemon.Base.Name} sufre los efectos del veneno.");
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