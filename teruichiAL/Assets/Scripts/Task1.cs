using UnityEngine;

public class Task1 : MonoBehaviour
{
    [Header("タスク内容入力")]
    public string taskSceneName; // ★インスペクターに「CardScanScene」などを打ち込む

    private bool isPlayerNearby = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!string.IsNullOrEmpty(taskSceneName))
            {
                // TaskManagerにシーンを開くよう命じる
                TaskManager.Instance.OpenTaskScene(taskSceneName);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log($"タスクに近づきました。[E]キーで「{taskSceneName}」を開始");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}