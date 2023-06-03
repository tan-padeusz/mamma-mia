using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsManagerScript : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("Scenes/MenuScene");
    }
}