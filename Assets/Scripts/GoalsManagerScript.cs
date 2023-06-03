using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalsManagerScript : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("Scenes/MenuScene");
    }
}
