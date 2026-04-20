using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float gameDuration = 180f;
    private float timer;

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

        if (timer <= 0f)
        {
            timer = 0f;
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
            Debug.Log("Player 1 wins !");
        else if (p2 > p1)
            Debug.Log("Player 2 wins !");
        else
            Debug.Log("Draw !");
    }

    public float GetRemainingTime()
    {
        return timer;
    }
}