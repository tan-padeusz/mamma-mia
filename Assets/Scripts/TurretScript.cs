using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurretScript : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float bulletInterval = 0.95F;
    [SerializeField] private float bulletSpeed = 15F;
    private bool _canShoot = true;
    
    [Header("Turret")]
    [SerializeField] private int baseHealth = 4;
    private int _health;
    [SerializeField] private float maxRotation = 30F;
    [SerializeField] private float detectionRange = 3.5F;
    
    [Header("Materials")]
    [SerializeField] private Material teamBlueMaterial;
    [SerializeField] private Material teamNeutralMaterial;
    [SerializeField] private Material teamRedMaterial;
    
    private Renderer[] _childRenderers;
    private TMP_Text[] _healthLabels;
    private Material _myMaterial;
    private Team _myTeam = Team.Neutral;

    private Camera _playerBlueCamera;
    private Camera _playerRedCamera;

    private GameObject _player;
    
    public Team GetTurretTeam()
    {
        return this._myTeam;
    }

    private void Start()
    {
        this._health = this.baseHealth;
        this._myMaterial = teamNeutralMaterial;
        this.GetComponent<BoxCollider>().size = new Vector3(2 * this.detectionRange, 0.65F, 2 * this.detectionRange);
        this._childRenderers = this.GetComponentsInChildren<Renderer>();
        this._healthLabels = this.GetComponentsInChildren<TMP_Text>();
    }

    private void Update()
    {
        this.UpdateMaterials();
        this.UpdateHealthLabels();
        if (!this._canShoot) return;
        this.StartCoroutine(this.ShootBullet());
    }

    private void UpdateMaterials()
    {
        foreach (var childRenderer in this._childRenderers)
        {
            if (childRenderer.gameObject.name == "Cannon")
                continue;
            childRenderer.material = this._myMaterial;
        }
    }

    private void UpdateHealthLabels()
    {
        for (var index = 0; index < this._healthLabels.Length; index++)
        {
            var healthLabel = this._healthLabels[index];
            var playerCamera = index % 2 == 0 ? this._playerBlueCamera : this._playerRedCamera;
            var healthLabelTransform = healthLabel.transform;
            healthLabelTransform.rotation = Quaternion.LookRotation(healthLabelTransform.position - playerCamera.transform.position);
            healthLabel.text = this._health.ToString();
        }
    }

    private IEnumerator ShootBullet()
    {
        this._canShoot = false;
        var myTransform = this.transform;
        if (this._player != null)
        {
            var playerPosition = this._player.transform.position;
            myTransform.LookAt(new Vector3(playerPosition.x, 0.325F, playerPosition.z));
        }
        else
        {
            var rotationAngle = Random.Range(-this.maxRotation, this.maxRotation);
            myTransform.Rotate(0, rotationAngle, 0);
        }
        var bulletPosition = myTransform.position + myTransform.forward;
        var bullet = Instantiate(this.bulletPrefab, bulletPosition, myTransform.rotation);
        var bulletScript = bullet.GetComponent<BulletScript>();
        if (bulletScript != null) bulletScript.ResetBullet(this._myTeam, this.bulletDamage, this.bulletSpeed);
        
        yield return new WaitForSeconds(this.bulletInterval);
        this._canShoot = true;
    }

    public void SetCamera(Camera playerBlueCamera, Camera playerRedCamera)
    {
        this._playerBlueCamera = playerBlueCamera;
        this._playerRedCamera = playerRedCamera;
    }

    public Camera GetCameraForTeam(Team team)
    {
        return team switch
        {
            Team.Blue => this._playerBlueCamera,
            Team.Red => this._playerRedCamera,
            Team.Neutral => null,
            _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
        };
    }

    public int GetHealth()
    {
        return this._health;
    }

    public int DecreaseHealth(int value)
    {
        this._health -= value;
        return this._health;
    }

    public void ResetTurret(Team team)
    {
        GameManagerScript.AddTurretForTeam(this._myTeam, team);
        this._myTeam = team;
        this._health = this.baseHealth;

        this._myMaterial = team switch
        {
            Team.Blue => this.teamBlueMaterial,
            Team.Red => this.teamRedMaterial,
            _ => this.teamNeutralMaterial
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && this._player == null)
            this._player = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && this._player != null && this._player.name == other.name)
            this._player = null;
    }
}