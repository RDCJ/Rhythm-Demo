using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeWidget : MonoBehaviour
{
    InputField minute_input;
    InputField second_input;
    InputField hundredths_input;
    public Times times;

    private void Awake()
    {
        times = new Times(0);
        minute_input = transform.Find("minute").GetComponent<InputField>();
        second_input = transform.Find("second").GetComponent <InputField>();
        hundredths_input = transform.Find("hundredths").GetComponent<InputField>();

        minute_input.onValueChanged.AddListener((string value) =>{
            times.minutes = int.Parse(value);
        });
        second_input.onValueChanged.AddListener((string value) => {
            times.remainingSeconds = int.Parse(value);
        });
        hundredths_input.onValueChanged.AddListener((string value) => {
            times.hundredths = int.Parse(value);
        });

    }

    public void SetTime(Times t)
    {
        minute_input.text = t.minutes.ToString();
        second_input.text = t.remainingSeconds.ToString();
        hundredths_input.text = t.hundredths.ToString();
    }
}
