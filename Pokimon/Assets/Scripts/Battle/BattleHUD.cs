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

    public void SetPokemon(Pokemon pokemon)
    {
        pokemonName.text = pokemon.Base.Name;
        pokemonLevel.text = String.Format("Lv {0}", pokemon.Level);
        healthBar.SetHP(pokemon.HP / pokemon.MaxHP);
        pokemonHealth.text = String.Format("{0} / {1}", pokemon.HP, pokemon.MaxHP);
    }
}
