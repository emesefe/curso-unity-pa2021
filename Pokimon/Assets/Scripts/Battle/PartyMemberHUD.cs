using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{
    public Text nameText, levelText, typeText, hpText;
    public HealthBar healtBar;
    public Image pokemonImage;

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        levelText.text = $"Lv {pokemon.Level}";
        if (pokemon.Base.Type2 == PokemonType.None)
        {
            typeText.text = pokemon.Base.Type1.ToString().ToUpper();
        }
        else
        { 
            typeText.text = $"{pokemon.Base.Type1.ToString().ToUpper()}\t {pokemon.Base.Type2.ToString().ToUpper()}";
        }
        hpText.text = $"{pokemon.HP} / {pokemon.MaxHP}";
        
        healtBar.SetHP((float) pokemon.HP / pokemon.MaxHP);
        pokemonImage.sprite = pokemon.Base.FrontSprite;
        GetComponent<Image>().color = ColorType.GetColorFromType(pokemon.Base.Type1);
    }

    public void SetSelectedPokemon(bool selected)
    {
        if (selected)
        {
            nameText.fontStyle = FontStyle.Bold;
        }
        else
        {
            nameText.fontStyle = FontStyle.Normal;
        }
    }
}
