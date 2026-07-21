using UnityEngine;
using UnityEngine.SceneManagement; // シーン切り替えに必要

public class MiniGameTrigger : MonoBehaviour
{
    [SerializeField] private string miniGameSceneName = "Download"; // 遷移先のシーン名
    [SerializeField] private GameObject actionButton; // 画面に表示するボタン（スプライト自身、または目印のUIなど）

    private bool isPlayerNearby = false;

    void Start()
    {
        // 最初はボタン（画像やUI）を非表示にしておく場合
        if (actionButton != null)
        {
            actionButton.SetActive(false);
        }
    }

    // スプライト（コライダー）がマウスでクリックされたとき
    private void OnMouseDown()
    {
        // プレイヤーが近づいているときだけシーンを読み込む
        if (isPlayerNearby)
        {
            LoadMiniGame();
        }
    }

    // プレイヤーが範囲に入ったとき
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (actionButton != null) actionButton.SetActive(true);
        }
    }

    // プレイヤーが範囲から出たとき
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (actionButton != null) actionButton.SetActive(false);
        }
    }

    // シーンを読み込む関数
    private void LoadMiniGame()
    {
        SceneManager.LoadScene(miniGameSceneName);
    }
}