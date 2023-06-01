using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GridScript : MonoBehaviour
{
        [Header("Cameras")]
        [SerializeField] private Camera bluePlayerCamera;
        [SerializeField] private Camera redPlayerCamera;
        
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

        private void Start()
        {
                this.gridLevel = Math.Max(this.gridLevel, 1);
                this._gridSize = 2 * this.gridLevel + 1;
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
                if (playerBlueScript != null) playerBlueScript.ResetPlayer(Team.Blue, this.bluePlayerCamera.transform);

                var playerRed = Instantiate(this.playerPrefab, playerRedPosition, new Quaternion());
                var playerRedScript = playerRed.GetComponent<PlayerScript>();
                if (playerRedScript != null) playerRedScript.ResetPlayer(Team.Red, this.redPlayerCamera.transform);
                
                playerBlue.transform.LookAt(new Vector3(0, 0.5F, 0));
                playerRed.transform.LookAt(new Vector3(0, 0.5F, 0));
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