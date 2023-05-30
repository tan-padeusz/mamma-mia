using System;
using UnityEngine;
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
        [SerializeField] private int teamSize = 3;
        private bool[,] _grid;

        [Header("Materials")]
        [SerializeField] private Material blueTeamMaterial;
        [SerializeField] private Material redTeamMaterial;

        [Header("Prefabs")]
        [SerializeField] private GameObject blueBulletPrefab;
        [SerializeField] private GameObject bluePlayerPrefab;
        [SerializeField] private GameObject redBulletPrefab;
        [SerializeField] private GameObject redPlayerPrefab;
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
                
                this.SpawnPlayers();
                for (var index = 0; index < this.teamSize * 2; index++)
                {
                        var teamIndex = index % 2;
                        if (teamIndex == 0) this.SpawnTurret(this.blueBulletPrefab, this.blueTeamMaterial);
                        else this.SpawnTurret(this.redBulletPrefab, this.redTeamMaterial);
                }
        }

        private void SpawnPlayers()
        {
                var midpoint = this._gridSize / 2;
                
                var playerBluePositionX = -midpoint * this.nodeDistance;
                var playerBluePositionZ = -midpoint * this.nodeDistance;
                var playerBluePosition = new Vector3(playerBluePositionX, 0.5F, playerBluePositionZ);
                var playerBlue = Instantiate(this.bluePlayerPrefab, playerBluePosition, new Quaternion());
                var playerBlueScript = playerBlue.GetComponent<PlayerBlueScript>();
                if (playerBlueScript != null) playerBlueScript.SetCamera(this.bluePlayerCamera);
                
                var playerRedPositionX = (this._gridSize - 1 - midpoint) * this.nodeDistance;
                var playerRedPositionZ = (this._gridSize - 1 - midpoint) * this.nodeDistance;
                var playerRedPosition = new Vector3(playerRedPositionX, 0.5F, playerRedPositionZ);
                var playerRed = Instantiate(this.redPlayerPrefab, playerRedPosition, new Quaternion());
                var playerRedScript = playerRed.GetComponent<PlayerRedScript>();
                if (playerRedScript != null) playerRedScript.SetCamera(this.redPlayerCamera);
        }

        private void SpawnTurret(GameObject bulletPrefab, Material turretMaterial)
        {
                int row;
                int column;
                do
                { 
                    row = Random.Range(0, this._gridSize);
                    column = Random.Range(0, this._gridSize);
                } while (this._grid[row, column]);
                this._grid[row, column] = true;

                var midpoint = this._gridSize / 2;
                var positionX = (row - midpoint) * this.nodeDistance;
                var positionZ = (column - midpoint) * this.nodeDistance;
                var turretPosition = new Vector3(positionX, 0.5F, positionZ);
                var turret = Instantiate(this.turretPrefab, turretPosition, new Quaternion());
                var turretScript = turret.GetComponent<TurretScript>();
                if (turretScript == null) return;
                turretScript.ResetTurret(bulletPrefab, turretMaterial);
        }
}