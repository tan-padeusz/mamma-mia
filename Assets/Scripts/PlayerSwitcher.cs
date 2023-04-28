using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject playerFirst;
    [SerializeField] private GameObject playerSecond;

    private PlayerMovement _playerFirstScript;
    private PlayerMovement _playerSecondScript;
    private bool _switchable = true;

    private void Start()
    {
        this._playerFirstScript = this.playerFirst.GetComponent<PlayerMovement>();
        this._playerSecondScript = this.playerSecond.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)) && this._switchable)
            this.StartCoroutine(this.SwitchPlayer(this.GetCurrentPlayer(), this.GetTargetPlayer()));
    }

    private GameObject GetCurrentPlayer()
    {
        return this._playerFirstScript.enabled ? this.playerFirst : this.playerSecond;
    }

    private GameObject GetTargetPlayer()
    {
        return this._playerFirstScript.enabled ? this.playerSecond : this.playerFirst;
    }

    private IEnumerator SwitchPlayer(GameObject currentPlayer, GameObject newPlayer)
    {
        this._switchable = false;
        var movementScript = currentPlayer.GetComponent<PlayerMovement>();
        if (movementScript != null) movementScript.enabled = false;
        movementScript = newPlayer.GetComponent<PlayerMovement>();
        if (movementScript != null) movementScript.enabled = true;
        movementScript.ResetCamera();
        yield return new WaitForSeconds(0.3F);
        this._switchable = true;
    }
}
