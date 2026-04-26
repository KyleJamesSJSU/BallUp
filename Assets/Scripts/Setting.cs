using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    // output value
    public float currentValue;

    public float defaultValue = 50.0f;

    // limits to set
    private float min {get; set;} = 0.0f; 
    private float max {get; set;} = 100.0f;
    
    // references
    [SerializeField]
    private Slider m_slider;

    [SerializeField]
    private TMP_InputField m_field;

    // Start is called before the first frame update
    void Start()
    {
        // find slider and field
        //Transform holder = transform.Find("SettingModify");
        //m_field = holder.Find("InputField (TMP)").gameObject.GetComponent<TMP_InputField>();
        //m_slider = holder.Find("Slider").gameObject.GetComponent<Slider>();

        // initialize objects to values
        currentValue = defaultValue;
        UpdateBoth();

        // add listeners
        m_slider.onValueChanged.AddListener(delegate {OnSliderChange();} );
        m_field.onValueChanged.AddListener(delegate {OnFieldChange();});
    }

    // initializers
    public void SetRange(float min, float max)
    {
        // set internal variables
        this.min = min;
        this.max = max;
        // clamp values to range
        currentValue = Mathf.Clamp(currentValue, min, max);

        // update
        UpdateBoth();
    }

    public void SetCurrentValue(float value)
    {
        currentValue = Mathf.Clamp(value, min, max);
        // update
        UpdateBoth();
    }

    private void UpdateBoth()
    {
        UpdateSlider();
        UpdateField();
    }

    private void UpdateField()
    {
        // set text field to value
        m_field.text = (Mathf.Round(currentValue * 10.0f) / 10.0f).ToString();
        //Debug.Log("changed field");
    }

    private void UpdateSlider()
    {
        // set slider limits
        m_slider.minValue = min;
        m_slider.maxValue = max;
        // set slider current
        m_slider.value = currentValue;
        //Debug.Log("changed slider");
        
    }

    // listeners for value changes
    void OnSliderChange()
    {
        // update internal value & field
        currentValue = m_slider.value;
        UpdateField();
    }

    void OnFieldChange() {
        // try to parse value in field into a number
        if (float.TryParse(m_field.text, out float result))
        {
            // update internal value & slider
            currentValue = result;
            // conditionally update slider
            if (m_slider.value != currentValue) {
                UpdateSlider();
            }
        }
        
    }

}
