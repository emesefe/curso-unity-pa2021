using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text pokemonName;
    public Text pokemonLevel;
    public Text pokemonHealth;
    public HealthBar healthBar;

    private Pokemon _pokemon;

    public void SetPokemon(Pokemon pokemon)
    {
        _pokemon = pokemon;
        
        pokemonName.text = pokemon.Base.Name;
        pokemonLevel.text = String.Format("Lv {0}", pokemon.Level);
        UpdatePokemonData();
    }

    public void UpdatePokemonData()
    {
        StartCoroutine(healthBar.SetSmoothHP((float) _pokemon.HP / _pokemon.MaxHP));
        pokemonHealth.text = $"{_pokemon.HP} / {_pokemon.MaxHP}";
    }
}
