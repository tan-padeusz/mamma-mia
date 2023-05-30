using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurretScript : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private float bulletInterval = 0.95F;
    private GameObject _bulletPrefab;
    private bool _canShoot = true;
    
    [Header("Turret")]
    [SerializeField] private int baseHealth = 4;
    private int _health;
    [SerializeField] private float maxRotation = 30F;

    private void Start()
    {
        this._health = this.baseHealth;
    }

    private void Update()
    {
        if (!this._canShoot) return;
        this.StartCoroutine(this.ShootBullet());
    }

    private IEnumerator ShootBullet()
    {
        this._canShoot = false;
        var thisTransform = this.transform;
        var rotation = Random.Range(-this.maxRotation, this.maxRotation);
        thisTransform.Rotate(0, rotation, 0);
        var bulletPosition = thisTransform.position + thisTransform.forward;
        Instantiate(this._bulletPrefab, bulletPosition, thisTransform.rotation);
        yield return new WaitForSeconds(this.bulletInterval);
        this._canShoot = true;
    }

    public int DecreaseHealth()
    {
        return --this._health;
    }

    public void ResetTurret(GameObject bulletPrefab, Material turretMaterial)
    {
        this._health = this.baseHealth;
        this._bulletPrefab = bulletPrefab;
        this.GetComponent<Renderer>().material = turretMaterial;
    }
}