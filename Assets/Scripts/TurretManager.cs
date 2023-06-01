using System;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    public static TurretManager Instance { get; private set; }

    private int _blueTurretCount = -1;
    private int _redTurretCount = -1;

    private void Start()
    {
        TurretManager.Instance = this;
    }

    private void Update()
    {
        if (this._blueTurretCount == 0) Debug.Log("Red team won!");
        if (this._redTurretCount == 0) Debug.Log("Blue Team won!");
    }

    public void ChangeTurretTeam(Team currentTurretTeam)
    {
        switch (currentTurretTeam)
        {
            case Team.Blue:
                this._blueTurretCount--;
                this._redTurretCount++;
                break;
            case Team.Red:
                this._blueTurretCount++;
                this._redTurretCount--;
                break;
            case Team.Neutral:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentTurretTeam), currentTurretTeam, null);
        }
    }

    public void SetTurretCount(int count)
    {
        this._blueTurretCount = count;
        this._redTurretCount = count;
    }
}
