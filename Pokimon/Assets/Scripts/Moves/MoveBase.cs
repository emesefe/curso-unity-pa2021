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
   

   [SerializeField] private string name;
   [TextArea] [SerializeField] private string description;
   [SerializeField] private PokemonType type;
   [SerializeField] private int pp;
   [SerializeField] private int power;
   [SerializeField] private int accuracy;
   
}
