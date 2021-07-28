using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BattleState
{
    StartBattle,
    ActionSelection,
    MovementSelection,
    PerformMovement,
    LoseTurn,
    Busy,
    PartySelectScreen,
    ItemSelectScreen,
    FinishBattle
}

public class BattleManager : MonoBehaviour
{
    #region VARIABLES PUBLICAS
    
    public BattleState battleState;
    public event Action<bool> OnBattleFinished;
    
    #endregion
    
    #region VARIABLES PRIVADAS
    
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogBox battleDialogBox;

    private int currentSelectedAction;
    private int currentSelectedMovement;
    private int currentSelectedPokemon;
    
    private float timeSinceLastClick;
    [SerializeField] private float timeBetweenClicks = 1.0f;

    private PokemonParty playerParty;
    private Pokemon wildPokemon;

    [SerializeField] private PartyHUD partyHud;

    [SerializeField] private GameObject pokeball;
    
    private float attackAnimationDuration = 0.6f;
    private float weakenedAnimationDuration = 2f;
    private Vector3 pokeballInitialOffset = new Vector3(-5, 0, 0);
    private Vector3 pokeballFinalOffset = new Vector3(0, 1.5f, 0);
    private float pokeballThrowAnimationDuration = 1.5f;
    private float pokeballFallAnimationDuration = 0.75f;
    private float jumpPower = 2;
    private int numberJumps = 1;
    private float pokeballToGroundDistance = 1.5f;
    private int numberOfShakes;
    private float timeBetweenPokeballShakes = 0.5f;
    private float pokeballShakeDuration = 0.6f;
    private float shakeDegrees = 15;
    private float pokemonCapturedDuration = 1;
    private float pokeballFadeDuration = 0.2f;

    #endregion
    
    #region METODOS

