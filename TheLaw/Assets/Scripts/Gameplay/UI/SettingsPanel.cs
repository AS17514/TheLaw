using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsPanel : PanelBase
{
    Transform TipTrans;
    TextMeshProUGUI tipText;
    bool isClosing;
    int _easterEggClicks;
    float x;
    public float rotationSpeed = 1;
    public float speedMultiple = 50;
    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    void Update()
    {
        x += 1 * rotationSpeed * Time.deltaTime;
        TipTrans.rotation = Quaternion.Euler(x, x, x);
    }
    void Init()
    {
        GetControl<Slider>("Slider_BGMVolume").value = AudioManager.Instance.BGMVolume;
        GetControl<Slider>("Slider_SFXVolume").value = AudioManager.Instance.SFXVolume;
        GetControl<Slider>("Slider_TipSize").value = 0;
        GetControl<TextMeshProUGUI>("Text (TMP)_BGMVolume").text = Mathf.RoundToInt(AudioManager.Instance.BGMVolume * 100).ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_SFXVolume").text = Mathf.RoundToInt(AudioManager.Instance.SFXVolume * 100).ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TipSize").text = "00";
        tipText = GetControl<TextMeshProUGUI>("Text (TMP)_Tip");
        tipText.fontSize = 0;
        TipTrans = tipText.transform;
        // 面板背景透明区域穿透射线
        GetControl<Image>("Image_bg").alphaHitTestMinimumThreshold = 0.01f;
        // 点击背景关闭
        RawImage rawBg = GetComponentInChildren<RawImage>();
        EventTrigger trigger = rawBg.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) =>
        {
            if (isClosing) return;
            isClosing = true;
            JsonManager.Instance.SaveAudioSettings(AudioManager.Instance.BGMVolume, AudioManager.Instance.SFXVolume);
            UIManager.Instance.RemovePanel<SettingsPanel>();
        });
        trigger.triggers.Add(entry);
        // 齿轮彩蛋：点击10次自动获胜
        Image gear = GetControl<Image>("Image_EasterEgg");
        gear.raycastTarget = true;
        EventTrigger gearTrigger = gear.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry gearEntry = new EventTrigger.Entry();
        gearEntry.eventID = EventTriggerType.PointerClick;
        gearEntry.callback.AddListener((data) =>
        {
            _easterEggClicks++;
            if (_easterEggClicks >= 10)
            {
                _easterEggClicks = 0;
                UIManager.Instance.RemovePanel<SettingsPanel>();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
            }
        });
        gearTrigger.triggers.Add(gearEntry);
        StartCrazyEffects();
    }
    void StartCrazyEffects()
    {
        // 色相循环
        DOVirtual.Float(0, 1, 5f, v =>
        {
            if (tipText != null)
                tipText.color = Color.HSVToRGB(v, 0.7f, 1f);
        }).SetLoops(-1).SetEase(Ease.Linear);

        // 持续微抖
        ((RectTransform)TipTrans).DOShakeAnchorPos(999f, new Vector2(3f, 3f), 15, 90, false, false);

        // 间歇抽搐
        StartPunchCycle();
    }
    void StartPunchCycle()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(Random.Range(2f, 6f));
        seq.Append(TipTrans.DOPunchScale(Vector3.one * 0.3f, 0.3f, 8));
        seq.OnComplete(() => StartPunchCycle());
    }
    protected override void ButtonOnClick(string buttonName)
    {
        switch (buttonName)
        {
            case "Button_QuitSettings":
                if (isClosing) return;
                isClosing = true;
                JsonManager.Instance.SaveAudioSettings(AudioManager.Instance.BGMVolume, AudioManager.Instance.SFXVolume);
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
                rotationSpeed = value * speedMultiple;
                GetControl<TextMeshProUGUI>("Text (TMP)_TipSize").text = Mathf.RoundToInt(value * 100).ToString();
                GetControl<TextMeshProUGUI>("Text (TMP)_Tip").fontSize = Mathf.Lerp(0, 100, value);
                break;
            default:
                return;
        }
    }
}
