using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsHandler : MonoBehaviour
{
    
    public GameObject jumpBar;
    public GameObject jumpCount;
    public GameObject brakeBar;
    public Color activeBrakeColor = new Color(0.5f, 1.0f, 0.0f);
    public GameObject brakeCount;
    public GameObject player;

    private Color jumpBarColor;
    private Color brakeBarColor;
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f);

    private PlayerController playerControllerScript;

    private float barMaxLength;

    private RectTransform jumpBarRT;
    private RectTransform brakeBarRT;
    
    private Image jumpBarImage;
    private Image brakeBarImage;

    private TMP_Text jumpTextField;
    private TMP_Text brakeTextField;

    void Start()
    {
        jumpBarRT = (RectTransform)jumpBar.transform;
        brakeBarRT = (RectTransform)brakeBar.transform;
        barMaxLength = jumpBarRT.rect.width;
        playerControllerScript = player.GetComponent<PlayerController>();
        jumpBarImage = jumpBar.GetComponent<Image>();
        brakeBarImage = brakeBar.GetComponent<Image>();
        jumpBarColor = jumpBarImage.color;
        brakeBarColor = brakeBarImage.color;
        jumpTextField = jumpCount.GetComponent<TMP_Text>();
        brakeTextField = brakeCount.GetComponent<TMP_Text>();
    }

    void LateUpdate()
    {
        // modify bar lengths to match values in PlayerController
        float timeUntilJump = playerControllerScript.getTimeUntilNextJump();
        float jumpBarPercent = (playerControllerScript.jumpCooldown - timeUntilJump) / playerControllerScript.jumpCooldown;
        jumpBarRT.sizeDelta = new Vector2(jumpBarPercent * barMaxLength, jumpBarRT.rect.height);
        // set text
        if (playerControllerScript.canJump())
        {
            jumpTextField.text = "";
        } else
        {
            jumpTextField.text = Mathf.Round(timeUntilJump * 10) / 10 + "s";
        }

        // brake bar
        float brakeCapacity = playerControllerScript.getBrakeCapacity();
        float brakeBarPercent = brakeCapacity / playerControllerScript.brakeLimit;
        brakeBarRT.sizeDelta = new Vector2(brakeBarPercent * barMaxLength, brakeBarRT.rect.height);
        // set text
        brakeTextField.text = (int)Mathf.Round(brakeBarPercent * 100) + "";

        // set colors
        if (playerControllerScript.isGrounded())
        {
            if (playerControllerScript.canJump())
            {
                jumpBarImage.color = jumpBarColor;
            }
            else
            {
                jumpBarImage.color = disabledColor;
            }
            
            if (playerControllerScript.isPlayerBraking())
            {
                brakeBarImage.color = activeBrakeColor;
            } 
            else
            {
                brakeBarImage.color = brakeBarColor;
            }
            
        } else
        {
            jumpBarImage.color = disabledColor;
            brakeBarImage.color = disabledColor;
        }
    }
}
