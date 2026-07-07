using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // ★これが必要

public partial class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [Header("Task Settings")]
    public int totalTasks = 3;
    private int completedTasks = 0;

    [Header("UI Elements")]
    public TextMeshProUGUI taskCountText;
    public GameObject player;

    private string mainSceneName; // ★元のメインシーンの名前を記憶する用

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 各タスクにくっつける場合は、バグ防止のためコメントアウト（または削除）しておきます
        }
        else
        {
            // ↓ここを「gameObject」から「this」に変更します！
            Destroy(this); // すでに他にあるなら、このスクリプト（コンポーネント）だけを消す
            return;
        }
    }

    private void Start()
    {
        // ゲーム開始時のシーン名を記憶
        mainSceneName = SceneManager.GetActiveScene().name;
        UpdateTaskUI();
    }

    // ★UIオブジェクトの代わりに「シーン名」を受け取るように変更
    // ★UIオブジェクトの代わりに「シーン名」を受け取るように変更
    // TaskManager.cs の修正版

    public void OpenTaskScene(string sceneName)
    {
        SetPlayerMovement(false); // プレイヤーを止める

        // メインカメラを取得して、描画優先度を下げる
        GameObject mainCamObj = GameObject.FindWithTag("MainCamera");
        if (mainCamObj != null)
        {
            var camera = mainCamObj.GetComponent<Camera>();
            if (camera != null)
            {
                camera.depth = -1; // タスクカメラ（初期値0など）より低くする
            }
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void CompleteTaskScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
        SetPlayerMovement(true);

        // メインカメラの描画優先度を元に戻す
        GameObject mainCamObj = GameObject.FindWithTag("MainCamera");
        if (mainCamObj != null)
        {
            var camera = mainCamObj.GetComponent<Camera>();
            if (camera != null)
            {
                camera.depth = 0; // 元の優先度に戻す
            }
        }

        completedTasks++;
        UpdateTaskUI();

        if (completedTasks >= totalTasks)
        {
            Debug.Log("すべてのタスクが完了しました！ゲームクリア！");
        }
    }

    public void CloseTaskScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);

        // メインカメラの描画優先度を元に戻す
        GameObject mainCamObj = GameObject.FindWithTag("MainCamera");
        if (mainCamObj != null)
        {
            var camera = mainCamObj.GetComponent<Camera>();
            if (camera != null)
            {
                camera.depth = 0;
            }
        }

        SetPlayerMovement(true);
    }

    private void UpdateTaskUI()
    {
        if (taskCountText != null)
        {
            taskCountText.text = $"Tasks: {completedTasks} / {totalTasks}";
        }
    }

    private void SetPlayerMovement(bool canMove)
    {
        // メインシーンにいるプレイヤーを名前などで探す（シーンを跨ぐため）
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        if (player != null)
        {
            var controller = player.GetComponent<PlayerController>();
            var rb = player.GetComponent<Rigidbody2D>();
            if (controller != null) controller.enabled = canMove;
            if (rb != null && !canMove) rb.linearVelocity = Vector2.zero;
        }
    }
}