using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  System;
using System.Linq;

public class PokemonParty : MonoBehaviour
{
    public List<Pokemon> Pokemons { get => pokemons;}
    
    [SerializeField] private List<Pokemon> pokemons;

    private void Start()
    {
        foreach (Pokemon pokemon in pokemons)
        {
            pokemon.InitPokemon();
        }
    }

    /// <summary>
    /// Devuelve el primer pokemon con HP > 0
    /// </summary>
    /// <returns></returns>
    public Pokemon GetFirstNonWeakenedPokemon()
    {
        return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }

    public int GetPositionFromPokemonInBattle(Pokemon pokemon)
    {
        for (int i = 0; i < Pokemons.Count; i++)
        {
            if (pokemon == Pokemons[i])
            {
                return i;
            }
        }

        return -1;
    }
}
