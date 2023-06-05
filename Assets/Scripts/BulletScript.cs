using UnityEngine;
using UnityEngine.Audio;

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

    public Team GetBulletTeam()
    {
        return this._myTeam;
    }
    
    private void Start()
    {
        this._myRenderer = this.GetComponent<Renderer>();
        this._startTime = Time.time;
        this.GetComponent<AudioSource>().Play();
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
            GameManagerScript.AddTurretForTeam(turretTeam, this._myTeam);
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