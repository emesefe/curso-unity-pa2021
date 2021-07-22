using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    public PokemonBase _base;
    public int _level;
    public bool isPlayer;
    public Pokemon Pokemon { get; set; }

    public void SetUpPokemon()
    {
        Pokemon = new Pokemon(_base, _level);

        GetComponent<Image>().sprite = (isPlayer ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite);
    }
}
