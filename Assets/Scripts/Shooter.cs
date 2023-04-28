using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float newBulletInterval = 0.15F;
    
    private bool _canShoot = true;

    private void FixedUpdate()
    {
        this.bulletPrefab.GetComponent<Renderer>().material = this.GetComponent<Renderer>().material;
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0) || !this._canShoot) return;
        this.StartCoroutine(this.ShootBullet());
    }

    private IEnumerator ShootBullet()
    {
        this._canShoot = false;
        var selfTransform = this.transform;
        var currentPosition = selfTransform.position;
        var bulletPosition = currentPosition + selfTransform.forward;
        Instantiate(this.bulletPrefab, bulletPosition, selfTransform.rotation);
        yield return new WaitForSeconds(this.newBulletInterval);
        this._canShoot = true;
    }
}