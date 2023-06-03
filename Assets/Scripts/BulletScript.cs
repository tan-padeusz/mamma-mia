using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material teamBlueMaterial;
    [SerializeField] private Material teamNeutralMaterial;
    [SerializeField] private Material teamRedMaterial;
    
    private int _damage = 1;
    private float _speed = 15F;
    
    private const float Lifespan = 3F;
    private float _startTime;
    
    private Team _myTeam = Team.Neutral;
    private Material _myMaterial;
    private Renderer _myRenderer;

    private void SelfDestruct()
    {
        Destroy(this.gameObject);
        Destroy(this);
    }
    
    private void Start()
    {
        this._myRenderer = this.GetComponent<Renderer>();
        this._startTime = Time.time;
    }

    private void Update()
    {
        this._myRenderer.material = this._myMaterial;
        if (Time.time - this._startTime > BulletScript.Lifespan) this.SelfDestruct();
        var selfTransform = this.transform;
        selfTransform.position += selfTransform.forward * (this._speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Turret"))
        {
            var turretScript = collision.gameObject.GetComponent<TurretScript>();
            if (turretScript == null) return;
            var turretTeam = turretScript.GetTurretTeam();
            if (turretTeam == this._myTeam) return;
            if (turretScript.DecreaseHealth(this._damage) > 0) return;
            turretScript.ResetTurret(this._myTeam);
            GameManagerScript.Instance.AddTurretForTeam(turretTeam, this._myTeam);
        }

        if (collision.collider.CompareTag("Player"))
        {
            var playerScript = collision.gameObject.GetComponent<PlayerScript>();
            if (playerScript == null) return;
            playerScript.SlowDown(this._myTeam);
        }
        this.SelfDestruct();
    }

    public void ResetBullet(Team team, int damage, float speed)
    {
        this._myTeam = team;
        this._damage = damage;
        this._speed = speed;
        this._myMaterial = team switch
        {
            Team.Blue => this.teamBlueMaterial,
            Team.Red => this.teamRedMaterial,
            _ => this.teamNeutralMaterial
        };
    }
}