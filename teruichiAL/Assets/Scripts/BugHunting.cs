using UnityEngine;

public class BugHunting : MonoBehaviour
{
    [Header("バグのクリック判定用（Scene空間の白い四角）")]
    public GameObject bug1Target;
    public GameObject bug2Target;

    [Header("バグ発見時に表示するマル（Canvas内のText TMP）")]
    public GameObject bug1Circle;
    public GameObject bug2Circle;

    private bool isBug1Found = false;
    private bool isBug2Found = false;

    void Start()
    {
        // ゲーム開始時に、マル（◯）を非表示にする
        if (bug1Circle != null) bug1Circle.SetActive(false);
        if (bug2Circle != null) bug2Circle.SetActive(false);
    }

    // ★【ここを新設】バグ1（if文）がクリックされたときに、Unity側から直接呼び出す関数
    // バグ1（if文）がクリックされたとき
    public void OnClickBug1()
    {
        if (isBug1Found) return;
        isBug1Found = true;
        Debug.Log("【バグ1】条件式のミスを発見！");

        if (bug1Circle != null)
        {
            // ★【ここを修正】カメラから見た2D座標（四角の位置）を、UI用の画面座標に変換してピタッと合わせる！
            Vector2 screenPos = Camera.main.WorldToScreenPoint(bug1Target.transform.position);
            RectTransform rt = bug1Circle.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.position = screenPos;
            }

            bug1Circle.SetActive(true); // マルを表示
        }

        MakeTransparent(bug1Target); // 白い四角を透明にする
        CheckGameClear();
    }

    // バグ2（閉じ括弧）がクリックされたとき
    public void OnClickBug2()
    {
        if (isBug2Found) return;
        isBug2Found = true;
        Debug.Log("【バグ2】閉じ括弧のミスを発見！");

        if (bug2Circle != null)
        {
            // ★【ここを修正】カメラから見た2D座標（四角の位置）を、UI用の画面座標に変換してピタッと合わせる！
            Vector2 screenPos = Camera.main.WorldToScreenPoint(bug2Target.transform.position);
            RectTransform rt = bug2Circle.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.position = screenPos;
            }

            bug2Circle.SetActive(true); // マルを表示
        }

        MakeTransparent(bug2Target); // 白い四角を透明にする
        CheckGameClear();
    }

    void CheckGameClear()
    {
        if (isBug1Found && isBug2Found)
        {
            Debug.Log("🎉 ステージクリア！すべてのバグを修正しました！");
        }
    }

    // オブジェクトを透明にする関数
    void MakeTransparent(GameObject target)
    {
        if (target != null)
        {
            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 0f; // アルファ値を0にして透明にする
                sr.color = c;
            }
        }
    }
}