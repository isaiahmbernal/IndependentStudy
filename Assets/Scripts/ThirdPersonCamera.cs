using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerMovement pm;
    [SerializeField] private Transform combatLookAt;
    [SerializeField] private GameObject basicCam;
    [SerializeField] private GameObject combatCam;

    [Header("Variables")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private CameraStyle cameraStyle;

    [Header("Keybinds")]
    [SerializeField] private KeyCode aimKey = KeyCode.Mouse1;

    private enum CameraStyle {
        Basic,
        Combat
    }

    public Transform getOrientation() {

        return orientation;

    }

    void Start() {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update() {

        // Rotate Orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        if (Input.GetKeyDown(aimKey)) {
            SwitchCameraStyle(CameraStyle.Combat);
        } else if (Input.GetKeyUp(aimKey)) {
            SwitchCameraStyle(CameraStyle.Basic);
        }

        // If Camera Style Basic
        if (cameraStyle == CameraStyle.Basic) {

            if (pm.getCanMove()) {

                // Rotate Player Object
                float horizontalInput = Input.GetAxis("Horizontal");
                float verticalInput = Input.GetAxis("Vertical");
                Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

                if (inputDir != Vector3.zero) {

                    playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);

                }

            }

        // If Camera Style Combat
        } else if (cameraStyle == CameraStyle.Combat) {

            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerObj.forward = dirToCombatLookAt.normalized;

        }
        
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {

        combatCam.SetActive(false);
        basicCam.SetActive(false);

        if (newStyle == CameraStyle.Basic) basicCam.SetActive(true);
        if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);

        cameraStyle = newStyle;

    }

}
