using UnityEngine;

public class WireManager : MonoBehaviour
{
    public WireDrag[] wires;
    private bool isTaskCompleted = false; // ★連続呼び出しを防ぐフラグ

    void Update()
    {
        // すでに完了処理が走っていたら何もしない
        if (isTaskCompleted) return;

        // 配列が空っぽ（インスペクターでの登録忘れ）なら処理しない
        if (wires == null || wires.Length == 0) return;

        bool clear = true;

        foreach (var wire in wires)
        {
            // 登録されている配線のうち、1つでも None（空っぽ）があったらスキップ
            if (wire == null)
            {
                clear = false;
                break;
            }

            // まだ配線が生きている（enabledがtrue）ならクリアではない
            if (wire.enabled)
            {
                clear = false;
                break;
            }
        }

        // すべての配線の enabled が false になった瞬間だけ実行される
        if (clear)
        {
            isTaskCompleted = true; // フラグを立てて2回目以降をブロック
            Debug.Log("Task Complete!");

            // TaskManagerのインスタンスを通じてシーンを閉じる
            if (TaskManager.Instance != null)
            {
                TaskManager.Instance.CompleteTaskScene("Wire");
            }
            else
            {
                Debug.LogWarning("TaskManagerのインスタンスが見つかりません。");
            }
        }
    }
}