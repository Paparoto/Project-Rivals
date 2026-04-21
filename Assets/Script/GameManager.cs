using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float gameDuration = 180f;
    private float timer;

    public TMP_Text decompte;

    public bool isGameRunning = false;

    public TransactionManager transactionManager;

    void Start()
    {
        transactionManager = GetComponent<TransactionManager>();
        StartGame();
    }

    void Update()
{
    if (!isGameRunning) return;

    timer -= Time.deltaTime;

    int minutes = Mathf.FloorToInt(timer / 60f);
    int seconds = Mathf.FloorToInt(timer % 60f);
    decompte.text = $"{minutes:00}:{seconds:00}";

    if (timer <= 0f)
    {
        timer = 0f;
        decompte.text = "00:00";
        EndGame();
    }
}

    public void StartGame()
    {
        timer = gameDuration;
        isGameRunning = true;
    }

    void EndGame()
    {
        isGameRunning = false;

        int p1 = transactionManager.GetProfit(transactionManager.player1);
        int p2 = transactionManager.GetProfit(transactionManager.player2);

        Debug.Log("P1 profit: " + p1);
        Debug.Log("P2 profit: " + p2);

        if (p1 > p2)
            SceneManager.LoadScene("End");
        else if (p2 > p1)
            SceneManager.LoadScene("End 1");
        else
            SceneManager.LoadScene("End 2");
    }

    public float GetRemainingTime()
    {
        return timer;
    }
}