using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyHUD : MonoBehaviour
{
    [SerializeField] private Text messageText;
    private PartyMemberHUD[] memberHUds;
    private List<Pokemon> pokemons;

    public void InitPartyHud()
    {
        memberHUds = GetComponentsInChildren<PartyMemberHUD>(true);
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        this.pokemons = pokemons;
        messageText.text = "Selecciona un Pokemon";
        
        for (int i = 0; i < memberHUds.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberHUds[i].SetPokemonData(pokemons[i]);
                memberHUds[i].gameObject.SetActive(true);
            }
            else
            {
                memberHUds[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateSelectedPokemon(int selectedPokemon)
    {
        for (int i = 0; i < pokemons.Count; i++)
        {
            memberHUds[i].SetSelectedPokemon(i == selectedPokemon);
        }
    }

    public void SetMessage(string message)
    {
        messageText.text = message;
    }
}
