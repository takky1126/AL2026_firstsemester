using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerRowUI : MonoBehaviour
{   
    public int playerId;
    public TMP_Text nameText;
    
    [Header("Vote Status")]
    public TMP_Text voteStatusText;

    public bool isAlive = true;

    public string GetName()
    {
        return nameText.text;
    }

    
    public void SetVoted(bool voted)
    {
        if (voteStatusText == null) return;

        if (voted)
        {
            voteStatusText.text = "VOTED";
            voteStatusText.color = Color.green;
        }
        else
        {
            voteStatusText.text = "WAIT";
            voteStatusText.color = Color.gray;
        }
    }

    public void SetAlive(bool alive)
    {
        isAlive = alive;
        
    if (nameText != null)
        {
            if (!alive)
            {
                nameText.color = Color.gray;
            }
            else
            {
                nameText.color = Color.white;
            }
        }

        Button btn = GetComponentInChildren<Button>();

        if (btn != null)
        {
            btn.interactable = alive;
        }

    }

}
