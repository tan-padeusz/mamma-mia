using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI turretBlueCounter;
        [SerializeField] private TextMeshProUGUI turretRedCounter;
        [SerializeField] private TextMeshProUGUI gameOverText;
        [SerializeField] private GameObject backToMenuPanel;
        // [SerializeField] private Image backToMenuButtonImage;
        // [SerializeField] private TextMeshProUGUI backToMenuButtonText;
        
        [Header("Cameras")]
        [SerializeField] private Camera playerBlueCamera;
        [SerializeField] private Camera playerRedCamera;
        
        [Header("Grid")]
        [SerializeField] private int gridLevel;
        private int _gridSize;
        [SerializeField] private float nodeDistance = 3.5F;
        [SerializeField] private int obstacles = 4;
        [SerializeField] private int teamSize = 3;
        private int _maxObstacles;
        
        private bool[,] _grid;
        private bool[,] _obstacleColumnGrid;
        private bool[,] _obstacleRowGrid;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject obstacleColumnPrefab;
        [SerializeField] private GameObject obstacleRowPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject turretPrefab;

        private int _blueTurretCount = -1;
        private int _redTurretCount = -1;
        
        public static GameManagerScript Instance { get; private set; }

        private void Start()
        {
                this.gridLevel = Math.Max(this.gridLevel, 1);
                this._gridSize = 2 * this.gridLevel + 1;
                while (2 * this.teamSize > this._gridSize * this._gridSize - 2)
                        this.teamSize--;
                this._blueTurretCount = this.teamSize;
                this._redTurretCount = this.teamSize;
                
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
        }

        private void Update()
        {
                this.turretBlueCounter.text = "blue turrets : " + this._blueTurretCount;
                this.turretRedCounter.text = "red turrets : " + this._redTurretCount;

                if (this._blueTurretCount > 0 && this._redTurretCount > 0) return;
                Time.timeScale = 0;
                this.gameOverText.enabled = true;
                // this.backToMenuButtonImage.enabled = true;
                // this.backToMenuButtonText.enabled = true;
                this.backToMenuPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
        }

        public void BackToMenu()
        {
                SceneManager.LoadScene("Scenes/MenuScene");
        }

        public void AddTurretForTeam(Team team)
        {
                switch (team)
                {
                        case Team.Blue:
                                this._blueTurretCount++;
                                this._redTurretCount--;
                                break;
                        case Team.Neutral:
                                break;
                        case Team.Red:
                                this._blueTurretCount--;
                                this._redTurretCount++;
                                break;
                        default:
                                throw new ArgumentOutOfRangeException(nameof(team), team, null);
                }
        }

        private void SpawnPlayers()
        {
                var center = this._gridSize / 2;

                var playerBlueRow = 0;
                var playerBlueColumn = 0;
                var playerRedRow = this._gridSize - 1;
                var playerRedColumn = this._gridSize - 1;

                var playerBlueXPosition = (playerBlueColumn - center) * this.nodeDistance;
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
                
                playerBlue.transform.LookAt(new Vector3(0, 0.5F, 0));
                playerRed.transform.LookAt(new Vector3(0, 0.5F, 0));
        }

        private void SpawnObstacles()
        {
                for (var index = 0; index < this.obstacles; index++)
                {
                        var random = Random.Range(0, 2);
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
                        row = Random.Range(0, this._gridSize);
                        column = Random.Range(0, this._gridSize - 1);
                } while (this._obstacleColumnGrid[row, column]);
                this._obstacleColumnGrid[row, column] = true;
                
                var center = this._gridSize / 2;
                var xPosition = (column - center + 0.5F) * this.nodeDistance;
                var zPosition = (center - row) * this.nodeDistance;
                var obstaclePosition = new Vector3(xPosition, 1F, zPosition);
                Instantiate(this.obstacleColumnPrefab, obstaclePosition, new Quaternion());
                return true;
        }

        private bool SpawnRowObstacle()
        {
                if (this.CountTaken(this._obstacleColumnGrid) == this._maxObstacles / 2) return false;
                
                int row, column;
                do
                {
                        row = Random.Range(0, this._gridSize - 1);
                        column = Random.Range(0, this._gridSize);
                } while (this._obstacleRowGrid[row, column]);
                this._obstacleRowGrid[row, column] = true;
                
                var center = this._gridSize / 2;
                var xPosition = (column - center) * this.nodeDistance;
                var zPosition = (center - row - 0.5F) * this.nodeDistance;
                var obstaclePosition = new Vector3(xPosition, 1F, zPosition);
                Instantiate(this.obstacleRowPrefab, obstaclePosition, new Quaternion());
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
                                row = Random.Range(0, this._gridSize);
                                column = Random.Range(0, this._gridSize);
                        } while (this._grid[row, column]);
                        this._grid[row, column] = true;

                        var center = this._gridSize / 2;
                        var xPosition = (column - center) * this.nodeDistance;
                        var zPosition = (center - row) * this.nodeDistance;
                        var position = new Vector3(xPosition, 0.5F, zPosition);
                        var turret = Instantiate(this.turretPrefab, position, new Quaternion());
                        var turretScript = turret.GetComponent<TurretScript>();
                        if (turretScript == null) return;
                        var team = index % 2 == 0 ? Team.Blue : Team.Red;
                        turretScript.ResetTurret(team);
                }
        }
}