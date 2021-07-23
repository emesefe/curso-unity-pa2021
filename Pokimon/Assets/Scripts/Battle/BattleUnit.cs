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

    private Image pokemonImage;
    private Vector3 initialPosition;
    private float initialOffset = 400;
    [SerializeField] private float startAnimationDuration;
    
    public Pokemon Pokemon { get; set; }

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
}
