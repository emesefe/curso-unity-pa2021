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
        healthBar.SetHP(1);
        UpdatePokemonData(pokemon.HP);
    }

    public void UpdatePokemonData(int oldHPValue)
    {
        StartCoroutine(healthBar.SetSmoothHP((float) _pokemon.HP / _pokemon.MaxHP));
        StartCoroutine(DecreaseHealthPoints(oldHPValue));
    }

    private IEnumerator DecreaseHealthPoints(int oldHPValue)
    {
        while (oldHPValue > _pokemon.HP)
        {
            oldHPValue--;
            pokemonHealth.text = $"{oldHPValue} / {_pokemon.MaxHP}";
            yield return new WaitForSeconds(0.1f);
        }
        
        pokemonHealth.text = $"{_pokemon.HP} / {_pokemon.MaxHP}";
    }
}
