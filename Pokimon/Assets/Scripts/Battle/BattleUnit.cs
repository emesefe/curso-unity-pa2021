using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    #region VARIABLES PUBLICAS
    
    public PokemonBase _base;
    public int _level;
    public bool IsPlayer => isPlayer;

    public Pokemon Pokemon { get; set; }
    public BattleHUD HUD => hud;
    
    #endregion


    #region VARIABLES PRIVADAS
    
    [SerializeField] private bool isPlayer;
    
    private Image pokemonImage;
    private Vector3 initialPosition;
    private Color initialColor;
    private float initialOffset = 400;
    private float weakenedOffset = 150;
    private float attackOffset = 50;
    private int totalBlinksRecieveAttackAnimation = 2;

    private float startAnimationDuration = 2;
    private float weakenedAnimationDuration = 2;
    private float attackAnimationDuration = 0.3f;
    private float recieveAttackAnimationDuration = 0.2f;
    private float capturedAnimationDuration = 0.75f;

    private Vector3 finalCapturedScale = new Vector3(0.25f, 0.25f, 1);
    private float capturedOffsetYPosition = 50;

    [SerializeField] private BattleHUD hud;

    #endregion


    private void Awake()
    {
        pokemonImage = GetComponent<Image>();
        initialPosition = pokemonImage.transform.localPosition;
        initialColor = pokemonImage.color;
    }

    public void SetUpPokemon(Pokemon pokemon)
    {
        Pokemon = pokemon;

        pokemonImage.sprite = (isPlayer ? Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite);
        pokemonImage.color = initialColor;
        
        hud.SetPokemonData(pokemon);
        transform.localScale = Vector3.one;
        
        PlayStartAnimation();
    }

    public void PlayStartAnimation()
    {
        pokemonImage.transform.localPosition = new Vector3(initialPosition.x + (isPlayer ? -1 : 1) * initialOffset, initialPosition.y);
        pokemonImage.transform.DOLocalMoveX(initialPosition.x, startAnimationDuration);
    }

    public void PlayWeakenedAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(pokemonImage.transform.DOLocalMoveY(initialPosition.y - weakenedOffset, weakenedAnimationDuration));
        sequence.Join(pokemonImage.DOFade(0, weakenedAnimationDuration));
    }
    
    public void PlayAttackAnimation()
    {
        // Primero se reproduce una animación e inmediatamente después la siguiente. Eso es una secuencia
        Sequence sequence = DOTween.Sequence();
        sequence.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x + (isPlayer ? 1 : -1) * attackOffset,
            attackAnimationDuration));
        sequence.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x, attackAnimationDuration));

    }
    
    public void PlayReceiveAttackAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        
        for (int i = 0; i < totalBlinksRecieveAttackAnimation; i++)
        {
            sequence.Append(pokemonImage.DOColor(Color.gray, recieveAttackAnimationDuration));
            sequence.Append(pokemonImage.DOColor(initialColor, recieveAttackAnimationDuration));
        }
    }

    public IEnumerator PlayCaputredAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(pokemonImage.DOFade(0, capturedAnimationDuration));
        sequence.Join(transform.DOScale(finalCapturedScale, capturedAnimationDuration));
        sequence.Join(transform.DOLocalMoveY(initialPosition.y + capturedOffsetYPosition, capturedAnimationDuration));
        yield return sequence.WaitForCompletion();
    }
    
    public IEnumerator PlayEscapeFromPokeballAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(pokemonImage.DOFade(1, capturedAnimationDuration));
        sequence.Join(transform.DOScale(Vector3.one, capturedAnimationDuration));
        sequence.Join(transform.DOLocalMoveY(initialPosition.y, capturedAnimationDuration));
        yield return sequence.WaitForCompletion();
    }
}
