using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Script to be attached to the Main Camera,
/// controlling the current camera style as well
/// as the direction the player moves, which
/// depends on said styles.
/// 
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _orientation;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _playerObj;
    [SerializeField] private Transform _combatLookAt;
    [SerializeField] private GameObject _basicCam;
    [SerializeField] private GameObject _combatCam;
    [SerializeField] private CharacterStateManager _myState;

    [Header("Variables")]
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private CameraStyle _cameraStyle;

    [Header("Keybinds")]
    [SerializeField] private KeyCode _aimKey = KeyCode.Mouse1;

    private enum CameraStyle {
        Basic,
        Combat
    }

    public Transform getOrientation() {
        return _orientation;
    }

    void Start() {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update() {

        // Rotate orientation
        Vector3 viewDir = _player.position - new Vector3(transform.position.x, _player.position.y, transform.position.z);
        _orientation.forward = viewDir.normalized;

        // If the player is aiming, switch
        // to the combat stance
        if (Input.GetKeyDown(_aimKey)) {
            SwitchCameraStyle(CameraStyle.Combat);
        } else if (Input.GetKeyUp(_aimKey)) {
            SwitchCameraStyle(CameraStyle.Basic);
        }

        // If the current camera style is Basic
        if (_cameraStyle == CameraStyle.Basic) {

            // If the animator was set
            // to combatStance, set it to false
            if (_anim.GetBool("isCombatStance")) {
                _anim.SetBool("isCombatStance", false);
            }

            // If the player is in a normal state and not attacking
            // allow them to rotate the player character
            if (_myState.GetAbleState() == CharacterStateManager.AbleState.Normal
                && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Attacking) {

                // Calculate the input diretion
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");
                Vector3 inputDir = _orientation.forward * verticalInput + _orientation.right * horizontalInput;

                // If input is detected, rotate the 
                // player to the inputted direction
                if (inputDir != Vector3.zero) {

                    _playerObj.forward = Vector3.Slerp(
                        _playerObj.forward, inputDir.normalized, Time.deltaTime * _rotationSpeed);

                }

            }

        // If the camera style is set to combat
        } else if (_cameraStyle == CameraStyle.Combat) {

            // If the animator wasn't set 
            // to combat stance, set it to true
            if (!_anim.GetBool("isCombatStance")) {
                _anim.SetBool("isCombatStance", true);
            }

            // Calculate the forwards direction and
            // point the player character towards it
            Vector3 dirToCombatLookAt = _combatLookAt.position - new Vector3(
                transform.position.x, _combatLookAt.position.y, transform.position.z);
            _orientation.forward = dirToCombatLookAt.normalized;

            _playerObj.forward = dirToCombatLookAt.normalized;

        }
        
    }

    /// <summary>
    /// 
    /// Call to set the current camera style.
    /// 
    /// </summary>
    /// <param name="newStyle"></param>
    private void SwitchCameraStyle(CameraStyle newStyle)
    {

        _combatCam.SetActive(false);
        _basicCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) _basicCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) _combatCam.SetActive(true);

        _cameraStyle = newStyle;

    }

}
