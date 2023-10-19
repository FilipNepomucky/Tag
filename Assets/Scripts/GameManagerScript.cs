using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerScript : MonoBehaviour
{
    public static bool gamePlaying { get; private set; }

    [Header("3, 2, 1, GO! Timer")]
    [SerializeField] private int timeToStart = 3;
    [SerializeField] private TextMeshProUGUI delayStartTimerText;
    
    [Header("Countdown Timer")]
    private float remainingTime = MenuScript.timerDuration;
    [SerializeField] private bool timerIsRunning = true;
    [SerializeField] private TextMeshProUGUI countdownTimerText;

    [Header("Players")]
    [SerializeField] private Player1Script player1;
    [SerializeField] private Player2Script player2;
    [SerializeField] private int randomChaserID;

    [Header("Player Images")]
    [SerializeField] private GameObject player1Image;
    [SerializeField] private GameObject player2Image;

    [Header("Maps")]
    [SerializeField] private GameObject map1;
    [SerializeField] private GameObject map2;
    [SerializeField] private GameObject map3;
    [SerializeField] private GameObject map4;

    [Header("Screens")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private GameObject pauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        // Unfreeze the time
        Time.timeScale = 1f;

        // Set gamePlaying variable to false (because of the "3, 2, 1, GO!" timer)
        gamePlaying = false;

        // Load the correct map
        switch (MenuScript.chosenMapIndex)
        {
            case 1:
                map2.SetActive(false);
                map3.SetActive(false);
                map4.SetActive(false);
                map1.SetActive(true);
                MenuScript.lastPlayedMap = 1;
                break;

            case 2:
                map1.SetActive(false);
                map3.SetActive(false);
                map4.SetActive(false);
                map2.SetActive(true);
                MenuScript.lastPlayedMap = 2;
                break;
            case 3:
                map1.SetActive(false);
                map2.SetActive(false);
                map4.SetActive(false);
                map3.SetActive(true);
                MenuScript.lastPlayedMap = 3;
                break;
            case 4:
                map1.SetActive(false);
                map2.SetActive(false);
                map3.SetActive(false);
                map4.SetActive(true);
                MenuScript.lastPlayedMap = 4;
                break;
        }

        // Choose a random chaser
        randomChaserID = Random.Range(1, 3);
        if (randomChaserID == 1)
        {
            player2.MakePlayer1Chaser();
        }
        else
        {
            player1.MakePlayer2Chaser();
        }

        // Play the "3, 2, 1, go!" animation
        StartCoroutine(CountdownToStart());

    }

    // Update is called once per frame
    void Update()
    {
        if (gamePlaying)
        {
            // Start the countdown timer
            if (timerIsRunning)
            {
                remainingTime -= Time.deltaTime;

                if (remainingTime <= 5)
                {
                    countdownTimerText.faceColor = new Color32(255, 51, 51, 255);

                    if (remainingTime <= 0)
                    {
                        remainingTime = 0;
                        countdownTimerText.text = remainingTime.ToString("0") + "s";
                        timerIsRunning = false;
                    }
                }

                countdownTimerText.text = remainingTime.ToString("0") + "s";
            }
            else
            {
                // End the game (trigger the "Game Over" screen, and choose the winner) when the timer reaches 0
                EndGame();
            }
        }
        
        // Check for pause
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (gamePlaying == false)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // Game over screen controls
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) && gameOverScreen.activeSelf == true)
        {
            RestartGame();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && gameOverScreen.activeSelf == true)
        {
            LoadMenuScreen();
        }
    }



    // ============================ MY METHODS ============================



    // Begin game function
    private void BeginGame()
    {
        gamePlaying = true;
    }

    // Pause the game
    public void PauseGame()
    {
        Time.timeScale = 0f;
        gamePlaying = false;
        pauseScreen.SetActive(true);
    }

    // Resume the game
    public void ResumeGame()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
        gamePlaying = true;
    }

    // End the game
    [ContextMenu("End Game")]
    public void EndGame()
    {
        Time.timeScale = 0f;
        gamePlaying = false;

        // Decide who is the winner
        if (player1.isChaser)
        {
            player1Image.SetActive(false);
            player2Image.SetActive(true);
            winnerText.text = "Blue Wins!";
        }
        else
        {
            player2Image.SetActive(false);
            player1Image.SetActive(true);
            winnerText.text = "Red Wins!";
        }

        gameOverScreen.SetActive(true);
    }

    // Restart game
    public void RestartGame()
    {
        if (MenuScript.randomMapActive)
        {
            do
            {
                MenuScript.ChooseRandomMap();
            }
            while (MenuScript.chosenMapIndex == MenuScript.lastPlayedMap);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Load the menu scene
    public void LoadMenuScreen()
    {
        SceneManager.LoadScene(0);
    }

    // The "3, 2, 1, go!" animation set-up
    IEnumerator CountdownToStart()
    {
        while (timeToStart > 0)
        {
            delayStartTimerText.text = timeToStart.ToString();
            yield return new WaitForSeconds(1f);
            timeToStart--;
        }

        delayStartTimerText.text = "GO!";
        BeginGame();
        yield return new WaitForSeconds(0.4f);
        delayStartTimerText.gameObject.SetActive(false);
    }
}
