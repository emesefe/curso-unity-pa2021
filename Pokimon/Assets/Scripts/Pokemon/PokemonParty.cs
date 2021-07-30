using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  System;
using System.Linq;

public class PokemonParty : MonoBehaviour
{
    public List<Pokemon> Pokemons { get => pokemons;}
    
    [SerializeField] private List<Pokemon> pokemons;
    private const int MAX_NUM_POKEMON_IN_PARTY = 6;

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

    /// <summary>
    /// Añade un nuevo Pokemon a la Party
    /// </summary>
    /// <param name="pokemon">Pokemon a añadir</param>
    public bool AddPokemonToParty(Pokemon pokemon)
    {
        if (pokemons.Count < MAX_NUM_POKEMON_IN_PARTY)
        {
            pokemons.Add(pokemon);
            return true;
        }else
        {
            // TODO: Añadir la funcionalidad de enviarlo al PC
            return false;
        }
    }
}
