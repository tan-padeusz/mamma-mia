using System;
using TMPro;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField gameLengthInput;
    [SerializeField] private TMP_InputField gridSizeInput;
    [SerializeField] private TMP_InputField gridNodeDistanceInput;
    [SerializeField] private TMP_InputField turretsInput;
    [SerializeField] private TMP_InputField obstaclesInput;

    [Header("Values")]
    [SerializeField] private int gameLength = 60;
    private int _defaultGameLength;
    [SerializeField] private int gridSize = 5;
    private int _defaultGridSize;
    [SerializeField] private int gridNodeDistance = 7;
    private int _defaultGridNodeDistance;
    [SerializeField] private int turrets = 9;
    private int _defaultTurrets;
    [SerializeField] private int obstacles = 15;
    private int _defaultObstacles;
    
    private void Start()
    {
        this.Validate();
        this._defaultGameLength = this.gameLength;
        this._defaultGridSize = this.gridSize;
        this._defaultGridNodeDistance = this.gridNodeDistance;
        this._defaultTurrets = this.turrets;
        this._defaultObstacles = this.obstacles;
    }

    public int GetGameLength()
    {
        return this.gameLength;
    }

    public int GetGridSize()
    {
        return this.gridSize;
    }

    public int GetGridNodeDistance()
    {
        return this.gridNodeDistance;
    }

    public int GetTurrets()
    {
        return this.turrets;
    }

    public int GetObstacles()
    {
        return this.obstacles;
    }

    public void Validate()
    {
        this.gameLength = Math.Clamp(this.gameLength, 30, 180);
        if (this.gridSize % 2 == 0) this.gridSize++;
        this.gridSize = Math.Clamp(this.gridSize, 5, 11);
        this.gridNodeDistance = Math.Clamp(this.gridNodeDistance, 4, 10);
        var maxTurrets = this.gridSize * this.gridSize - 2;
        this.turrets = Math.Clamp(this.turrets, maxTurrets / 4, maxTurrets);
        if (this.turrets % 2 == 0) this.turrets++;
        var maxObstacles = 2 * this.gridSize * (this.gridSize - 1);
        this.obstacles = Math.Clamp(this.obstacles, maxObstacles / 4, maxObstacles);
        
        this.gameLengthInput.text = $"{this.gameLength}";
        this.gridSizeInput.text = $"{this.gridSize}";
        this.gridNodeDistanceInput.text = $"{this.gridNodeDistance}";
        this.turretsInput.text = $"{this.turrets}";
        this.obstaclesInput.text = $"{this.obstacles}";
    }

    public void ApplyButtonClicked()
    {
        this.gameLength = int.TryParse(this.gameLengthInput.text, out var numericValue) ? numericValue : this._defaultGameLength;
        this.gridSize = int.TryParse(this.gridSizeInput.text, out numericValue) ? numericValue : this._defaultGridSize;
        this.gridNodeDistance = int.TryParse(this.gridNodeDistanceInput.text, out numericValue) ? numericValue : this._defaultGridNodeDistance;
        this.turrets = int.TryParse(this.turretsInput.text, out numericValue) ? numericValue : this._defaultTurrets;
        this.obstacles = int.TryParse(this.obstaclesInput.text, out numericValue) ? numericValue : this._defaultObstacles;
        this.Validate();
    }
}
