using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHUD;
    
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox battleDialogBox;

    private void Start()
    {
        SetUpBattle();
    }

    public void SetUpBattle()
    {
        playerUnit.SetUpPokemon();
        playerHUD.SetPokemon(playerUnit.Pokemon);
        enemyUnit.SetUpPokemon();
        enemyHUD.SetPokemon(enemyUnit.Pokemon);
        
        StartCoroutine(battleDialogBox.SetDialog($"Un {enemyUnit.Pokemon.Base.Name} salvaje apareció"));
    }
}
