using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
   public void StartGame()
   {
      SceneManager.LoadScene("Scenes/GameScene");
   }

   public void QuitGame()
   {
      Application.Quit();
   }
}
