using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float gameDuration = 180f;
    private float timer;

    public bool isGameRunning = false;

    public PlayerEconomy player1;
    public PlayerEconomy player2;

    void Start()
    {
        player1 = new PlayerEconomy();
        player2 = new PlayerEconomy();

        StartGame();
    }

    void Update()
    {
        if (isGameRunning)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                timer = 0f;
                EndGame();
            }
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

        int p1Profit = player1.GetProfit();
        int p2Profit = player2.GetProfit();

        Debug.Log("P1 profit: " + p1Profit);
        Debug.Log("P2 profit: " + p2Profit);

        if (p1Profit > p2Profit)
            Debug.Log("Player 1 wins !");
        else if (p2Profit > p1Profit)
            Debug.Log("Player 2 wins !");
        else
            Debug.Log("Draw !");
    }

    public float GetRemainingTime()
    {
        return timer;
    }
}