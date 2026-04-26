using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RunTracker : MonoBehaviour
{
    public GameObject heightText;
    public GameObject timeText;
    public GameObject player;

    private TMP_Text heightTextField;
    private TMP_Text timeTextField;
    [SerializeField]
    private TMP_Text winTimeField;

    private float timeStart;

    void Start()
    {
        heightTextField = heightText.GetComponent<TMP_Text>();
        timeTextField = timeText.GetComponent<TMP_Text>();
        timeStart = Time.fixedTime;
    }

    void LateUpdate()
    {
        // update height to match player's y value?
        heightTextField.text = Mathf.Round(player.transform.position.y) + " m";
        timeTextField.text = Mathf.Round(Time.fixedTime - timeStart) + "s";
        winTimeField.text = Mathf.Round(Time.fixedTime - timeStart) + "s";
    }

    // returns the playtime of the player
    public float GetPlaytime()
    {
        return Time.fixedTime - timeStart;
    }
}
