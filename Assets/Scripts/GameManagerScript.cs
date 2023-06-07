using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI turretBlueCounter;
        [SerializeField] private TextMeshProUGUI turretRedCounter;
        [SerializeField] private TextMeshProUGUI gameOverText;
        [SerializeField] private GameObject backToMenuPanel;
        [SerializeField] private TextMeshProUGUI timeCounter;
        
        [Header("Cameras")]
        [SerializeField] private Camera playerBlueCamera;
        [SerializeField] private Camera playerRedCamera;
        
        [Header("Game")]
        [SerializeField] private int length = 60;
        private float _startTime;
        [SerializeField] private int gridSize = 5;
        [SerializeField] private float gridNodeDistance = 7;
        [SerializeField] private int turrets = 7;
        [SerializeField] private int obstacles = 7;

        [Header("Prefabs")]
        [SerializeField] private GameObject groundPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private List<GameObject> turretPrefabs;
        [SerializeField] private GameObject obstaclePrefab;

        private int _blueTurretCount;
        private int _redTurretCount;
        
        private static GameManagerScript Instance { get; set; }

        public static int GetTurretsForTeam(Team team)
        {
                var turretCount = team switch
                {
                        Team.Blue => GameManagerScript.Instance._blueTurretCount,
                        Team.Red => GameManagerScript.Instance._redTurretCount,
                        _ => 0
                };
                return Math.Max(turretCount, 1);
        }

        private void Start()
        {
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 1;

                if (this.gridSize % 2 == 0) this.gridSize++;
                this.gridSize = Math.Clamp(this.gridSize, 5, 11);
                this.gridNodeDistance = Math.Clamp(this.gridNodeDistance, 4F, 10F);
                if (this.turrets % 2 == 0) this.turrets--;
                this.turrets = Math.Clamp(this.turrets, 11, this.gridSize * this.gridSize - 2);
                var maxObstacles = 2 * this.gridSize * (this.gridSize - 1);
                this.obstacles = Math.Clamp(this.obstacles, maxObstacles / 4, maxObstacles);

                this.SpawnArena();
                this.SpawnPlayers();
                this.SpawnTurrets();
                this.SpawnObstacles();

                this.turretBlueCounter.text = "blue turrets : 0";
                this.turretRedCounter.text = "red turrets : 0";

                GameManagerScript.Instance = this;

                this._startTime = Time.time;
        }

        private void Update()
        {
                this.turretBlueCounter.text = $"blue turrets : {this._blueTurretCount}";
                this.turretRedCounter.text = $"red turrets : {this._redTurretCount}";
                this.timeCounter.text = $"remaining time : {Math.Ceiling(this.length - (Time.time - this._startTime))}";

                if (this._blueTurretCount < 2 * this.turrets && this._redTurretCount < 2 * this.turrets &&
                    !(Time.time - this._startTime >= this.length)) return;
                Time.timeScale = 0;
                
                if (this._blueTurretCount > this._redTurretCount) this.gameOverText.text = "player blue won";
                else if (this._redTurretCount > this._blueTurretCount) this.gameOverText.text = "player red won";
                else this.gameOverText.text = "game over : tie";
                
                this.backToMenuPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
        }

        public void BackToMenu()
        {
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene("Scenes/MenuScene");
        }

        public static void AddTurretForTeam(Team oldTeam, Team newTeam)
        {
                switch (oldTeam)
                {
                        case Team.Blue:
                                GameManagerScript.Instance._blueTurretCount--;
                                break;
                        case Team.Red:
                                GameManagerScript.Instance._redTurretCount--;
                                break;
                }

                switch (newTeam)
                {
                        case Team.Blue:
                                GameManagerScript.Instance._blueTurretCount++;
                                break;
                        case Team.Red:
                                GameManagerScript.Instance._redTurretCount++;
                                break;
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

                var positionX = -center * this.gridNodeDistance;
                var positionZ = 0F;
                var obstaclePosition = new Vector3(positionX, 1F, positionZ);
                var obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(1F, 2F, physicalSize);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);

                positionX = (extendedSize - 1 - center) * this.gridNodeDistance;
                obstaclePosition = new Vector3(positionX, 1F, positionZ);
                obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(1F, 2F, physicalSize);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);

                positionX = 0;
                positionZ = center * this.gridNodeDistance;
                obstaclePosition = new Vector3(positionX, 1F, positionZ);
                obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(physicalSize, 2F, 1F);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);

                positionZ = (center - extendedSize + 1) * this.gridNodeDistance;
                obstaclePosition = new Vector3(positionX, 1F, positionZ);
                obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.GetComponent<Transform>().localScale = new Vector3(physicalSize, 2F, 1F);
                obstacle.GetComponent<Renderer>().material.mainTextureScale = new Vector2(20, 2);
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
                if (playerBlueScript != null) playerBlueScript.ResetPlayer(Team.Blue, this.playerBlueCamera.transform);

                var playerRed = Instantiate(this.playerPrefab, playerRedPosition, new Quaternion());
                var playerRedScript = playerRed.GetComponent<PlayerScript>();
                if (playerRedScript != null) playerRedScript.ResetPlayer(Team.Red, this.playerRedCamera.transform);
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
                        Instantiate(turretPrefab, position, new Quaternion());
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
                }
        }
}