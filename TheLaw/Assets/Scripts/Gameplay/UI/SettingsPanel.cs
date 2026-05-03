using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : PanelBase
{
    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    void Init()
    {
        GetControl<Slider>("Slider_BGMVolume").value = AudioManager.Instance.BGMVolume;
        GetControl<Slider>("Slider_SFXVolume").value = AudioManager.Instance.SFXVolume;
        GetControl<Slider>("Slider_TipSize").value = 0;
        GetControl<TextMeshProUGUI>("Text (TMP)_TipSize").text = "00";
        GetControl<TextMeshProUGUI>("Text (TMP)_Tip").fontSize = 0;
    }
    protected override void ButtonOnClick(string buttonName)
    {
        switch (buttonName)
        {
            case "Button_QuitSettings":
                UIManager.Instance.RemovePanel<SettingsPanel>();
                break;
            default:
                return;
        }
    }
    protected override void SliderOnValueChanged(string sliderName, float value)
    {
        switch (sliderName)
        {
            // 音量条
            case "Slider_BGMVolume":
                AudioManager.Instance.SetBGMVolume(value);
                EventCenter.Instance.EventTrigger(E_EventType.Audio_Update_BGMVolume, value);
                GetControl<TextMeshProUGUI>("Text (TMP)_BGMVolume").text = Mathf.RoundToInt(value * 100).ToString();
                break;
            // 音效条
            case "Slider_SFXVolume":
                AudioManager.Instance.SetSFXVolume(value);
                GetControl<TextMeshProUGUI>("Text (TMP)_SFXVolume").text = Mathf.RoundToInt(value * 100).ToString();
                break;
            // 提示大小条
            case "Slider_TipSize":
                GetControl<TextMeshProUGUI>("Text (TMP)_TipSize").text = Mathf.RoundToInt(value * 100).ToString();
                GetControl<TextMeshProUGUI>("Text (TMP)_Tip").fontSize = Mathf.Lerp(0, 100, value);
                break;
            default:
                return;
        }
    }
}
