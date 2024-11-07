using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsPanel; // Assign your panel in the Inspector
    public bool isPaused = false;   // Track if the game is paused

    void Start()
    {
        settingsPanel.SetActive(false);
    }
    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings(); // Call the toggle function
        }
    }

    public void ToggleSettings()
    {
        // Toggle the panel visibility
        isPaused = !isPaused;
        settingsPanel.SetActive(isPaused);
    }
}