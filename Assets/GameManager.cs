using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public GameObject player;
    public Vector3 playerStartPosition = new Vector3(0, 1, 0);
    private int score = 0;


    void Awake()
    {
        Instance = this;
        UpdateScoreUI();
        gameOverText.gameObject.SetActive(false);  // Hide at start
    }

    public void StartGameRestart()
    {
        // Show game over message with final score
        gameOverText.text = "Game Over!\nScore: " + score;
        gameOverText.gameObject.SetActive(true);
        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        gameOverText.gameObject.SetActive(false);
        RestartGame();
    }

    public void RestartGame()
    {
        score = 0;
        UpdateScoreUI();
        player.GetComponent<PlayerTestControls>().enabled = true;
        player.GetComponent<PlayerController>().enabled = true;
        player.transform.position = playerStartPosition;
        FloorManager.Instance.ResetFloor();
    }

    public void AddScore()
    {
        score++;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}



