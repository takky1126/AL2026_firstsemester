using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; // Required for TextMeshPro

public class CardTouch : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Settings")]
    public RectTransform scanArea;       // Assign ScanArea here
    public TextMeshProUGUI statusText;   // Assign StatusText here
    public float requiredTime = 2.0f;    // Time required to scan (seconds)

    private RectTransform cardRect;
    private bool isInArea = false;
    private float currentTimer = 0f;
    private bool isUnlocked = false;

    void Start()
    {
        cardRect = GetComponent<RectTransform>();

        // Initial message when the game starts
        if (statusText != null)
        {
            statusText.text = "PLEASE INSERT CARD";
        }
    }

    void Update()
    {
        if (isUnlocked) return;

        // Check if the card is overlapping with the scan area hoge
        if (CheckOverlap())
        {
            if (!isInArea)
            {
                isInArea = true;
            }

            // Progress the timer
            currentTimer += Time.deltaTime;

            // Update the label with the current scanning progress
            if (statusText != null)
            {
                statusText.text = $"SCANNING... {currentTimer:F1}s / {requiredTime}s";
            }

            // Check if the holding time meets the requirement
            if (currentTimer >= requiredTime)
            {
                UnlockDoor();
            }
        }
        else
        {
            // If the card leaves the area before completing, reset the timer
            if (isInArea)
            {
                isInArea = false;
                currentTimer = 0f;

                if (statusText != null)
                {
                    statusText.text = "<color=red>BAD SCAN. TRY AGAIN.</color>";
                }
            }
        }
    }

    // Required for IDragHandler interface, can be left empty
    public void OnBeginDrag(PointerEventData eventData) { }

    // Handles dragging the card smoothly
    public void OnDrag(PointerEventData eventData)
    {
        if (isUnlocked) return;
        cardRect.anchoredPosition += eventData.delta / cardRect.lossyScale.x;
    }

    // Resets if the player lets go of the card before it's unlocked
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isUnlocked)
        {
            isInArea = false;
            currentTimer = 0f;
            if (statusText != null)
            {
                statusText.text = "PLEASE INSERT CARD";
            }
        }
    }

    // Calculates if the two UI Rects are overlapping in world space
    private bool CheckOverlap()
    {
        if (scanArea == null) return false;
        Rect rect1 = GetWorldRect(cardRect);
        Rect rect2 = GetWorldRect(scanArea);
        return rect1.Overlaps(rect2);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
    }

    private void UnlockDoor()
    {
        isUnlocked = true;

        if (statusText != null)
        {
            statusText.text = "<color=green>ACCESS GRANTED</color>";
        }

        Debug.Log("Task Complete! Door Unlocked.");

        // ★ここを追加：1秒後にタスク画面を閉じてカウントを増やす
        Invoke("NotifyManagerComplete", 1.0f);
    }

    private void NotifyManagerComplete()
    {
        // スクリプトの最後の方の処理
        Debug.Log("ダウンロード完了！");

        // 自分の所属しているシーンの名前を取得して渡す
        string currentSceneName = gameObject.scene.name;
        TaskManager.Instance.CompleteTaskScene(currentSceneName);
    }
}