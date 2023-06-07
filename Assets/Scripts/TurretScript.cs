using System.Collections;
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
    
    [Header("Materials")]
    [SerializeField] private Material teamBlueMaterial;
    [SerializeField] private Material teamNeutralMaterial;
    [SerializeField] private Material teamRedMaterial;

    private Renderer _myRenderer;
    private Material _myMaterial;
    private Team _myTeam = Team.Neutral;
    
    public Team GetTurretTeam()
    {
        return this._myTeam;
    }

    private void Start()
    {
        this._health = this.baseHealth;
        this._myMaterial = teamNeutralMaterial;
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
        if (bulletScript != null) bulletScript.ResetBullet(this._myTeam, this.bulletDamage, this.bulletSpeed);
        
        yield return new WaitForSeconds(this.bulletInterval);
        this._canShoot = true;
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
}