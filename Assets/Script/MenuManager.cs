using UnityEngine;
using UnityEngine.SceneManagement; 

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LePlusBeau"); 
    }

    public void QuitGame()
    {
        Debug.Log("The game is quitting!"); 
        Application.Quit(); 
    }
}