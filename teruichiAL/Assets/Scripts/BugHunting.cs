//using UnityEngine;

//public class BugHunting : MonoBehaviour
//{
//    [Header("バグのクリック判定用（Scene空間の白い四角）")]
//    public GameObject bug1Target;
//    public GameObject bug2Target;

//    [Header("バグ発見時に表示するマル（Canvas内のText TMP）")]
//    public GameObject bug1Circle;
//    public GameObject bug2Circle;

//    private bool isBug1Found = false;
//    private bool isBug2Found = false;

//    void Start()
//    {
//        // ゲーム開始時に、マル（◯）を非表示にする
//        if (bug1Circle != null) bug1Circle.SetActive(false);
//        if (bug2Circle != null) bug2Circle.SetActive(false);
//    }

//    // ★【ここを新設】バグ1（if文）がクリックされたときに、Unity側から直接呼び出す関数
//    // バグ1（if文）がクリックされたとき
//    // バグ1（if文）がクリックされたとき
//    public void OnClickBug1()
//    {
//        if (isBug1Found) return;
//        isBug1Found = true;
//        Debug.Log("【バグ1】条件式のミスを発見！");

//        if (bug1Circle != null)
//        {
//            RectTransform rt = bug1Circle.GetComponent<RectTransform>();
//            if (rt != null)
//            {
//                // ★2Dカメラ用の正しい座標変換処理
//                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, bug1Target.transform.position);
//                rt.position = screenPos;
//            }
//            Debug.Log("一つ目の丸の頭が実行");
//            bug1Circle.SetActive(true); // マルを表示
//        }

//        MakeTransparent(bug1Target); // 白い四角を透明にする
//        CheckGameClear();
//    }

//    // バグ2（閉じ括弧）がクリックされたとき
//    public void OnClickBug2()
//    {
//        if (isBug2Found) return;
//        isBug2Found = true;
//        Debug.Log("【バグ2】閉じ括弧のミスを発見！");

//        if (bug2Circle != null)
//        {
//            RectTransform rt = bug2Circle.GetComponent<RectTransform>();
//            if (rt != null)
//            {
//                // ★2Dカメラ用の正しい座標変換処理
//                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, bug2Target.transform.position);
//                rt.position = screenPos;
//            }

//            bug2Circle.SetActive(true); // マルを表示
//        }

//        MakeTransparent(bug2Target); // 白い四角を透明にする
//        CheckGameClear();
//    }

//    void CheckGameClear()
//    {
//        if (isBug1Found && isBug2Found)
//        {
//            Debug.Log("🎉 ステージクリア！すべてのバグを修正しました！");
//        }
//    }

//    // オブジェクトを透明にする関数
//    void MakeTransparent(GameObject target)
//    {
//        if (target != null)using UnityEngine;

using UnityEngine;

public class BugHunting : MonoBehaviour
{
    [Header("クリック判定")]
    public GameObject bug1Target;
    public GameObject bug2Target;

    [Header("赤丸(Image)")]
    public GameObject bug1Circle;
    public GameObject bug2Circle;

    private bool isBug1Found = false;
    private bool isBug2Found = false;

    void Start()
    {
        bug1Circle.SetActive(false);
        bug2Circle.SetActive(false);
    }

    public void OnClickBug1()
    {
        if (isBug1Found) return;

        isBug1Found = true;

        ShowCircle(bug1Circle, bug1Target);

        MakeTransparent(bug1Target);

        CheckGameClear();
    }

    public void OnClickBug2()
    {
        if (isBug2Found) return;

        isBug2Found = true;

        ShowCircle(bug2Circle, bug2Target);

        MakeTransparent(bug2Target);

        CheckGameClear();
    }

    void ShowCircle(GameObject circle, GameObject target)
    {
        circle.SetActive(true);

        RectTransform circleRT = circle.GetComponent<RectTransform>();
        RectTransform canvasRT = circle.transform.parent.GetComponent<RectTransform>();

        Vector2 screenPos =
            Camera.main.WorldToScreenPoint(target.transform.position);

        Vector2 localPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT,
            screenPos,
            null,
            out localPos);

        circleRT.anchoredPosition = localPos;
    }

    void MakeTransparent(GameObject target)
    {
        SpriteRenderer sr = target.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0;
            sr.color = c;
        }
    }

    void CheckGameClear()
    {
        if (isBug1Found && isBug2Found)
        {
            Debug.Log("ステージクリア！");
        }
    }
}
//        {
//            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
//            if (sr != null)
//            {
//                Color c = sr.color;
//                c.a = 0f; // アルファ値を0にして透明にする
//                sr.color = c;
//            }
//        }
//    }
//}