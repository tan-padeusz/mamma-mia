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
        [SerializeField] private float length = 60;
        private float _startTime;
        [SerializeField] private int size = 2;
        private int _gridSize;
        [SerializeField] private float nodeDistance = 3.5F;
        [SerializeField] private int teamSize = 3;
        [SerializeField] private int obstacles = 4;
        private int _maxObstacles;
        
        private bool[,] _grid;
        private bool[,] _obstacleColumnGrid;
        private bool[,] _obstacleRowGrid;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject obstaclePrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private List<GameObject> turretPrefabs;

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
                
                this.size = Math.Max(this.size, 1);
                this._gridSize = 2 * this.size + 1;
                while (2 * this.teamSize > this._gridSize * this._gridSize - 2)
                        this.teamSize--;
                
                this._grid = new bool[this._gridSize, this._gridSize];
                this._grid[0, 0] = true;
                this._grid[this._gridSize - 1, this._gridSize - 1] = true;

                this._maxObstacles = 2 * this._gridSize * (this._gridSize - 1);
                this.obstacles = Math.Min(this._maxObstacles, this.obstacles);
                this._obstacleColumnGrid = new bool[this._gridSize, this._gridSize - 1];
                this._obstacleRowGrid = new bool[this._gridSize - 1, this._gridSize];
                
                this.SpawnPlayers();
                this.SpawnTurrets();
                this.SpawnObstacles();

                this.turretBlueCounter.text = "blue turrets : " + this._blueTurretCount;
                this.turretRedCounter.text = "red turrets : " + this._redTurretCount;

                GameManagerScript.Instance = this;

                this._startTime = Time.time;
        }

        private void Update()
        {
                this.turretBlueCounter.text = "blue turrets : " + this._blueTurretCount;
                this.turretRedCounter.text = "red turrets : " + this._redTurretCount;
                this.timeCounter.text = $"remaining time : {Math.Ceiling(this.length - (Time.time - this._startTime))}";

                if (this._blueTurretCount < 2 * this.teamSize && this._redTurretCount < 2 * this.teamSize &&
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
                switch (newTeam)
                {
                        case Team.Blue:
                                GameManagerScript.Instance._blueTurretCount++;
                                if (oldTeam == Team.Red) GameManagerScript.Instance._redTurretCount--;
                                break;
                        case Team.Neutral:
                                break;
                        case Team.Red:
                                GameManagerScript.Instance._redTurretCount++;
                                if (oldTeam == Team.Blue) GameManagerScript.Instance._blueTurretCount--;
                                break;
                        default:
                                throw new ArgumentOutOfRangeException(nameof(newTeam), newTeam, null);
                }
        }

        private void SpawnPlayers()
        {
                var center = this._gridSize / 2;
                
                // var playerBlueRow = 0;
                // var playerBlueColumn = 0;
                var playerRedRow = this._gridSize - 1;
                var playerRedColumn = this._gridSize - 1;

                var playerBlueXPosition = -center * this.nodeDistance;
                var playerBlueZPosition = center * this.nodeDistance;
                var playerRedXPosition = (playerRedColumn - center) * this.nodeDistance;
                var playerRedZPosition = (center - playerRedRow) * this.nodeDistance;

                var playerBluePosition = new Vector3(playerBlueXPosition, 0.5F, playerBlueZPosition);
                var playerRedPosition = new Vector3(playerRedXPosition, 0.5F, playerRedZPosition);

                var playerBlue = Instantiate(this.playerPrefab, playerBluePosition, new Quaternion());
                var playerBlueScript = playerBlue.GetComponent<PlayerScript>();
                if (playerBlueScript != null) playerBlueScript.ResetPlayer(Team.Blue, this.playerBlueCamera.transform);

                var playerRed = Instantiate(this.playerPrefab, playerRedPosition, new Quaternion());
                var playerRedScript = playerRed.GetComponent<PlayerScript>();
                if (playerRedScript != null) playerRedScript.ResetPlayer(Team.Red, this.playerRedCamera.transform);
        }

        private void SpawnObstacles()
        {
                for (var index = 0; index < this.obstacles; index++)
                {
                        var random = UnityEngine.Random.Range(0, 2);
                        if (random == 0)
                        {
                                if (!this.SpawnColumnObstacle()) this.SpawnRowObstacle();
                        }
                        else
                        {
                                if (!this.SpawnRowObstacle()) this.SpawnColumnObstacle();
                        }
                }
        }
        
        private bool SpawnColumnObstacle()
        {
                if (this.CountTaken(this._obstacleRowGrid) == this._maxObstacles / 2) return false;

                int row, column;
                do
                {
                        row = UnityEngine.Random.Range(0, this._gridSize);
                        column = UnityEngine.Random.Range(0, this._gridSize - 1);
                } while (this._obstacleColumnGrid[row, column]);
                this._obstacleColumnGrid[row, column] = true;
                
                var center = this._gridSize / 2;
                var xPosition = (column - center + 0.5F) * this.nodeDistance;
                var zPosition = (center - row) * this.nodeDistance;
                var obstaclePosition = new Vector3(xPosition, 1F, zPosition);
                var obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.transform.localScale = new Vector3(1, 2, 2);
                return true;
        }

        private bool SpawnRowObstacle()
        {
                if (this.CountTaken(this._obstacleColumnGrid) == this._maxObstacles / 2) return false;
                
                int row, column;
                do
                {
                        row = UnityEngine.Random.Range(0, this._gridSize - 1);
                        column = UnityEngine.Random.Range(0, this._gridSize);
                } while (this._obstacleRowGrid[row, column]);
                this._obstacleRowGrid[row, column] = true;
                
                var center = this._gridSize / 2;
                var xPosition = (column - center) * this.nodeDistance;
                var zPosition = (center - row - 0.5F) * this.nodeDistance;
                var obstaclePosition = new Vector3(xPosition, 1F, zPosition);
                var obstacle = Instantiate(this.obstaclePrefab, obstaclePosition, new Quaternion());
                obstacle.transform.localScale = new Vector3(2, 2, 1);
                return true;
        }

        private int CountTaken(bool[,] grid)
        {
                return grid.Cast<bool>().Count(element => element);
        }

        private void SpawnTurrets()
        {
                for (var index = 0; index < this.teamSize * 2; index++)
                {
                        int row, column;
                        do
                        {
                                row = UnityEngine.Random.Range(0, this._gridSize);
                                column = UnityEngine.Random.Range(0, this._gridSize);
                        } while (this._grid[row, column]);
                        this._grid[row, column] = true;

                        var center = this._gridSize / 2;
                        var xPosition = (column - center) * this.nodeDistance;
                        var zPosition = (center - row) * this.nodeDistance;
                        var position = new Vector3(xPosition, 0.5F, zPosition);
                        var turretPrefab = this.turretPrefabs[UnityEngine.Random.Range(0, this.turretPrefabs.Count)];
                        Instantiate(turretPrefab, position, new Quaternion());
                }
        }
}