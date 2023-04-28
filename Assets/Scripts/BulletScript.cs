using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float destroyAfterSeconds = 4F;
    [SerializeField] private float speed = 15F;

    private Material _myMaterial;
    private float _startTime = 0F;

    public Material GetBulletMaterial()
    {
        return this._myMaterial;
    }

    public void SelfDestruct()
    {
        Destroy(this.gameObject);
        Destroy(this);
    }
    
    private void Start()
    {
        this._myMaterial = this.GetComponent<Renderer>().material;
        this._startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - this._startTime > this.destroyAfterSeconds) this.SelfDestruct();
        var selfTransform = this.transform;
        selfTransform.position += selfTransform.forward * (this.speed * Time.deltaTime);
    }
}