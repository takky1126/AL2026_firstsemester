using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Download : MonoBehaviour
{
    public Button targetButton;
    public Slider progressBar;
    public TextMeshProUGUI timeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (targetButton != null) {
            targetButton.onClick.AddListener(OnButtonClicked);
        }

        if (progressBar != null)
        {
            progressBar.value = 0f;
        }

        if (timeText != null)
        {
            timeText.text = "5s";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnButtonClicked()
    {
        Debug.Log("Clicked");
        StartCoroutine(DownloadRoutine());
    }

    IEnumerator DownloadRoutine()
    {
        float elapsedTime = 0f;
        float duration = 5.0f; // 5秒

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            if (progressBar != null)
            {
                progressBar.value = elapsedTime / duration;
            }

            // --------------------------------------------------
            // ★ここに追加：残り時間の計算と表示
            // --------------------------------------------------
            if (timeText != null)
            {
                float remainingTime = duration - elapsedTime; // 残り時間を計算

                // 「あと〇秒」を小数点以下1桁（例: 4.2s）で表示する
                timeText.text = remainingTime.ToString("F1") + "s";
            }

            yield return null;
        }

        if (progressBar != null)
        {
            progressBar.value = 1.0f;
        }

        // 終了時はきれいに 0s にする
        if (timeText != null)
        {
            timeText.text = "0s";
        }
        Debug.Log("ダウンロード完了！");

        // 自分が所属しているタスクシーンの名前を取得して、TaskManagerに伝える
        string currentSceneName = gameObject.scene.name;
        TaskManager.Instance.CompleteTaskScene(currentSceneName); // 👈 新しいメソッド名に変える

//#if UNITY_EDITOR
//        // Unityエディタ上でのテストプレイも自動で停止させる設定
//        UnityEditor.EditorApplication.isPlaying = false;
//#endif
    }
}
