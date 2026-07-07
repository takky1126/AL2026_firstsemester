using UnityEngine;
using TMPro;

public class ipaddTask : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipaddInputField;
    [SerializeField] private TMP_InputField subnetInputField;
    [SerializeField] private TMP_Text resultText;

    private void Start()
    {
        // ゲーム開始時は結果テキストを消しておく
        if (resultText != null) resultText.text = "";
    }

    // Submitボタンが押されたときに実行される関数
    public void OnSubmitSettings()
    {
        // 入力された文字を取得（前後の余計な空白をカット）
        string ipAddress = ipaddInputField.text.Trim();
        string subnetMask = subnetInputField.text.Trim();

        // 正解のデータ
        string correctIP = "192.168.1.1";
        string correctSubnet = "255.255.255.0";

        // 正誤判定
        if (ipAddress == correctIP && subnetMask == correctSubnet)
        {
            Debug.Log("【正解】ネットワーク設定が成功しました！");
            if (resultText != null)
            {
                resultText.text = "Success!";
                resultText.color = Color.green; // 文字を緑に
            }
        }
        else
        {
            Debug.Log("【不正解】設定が間違っています。");
            if (resultText != null)
            {
                resultText.text = "Incorrect Settings. Try again.";
                resultText.color = Color.red; // 文字を赤に
            }
        }
    }
}