using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{

    // publicly accessible values that other functions should use
    public static float SENSITIVITY_HORIZONTAL {get; private set;}
    public static float SENSITIVITY_VERTICAL {get; private set;}
    public static float VOLUME_MASTER {get; private set;}
    public static float VOLUME_MUSIC {get; private set;}
    public static float VOLUME_SOUND {get; private set;}

    [Header("Sensitivity Limits")]
    public float MIN_HORZ = 0.1f;
    public float MAX_HORZ = 10.0f;
    [Space]
    public float MIN_VERT = 0.1f;
    public float MAX_VERT = 10.0f;
    
    [Header("Volume Limits")]
    public float MIN_VOLUME = 0.0f;
    public float MAX_VOLUME = 100.0f; 
    
    // menu references
    private Setting setting_Horizontal;
    private Setting setting_Vertical;
    private Setting setting_Master;
    private Setting setting_Music;
    private Setting setting_Effects;

    // object references for value changes
    [Header("References")]
    public CinemachineFreeLook cinemachineController;
    public AudioMixer audioMixer;

    // audio mixer controller references

    
    // Start is called before the first frame update
    void Start()
    {
        // initialize references
        setting_Horizontal = transform.Find("HorizontalSensitivity").gameObject.GetComponent<Setting>();
        setting_Vertical = transform.Find("VerticalSensitivity").gameObject.GetComponent<Setting>();
        setting_Master = transform.Find("MasterVolume").gameObject.GetComponent<Setting>();
        setting_Music = transform.Find("MusicVolume").gameObject.GetComponent<Setting>();
        setting_Effects = transform.Find("EffectsVolume").gameObject.GetComponent<Setting>();

        // initialize fields and sliders to current values
        setting_Horizontal.SetRange(MIN_HORZ, MAX_HORZ);
        setting_Vertical.SetRange(MIN_VERT, MAX_VERT);
        setting_Master.SetRange(MIN_VOLUME, MAX_VOLUME);
        setting_Music.SetRange(MIN_VOLUME, MAX_VOLUME);
        setting_Effects.SetRange(MIN_VOLUME, MAX_VOLUME);
        SENSITIVITY_HORIZONTAL = setting_Horizontal.defaultValue;
        SENSITIVITY_VERTICAL = setting_Vertical.defaultValue;
        VOLUME_MASTER = setting_Master.defaultValue;
        VOLUME_MUSIC = setting_Music.defaultValue;
        VOLUME_SOUND = setting_Effects.defaultValue;

        // get other gameobjects
        //cinemachineController = GameObject.Find("Cinemachine_FreeLook_Camera_Controller").GetComponent<CinemachineFreeLook>();
        //audiomixer (defined in unity already)

        // set values to defaults
        UpdateHorizontal();
        UpdateVertical();
        UpdateMaster();
        UpdateMusic();
        UpdateSound();
    }

    void UpdateHorizontal()
    {
        cinemachineController.m_XAxis.m_MaxSpeed = SENSITIVITY_HORIZONTAL / 3.0f;
    }

    void UpdateVertical()
    {
        cinemachineController.m_YAxis.m_MaxSpeed = SENSITIVITY_VERTICAL / 1000.0f;
    }

    void UpdateMaster()
    {
        audioMixer.SetFloat("MasterVolume", VolumeScaled(VOLUME_MASTER));
    }

    void UpdateMusic()
    {
        audioMixer.SetFloat("MusicVolume", VolumeScaled(VOLUME_MUSIC));
    }

    void UpdateSound()
    {
        audioMixer.SetFloat("EffectsVolume", VolumeScaled(VOLUME_SOUND));
    }

    private float VolumeScaled(float input)
    {
        return (input == 0) ? -80.0f : Mathf.Log10(input/50.0f) * 40;
    }

    void Update()
    {
        // this code sucks, but if values change, update the appropriate settings
        if (SENSITIVITY_HORIZONTAL != setting_Horizontal.currentValue)
        {
            SENSITIVITY_HORIZONTAL = setting_Horizontal.currentValue;
            // update horz sens in cinemachine camera
            UpdateHorizontal();
        }
        if (SENSITIVITY_VERTICAL != setting_Vertical.currentValue)
        {
            SENSITIVITY_VERTICAL = setting_Vertical.currentValue;
            // update vert sens in cinemachine camera
            UpdateVertical();
        }
        if (VOLUME_MASTER != setting_Master.currentValue)
        {
            VOLUME_MASTER = setting_Master.currentValue;
            // update sound value
            UpdateMaster();
        }
        if (VOLUME_MUSIC != setting_Music.currentValue)
        {
            VOLUME_MUSIC = setting_Music.currentValue;
            // update sound value
            UpdateMusic();
        }
        if (VOLUME_SOUND != setting_Effects.currentValue)
        {
            VOLUME_SOUND = setting_Effects.currentValue;
            // update sound value
            UpdateSound();
        }

    }
}
