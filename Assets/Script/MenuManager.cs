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
        Application.Quit(); 
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("CamilleScene"); 
    }

    public void Commande()
    {
        SceneManager.LoadScene("Tuto"); 
    }
}