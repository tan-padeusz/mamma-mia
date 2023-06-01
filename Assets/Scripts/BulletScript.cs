using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private float destroyAfterSeconds = 4F;
    [SerializeField] private float speed = 15F;
    
    [Header("Materials")]
    [SerializeField] private Material teamBlueMaterial;
    [SerializeField] private Material teamNeutralMaterial;
    [SerializeField] private Material teamRedMaterial;

    private Material _myMaterial;
    private Team _myTeam = Team.Neutral;
    private float _startTime;
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
        if (Time.time - this._startTime > this.destroyAfterSeconds) this.SelfDestruct();
        var selfTransform = this.transform;
        selfTransform.position += selfTransform.forward * (this.speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var colliderTag = collision.gameObject.tag;
        Debug.Log("bullet collided with :" + colliderTag);
        if (collision.collider.CompareTag("Turret"))
        {
            if (!this._myMaterial.name.Contains("Neutral"))
            {
                var turretScript = collision.gameObject.GetComponent<TurretScript>();
                if (turretScript == null) return;
                var turretMaterial = turretScript.GetTurretMaterial();
                if (turretMaterial.name == this._myMaterial.name) return;
                var remainingHealth = turretScript.DecreaseHealth();
                if (remainingHealth == 0) turretScript.ResetTurret(this._myTeam, false);
            }
        }
        this.SelfDestruct();
    }

    public void ResetBullet(Team team)
    {
        this._myTeam = team;
        this._myMaterial = team switch
        {
            Team.Blue => this.teamBlueMaterial,
            Team.Red => this.teamRedMaterial,
            _ => this.teamNeutralMaterial
        };
    }
}