using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    public PokemonBase _base;
    public int _level;
    public bool isPlayer;
    
    public Pokemon Pokemon { get; set; }

    private Image pokemonImage;
    private Vector3 initialPosition;
    private float initialOffset = 400;
    private float weakenedOffset = 150;
    
    [SerializeField] private float startAnimationDuration;
    [SerializeField] private float weakenedAnimationDuration;


    private void Awake()
    {
        pokemonImage = GetComponent<Image>();
        initialPosition = pokemonImage.transform.localPosition;
    }

    public void SetUpPokemon()
    {
        Pokemon = new Pokemon(_base, _level);

        pokemonImage.sprite = (isPlayer ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite);
        PlayStartAnimation();
    }

    public void PlayStartAnimation()
    {
        pokemonImage.transform.localPosition = new Vector3(initialPosition.x + (isPlayer ? -1 : 1) * initialOffset, initialPosition.y);
        pokemonImage.transform.DOLocalMoveX(initialPosition.x, startAnimationDuration);
    }

    public void PlayWeakenedAnimation()
    {
        pokemonImage.transform.DOLocalMoveY(initialPosition.y - weakenedOffset, weakenedAnimationDuration);
        pokemonImage.DOFade(0, weakenedAnimationDuration);
    }
    
    public void PlayAttackAnimation()
    {
        // TODO
    }
    
    public void PlayReceiveDamageAnimation()
    {
        // TODO
    }
}
