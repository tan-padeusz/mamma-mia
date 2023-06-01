using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurretScript : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private float bulletInterval = 0.95F;
    private bool _canShoot = true;
    
    [Header("Turret")]
    [SerializeField] private int baseHealth = 4;
    private int _health;
    [SerializeField] private float maxRotation = 30F;

    [Header("Materials")]
    [SerializeField] private Material teamBlueMaterial;
    [SerializeField] private Material teamNeutralMaterial;
    [SerializeField] private Material teamRedMaterial;

    [Header("Prefabs")]
    [SerializeField] private GameObject bulletPrefab;

    private Renderer _myRenderer;
    private Material _myMaterial;
    private Team _myTeam;
    
    public Material GetTurretMaterial()
    {
        return this._myRenderer.material;
    }

    private void Start()
    {
        this._health = this.baseHealth;
        this._myRenderer = this.GetComponent<Renderer>();
    }

    private void Update()
    {
        this._myRenderer.material = this._myMaterial;
        if (!this._canShoot) return;
        this.StartCoroutine(this.ShootBullet());
    }

    private IEnumerator ShootBullet()
    {
        this._canShoot = false;
        var myTransform = this.transform;
        var rotation = Random.Range(-this.maxRotation, this.maxRotation);
        myTransform.Rotate(0, rotation, 0);
        var bulletPosition = myTransform.position + myTransform.forward;
        
        var bullet = Instantiate(this.bulletPrefab, bulletPosition, myTransform.rotation);
        var bulletScript = bullet.GetComponent<BulletScript>();
        if (bulletScript != null) bulletScript.ResetBullet(this._myTeam);
        
        yield return new WaitForSeconds(this.bulletInterval);
        this._canShoot = true;
    }

    public int DecreaseHealth()
    {
        return --this._health;
    }

    public void ResetTurret(Team team)
    {
        this._health = this.baseHealth;
        this._myTeam = team;
        this._myMaterial = team switch
        {
            Team.Blue => this.teamBlueMaterial,
            Team.Red => this.teamRedMaterial,
            _ => this.teamNeutralMaterial
        };
    }
}