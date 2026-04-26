using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject startMenu;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject settingsMenu;
    [SerializeField]
    private GameObject creditsMenu;
    [SerializeField]
    private GameObject endMenu;
    [Space]
    [SerializeField]
    private GameObject gameplayUI;
    
    public enum MenuState
    {
        None,
        Start,
        Settings,
        Credits,
        End
    }

    private bool paused = true;
    private bool gameStarted = false;
    private bool settings = false;
    private bool credits = false;
    private bool gameEnded = false;

    private AudioSource audioClick;

    public void Pause()
    {
        // pause game by changing time values
        Time.timeScale = 0.0f;
        // show and unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // set states
        paused = true;
        // show pause menu if game is started
        if (gameStarted && !gameEnded)
        {
            pauseMenu.SetActive(true);
            // play click as well
            PlayClick();
        }
    }

    public void Unpause()
    {
        // don't unpause if a different menu is open
        if (!settings)
        {
            // unpause game by setting timescale back to 1.0f
            Time.timeScale = 1.0f;
            // set states
            paused = false;
            // hide pause menu
            if (gameStarted)
            {
                pauseMenu.SetActive(false);
            }
            // lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // play click
            PlayClick();
        }
        
    }

    public void StartGame()
    {
        // set stuff like the camera and whatever idk
        startMenu.SetActive(false);
        gameplayUI.SetActive(true);
        gameStarted = true;
        PlayClick();
        Unpause();
    }

    public void ToggleSettings()
    {
        // only do if game is paused
        if (!paused) return;
        PlayClick();
        // check state
        if (!settings)
        {
            // open settings menu, close other menus
            settingsMenu.SetActive(true);
            pauseMenu.SetActive(false);
            startMenu.SetActive(false);
            gameplayUI.SetActive(false);
            settings = true;
        } else
        {
            // close settings menu, open correct menu
            settingsMenu.SetActive(false);
            if (gameStarted)
            {
                // open pause menu & re-enable ui
                pauseMenu.SetActive(true);
                gameplayUI.SetActive(true);
            } 
            else
            {
                // open start menu
                startMenu.SetActive(true);
            }
            settings = false;
        }
    }

    public void ToggleCredits()
    {
        // only do if game is paused
        if (!paused) return;
        PlayClick();
        // check state
        if (!credits)
        {
            // open credits menu, close other menus
            creditsMenu.SetActive(true);
            endMenu.SetActive(false);
            startMenu.SetActive(false);
            credits = true;
        } else
        {
            // close settings menu, open correct menu
            creditsMenu.SetActive(false);
            if (gameEnded)
            {
                // open end menu
                endMenu.SetActive(true);
            } 
            else
            {
                // open start menu
                startMenu.SetActive(true);
            }
            credits = false;
        }
    }

    // call this when the player wins the game
    public void OnWin()
    {
        // set states and pause
        gameEnded = true;
        Pause();
        // hide ui
        gameplayUI.SetActive(false);
        // update end menu text values to match player states
        // TODO
        // show end menu
        endMenu.SetActive(true);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // initialize default states
        startMenu.SetActive(true);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        endMenu.SetActive(false);
        gameplayUI.SetActive(false);
        // set menu state variables
        gameStarted = false;
        settings = false;
        credits = false;
        gameEnded = false;
        // find audio source
        audioClick = GetComponent<AudioSource>();
        // pause game to set timescale
        Pause();
    }

    // handle input from player for pausing
    void OnPause()
    {
        if (!gameStarted || gameEnded) return;
        if (paused)
        {
            Unpause();
        } 
        else
        {
            Pause();
        }
    }

    void PlayClick()
    {
        audioClick.Play();
    }
}
