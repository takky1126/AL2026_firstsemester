using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理に必要

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
     {
         if (other.CompareTag("Player"))
         {
             SceneManager.LoadScene(nextSceneName);
         }
    }
}
