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

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        
        pokemonName.text = pokemon.Base.Name;
        pokemonLevel.text = String.Format("Lv {0}", pokemon.Level);
        healthBar.SetHP((float)_pokemon.HP / _pokemon.MaxHP);
        StartCoroutine(UpdatePokemonData(pokemon.HP));
    }

    public IEnumerator UpdatePokemonData(int oldHPValue)
    {
        StartCoroutine(healthBar.SetSmoothHP((float) _pokemon.HP / _pokemon.MaxHP));
        StartCoroutine(DecreaseHealthPoints(oldHPValue));
        yield return null;
    }

    private IEnumerator DecreaseHealthPoints(int oldHPValue)
    {
        float secondsBetweenChange = (float) 1 / (oldHPValue - _pokemon.HP);

        while (oldHPValue > _pokemon.HP)
        {
            oldHPValue--;
            pokemonHealth.text = $"{oldHPValue} / {_pokemon.MaxHP}";
            yield return new WaitForSeconds(secondsBetweenChange);
        }
        
        pokemonHealth.text = $"{_pokemon.HP} / {_pokemon.MaxHP}";
    }
}
