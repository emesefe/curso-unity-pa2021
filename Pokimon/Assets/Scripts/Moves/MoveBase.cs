using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon / Nuevo Movimiento")]
public class MoveBase : ScriptableObject
{
   public string Name { get => name; }
   public string Description { get => description; }
   public PokemonType Type { get => type; }
   public int PP { get => pp; }
   public int Power { get => power; }
   public int Accuracy { get => accuracy; }

   public bool IsSpecialMove
   {
      get
      {
         if (type == PokemonType.Fire || type == PokemonType.Water ||
             type == PokemonType.Grass || type == PokemonType.Ice ||
             type == PokemonType.Electric || type == PokemonType.Dragon||
             type == PokemonType.Dark || type == PokemonType.Psychic)
         {
             return true; 
         }
         
         return false;
      }
   }
   

   [SerializeField] private string name;
   [TextArea] [SerializeField] private string description;
   [SerializeField] private PokemonType type;
   [SerializeField] private int pp;
   [SerializeField] private int power;
   [SerializeField] private int accuracy;
   
}
