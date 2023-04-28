using System;
using UnityEngine;

public class Hitable : MonoBehaviour
{
    [SerializeField] private uint baseHitPoints = 4;
    
    private uint _hitPoints;
    private Renderer _myRenderer;
    
    private void OnCollisionEnter(Collision other)
    {
        if (!other.collider.CompareTag("Bullet")) return;
        Debug.Log("Bullet hit me!");
        var bulletScript = other.gameObject.GetComponent<BulletScript>();
        if (bulletScript == null) return;
        Debug.Log("It even had script attached!");
        var bulletMaterial = bulletScript.GetBulletMaterial();
        bulletScript.SelfDestruct();
        if (this._myRenderer.material.name == bulletMaterial.name) return;
        Debug.Log("Sadly, I lost some of my health...");
        if (--this._hitPoints != 0) return;
        Debug.Log("And I died!");
        this._hitPoints = this.baseHitPoints;
        this._myRenderer.material = bulletMaterial;
    }

    private void Start()
    {
        this._hitPoints = this.baseHitPoints;
        this._myRenderer = this.GetComponent<Renderer>();
    }
}
