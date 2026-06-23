using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class MeetingManager : MonoBehaviour
{
    [Header("UI")]

    public TMP_Text chatLogText;
    public TMP_InputField chatInputField;
    public Button sendButton;
    public ScrollRect scrollRect;


    [Header("Skip UI")]
    public Button skipButton;

    [Header("Player Data")]
    public Transform playersParent;
    
    [Header("Vote Counts")]
    private Dictionary<string, int> voteCounts = new Dictionary<string, int>();
    private Dictionary<int, int> voteMap = new Dictionary<int, int>();
    public int myPlayerId = 1;

    [Header("Timer")]
    public TMP_Text timerText;

    public float votingTime = 60f;   // 秒
    private float currentTime;
    private bool isVotingActive = true;

    private string selectedPlayer = null;
    private int selectedPlayerId;
    private bool hasVoted = false;

    [Header("Chat Toggle")]
    public GameObject chatPanel;
    public Button chatToggleButton;

    private bool isChatOpen = false;

    private PlayerRowUI selectedRow = null;
    
    public class PlayerData
    {
        public int playerId;
        public string playerName;
        public bool isAlive = true;
        public bool isMe = false;
    }

    public List<PlayerData> players = new List<PlayerData>();


    void Start()
    {
        players.Add(new PlayerData { playerId = 1, playerName = "Player1" });
        players.Add(new PlayerData { playerId = 2, playerName = "Player2" });
        players.Add(new PlayerData { playerId = 3, playerName = "Player3" });
        players.Add(new PlayerData { playerId = 4, playerName = "Player4" });


        sendButton.onClick.AddListener(SendChatMessage);
        skipButton.onClick.AddListener(OnSkipPressed);
        chatToggleButton.onClick.AddListener(ToggleChatPanel);

        chatPanel.SetActive(false);
        isChatOpen = false;

        currentTime = votingTime;

        foreach (Transform child in playersParent)
        {
            PlayerRowUI row = child.GetComponent<PlayerRowUI>();

            if (row == null) continue;

            row.SetAlive(row.isAlive);//初回開始時にも生死の見た目へ反映

            int id = row.playerId;
            string name = row.GetName();

            voteCounts[name] = 0;

            // 最初はチェック/バツを隠す
            row.ShowVoteActions(false);

            // プレイヤー選択ボタン
            if (row.selectButton != null)
            {
                row.selectButton.onClick.AddListener(() => SelectPlayer(id, name));
            }

            // チェックボタン：投票確定
            if (row.checkButton != null)
            {
                row.checkButton.onClick.AddListener(OnCheckPressed);
            }

            // バツボタン：キャンセル
            if (row.cancelButton != null)
            {
                row.cancelButton.onClick.AddListener(OnCancelPressed);
            }
        }

        voteCounts["Skip"] = 0;
        UpdateVoteStatusUI();
    }

    
    public void SetPlayerAliveState(int playerId, bool alive)
    {
        PlayerRowUI row = GetRowById(playerId);

        if (row == null)
        {
            Debug.LogWarning("PlayerRow not found: " + playerId);
            return;
        }

        row.SetAlive(alive);
    }


    void SendChatMessage()
    {
        PlayerRowUI me = GetRowById(myPlayerId);
        if (me != null && !me.isAlive)
        {
            return;
        }

        string message = chatInputField.text;

        if (message == "") return;

        chatLogText.text += "\nPlayer: " + message;
        chatInputField.text = "";
        chatInputField.ActivateInputField();

        //追加：スクロールを下へ
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }   

    

    void KillPlayerByName(string playerName)
    {
        foreach (Transform child in playersParent)
        {
            PlayerRowUI row = child.GetComponent<PlayerRowUI>();

            if (row == null) continue;

            if (row.GetName().Trim() == playerName.Trim())
            {
                row.SetAlive(false);
                Debug.Log(playerName + " is dead");
                return;
            }
        }
    }

    int GetAlivePlayerCount()
    {
        int count = 0;

        foreach (Transform child in playersParent)
        {
            PlayerRowUI row = child.GetComponent<PlayerRowUI>();

            if (row == null) continue;

            if (row.isAlive)
            {
                count++;
            }
        }
        return count;
    }


    //  選択
    void SelectPlayer(int playerId, string playerName)
    {
        if (hasVoted) return;
        if (!isVotingActive) return;

        PlayerRowUI row = GetRowById(playerId);

        if (row == null) return;
        if (!row.isAlive) return;

        // 他のRowのチェック/バツを全部消す
        HideAllVoteActions();

        selectedPlayerId = playerId;
        selectedPlayer = playerName;
        selectedRow = row;

        // 選んだRowだけチェック/バツを出す
        selectedRow.ShowVoteActions(true);
    }
    
    PlayerRowUI GetRowById(int id)
    {
        foreach (Transform child in playersParent)
        {
            PlayerRowUI row = child.GetComponent<PlayerRowUI>();
            if (row != null && row.playerId == id)
            {
                return row;
            }
        }
        return null;
}


    //  投票確定
    void ConfirmVote()
    {
        PlayerRowUI me = GetRowById(myPlayerId);
        if (me != null && !me.isAlive)
        {
            return;
        }
        // すでに投票してたら何もしない
        if (hasVoted) return;

        if (string.IsNullOrEmpty(selectedPlayer))
        {
            return;
        }

        voteCounts[selectedPlayer]++;
        voteMap[myPlayerId] = selectedPlayerId;
        UpdateVoteStatusUI();


        hasVoted = true; 
        HideAllVoteActions();

        // ボタンもロック（おすすめ）
        skipButton.interactable = false;

        SetAllPlayerSelectButtonsInteractable(false);
        if (voteMap.Count >= GetAlivePlayerCount())
        {
            EndVoting();
        }
        

    }
    
    void OnSkipPressed()
    {
        if (hasVoted) return;
        if (!isVotingActive) return;

        PlayerRowUI me = GetRowById(myPlayerId);
        if (me != null && !me.isAlive)        {
            return;
        }

        HideAllVoteActions();   
        selectedPlayer = "Skip";
        selectedPlayerId = -1; // Skipは特別扱い

        ConfirmVote();
    }

    void EndVoting()
    {
        isVotingActive = false;

        HideAllVoteActions();

        Debug.Log("Voting ended");

        string result = CalculateResult();

        // UIロック（おすすめ）
        skipButton.interactable = false;
        SetAllPlayerSelectButtonsInteractable(false);
        
        if (!result.Contains("No one"))
            {
                string eliminated = result.Replace(" was ejected", "");
                KillPlayerByName(eliminated);
            }

        }

    void UpdateVoteStatusUI()
    {
        foreach (Transform child in playersParent)
        {
            PlayerRowUI row = child.GetComponent<PlayerRowUI>();

            if (row == null) continue;

            bool voted = voteMap.ContainsKey(row.playerId);
            row.SetVoted(voted);
        }
    }

    string CalculateResult()
    {
        int maxVotes = 0;
        string winner = "";
        bool isTie = false;

        foreach (var pair in voteCounts)
        {
            if (pair.Value > maxVotes)
            {
                maxVotes = pair.Value;
                winner = pair.Key;
                isTie = false;
            }
            else if (pair.Value == maxVotes)
            {
                isTie = true;
            }
        }
        if (isTie)
        {
            return "No one was ejected (Tie)";
        }

        //Skip判定
        if (winner.ToLower() == "skip")
        {
            return "No one was ejected (Skip)";
        }

        return winner + " was ejected";
    }

    void SetAllPlayerSelectButtonsInteractable(bool interactable)
    {
        foreach (Transform child in playersParent)
        {
            PlayerRowUI row = child.GetComponent<PlayerRowUI>();

            if (row == null) continue;

            if (row.selectButton != null)
            {
                row.selectButton.interactable = interactable && row.isAlive;
            }
        }
}
    


    
    void Update()
    {
        if (isChatOpen &&
            chatInputField.isFocused &&
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SendChatMessage();
        }

        
        if (!isVotingActive) return;

        currentTime -= Time.deltaTime;

        // 表示更新
        timerText.text = "Time: " + Mathf.CeilToInt(currentTime).ToString();

        // 0以下で終了
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            EndVoting();
        }

    }

        public void StartNewMeeting()
    {
        // 選択状態をリセット
        selectedPlayer = null;
        selectedPlayerId = 0;

        // 自分の投票状態をリセット
        hasVoted = false;

        // 投票フェーズ再開
        isVotingActive = true;

        // タイマーリセット
        currentTime = votingTime;

        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(currentTime).ToString();
        }

        // 投票データをリセット
        voteMap.Clear();
        voteCounts.Clear();

        // プレイヤーの票数を再登録
        foreach (Transform child in playersParent)
        {
            PlayerRowUI row = child.GetComponent<PlayerRowUI>();

            if (row == null) continue;

            string name = row.GetName();
            voteCounts[name] = 0;

            row.SetAlive(row.isAlive);
        }

        // Skip票も再登録
        voteCounts["Skip"] = 0;

        // Confirm / Skipを復活
        skipButton.interactable = true;

        // 投票状態表示を更新
        UpdateVoteStatusUI();
    }

    void ToggleChatPanel()
    {
        isChatOpen = !isChatOpen;
        chatPanel.SetActive(isChatOpen);

        if (isChatOpen)
        {
            chatInputField.ActivateInputField();
        }
    }

    
void OnCheckPressed()
{
    if (hasVoted) return;
    if (!isVotingActive) return;

    if (selectedRow == null)
    {
        return;
    }
    ConfirmVote();

    HideAllVoteActions();
    }

    void OnCancelPressed()
    {
        if (hasVoted) return;
        if (!isVotingActive) return;

        selectedPlayer = null;
        selectedPlayerId = 0;
        selectedRow = null;

        HideAllVoteActions();

    }

    void HideAllVoteActions()
    {
        foreach (Transform child in playersParent)
        {
            PlayerRowUI row = child.GetComponent<PlayerRowUI>();

            if (row == null) continue;

            row.ShowVoteActions(false);
        }
    }


}