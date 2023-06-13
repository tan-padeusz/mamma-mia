using System;
using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private GameManagerScript _gameManager;
    
    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float bulletInterval = 0.35F;
    [SerializeField] private float bulletSpeed = 12.5F;
    private bool _canShoot = true;
    
    [Header("Player")]
    [SerializeField] private float movementSpeed = 5F;
    [SerializeField] private float rotationSpeed = 100F;
    [SerializeField] private float bdTime = 0.6F;

    [Header("Materials")]
    [SerializeField] private Material teamBlueMaterial;
    [SerializeField] private Material teamNeutralMaterial;
    [SerializeField] private Material teamRedMaterial;
        
    private Transform _cameraTransform;
    
    private Team _myTeam;
    private Material _myMaterial;
    private Renderer _myRenderer;
    private Transform _myTransform;

    private Action _playerAction;
    private bool _isSpeedModified;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        this._myRenderer = this.GetComponent<Renderer>();
        this._myTransform = this.GetComponent<Transform>();
    }

    private void Update()
    {
        if (Time.timeScale < 1) return;
        this._myRenderer.material = this._myMaterial;
        if (this._cameraTransform != null) this._playerAction?.Invoke();
    }

    private void PlayerBlueAction()
    {
        this._myTransform.rotation = this._cameraTransform.rotation;

        var deltaTime = Time.deltaTime;
        if (Input.GetKey(KeyCode.W)) this._myTransform.position += this._myTransform.forward * (this.movementSpeed * deltaTime);
        if (Input.GetKey(KeyCode.S)) this._myTransform.position -= this._myTransform.forward * (this.movementSpeed * deltaTime);
        if (Input.GetKey(KeyCode.A)) this._myTransform.position -= this._myTransform.right * (this.movementSpeed * deltaTime);
        if (Input.GetKey(KeyCode.D)) this._myTransform.position += this._myTransform.right * (this.movementSpeed * deltaTime);

        if (Input.GetKey(KeyCode.Q)) this._cameraTransform.Rotate(Vector3.down * (this.rotationSpeed * deltaTime));
        if (Input.GetKey(KeyCode.E)) this._cameraTransform.Rotate(Vector3.up * (this.rotationSpeed * deltaTime));

        this._cameraTransform.position = this._myTransform.position;

        if (!this._canShoot) return;
        if (Input.GetKey(KeyCode.K)) this.StartCoroutine(this.ShootBullet());
    }

    private void PlayerRedAction()
    {
        this._myTransform.rotation = this._cameraTransform.rotation;
        
        var deltaTime = Time.deltaTime;
        if (Input.GetKey(KeyCode.Keypad8)) this._myTransform.position += this._myTransform.forward * (this.movementSpeed * deltaTime);
        if (Input.GetKey(KeyCode.Keypad5)) this._myTransform.position -= this._myTransform.forward * (this.movementSpeed * deltaTime);
        if (Input.GetKey(KeyCode.Keypad4)) this._myTransform.position -= this._myTransform.right * (this.movementSpeed * deltaTime);
        if (Input.GetKey(KeyCode.Keypad6)) this._myTransform.position += this._myTransform.right * (this.movementSpeed * deltaTime);
                
        // if (Input.GetKey(KeyCode.Keypad7)) this._cameraTransform.Rotate(Vector3.down * (this.rotationSpeed * deltaTime));
        // if (Input.GetKey(KeyCode.Keypad9)) this._cameraTransform.Rotate(Vector3.up * (this.rotationSpeed * deltaTime));

        var mouseX = Input.GetAxis("Mouse X");
        this._cameraTransform.Rotate(Vector3.up * (2 * mouseX * this.rotationSpeed * deltaTime));

        this._cameraTransform.position = this._myTransform.position;
                
        if (!this._canShoot) return;
        if (Input.GetMouseButton(0)) this.StartCoroutine(this.ShootBullet());
        // if (Input.GetKey(KeyCode.Keypad2)) this.StartCoroutine(this.ShootBullet());
    }

    private IEnumerator ShootBullet()
    {
        this._canShoot = false;
        var bulletPosition = this._myTransform.position + this._myTransform.forward;
        var bullet = Instantiate(this.bulletPrefab, bulletPosition, this._myTransform.rotation);
        var bulletScript = bullet.GetComponent<BulletScript>();
        if (bulletScript != null) bulletScript.ResetBullet(this._myTeam, this.bulletDamage, this.bulletSpeed);
        yield return new WaitForSeconds(this.bulletInterval);
        this._canShoot = true;
    }

    public void ResetPlayer(Team team, Transform cameraTransform)
    {
        this._myTeam = team;

        this._myMaterial = team switch
        {
            Team.Blue => this.teamBlueMaterial,
            Team.Red => this.teamRedMaterial,
            _ => this.teamNeutralMaterial
        };

        this._playerAction = team switch
        {
            Team.Blue => this.PlayerBlueAction,
            Team.Red => this.PlayerRedAction,
            _ => null
        };
        
        this._cameraTransform = cameraTransform;
        this._cameraTransform.LookAt(new Vector3(0, 0, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Bullet")) return;
        var bulletScript = collision.gameObject.GetComponent<BulletScript>();
        if (bulletScript == null || bulletScript.GetBulletTeam() == Team.Neutral) return;
        this.ModifySpeed(bulletScript.GetBulletTeam());
    }

    private void ModifySpeed(Team team)
    {
        if (this._isSpeedModified) return;
        var turretCount = Math.Max(GameManagerScript.GetTurretsForTeam(this._myTeam), 1);
        var turretCountRoot = (float) Math.Pow(turretCount, 0.25);
        this._isSpeedModified = true;
        this.StartCoroutine(team == this._myTeam ? this.SpeedUp(turretCountRoot) : this.SlowDown(turretCountRoot));
    }

    private IEnumerator SpeedUp(float value)
    {
        var currentSpeed = this.movementSpeed;
        this.movementSpeed *= value;
        yield return new WaitForSeconds(this.bdTime);
        this.movementSpeed = currentSpeed;
        this._isSpeedModified = false;
    }

    private IEnumerator SlowDown(float value)
    {
        var currentSpeed = this.movementSpeed;
        this.movementSpeed /= value;
        yield return new WaitForSeconds(this.bdTime);
        this.movementSpeed = currentSpeed;
        this._isSpeedModified = false;
    }

    public void SetGameManager(GameManagerScript gameManager)
    {
        this._gameManager = gameManager;
    }
}
