using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BattleState
{
    StartBattle,
    ActionSelection,
    MovementSelection,
    PerformMovement,
    Busy,
    PartySelectScreen,
    ItemSelectScreen,
    ForgetMovement,
    LoseTurn,
    FinishBattle
}

public enum BattleType
{
    WildPokemon,
    Trainer,
    Leader
}

public class BattleManager : MonoBehaviour
{
    #region VARIABLES PUBLICAS
    
    public BattleState battleState;
    public BattleType battleType;
    public event Action<bool> OnBattleFinished;

    public bool isLeader;

    public AudioClip attackClip, damageClip, levelUpClip, battleFinishClip;

    #endregion
    
    #region VARIABLES PRIVADAS
    
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogBox battleDialogBox;

    private int currentSelectedAction;
    private int currentSelectedMovement;
    private int currentSelectedPokemon;
    private int escapeAttempts;
    
    private float timeSinceLastClick;
    private float timeBetweenClicks = 0.5f;
    private float timeAfterText = 1;
    private float timeAfterLevelUp = 1f;

    private PokemonParty playerParty;
    private Pokemon wildPokemon;

    [SerializeField] private PartyHUD partyHud;

    [SerializeField] private MovementSelectionUI movementSelectionUI;

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

    private MoveBase moveToLearn;
    
    #endregion
    
    #region METODOS

