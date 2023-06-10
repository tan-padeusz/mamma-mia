using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
        [Header("Canvases")]
        [SerializeField] private Canvas menuCanvas;
        [SerializeField] private Canvas gameCanvas;
        [SerializeField] private Canvas gameOverCanvas;
        [SerializeField] private Canvas goalsCanvas;
        [SerializeField] private Canvas controlsCanvas;
        [SerializeField] private Canvas settingsCanvas;
        [SerializeField] private Canvas creditsCanvas;
        
        [Header("Settings")]
        [SerializeField] private TMP_InputField lengthInput;
        [SerializeField] private TMP_InputField gridSizeInput;
        [SerializeField] private TMP_InputField gridNodeDistanceInput;
        [SerializeField] private TMP_InputField turretsInput;
        [SerializeField] private TMP_InputField obstaclesInput;

        [Header("Game UI")]
        [SerializeField] private TMP_Text turretBlueCounter;
        [SerializeField] private TMP_Text turretRedCounter;
        [SerializeField] private TMP_Text timeCounter;
        [SerializeField] private TMP_Text resultText;
        
        [Header("Cameras")]
        [SerializeField] private Camera playerBlueCamera;
        [SerializeField] private Camera playerRedCamera;
        
        [Header("Game")]
        [SerializeField] private int length = 60;
        private float _startTime;
        [SerializeField] private int gridSize = 5;
        [SerializeField] private int gridNodeDistance = 7;
        [SerializeField] private int turrets = 9;
        [SerializeField] private int obstacles = 15;

        [Header("Prefabs")]
        [SerializeField] private GameObject groundPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private List<GameObject> turretPrefabs;
        [SerializeField] private GameObject obstaclePrefab;

        private int _blueTurretCount;
        private int _redTurretCount;

        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        private static GameManagerScript _instance;

        public static int GetTurretsForTeam(Team team)
        {
                var turretCount = team switch
                {
                        Team.Blue => GameManagerScript._instance._blueTurretCount,
                        Team.Red => GameManagerScript._instance._redTurretCount,
                        _ => 0
                };
                return Math.Max(turretCount, 1);
        }

        private void EnableCanvas(GameCanvas canvas)
        {
                this.menuCanvas.enabled = canvas == GameCanvas.Menu;
                this.gameCanvas.enabled = canvas == GameCanvas.Game;
                this.gameOverCanvas.enabled = canvas == GameCanvas.GameOver;
                this.goalsCanvas.enabled = canvas == GameCanvas.Goals;
                this.controlsCanvas.enabled = canvas == GameCanvas.Controls;
                this.settingsCanvas.enabled = canvas == GameCanvas.Settings;
                this.creditsCanvas.enabled = canvas == GameCanvas.Credits;
        }

        private void Start()
        {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                this.EnableCanvas(GameCanvas.Menu);

                this.lengthInput.text = $"{this.length}";
                this.gridSizeInput.text = $"{this.gridSize}";
                this.gridNodeDistanceInput.text = $"{this.gridNodeDistance}";
                this.turretsInput.text = $"{this.turrets}";
                this.obstaclesInput.text = $"{this.obstacles}";

                GameManagerScript._instance = this;

                // Cursor.lockState = CursorLockMode.Confined;
                // Time.timeScale = 1;
                //
                // if (this.gridSize % 2 == 0) this.gridSize++;
                // this.gridSize = Math.Clamp(this.gridSize, 5, 11);
                // this.gridNodeDistance = Math.Clamp(this.gridNodeDistance, 4F, 10F);
                // if (this.turrets % 2 == 0) this.turrets--;
                // this.turrets = Math.Clamp(this.turrets, 11, this.gridSize * this.gridSize - 2);
                // var maxObstacles = 2 * this.gridSize * (this.gridSize - 1);
                // this.obstacles = Math.Clamp(this.obstacles, maxObstacles / 4, maxObstacles);
                //
                // this.SpawnArena();
                // this.SpawnPlayers();
                // this.SpawnTurrets();
                // this.SpawnObstacles();
                //
                // this.turretBlueCounter.text = "blue turrets : 0";
                // this.turretRedCounter.text = "red turrets : 0";
                //
                // GameManagerScript.Instance = this;
                //
                // this._startTime = Time.time;
        }

        private void Update()
        {
                if (Math.Abs(Time.timeScale - 1.0F) > 0.001F) return;

                if (Input.GetKey(KeyCode.Backspace))
                {
                        Time.timeScale = 0;
                        Cursor.lockState = CursorLockMode.None;
                        this.BackButtonClicked();
                }
                
                this.turretBlueCounter.text = $"BLUE TURRETS\n{this._blueTurretCount}";
                this.turretRedCounter.text = $"RED TURRETS\n{this._redTurretCount}";
                this.timeCounter.text = $"REMAINING TIME\n{Math.Ceiling(this.length - (Time.time - this._startTime))}";

                if (this._blueTurretCount < this.turrets && this._redTurretCount < this.turrets && !(Time.time - this._startTime >= this.length)) return;
                Time.timeScale = 0;
                
                if (this._blueTurretCount > this._redTurretCount) this.resultText.text = "PLAYER BLUE WON!";
                else if (this._redTurretCount > this._blueTurretCount) this.resultText.text = "PLAYER RED WON!";
                else this.resultText.text = "TIE!";
                
                this.EnableCanvas(GameCanvas.GameOver);
                
                Cursor.lockState = CursorLockMode.None;
        }

        public static void AddTurretForTeam(Team oldTeam, Team newTeam)
        {
                switch (oldTeam)
                {
                        case Team.Blue:
                                GameManagerScript._instance._blueTurretCount--;
                                break;
                        case Team.Red:
                                GameManagerScript._instance._redTurretCount--;
                                break;
                        case Team.Neutral:
                                break;
                        default:
                                throw new ArgumentOutOfRangeException(nameof(oldTeam), oldTeam, null);
                }

                switch (newTeam)
                {
                        case Team.Blue:
                                GameManagerScript._instance._blueTurretCount++;
                                break;
                        case Team.Red:
                                GameManagerScript._instance._redTurretCount++;
                                break;
                        case Team.Neutral:
                                break;
                        default:
                                throw new ArgumentOutOfRangeException(nameof(newTeam), newTeam, null);
                }
        }

        private void SpawnArena()
        {
                var extendedSize = this.gridSize + 2;
                var center = extendedSize / 2;
                var physicalSize = (extendedSize - 1) * this.gridNodeDistance + 2F;
                var groundPosition = new Vector3(0, -0.5F, 0);
                var ground = Instantiate(this.groundPrefab, groundPosition, new Quaternion());
                ground.transform.localScale = new Vector3(physicalSize, 1F, physicalSize);
                this._gameObjects.Add(ground);

                var positionX = -center * this.gridNodeDistance;
                var positionZ = 0F;
                var obstaclePosition = new Vector3(positionX, 1F, positionZ);
                var obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(1F, 2F, physicalSize);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);
                this._gameObjects.Add(obstacle);

                positionX = (extendedSize - 1 - center) * this.gridNodeDistance;
                obstaclePosition = new Vector3(positionX, 1F, positionZ);
                obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(1F, 2F, physicalSize);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);
                this._gameObjects.Add(obstacle);

                positionX = 0;
                positionZ = center * this.gridNodeDistance;
                obstaclePosition = new Vector3(positionX, 1F, positionZ);
                obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(physicalSize, 2F, 1F);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);
                this._gameObjects.Add(obstacle);

                positionZ = (center - extendedSize + 1) * this.gridNodeDistance;
                obstaclePosition = new Vector3(positionX, 1F, positionZ);
                obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(physicalSize, 2F, 1F);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);
                this._gameObjects.Add(obstacle);
        }

        private void SpawnPlayers()
        {
                var center = this.gridSize / 2;
                
                // var playerBlueRow = 0;
                // var playerBlueColumn = 0;
                var playerRedRow = this.gridSize - 1;
                var playerRedColumn = this.gridSize - 1;

                var playerBlueXPosition = -center * this.gridNodeDistance;
                var playerBlueZPosition = center * this.gridNodeDistance;
                var playerRedXPosition = (playerRedColumn - center) * this.gridNodeDistance;
                var playerRedZPosition = (center - playerRedRow) * this.gridNodeDistance;

                var playerBluePosition = new Vector3(playerBlueXPosition, 0.5F, playerBlueZPosition);
                var playerRedPosition = new Vector3(playerRedXPosition, 0.5F, playerRedZPosition);

                var playerBlue = Instantiate(this.playerPrefab, playerBluePosition, new Quaternion());
                var playerBlueScript = playerBlue.GetComponent<PlayerScript>();
                if (playerBlueScript != null)
                {
                        playerBlueScript.SetGameManager(this);
                        playerBlueScript.ResetPlayer(Team.Blue, this.playerBlueCamera.transform);
                }
                this._gameObjects.Add(playerBlue);

                var playerRed = Instantiate(this.playerPrefab, playerRedPosition, new Quaternion());
                var playerRedScript = playerRed.GetComponent<PlayerScript>();
                if (playerRedScript != null)
                {
                        playerRedScript.SetGameManager(this);
                        playerRedScript.ResetPlayer(Team.Red, this.playerRedCamera.transform);
                }
                this._gameObjects.Add(playerRed);
        }
        
        private void SpawnTurrets()
        {
                var size = this.gridSize;
                var grid = new bool[size, size];
                grid[0, 0] = true;
                grid[size - 1, size - 1] = true;
                
                for (var index = 0; index < this.turrets; index++)
                {
                        int row, column;
                        do
                        {
                                row = UnityEngine.Random.Range(0, size);
                                column = UnityEngine.Random.Range(0, size);
                        } while (grid[row, column]);
                        grid[row, column] = true;

                        var center = size / 2;
                        var xPosition = (column - center) * this.gridNodeDistance;
                        var zPosition = (center - row) * this.gridNodeDistance;
                        var position = new Vector3(xPosition, 0.5F, zPosition);
                        var turretPrefab = this.turretPrefabs[UnityEngine.Random.Range(0, this.turretPrefabs.Count)];
                        var turret = Instantiate(turretPrefab, position, new Quaternion());
                        turret.GetComponent<TurretScript>().SetCamera(this.playerBlueCamera, this.playerRedCamera);
                        this._gameObjects.Add(turret);
                }
        }

        private void SpawnObstacles()
        {
                var size = this.gridSize;
                var center = size / 2;
                var rowObstaclesGrid = new bool[size - 1, size];
                var columnObstaclesGrid = new bool[size, size - 1];
                
                for (var index = 0; index < this.obstacles; index++)
                {
                        var rowObstaclesSpawned = rowObstaclesGrid.Cast<bool>().Count(element => element);
                        var columnObstaclesSpawned = columnObstaclesGrid.Cast<bool>().Count(element => element);
                        var random = UnityEngine.Random.Range(0, 2);
                        if (random == 0 && rowObstaclesSpawned == rowObstaclesGrid.Length) random = 1;
                        else if (columnObstaclesSpawned == columnObstaclesGrid.Length) random = 0;
                        
                        
                        int row, column;
                        float positionX, positionZ;
                        Vector3 scale;
                        
                        // spawning row obstacle
                        if (random == 0)
                        {
                                do
                                {
                                        row = UnityEngine.Random.Range(0, size - 1);
                                        column = UnityEngine.Random.Range(0, size);
                                } while (rowObstaclesGrid[row, column]);
                                rowObstaclesGrid[row, column] = true;

                                positionX = (column - center) * this.gridNodeDistance;
                                positionZ = (center - row - 0.5F) * this.gridNodeDistance;
                                scale = new Vector3(2F, 2F, 1F);
                        }

                        // spawning column obstacle
                        else
                        {
                                do
                                {
                                        row = UnityEngine.Random.Range(0, size);
                                        column = UnityEngine.Random.Range(0, size - 1);
                                } while (columnObstaclesGrid[row, column]);
                                columnObstaclesGrid[row, column] = true;

                                positionX = (column - center + 0.5F) * this.gridNodeDistance;
                                positionZ = (center - row) * this.gridNodeDistance;
                                scale = new Vector3(1F, 2F, 2F);
                        }

                        var position = new Vector3(positionX, 1F, positionZ);
                        var obstacle = Instantiate(this.obstaclePrefab, position, new Quaternion());
                        obstacle.transform.localScale = scale;
                        this._gameObjects.Add(obstacle);
                }
        }
        
        #region Button Events

        public void StartButtonClicked()
        {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Confined;
                
                if (this.gridSize % 2 == 0) this.gridSize++;
                this.gridSize = Math.Clamp(this.gridSize, 5, 11);
                this.gridNodeDistance = Math.Clamp(this.gridNodeDistance, 4, 10);
                if (this.turrets % 2 == 0) this.turrets--;
                this.turrets = Math.Clamp(this.turrets, 11, this.gridSize * this.gridSize - 2);
                var maxObstacles = 2 * this.gridSize * (this.gridSize - 1);
                this.obstacles = Math.Clamp(this.obstacles, maxObstacles / 4, maxObstacles);
                
                this.lengthInput.text = $"{this.length}";
                this.gridSizeInput.text = $"{this.gridSize}";
                this.gridNodeDistanceInput.text = $"{this.gridNodeDistance}";
                this.turretsInput.text = $"{this.turrets}";
                this.obstaclesInput.text = $"{this.obstacles}";

                this.SpawnArena();
                this.SpawnPlayers();
                this.SpawnTurrets();
                this.SpawnObstacles();

                this._blueTurretCount = 0;
                this.turretBlueCounter.text = $"BLUE TURRETS\n{this._blueTurretCount}";
                this._redTurretCount = 0;
                this.turretRedCounter.text = $"RED TURRETS\n{this._redTurretCount}";

                this.EnableCanvas(GameCanvas.Game);
                
                this._startTime = Time.time;
        }

        public void GoalsButtonClicked()
        {
                this.EnableCanvas(GameCanvas.Goals);
        }

        public void ControlsButtonClicked()
        {
                this.EnableCanvas(GameCanvas.Controls);
        }

        public void SettingsButtonClicked()
        {
                this.EnableCanvas(GameCanvas.Settings);
        }

        public void CreditsButtonClicked()
        {
                this.EnableCanvas(GameCanvas.Credits);
        }

        public void QuitButtonClicked()
        {
                Application.Quit();
        }

        public void BackButtonClicked()
        {
                foreach (var element in this._gameObjects)
                        Destroy(element);
                this._gameObjects.Clear();
                this.EnableCanvas(GameCanvas.Menu);
        }

        #endregion

        #region Settings Events

        public void OnLengthValueChanged(string value)
        {
                Debug.Log("length value changed");
                // 60 seconds is default value
                this.length = int.TryParse(value, out var numericValue) ? numericValue : 60;
        }

        public void OnGridSizeValueChanged(string value)
        {
                Debug.Log("grid size value changed");
                // size of 5 is default grid size value
                this.gridSize = int.TryParse(value, out var numericValue) ? numericValue : 5;
        }

        public void OnGridNodeDistanceValueChanged(string value)
        {
                Debug.Log("grid node distance value changed");
                // 7 units is default node distance value
                this.gridNodeDistance = int.TryParse(value, out var numericValue) ? numericValue : 7;
        }

        public void OnTurretsValueChanged(string value)
        {
                Debug.Log("turrets value changed");
                // 9 turrets is default value
                this.turrets = int.TryParse(value, out var numericValue) ? numericValue : 9;
        }

        public void OnObstaclesValueChanged(string value)
        {
                Debug.Log("obstacles value changed");
                // 15 obstacles is default value
                this.obstacles = int.TryParse(value, out var numericValue) ? numericValue : 15;
        }

        #endregion
}