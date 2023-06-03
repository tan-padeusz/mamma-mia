using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
   [SerializeField] private GameObject menuCanvas;
   [SerializeField] private GameObject alternativeMenuCanvas;

   private void Start()
   {
      this.menuCanvas.SetActive(true);
      this.alternativeMenuCanvas.SetActive(false);
   }

   public void StartGame()
   {
      SceneManager.LoadScene("Scenes/GameScene");
      this.alternativeMenuCanvas.SetActive(true);
      this.menuCanvas.SetActive(false);
   }

   public void ShowGoals()
   {
      SceneManager.LoadScene("Scenes/GoalsScene");
   }

   public void ShowControls()
   {
      SceneManager.LoadScene("Scenes/ControlsScene");
   }

   public void QuitGame()
   {
      Application.Quit();
   }
}