    public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        battleType = BattleType.WildPokemon;
        escapeAttempts = 0;
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetUpBattle());
    }

    public void HandleStartTrainerBattle()
    {
        battleType = (isLeader ? BattleType.Leader : BattleType.Trainer);
        // TODO: Implemenetar la lógica
    }

    public void HandleUpdate()
    {
        timeSinceLastClick += Time.deltaTime;
        
        if (timeSinceLastClick < timeBetweenClicks || battleDialogBox.isWriting)
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
        }else if (battleState == BattleState.ForgetMovement)
        {
            movementSelectionUI.HandleForgetMovementSelection((forgetMoveIndex) =>
            {
                if (forgetMoveIndex < 0)
                {
                    timeSinceLastClick = 0;
                    return;
                }
                
                StartCoroutine(ForgetOldMovement(forgetMoveIndex));
            });
        }
    }

    private void BattleFinish(bool playerHasWon)
    {
        AudioManager.SharedInstance.PlaySound(battleFinishClip);
        battleState = BattleState.FinishBattle;
        playerParty.Pokemons.ForEach(p => p.OnBattleFinish());
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
    private void OpenInventoryScreen()
    {
        // TODO: Implementar inventario y lógica de items

        battleDialogBox.ToggleDialogText(true);
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(false);
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
                OpenInventoryScreen();

            }else if (currentSelectedAction == 3)
            {
                // Seleccionamos Huir
                StartCoroutine(TryToEscapeFromBattle());
            }
        }
    }
    
    private void HandlePlayerMovementSelection()
    {
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            timeSinceLastClick = 0;
            
            // Tenemos en cuenta 4 movimientos
            int oldSelectedMovement = currentSelectedMovement;
            currentSelectedMovement = (currentSelectedMovement + 2) % PokemonBase.NUMBER_OF_LEARNABLE_MOVES;

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
        yield return ChooseFirstTurn(true);
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
        // Comprobamos estados alterados que impidan atacar en este turno (parálisis, dormido, congelado...)
        bool canRunMovement = attacker.Pokemon.OnStartTurn();
        if (!canRunMovement)
        {
            yield return ShowStatsMessages(attacker.Pokemon);
            yield break;
        }
        
        // Por si ha quedado algún mensaje encolado
        yield return ShowStatsMessages(attacker.Pokemon);
        
        move.PP--;
        yield return battleDialogBox.SetDialog($"{attacker.Pokemon.Base.Name} ha usado {move.Base.Name}.");

        yield return RunMoveAnims(attacker, target);

        if (move.Base.MoveType == MoveType.Stats)
        {
            yield return RunMoveStats(attacker.Pokemon, target.Pokemon, move);
        }
        else
        {
            DamageDescription damageDescription = target.Pokemon.ReceiveDamage(attacker.Pokemon, move);
            yield return target.HUD.UpdatePokemonData();

            yield return ShowDamageDescription(damageDescription); 
        }
        
        if (target.Pokemon.HP <= 0)
        {
            yield return HandlePokemonWeakened(target);
        }
        
        // Acaba el turno del atacante (attacker)
        // Comprobamos estados alterados (quemadura, envenenamiento...)
        attacker.Pokemon.OnFinishTurn();
        yield return ShowStatsMessages(attacker.Pokemon);
        yield return attacker.HUD.UpdatePokemonData();
        
        if (attacker.Pokemon.HP <= 0)
        {
            yield return HandlePokemonWeakened(attacker);
        }
    }

    private IEnumerator ShowStatsMessages(Pokemon pokemon)
    {
        while (pokemon.StatusChangeMessages.Count > 0)
        { 
            string message = pokemon.StatusChangeMessages.Dequeue();
            yield return battleDialogBox.SetDialog(message);
            yield return new WaitForSeconds(timeAfterText);
        }
    }

    private IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        bool currentPokemonWeakened = true;
        if (playerUnit.Pokemon.HP > 0)
        {
            currentPokemonWeakened = false;
            yield return battleDialogBox.SetDialog($"¡Vuelve, {playerUnit.Pokemon.Base.Name}!");
            playerUnit.PlayWeakenedAnimation();
            yield return new WaitForSeconds(weakenedAnimationDuration);
        }
        
        playerUnit.SetUpPokemon(newPokemon);
        battleDialogBox.SetPokemonMovements(newPokemon.Moves);
        
        yield return battleDialogBox.SetDialog($"¡Adelante, {playerUnit.Pokemon.Base.Name}!");

        if (currentPokemonWeakened)
        {
            yield return ChooseFirstTurn();
        }
        else
        {
            yield return PerformEnemyMovement();
        }
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

        if (battleType != BattleType.WildPokemon)
        {
            yield return battleDialogBox.SetDialog("No puedes capturar los Pokemon de otros entrenadores");
            battleState = BattleState.LoseTurn;
            yield break;
        }
        
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
            yield return pokeballSprite.DOColor(Color.gray, pokemonCapturedDuration).WaitForCompletion();
            yield return battleDialogBox.SetDialog($"¡{enemyUnit.Pokemon.Base.Name} capturado!");
            yield return pokeballSprite.DOFade(0, pokemonCapturedDuration).WaitForCompletion();
            
            if (playerParty.AddPokemonToParty(enemyUnit.Pokemon))
            { 
                yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.Name} se ha añadido a tu equipo.");
            }
            else
            {
                yield return battleDialogBox.SetDialog($"En algún momento {enemyUnit.Pokemon.Base.Name} se añadirá al PC.");
            }

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
    
    private IEnumerator TryToEscapeFromBattle()
    {
        battleState = BattleState.Busy;

        if (battleType != BattleType.WildPokemon)
        {
            yield return battleDialogBox.SetDialog("No puedes huir de combates contra entrenadores Pokemon");
            battleState = BattleState.LoseTurn;
            yield break;
        }
        
        // Estamos en una batalla contra Pokemon salvaje
        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;

        if (playerSpeed >= enemySpeed)
        {
            yield return battleDialogBox.SetDialog("¡Has escapado con éxito!");
            yield return new WaitForSeconds(timeAfterText);
            OnBattleFinished(true);
        }
        else
        {
            int oddsEscape = (Mathf.FloorToInt(playerSpeed * 128 / enemySpeed) + 30 * escapeAttempts) % 256;
            if (Random.Range(0, 256) < oddsEscape)
            {
                yield return battleDialogBox.SetDialog("¡Has escapado con éxito!");
                yield return new WaitForSeconds(timeAfterText);
                OnBattleFinished(true);
            }
            else
            {
                yield return battleDialogBox.SetDialog("No puedes escapar...");
                battleState = BattleState.LoseTurn;
            }
        }
    }

    private IEnumerator HandlePokemonWeakened(BattleUnit weakendedUnit)
    {
        string additionalText = (weakendedUnit.IsPlayer ? "¡Oh no! " : "");
        yield return battleDialogBox.SetDialog($"{additionalText}{weakendedUnit.Pokemon.Base.Name} ha sido debilitado.");
        weakendedUnit.PlayWeakenedAnimation();
        yield return new WaitForSeconds(weakenedAnimationDuration);

        if (!weakendedUnit.IsPlayer)
        {
            // Ganar experiencia
            int expBase = weakendedUnit.Pokemon.Base.ExpBase;
            int level = weakendedUnit.Pokemon.Level;
            float multiplier = (battleType == BattleType.WildPokemon ? 1 : 1.5f);

            int wonExpInBattle = Mathf.FloorToInt(expBase * level * multiplier / 7);
            playerUnit.Pokemon.Experience += wonExpInBattle;
            yield return battleDialogBox.SetDialog(
                $"{playerUnit.Pokemon.Base.Name} ha ganado {wonExpInBattle} puntos de experiencia.");
            yield return playerUnit.HUD.SetExpBarSmooth();
            yield return new WaitForSeconds(timeAfterText);
            
            // Checkear si hay que subir de nivel
            while (playerUnit.Pokemon.NeedsToLevelUp())
            {
                AudioManager.SharedInstance.PlaySound(levelUpClip);
                playerUnit.HUD.SetLevelText();
                playerUnit.Pokemon.HasHPChanged = true;
                yield return playerUnit.HUD.UpdatePokemonData();
                yield return new WaitForSeconds(timeAfterLevelUp);
                yield return battleDialogBox.SetDialog($"¡{playerUnit.Pokemon.Base.Name} sube de nivel!");
                
                
                // Comprobamos si el Pokemon debe aprender nuevo movimiento
                LearnableMove newLearnableMove = playerUnit.Pokemon.GetLearnableMoveAtCurrentLevel();

                if (newLearnableMove != null)
                {
                    if (playerUnit.Pokemon.Moves.Count < PokemonBase.NUMBER_OF_LEARNABLE_MOVES)
                    {
                        // Podemos aprender el movimiento
                        playerUnit.Pokemon.LearnMove(newLearnableMove);
                        yield return battleDialogBox.SetDialog(
                            $"{playerUnit.Pokemon.Base.Name} ha aprendido {newLearnableMove.Move.Name}.");
                        battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);
                    }
                    else
                    {
                        yield return battleDialogBox.SetDialog(
                            $"{playerUnit.Pokemon.Base.Name} intenta aprender {newLearnableMove.Move.Name}.");
                        yield return battleDialogBox.SetDialog(
                            $"Pero no puede aprender más de {PokemonBase.NUMBER_OF_LEARNABLE_MOVES} movimientos.");
                        yield return ChooseMovementToForget(playerUnit.Pokemon, newLearnableMove.Move);
                        yield return new WaitUntil(() => battleState != BattleState.ForgetMovement); // Esto es un delegado
                    }
                }
                
                yield return playerUnit.HUD.SetExpBarSmooth(true);
            }
            yield return new WaitForSeconds(weakenedAnimationDuration);
        }
        
        CheckForBattleFinish(weakendedUnit);
    }

    private IEnumerator ChooseMovementToForget(Pokemon learner, MoveBase newMove)
    {
        battleState = BattleState.Busy;
        
        yield return battleDialogBox.SetDialog("Selecciona el movimiento que quieres olvidar.");
        movementSelectionUI.gameObject.SetActive(true);
        movementSelectionUI.SetMovements(learner.Moves.Select(x => x.Base).ToList(), newMove);

        moveToLearn = newMove;
        
        battleState = BattleState.ForgetMovement;
    }

    private IEnumerator ForgetOldMovement(int forgetMoveIndex)
    {
        movementSelectionUI.gameObject.SetActive(false);
        if (forgetMoveIndex == PokemonBase.NUMBER_OF_LEARNABLE_MOVES)
        {
            // Olvido el nuevo movimiento
            yield return battleDialogBox.SetDialog(
                    $"{playerUnit.Pokemon.Base.Name} no ha aprendido {moveToLearn.Name}.");
        }
        else
        {
            // Olvido el seleccionado y aprendo el nuevo
            MoveBase selectedMove = playerUnit.Pokemon.Moves[forgetMoveIndex].Base;
            yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} olvidó {selectedMove.Name} y aprendió {moveToLearn.Name}.");
            playerUnit.Pokemon.Moves[forgetMoveIndex] = new Move(moveToLearn);
        }
        
        yield return new WaitForSeconds(timeAfterText);
        moveToLearn = null;
        
        // TODO: Revisar más adelante cuando haya entrenadores
        
        battleState = BattleState.FinishBattle;
    }
    
    private IEnumerator ChooseFirstTurn(bool showFirstMessage = false)
    {
        if (enemyUnit.Pokemon.Speed > playerUnit.Pokemon.Speed)
        {
            // Enemigo se mueve primero
            if (showFirstMessage)
            {
                yield return battleDialogBox.SetDialog("El enemigo ataca primero.");
            }
            yield return PerformEnemyMovement();
        }
        else
        {
            // Player se mueve primero
            PlayerActionSelection();
        }
    }

    private IEnumerator RunMoveStats(Pokemon attacker, Pokemon target, Move move)
    {
        // Movimiento de tipo Cambio de Estado
        // Stats Boosting
        foreach (StatBoosting boost in move.Base.Effects.Boostings)
        {
            if (boost.target == MoveTarget.Self)
            {
                // Attacker recibe el Boosting
                attacker.ApplyBoost(boost);
            }
            else
            {
                // Target recibe el Boosting
                target.ApplyBoost(boost);
            }
        }
        
        // Status Condition
        if (move.Base.Effects.Status != StatusConditionID.None)
        {
            // Aplicamos el estado alterado
            if (move.Base.Target == MoveTarget.Other)
            {
                target.SetStatusCondition(move.Base.Effects.Status);
            }
            else
            {
                attacker.SetStatusCondition(move.Base.Effects.Status);
            }
        }

        yield return ShowStatsMessages(attacker);
        yield return ShowStatsMessages(target);
    }

    private IEnumerator RunMoveAnims(BattleUnit attacker, BattleUnit target)
    {
        AudioManager.SharedInstance.PlaySound(attackClip);
        attacker.PlayAttackAnimation();
        yield return new WaitForSeconds(attackAnimationDuration);
        AudioManager.SharedInstance.PlaySound(damageClip);
        target.PlayReceiveAttackAnimation();
        yield return new WaitForSeconds(attackAnimationDuration);
    }
    
    #endregion
    
    #endregion
}
