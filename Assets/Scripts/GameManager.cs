using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Global script to manage dev tools.
/// </summary>
public class GameManager : MonoBehaviour
{

    [Header("Keybinds")]
    [SerializeField] private KeyCode _resetKey = KeyCode.Tab;
    
    private void Awake() {

        // Limit The Framerate To 60
        Application.targetFrameRate = 60;

    }

    private void Update() {

        // If the reset key is pressed, reload the scene
        if (Input.GetKeyDown(_resetKey)) {
            // Debug.Log(SceneManager.GetActiveScene().ToString());
            SceneManager.LoadScene(0);
        }

    }

}
