using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class MeetingManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text statusText;   // ← 統一したText
    public TMP_Text chatLogText;
    public TMP_InputField chatInputField;
    public Button sendButton;
    public ScrollRect scrollRect;

    [Header("Vote UI")]
    public Button[] voteButtons;
    public Button confirmVoteButton;

    [Header("Skip UI")]
    public Button skipButton;

    [Header("Player Data")]
    public Transform playersParent;
    
    [Header("Vote Counts")]
    private Dictionary<string, int> voteCounts = new Dictionary<string, int>();
    private Dictionary<int, int> voteMap = new Dictionary<int, int>();
    public int myPlayerId = 1;
    private int totalPlayers;   // 全体人数

    [Header("Timer")]
    public TMP_Text timerText;

    public float votingTime = 60f;   // 秒
    private float currentTime;
    private bool isVotingActive = true;

    private string selectedPlayer = null;
    private int selectedPlayerId;
    private bool hasVoted = false;


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
        confirmVoteButton.onClick.AddListener(ConfirmVote);
        skipButton.onClick.AddListener(OnSkipPressed);

        totalPlayers = playersParent.childCount;
        currentTime = votingTime;

        foreach (Transform child in playersParent)
        {
            PlayerRowUI row = child.GetComponent<PlayerRowUI>();

            int id = row.playerId;
            string name = row.GetName();

            Button button = child.GetComponentInChildren<Button>();

            voteCounts[name] = 0;

            button.onClick.AddListener(() => SelectPlayer(id, name));
        }

        statusText.text = "No selection";
        voteCounts["Skip"] = 0;
        UpdateVoteStatusUI();
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

            if (row.GetName() == playerName)
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
        //  投票済みなら何もしない
        if (hasVoted) return;
        if (!isVotingActive) return;

        
        PlayerRowUI row = GetRowById(playerId);
        if (row != null && !row.isAlive) return;


        selectedPlayerId = playerId;
        selectedPlayer = playerName;

        //  Skipなら即投票
        if (playerName.ToLower().Trim() == "skip")
        {
            ConfirmVote();
            return;
        }

        statusText.text = "Select: " + playerName;
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
            statusText.text = "You are dead and cannot vote.";
            return;
        }
        // すでに投票してたら何もしない
        if (hasVoted) return;

        if (string.IsNullOrEmpty(selectedPlayer))
        {
            statusText.text = "No selection";
            return;
        }

        voteCounts[selectedPlayer]++;
        voteMap[myPlayerId] = selectedPlayerId;
        UpdateVoteStatusUI();

        // Skip
        if (selectedPlayer.ToLower().Trim() == "skip")
        {
            statusText.text = "Skipped";
        }
        else
        {
            statusText.text = "Select: " + selectedPlayer + " voted";
        }

        hasVoted = true;

        // ボタンもロック（おすすめ）
        confirmVoteButton.interactable = false;
        skipButton.interactable = false;

        foreach (Button button in voteButtons)
        {
            button.interactable = false;
        }
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
            statusText.text = "You are dead and cannot vote.";
            return;
        }

        selectedPlayer = "Skip";
        selectedPlayerId = -1; // Skipは特別扱い

        ConfirmVote();
    }

    void EndVoting()
    {
        isVotingActive = false;

        Debug.Log("Voting ended");

        string result = CalculateResult();

        statusText.text = result;

        // UIロック（おすすめ）
        confirmVoteButton.interactable = false;
        skipButton.interactable = false;

        foreach (Transform child in playersParent)
        {
            Button btn = child.GetComponentInChildren<Button>();
            
            if (btn != null)
            {
                btn.interactable = false;
            }

        }
        
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

    


    
    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Debug.Log("Enter pressed");
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

            Button button = row.GetComponentInChildren<Button>();

            if (button != null)
            {
                // 生きている人だけ押せる
                button.interactable = row.isAlive;
            }
        }

        // Skip票も再登録
        voteCounts["Skip"] = 0;

        // Confirm / Skipを復活
        confirmVoteButton.interactable = true;
        skipButton.interactable = true;

        // 表示リセット
        statusText.text = "No selection";

        // 投票状態表示を更新
        UpdateVoteStatusUI();
    }
}