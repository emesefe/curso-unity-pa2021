using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text pokemonName;
    public Text pokemonLevel;
    public Text pokemonHealth;
    public HealthBar healthBar;
    public GameObject expBar;

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        
        pokemonName.text = pokemon.Base.Name;
        SetLevelText();
        healthBar.SetHP((float)_pokemon.HP / _pokemon.MaxHP);
        SetExpBar();
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

    public IEnumerator SetExpBarSmooth(bool needsToResetBar = false)
    {
        if (expBar == null)
        {
            yield break;
        }

        if (needsToResetBar)
        {
            expBar.transform.localScale = new Vector3(0,1, 1);
        }

        yield return expBar.transform.DOScaleX(NormalizedExp(), 1).WaitForCompletion();
    }

    public void SetExpBar()
    {
        if (expBar == null)
        {
            return;
        }
        
        expBar.transform.localScale = new Vector3(NormalizedExp(), 1, 1);
    }

    private float NormalizedExp()
    {
        float currentLevelExp = _pokemon.Base.GetNecessaryExperienceForLevel(_pokemon.Level);
        float nextLevelExp = _pokemon.Base.GetNecessaryExperienceForLevel(_pokemon.Level + 1);

        float normalizedExp = (_pokemon.Experience - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public void SetLevelText()
    {
        pokemonLevel.text = $"Lv {_pokemon.Level}";
    }
}
