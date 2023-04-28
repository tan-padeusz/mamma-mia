using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5F;
    [SerializeField] private float rotationSpeed = 10F;
    [SerializeField] private float maxVerticalLookAngle = 60F;
    [SerializeField] private Transform cameraTransform;

    private float _verticalRotation = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        var selfTransform = this.transform;

        selfTransform.rotation = this.cameraTransform.rotation;
        if (Input.GetKey(KeyCode.W)) selfTransform.position += selfTransform.forward * (this.moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S)) selfTransform.position -= selfTransform.forward * (this.moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) selfTransform.position -= selfTransform.right * (this.moveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D)) selfTransform.position += selfTransform.right * (this.moveSpeed * Time.deltaTime);
        
        var mouseX = Input.GetAxis("Mouse X");
        this.cameraTransform.Rotate(Vector3.up * (mouseX * this.rotationSpeed));
        
        // var mouseY = Input.GetAxis("Mouse Y");
        // this._verticalRotation -= mouseY * this.rotationSpeed;
        // this._verticalRotation = Mathf.Clamp(this._verticalRotation, -this.maxVerticalLookAngle, this.maxVerticalLookAngle);
        // this.cameraTransform.localEulerAngles = new Vector3(this._verticalRotation, this.cameraTransform.localEulerAngles.y, 0);
        
        this.cameraTransform.position = selfTransform.position;
    }

    public void ResetCamera()
    {
        this.cameraTransform.rotation = this.transform.rotation;
    }
}