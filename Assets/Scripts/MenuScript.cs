using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;

public class MenuScript : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject defaultUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject chooseMapUI;
    [SerializeField] private GameObject howToPlayUI; // The one player can view from the home screen
    [SerializeField] private GameObject tutorialUI; // The one that shows automatically when the player launches the game for the first time

    [Header("UI Elements")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Dropdown timerDurationDropdown;

    [Header("Other")]
    public static float timerDuration;
    public static int chosenMapIndex;
    public static bool randomMapActive = false;
    public static int lastPlayedMap;

    void Awake()
    {
        LoadSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        // Open different screens using keyboard
        if (Input.GetKeyDown(KeyCode.Return) && defaultUI.activeSelf == true)
        {
            OpenChooseMapScreen();
        }
        if (Input.GetKeyDown(KeyCode.C) && defaultUI.activeSelf == true)
        {
            OpenSettings();
        }
        if (Input.GetKeyDown(KeyCode.I) && defaultUI.activeSelf == true)
        {
            OpenHowToPlay();
        }

        // Close different screens using keyboard
        if (Input.GetKeyDown(KeyCode.Escape) && chooseMapUI.activeSelf == true)
        {
            CloseChooseMapScreen();
        }
        if (Input.GetKeyDown(KeyCode.Return) && tutorialUI.activeSelf == true)
        {
            StartGame();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && settingsUI.activeSelf == true)
        {
            CloseSettings();
        }
        if (Input.GetKeyDown(KeyCode.Return) && settingsUI.activeSelf == true)
        {
            SaveSettings();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && howToPlayUI.activeSelf == true)
        {
            CloseHowToPlay();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && tutorialUI.activeSelf == true)
        {
            CloseTutorial();
        }

        // Load different maps using keyboard
        if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) && chooseMapUI.activeSelf == true)
        {
            LoadMap1();
        }
        if ((Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) && chooseMapUI.activeSelf == true)
        {
            LoadMap2();
        }
        if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) && chooseMapUI.activeSelf == true)
        {
            LoadMap3();
        }
        if ((Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) && chooseMapUI.activeSelf == true)
        {
            LoadMap4();
        }
        if ((Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) && chooseMapUI.activeSelf == true)
        {
            LoadRandomMap();
        }
    }

    // ============================ MY METHODS ============================

    // Set the volume based on the volume slider
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // Set the timer duration based on the dropdown
    public void SetTimerDuration(int index)
    {
        switch (index)
        {
            case 0:
                timerDuration = 60f;
                break;
            case 1:
                timerDuration = 120f;
                break;
            case 2:
                timerDuration = 180f;
                break;
        }
    }

    // Save user's settings into PlayerPrefs
    public void SaveSettings()
    {
        // Volume
        PlayerPrefs.SetFloat("Volume", AudioListener.volume);

        // Timer duration
        if(timerDurationDropdown.value == 0)
        {
            PlayerPrefs.SetFloat("TimerDuration", 60f);
        } 
        else if(timerDurationDropdown.value == 1)
        {
            PlayerPrefs.SetFloat("TimerDuration", 120f);
        }
        else
        {
            PlayerPrefs.SetFloat("TimerDuration", 180f);
        }

        CloseSettings();
    }

    // Load user's settings from PlayerPrefs
    public void LoadSettings()
    {
        // Volume
        if (PlayerPrefs.HasKey("Volume"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = AudioListener.volume;
        }
        else
        {
            AudioListener.volume = 1f;
            volumeSlider.value = AudioListener.volume;
        }

        // Timer duration
        if (PlayerPrefs.HasKey("TimerDuration"))
        {
            timerDuration = PlayerPrefs.GetFloat("TimerDuration");

            switch (PlayerPrefs.GetFloat("TimerDuration"))
            {
                case 60f:
                    timerDurationDropdown.value = 0;
                    break;
                case 120f:
                    timerDurationDropdown.value = 1;
                    break;
                case 180f:
                    timerDurationDropdown.value = 2;
                    break;
            }
        }
        else
        {
            timerDuration = 60f;
            timerDurationDropdown.value = 0;
        }
    }

    // Open settings
    public void OpenSettings()
    {
        defaultUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    // Close settings
    public void CloseSettings()
    {
        LoadSettings();
        settingsUI.SetActive(false);
        defaultUI.SetActive(true);
    }

    // Open "How to Play" screen from the home screen
    public void OpenHowToPlay()
    {
        defaultUI.SetActive(false);
        howToPlayUI.SetActive(true);
    }

    // Close the "How to Play" screen
    public void CloseHowToPlay()
    {
        howToPlayUI.SetActive(false);
        defaultUI.SetActive(true);
    }

    // Start the game
    public void StartGame()
    {
        // Check if the player has already viewed the tutorial, show it if not, start the game if yes
        if (PlayerPrefs.HasKey("TutorialViewed"))
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            chooseMapUI.SetActive(false);
            tutorialUI.SetActive(true);
            PlayerPrefs.SetString("TutorialViewed", "true");
        }
    }

    // Close the Tutorial screen (go back to choose a map screen)
    public void CloseTutorial()
    {
        tutorialUI.SetActive(false);
        chooseMapUI.SetActive(true);
    }

    public void LoadMap1()
    {
        chosenMapIndex = 1;
        randomMapActive = false;
        StartGame();
    }

    public void LoadMap2()
    {
        chosenMapIndex = 2;
        randomMapActive = false;
        StartGame();
    }

    public void LoadMap3()
    {
        chosenMapIndex = 3;
        randomMapActive = false;
        StartGame();
    }

    public void LoadMap4()
    {
        chosenMapIndex = 4;
        randomMapActive = false;
        StartGame();
    }

    public void LoadRandomMap()
    {
        randomMapActive = true;
        ChooseRandomMap();
        StartGame();
    }

    public static void ChooseRandomMap()
    {
        chosenMapIndex = Random.Range(1, 5);
        Debug.Log("Map " + chosenMapIndex + " was randomly chosen.");
    }

    // Open "Choose a Map" screen when user clicks the play button on Menu Scene's homepage
    public void OpenChooseMapScreen()
    {
        defaultUI.SetActive(false);
        chooseMapUI.SetActive(true);
    }

    // Close "Choose a Map" screen (go back to Menu Scene's homepage)
    public void CloseChooseMapScreen()
    {
        chooseMapUI.SetActive(false);
        defaultUI.SetActive(true);
    }

    // Quit the whole application
    public void QuitGame()
    {
        Debug.Log("User closed the application.");
        Application.Quit();
    }
}
