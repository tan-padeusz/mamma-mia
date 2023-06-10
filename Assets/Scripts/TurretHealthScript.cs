using TMPro;
using UnityEngine;

public class TurretHealthScript : MonoBehaviour
{
    [SerializeField] private Team team;
    
    private Camera _playerCamera;
    private TurretScript _turretScript;
    private TMP_Text _turretHealthText;

    private void Start()
    {
        this._turretHealthText = this.GetComponent<TMP_Text>();
        this._turretScript = this.GetComponentInParent<TurretScript>();
    }
    
    private void Update()
    {
        this._playerCamera = this._turretScript.GetCameraForTeam(this.team);
        if (_playerCamera == null) return;
        this.transform.rotation = Quaternion.LookRotation(this.transform.position - this._playerCamera.transform.position);
        this._turretHealthText.text = _turretScript.GetHealth().ToString();
    }
}