    public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetUpBattle());
    }

    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;

        if (battleDialogBox.isWriting)
        {
            return;
        }

        if (battleState == BattleState.ActionSelection)
        {
            HandlePlayerActionSelection();
        }else if (battleState == BattleState.MovementSelection)
        {
            HandlePlayerMovementSelection();
        }else if (battleState == BattleState.PartySelectScreen)
        {
            HandlePlayerPartySelection();
        }else if (battleState == BattleState.LoseTurn)
        {
            StartCoroutine(PerformEnemyMovement());
        }
    }

    private void BattleFinish(bool playerHasWon)
    {
        battleState = BattleState.FinishBattle;
        OnBattleFinished(playerHasWon);
    }

    /// <summary>
    /// Jugador elige acción
    /// </summary>
    private void PlayerActionSelection()
    {
        battleState = BattleState.ActionSelection;
        
        StartCoroutine(battleDialogBox.SetDialog("Selecciona una acción."));
        battleDialogBox.ToggleDialogText(true);
        battleDialogBox.ToggleActions(true);
        battleDialogBox.ToggleMovements(false);
        
        // Por defecto hacemos que se muestre seleccionada la primera acción
        currentSelectedAction = 0;
        battleDialogBox.SelectAction(currentSelectedAction);
    }

    /// <summary>
    ///  Jugador elige movimiento del Pokemon
    /// </summary>
    private void PlayerMovementSelection()
    {
        battleState = BattleState.MovementSelection;
        
        battleDialogBox.ToggleDialogText(false);
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(true);

        currentSelectedMovement = 0;
        battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
    }
    
    /// <summary>
    /// Jugador elige Pokemon de la pantalla de selección de Pokemon
    /// </summary>
    private void OpenPartySelectionScreen()
    {
        battleState = BattleState.PartySelectScreen;
        
        partyHud.SetPartyData(playerParty.Pokemons);
        partyHud.gameObject.SetActive(true);
        
        currentSelectedPokemon = playerParty.GetPositionFromPokemonInBattle(playerUnit.Pokemon);
        partyHud.UpdateSelectedPokemon(currentSelectedPokemon);
    }
    
    /// <summary>
    /// Jugador elige Objeto de la pantalla de inventario
    /// </summary>
    private void OpenInventaryScreen()
    {
        // TODO: Implementar inventario y lógica de items

        StartCoroutine(ThrowPokeball());
    }

    private void CheckForBattleFinish(BattleUnit weakenedUnit)
    {
        if (weakenedUnit.IsPlayer)
        {
            Pokemon nextPokemon = playerParty.GetFirstNonWeakenedPokemon();
            if (nextPokemon != null)
            {
                OpenPartySelectionScreen();
            }
            else
            {
                // Todos los pokemons del player han sido debilitados
                BattleFinish(false);
            }
        }
        else
        {
            // El Pokemon debilitado es el enemigo
            BattleFinish(true);
        }
    }

    private int TryToCatchPokemon(Pokemon pokemon)
    {
        // TODO: clase pokeball con su multiplicador
        float bonusPokeball = 1;
        // TODO: stats para chekear condición de modficación
        float bonusStat = 1;
        float a = (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * bonusPokeball * bonusStat / (3 * pokemon.MaxHP);
        if (a >= 255)
        {
            return 4; // Captura inmediata
        }
        
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;

        while (shakeCount < 4)
        {
            if (Random.Range(0, 65535) > b)
            {
                break;
            }
            else
            {
                shakeCount++;
            }
        }
        
        return shakeCount;
    }

    #region METODOS DE SELECCION
    
    private void HandlePlayerActionSelection()
    {
        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }
        
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            
            currentSelectedAction = (currentSelectedAction + 2) % 4;
            
            battleDialogBox.SelectAction(currentSelectedAction);
        }
        else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;

            currentSelectedAction = (currentSelectedAction + 1) % 2 + 2 * (currentSelectedAction / 2);

            battleDialogBox.SelectAction(currentSelectedAction);
        }

        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            
            // Seleccionamos acción
            if (currentSelectedAction == 0)
            {
                // Seleccionamos Luchar
                PlayerMovementSelection();
                
            }else if (currentSelectedAction == 1)
            {
                // Seleccionamos Pokemon
                OpenPartySelectionScreen();

            }else if (currentSelectedAction == 2)
            {
                // Seleccionamos Objetos
                OpenInventaryScreen();

            }else if (currentSelectedAction == 3)
            {
                // Seleccionamos Huir
                BattleFinish(false);
            }
        }
    }
    
    private void HandlePlayerMovementSelection()
    {
        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            
            // Tenemos en cuenta 4 movimientos
            int oldSelectedMovement = currentSelectedMovement;
            currentSelectedMovement = (currentSelectedMovement + 2) % 4;

            if (currentSelectedMovement >= playerUnit.Pokemon.Moves.Count)
            {
                currentSelectedMovement = oldSelectedMovement;
            }
            
            battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
            
        } else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            int oldSelectedMovement = currentSelectedMovement;

            currentSelectedMovement = (currentSelectedMovement + 1) % 2 + 2 * (currentSelectedMovement / 2);
            
            if (currentSelectedMovement >= playerUnit.Pokemon.Moves.Count)
            {
                currentSelectedMovement = oldSelectedMovement;
            }
            
            battleDialogBox.SelectMovement(currentSelectedMovement, playerUnit.Pokemon.Moves[currentSelectedMovement]);
        }

        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;
            
            battleDialogBox.ToggleDialogText(true);
            battleDialogBox.ToggleActions(false);
            battleDialogBox.ToggleMovements(false);

            StartCoroutine(PerformPlayerMovement());
        }

        if (Input.GetAxisRaw("Cancel") != 0)
        {
            // Volvemos a la pantalla de selección de acción
            timeSinceLastClick = 0;

            PlayerActionSelection();
        }
    }

    private void HandlePlayerPartySelection()
    {
        if (timeSinceLastClick < timeBetweenClicks)
        {
            return;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPokemon -= (int) Input.GetAxisRaw("Vertical") * 2;

        } else if (Input.GetAxisRaw("Horizontal") != 0)
        {
            timeSinceLastClick = 0;
            currentSelectedPokemon += (int) Input.GetAxisRaw("Horizontal");
        }

        currentSelectedPokemon = Mathf.Clamp(currentSelectedPokemon, 0, playerParty.Pokemons.Count - 1);
        
        partyHud.UpdateSelectedPokemon(currentSelectedPokemon);

        if (Input.GetAxisRaw("Submit") != 0)
        {
            timeSinceLastClick = 0;

            Pokemon selectedPokeon = playerParty.Pokemons[currentSelectedPokemon];
            if (selectedPokeon.HP <= 0)
            {
                partyHud.SetMessage("No puedes seleccionar un pokemon debilitado");
                return;
            }
            else if (selectedPokeon == playerUnit.Pokemon)
            {
                partyHud.SetMessage("No puedes seleccionar un pokemon en batalla");
                return;
            }

            partyHud.gameObject.SetActive(false);
            battleState = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedPokeon));
        }

        if (Input.GetAxisRaw("Cancel") != 0)
        {
            // Volvemos a la pantalla de selección de acción
            timeSinceLastClick = 0;
            
            partyHud.gameObject.SetActive(false);
            PlayerActionSelection();
        }
    }
    
    #endregion
    
    #region CORRUTINAS
    
    /// <summary>
    /// Empieza la batalla
    /// </summary>
    public IEnumerator SetUpBattle()
    {
        battleState = BattleState.StartBattle;
        
        playerUnit.SetUpPokemon(playerParty.GetFirstNonWeakenedPokemon());
        
        battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);
        
        enemyUnit.SetUpPokemon(wildPokemon);

        partyHud.InitPartyHud();
        
        battleDialogBox.ToggleActions(true);
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(false);
        
        yield  return battleDialogBox.SetDialog($"Un {enemyUnit.Pokemon.Base.Name} salvaje apareció.");
        
        if (enemyUnit.Pokemon.Speed > playerUnit.Pokemon.Speed)
        {
            yield return battleDialogBox.SetDialog("El enemigo ataca primero.");
            yield return PerformEnemyMovement();
        }
        else
        {
            PlayerActionSelection();
        }
    }
    
    private IEnumerator PerformPlayerMovement()
    {
        battleState = BattleState.PerformMovement;
        
        Move move = playerUnit.Pokemon.Moves[currentSelectedMovement];
        if (move.PP <= 0)
        {
            PlayerMovementSelection();
            yield break;
        }

        yield return RunMovement(playerUnit, enemyUnit, move);

        // Solo podemos pasar al movimiento del enemigo si durante la ejecución del movimiento del 
        // jugador no hemos cambiado de estado
        if (battleState == BattleState.PerformMovement)
        {
            StartCoroutine(PerformEnemyMovement());
        }
    }
    
    private IEnumerator PerformEnemyMovement()
    {
        battleState = BattleState.PerformMovement;

        Move move = enemyUnit.Pokemon.RandomMovement();

        yield return RunMovement(enemyUnit, playerUnit, move);
        
        // Solo podemos pasar al movimiento del jugador si durante la ejecución del movimiento del 
        // enemigo no hemos cambiado de estado
        if (battleState == BattleState.PerformMovement)
        {
            PlayerActionSelection();
        }
    }

    private IEnumerator RunMovement(BattleUnit attacker, BattleUnit target, Move move)
    {
        move.PP--;
        yield return battleDialogBox.SetDialog($"{attacker.Pokemon.Base.Name} ha usado {move.Base.Name}.");
        
        int oldHPValue = target.Pokemon.HP;
        
        attacker.PlayAttackAnimation();
        yield return new WaitForSeconds(attackAnimationDuration);
        
        target.PlayReceiveAttackAnimation();
        DamageDescription damageDescription = target.Pokemon.ReceiveDamage(attacker.Pokemon, move);
        yield return target.HUD.UpdatePokemonData(oldHPValue);

        yield return ShowDamageDescription(damageDescription);
        
        if (damageDescription.Weakened)
        {
            string additionalText = (target.IsPlayer ? "¡Oh no! " : "");
            yield return battleDialogBox.SetDialog($"{additionalText}{target.Pokemon.Base.Name} ha sido debilitado.");
            target.PlayWeakenedAnimation();
            yield return new WaitForSeconds(weakenedAnimationDuration);
            
            CheckForBattleFinish(target);
        }
    }

    private IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return battleDialogBox.SetDialog($"¡Vuelve, {playerUnit.Pokemon.Base.Name}!");
            playerUnit.PlayWeakenedAnimation();
            yield return new WaitForSeconds(weakenedAnimationDuration);
        }
        
        playerUnit.SetUpPokemon(newPokemon);
        battleDialogBox.SetPokemonMovements(newPokemon.Moves);
        
        yield return battleDialogBox.SetDialog($"¡Adelante, {playerUnit.Pokemon.Base.Name}!");
        StartCoroutine(PerformEnemyMovement());
    }

    private IEnumerator ShowDamageDescription(DamageDescription damageDescription)
    {
        if (damageDescription.Critical > 1)
        {
            yield return battleDialogBox.SetDialog("¡Golpe crítico!");
        }

        if (damageDescription.Type > 1)
        {
            yield return battleDialogBox.SetDialog("¡Es súper efectivo!");
            
        }else if (damageDescription.Type < 1)
        {
            yield return battleDialogBox.SetDialog("No es muy efectivo...");
        }
    }

    private IEnumerator ThrowPokeball()
    {
        battleState = BattleState.Busy;
        
        battleDialogBox.ToggleActions(true);
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(false);

        yield return battleDialogBox.SetDialog($"¡Has lanzado una {pokeball.name}!");

        GameObject pokeballInst = Instantiate(pokeball, playerUnit.transform.position + pokeballInitialOffset, Quaternion.identity);
        SpriteRenderer pokeballSprite = pokeballInst.GetComponent<SpriteRenderer>();

        yield return pokeballSprite.transform.DOLocalJump(enemyUnit.transform.position + pokeballFinalOffset, 
            jumpPower, numberJumps, pokeballThrowAnimationDuration).WaitForCompletion();

        yield return enemyUnit.PlayCaputredAnimation();
        yield return pokeballSprite.transform
            .DOLocalMoveY(enemyUnit.transform.position.y - pokeballToGroundDistance, 
                pokeballFallAnimationDuration).WaitForCompletion();

        numberOfShakes = TryToCatchPokemon(enemyUnit.Pokemon);
        for (int i = 0; i < Mathf.Min(numberOfShakes, 3); i++)
        {
            yield return new WaitForSeconds(timeBetweenPokeballShakes);
            yield return pokeballSprite.transform.DOPunchRotation(new Vector3(0, 0, shakeDegrees), pokeballShakeDuration).WaitForCompletion();
        }

        if (numberOfShakes == 4)
        {
            yield return battleDialogBox.SetDialog($"¡{enemyUnit.Pokemon.Base.Name} capturado!");
            yield return pokeballSprite.DOColor(Color.gray, pokemonCapturedDuration).WaitForCompletion();
            
            Destroy(pokeballInst);
            BattleFinish(true);
        }
        else
        {
            yield return new WaitForSeconds(timeBetweenPokeballShakes);
            pokeballSprite.DOFade(0,pokeballFadeDuration);
            yield return enemyUnit.PlayEscapeFromPokeballAnimation();

            if (numberOfShakes < 2)
            {
                yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} ha escapado...");
            }
            else
            {
                yield return battleDialogBox.SetDialog("¡Casi lo has atrapado!");
            }
            Destroy(pokeballInst);
            battleState = BattleState.LoseTurn;
        }
    }
    
    
    #endregion
    
    #endregion
}
