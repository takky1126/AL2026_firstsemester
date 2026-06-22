using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerRowUI : MonoBehaviour
{   
    public int playerId;
    public TMP_Text nameText;

    
    [Header("Buttons")]
    public Button selectButton;
    public Button checkButton;
    public Button cancelButton;

    
    [Header("Player Image")]
    public Image playerImage;
    public Sprite normalSprite;
    public Sprite votedSprite;
    public Sprite deadSprite;


    public bool isAlive = true;

    public string GetName()
    {
        return nameText.text;
    }
    
    public void ShowVoteActions(bool show)
    {
        if (checkButton != null)
        {
            checkButton.gameObject.SetActive(show);
        }

        if (cancelButton != null)
        {
            cancelButton.gameObject.SetActive(show);
        }
    }

    
    public void SetVoted(bool voted)
    {
        if (playerImage == null) return;
        if (!isAlive) return;
        
        if(voted)
        {
            if(votedSprite != null)
            {
                playerImage.sprite = votedSprite;
            }
        }
        else
        {
            if(normalSprite != null)
            {
                playerImage.sprite = normalSprite;
            }
        }
    }
    
    public void SetAlive(bool alive)
    {
        isAlive = alive;

        if (nameText != null)
        {
            nameText.color = alive ? Color.black : Color.gray;
        }

        if (playerImage != null)
        {
            if (!alive && deadSprite != null)
            {
                playerImage.sprite = deadSprite;
            }
            else if (alive && normalSprite != null)
            {
                playerImage.sprite = normalSprite;
            }
        }


        if (selectButton != null)
        {
            selectButton.interactable = alive;
        }

        ShowVoteActions(false);

    }

}
