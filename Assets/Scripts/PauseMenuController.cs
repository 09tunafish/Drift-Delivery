using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public void OnPauseClick()
    {

    }
    public void OnMenuClick()
    {
        SceneManager.LoadScene("Start Menu");
    }

    
}
