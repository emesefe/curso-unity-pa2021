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
    public HealthBar healthBar;
    public GameObject expBar;
    public GameObject statusBox;

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        
        pokemonName.text = pokemon.Base.Name;
        SetLevelText();
        healthBar.SetHP(_pokemon);
        SetExpBar();
        StartCoroutine(UpdatePokemonData());
        SetStatusConditionData();
        _pokemon.OnStatusConditionChanged += SetStatusConditionData;
    }

    public IEnumerator UpdatePokemonData()
    {
        if (_pokemon.HasHPChanged)
        {
            yield return healthBar.SetSmoothHP(_pokemon);
            _pokemon.HasHPChanged = false;
        }
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

    private void SetStatusConditionData()
    {
        if (_pokemon.StatusCondition == null)
        {
            statusBox.SetActive(false);
        }
        else
        {
            statusBox.SetActive(true);
            statusBox.GetComponent<Image>().color = Color.magenta;
            statusBox.GetComponentInChildren<Text>().text = _pokemon.StatusCondition.Id.ToString().ToUpper();
        }
    }
}
