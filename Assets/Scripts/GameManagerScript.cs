using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
        [Header("Game UI")]
        [SerializeField] private TMP_Text turretBlueCounter;
        [SerializeField] private TMP_Text turretRedCounter;
        [SerializeField] private TMP_Text timeCounter;
        [SerializeField] private TMP_Text resultText;
        
        [Header("Cameras")]
        [SerializeField] private Camera playerBlueCamera;
        [SerializeField] private Camera playerRedCamera;

        [Header("Prefabs")]
        [SerializeField] private GameObject groundPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private List<GameObject> turretPrefabs;
        [SerializeField] private GameObject obstaclePrefab;

        private Dictionary<Team, int> _turretCount;

        private readonly List<GameObject> _gameObjects = new List<GameObject>();
        private static GameManagerScript _instance;

        private GameSettings _gameSettings;
        private ScreenManager _screenManager;
        private float _startTime;

        public static int GetTurretsForTeam(Team team)
        {
                return GameManagerScript._instance._turretCount.TryGetValue(team, out var value) ? value : 0;
        }

        private void Start()
        {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                this._screenManager = this.GetComponent<ScreenManager>();
                this._gameSettings = this.GetComponent<GameSettings>();

                this._turretCount = new Dictionary<Team, int>
                {
                        { Team.Blue, 0 },
                        { Team.Neutral, this._gameSettings.GetTurrets() },
                        { Team.Red, 0 }
                };

                GameManagerScript._instance = this;
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

                var gameLength = this._gameSettings.GetGameLength();
                var turrets = this._gameSettings.GetTurrets();
                
                this.turretBlueCounter.text = $"BLUE TURRETS\n{this._turretCount[Team.Blue]}";
                this.turretRedCounter.text = $"RED TURRETS\n{this._turretCount[Team.Red]}";
                this.timeCounter.text = $"REMAINING TIME\n{Math.Ceiling(gameLength - (Time.time - this._startTime))}";

                if (this._turretCount[Team.Blue] < turrets && this._turretCount[Team.Red] < turrets && !(Time.time - this._startTime >= gameLength)) return;
                Time.timeScale = 0;
                
                if (this._turretCount[Team.Blue] > this._turretCount[Team.Red]) this.resultText.text = "PLAYER BLUE WON!";
                else if (this._turretCount[Team.Red] > this._turretCount[Team.Blue]) this.resultText.text = "PLAYER RED WON!";
                else this.resultText.text = "TIE!";
                
                this._screenManager.EnableScreen(GameScreen.GameOver);
                
                Cursor.lockState = CursorLockMode.None;
        }

        public static void AddTurretForTeam(Team oldTeam, Team newTeam)
        {
                GameManagerScript._instance._turretCount[oldTeam]--;
                GameManagerScript._instance._turretCount[newTeam]++;
        }

        private void SpawnArena(int gridSize, int gridNodeDistance)
        {
                var extendedSize = gridSize + 2;
                var center = extendedSize / 2;
                var physicalSize = (extendedSize - 1) * gridNodeDistance + 2F;
                var groundPosition = new Vector3(0, -0.5F, 0);
                var ground = Instantiate(this.groundPrefab, groundPosition, new Quaternion());
                ground.transform.localScale = new Vector3(physicalSize, 1F, physicalSize);
                this._gameObjects.Add(ground);

                var positionX = -center * gridNodeDistance;
                var positionZ = 0F;
                var obstaclePosition = new Vector3(positionX, 1F, positionZ);
                var obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(1F, 2F, physicalSize);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);
                this._gameObjects.Add(obstacle);

                positionX = (extendedSize - 1 - center) * gridNodeDistance;
                obstaclePosition = new Vector3(positionX, 1F, positionZ);
                obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(1F, 2F, physicalSize);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);
                this._gameObjects.Add(obstacle);

                positionX = 0;
                positionZ = center * gridNodeDistance;
                obstaclePosition = new Vector3(positionX, 1F, positionZ);
                obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(physicalSize, 2F, 1F);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);
                this._gameObjects.Add(obstacle);

                positionZ = (center - extendedSize + 1) * gridNodeDistance;
                obstaclePosition = new Vector3(positionX, 1F, positionZ);
                obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(physicalSize, 2F, 1F);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);
                this._gameObjects.Add(obstacle);
        }

        private void SpawnPlayers(int gridSize, int gridNodeDistance)
        {
                var center = gridSize / 2;
                
                // var playerBlueRow = 0;
                // var playerBlueColumn = 0;
                var playerRedRow = gridSize - 1;
                var playerRedColumn = gridSize - 1;

                var playerBlueXPosition = -center * gridNodeDistance;
                var playerBlueZPosition = center * gridNodeDistance;
                var playerRedXPosition = (playerRedColumn - center) * gridNodeDistance;
                var playerRedZPosition = (center - playerRedRow) * gridNodeDistance;

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
        
        private void SpawnTurrets(int gridSize, int gridNodeDistance, int turrets)
        {
                var size = gridSize;
                var grid = new bool[size, size];
                grid[0, 0] = true;
                grid[size - 1, size - 1] = true;
                
                for (var index = 0; index < turrets; index++)
                {
                        int row, column;
                        do
                        {
                                row = UnityEngine.Random.Range(0, size);
                                column = UnityEngine.Random.Range(0, size);
                        } while (grid[row, column]);
                        grid[row, column] = true;

                        var center = size / 2;
                        var xPosition = (column - center) * gridNodeDistance;
                        var zPosition = (center - row) * gridNodeDistance;
                        var position = new Vector3(xPosition, 0.5F, zPosition);
                        var turretPrefab = this.turretPrefabs[UnityEngine.Random.Range(0, this.turretPrefabs.Count)];
                        var turret = Instantiate(turretPrefab, position, new Quaternion());
                        turret.GetComponent<TurretScript>().SetCamera(this.playerBlueCamera, this.playerRedCamera);
                        this._gameObjects.Add(turret);
                }
        }

        private void SpawnObstacles(int gridSize, int gridNodeDistance, int obstacles)
        {
                var center = gridSize / 2;
                var rowObstaclesGrid = new bool[gridSize - 1, gridSize];
                var columnObstaclesGrid = new bool[gridSize, gridSize - 1];
                
                for (var index = 0; index < obstacles; index++)
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
                                        row = UnityEngine.Random.Range(0, gridSize - 1);
                                        column = UnityEngine.Random.Range(0, gridSize);
                                } while (rowObstaclesGrid[row, column]);
                                rowObstaclesGrid[row, column] = true;

                                positionX = (column - center) * gridNodeDistance;
                                positionZ = (center - row - 0.5F) * gridNodeDistance;
                                scale = new Vector3(2F, 2F, 1F);
                        }

                        // spawning column obstacle
                        else
                        {
                                do
                                {
                                        row = UnityEngine.Random.Range(0, gridSize);
                                        column = UnityEngine.Random.Range(0, gridSize - 1);
                                } while (columnObstaclesGrid[row, column]);
                                columnObstaclesGrid[row, column] = true;

                                positionX = (column - center + 0.5F) * gridNodeDistance;
                                positionZ = (center - row) * gridNodeDistance;
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
                
                this._gameSettings.Validate();
                var gridSize = this._gameSettings.GetGridSize();
                var gridNodeDistance = this._gameSettings.GetGridNodeDistance();
                var turrets = this._gameSettings.GetTurrets();
                var obstacles = this._gameSettings.GetObstacles();

                this.SpawnArena(gridSize, gridNodeDistance);
                this.SpawnPlayers(gridSize, gridNodeDistance);
                this.SpawnTurrets(gridSize, gridNodeDistance, turrets);
                this.SpawnObstacles(gridSize, gridNodeDistance, obstacles);

                this._turretCount[Team.Blue] = 0;
                this._turretCount[Team.Neutral] = turrets;
                this._turretCount[Team.Red] = 0;
                
                this.turretBlueCounter.text = $"BLUE TURRETS\n{this._turretCount[Team.Blue]}";
                this.turretRedCounter.text = $"RED TURRETS\n{this._turretCount[Team.Red]}";

                this._screenManager.EnableScreen(GameScreen.Game);
                
                this._startTime = Time.time;
        }

        public void GoalsButtonClicked()
        {
                this._screenManager.EnableScreen(GameScreen.Goals);
        }

        public void ControlsButtonClicked()
        {
                this._screenManager.EnableScreen(GameScreen.Controls);
        }

        public void SettingsButtonClicked()
        {
                this._screenManager.EnableScreen(GameScreen.Settings);
        }

        public void CreditsButtonClicked()
        {
                this._screenManager.EnableScreen(GameScreen.Credits);
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
                this._screenManager.EnableScreen(GameScreen.Menu);
        }

        #endregion
}