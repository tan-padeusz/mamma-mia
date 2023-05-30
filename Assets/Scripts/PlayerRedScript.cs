using System.Collections;
using UnityEngine;

public class PlayerRedScript : MonoBehaviour
{
        [Header("Player")]
        [SerializeField] private float movementSpeed = 5F;
        [SerializeField] private float rotationSpeed = 100F;

        [Header("Bullet")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float bulletInterval = 0.2F;
        
        private Transform _cameraTransform;
        private bool _canShoot = true;

        private void Start()
        {
                Cursor.lockState = CursorLockMode.Locked;
                this.bulletPrefab.GetComponent<Renderer>().material = this.GetComponent<Renderer>().material;
        }

        private void Update()
        {
                if (this._cameraTransform == null) return;
                
                var thisTransform = this.transform;
                thisTransform.rotation = this._cameraTransform.rotation;

                var deltaTime = Time.deltaTime;
                if (Input.GetKey(KeyCode.Keypad8)) thisTransform.position += thisTransform.forward * (this.movementSpeed * deltaTime);
                if (Input.GetKey(KeyCode.Keypad5)) thisTransform.position -= thisTransform.forward * (this.movementSpeed * deltaTime);
                if (Input.GetKey(KeyCode.Keypad4)) thisTransform.position -= thisTransform.right * (this.movementSpeed * deltaTime);
                if (Input.GetKey(KeyCode.Keypad6)) thisTransform.position += thisTransform.right * (this.movementSpeed * deltaTime);
                
                if (Input.GetKey(KeyCode.Keypad7)) this._cameraTransform.Rotate(Vector3.down * (this.rotationSpeed * deltaTime));
                if (Input.GetKey(KeyCode.Keypad9)) this._cameraTransform.Rotate(Vector3.up * (this.rotationSpeed * deltaTime));

                this._cameraTransform.position = thisTransform.position;
                
                if (!this._canShoot) return;
                if (Input.GetKey(KeyCode.Keypad2)) this.StartCoroutine(this.ShootBullet(thisTransform));
        }

        public void SetCamera(Camera playerCamera)
        {
                this._cameraTransform = playerCamera.transform;
        }

        private IEnumerator ShootBullet(Transform thisTransform)
        {
                this._canShoot = false;
                var currentPosition = thisTransform.position;
                var bulletPosition = currentPosition + thisTransform.forward;
                Instantiate(this.bulletPrefab, bulletPosition, thisTransform.rotation);
                yield return new WaitForSeconds(this.bulletInterval);
                this._canShoot = true;
        }
}