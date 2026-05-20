using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStepData
{
    public string title;
    public string content;
}

public class TutorialConfig
{
    public string type;
    public List<TutorialStepData> steps;
}

public class TutorialConfigData
{
    public List<TutorialConfig> tutorials;
}

public class TutorialPanel : PanelBase
{
    Material _mat;
    RectTransform _maskRect;
    static readonly int HoleCenter = Shader.PropertyToID("_HoleCenter");
    static readonly int HoleSize = Shader.PropertyToID("_HoleSize");

    public Vector2 holePadding = Vector2.one * 20f;

    public RectTransform tutorialText;
    private CanvasGroup _textCG;
    public RectTransform highlightBorder;
    private CanvasGroup _borderCG;

    private float screenWidth = 1920;
    private float screenHeight = 1080;

    // 分步相关
    TutorialConfig _currentTutorial;
    RectTransform[] _stepTargets;
    int _currentStep;
    TextMeshProUGUI _titleText;
    TextMeshProUGUI _contentText;
    CanvasGroup _promptCG;

    protected override void Awake()
    {
        base.Awake();
        Image mask = GetControl<Image>("Image_Mask");
        _mat = mask.material;
        _maskRect = mask.rectTransform;

        _textCG = tutorialText.GetComponent<CanvasGroup>();
        _borderCG = highlightBorder.GetComponent<CanvasGroup>();
        _borderCG.alpha = 0;
        _textCG.alpha = 0;

        _titleText = GetControl<TextMeshProUGUI>("Text (TMP)_IntroTitle");
        _contentText = GetControl<TextMeshProUGUI>("Text (TMP)_IntroContent");

        TextMeshProUGUI prompt = GetControl<TextMeshProUGUI>("Text (TMP)_Prompt");
        _promptCG = prompt.GetComponent<CanvasGroup>();
        if (_promptCG == null)
            _promptCG = prompt.gameObject.AddComponent<CanvasGroup>();

        HideHole();
    }

    void OnDestroy()
    {
        _promptCG?.DOKill();
    }

    void Update()
    {
        if (_currentTutorial != null && Input.anyKeyDown)
            AdvanceStep();
    }

    void HideHole()
    {
        _mat.SetVector(HoleCenter, Vector2.zero);
        _mat.SetVector(HoleSize, Vector2.zero);
        _borderCG.DOFade(0, 0.2f);
        _textCG.DOFade(0, 0.2f);
    }

    TutorialConfigData LoadConfig()
    {
        return JsonConvert.DeserializeObject<TutorialConfigData>(
            Resources.Load<TextAsset>("Tutorial/TutorialSteps").text);
    }

    /// <summary>
    /// 按命名约定自动查找目标：{枚举名}{步号}，从1开始，如 MeetDodge1, MeetDodge2。
    /// </summary>
    public void ShowTutorialByNaming(E_TutorialType tutorialType, PanelBase targetPanel)
    {
        TutorialConfigData config = LoadConfig();
        TutorialConfig tc = config?.tutorials?.Find(t => t.type == tutorialType.ToString());
        int stepCount = tc?.steps?.Count ?? 1;
        RectTransform[] targets = new RectTransform[stepCount];
        for (int i = 0; i < stepCount; i++)
        {
            try
            {
                Image img = targetPanel.GetControl<Image>($"{tutorialType}{i + 1}");
                if (img != null) targets[i] = img.rectTransform;
            }
            catch { Debug.LogWarning($"教程目标 {tutorialType}{i + 1} 未找到"); }
        }
        ShowTutorial(tutorialType, targets);
    }

    /// <summary>
    /// 外部调用。传入教程类型和目标控件。
    /// </summary>
    public void ShowTutorial(E_TutorialType tutorialType, params RectTransform[] targets)
    {
        _currentTutorial = LoadConfig().tutorials.Find(t => t.type == tutorialType.ToString());
        if (_currentTutorial == null || _currentTutorial.steps.Count == 0)
        {
            Debug.LogWarning($"教程 {tutorialType} 无配置，使用默认文本");
            _currentTutorial = new TutorialConfig
            {
                type = tutorialType.ToString(),
                steps = new List<TutorialStepData> { new() }
            };
        }
        _stepTargets = targets;
        _currentStep = -1;
        AdvanceStep();
    }

    void AdvanceStep()
    {
        _currentStep++;
        if (_currentStep >= _currentTutorial.steps.Count)
        {
            CloseTutorial();
            return;
        }

        var step = _currentTutorial.steps[_currentStep];
        _titleText.text = string.IsNullOrEmpty(step.title) ? "默认标题" : step.title;
        _contentText.text = string.IsNullOrEmpty(step.content) ? "默认说明文本" : step.content;

        if (_stepTargets != null && _currentStep < _stepTargets.Length)
            DoMoveHole(_stepTargets[_currentStep]);

        _promptCG.gameObject.SetActive(true);
        bool isLast = _currentStep >= _currentTutorial.steps.Count - 1;
        if (isLast)
        {
            _promptCG.DOKill();
            _promptCG.alpha = 1f;
        }
        else
        {
            StartPromptFlash();
        }
    }

    void StartPromptFlash()
    {
        _promptCG.DOKill();
        _promptCG.alpha = 1;
        _promptCG.DOFade(0.2f, 0.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    void CloseTutorial()
    {
        _currentTutorial = null;
        _stepTargets = null;
        _promptCG.DOKill();
        _promptCG.gameObject.SetActive(false);
        HideHole();
        UIManager.Instance.RemovePanel<TutorialPanel>();
    }

    protected override void ButtonOnClick(string buttonName)
    {
        if (buttonName == "Button_SkipTutorial")
            CloseTutorial();
    }

    public void DoMoveHole(RectTransform target, float duration = 0.3f)
    {
        RectTransform rootRect = (RectTransform)transform;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_maskRect, screenPoint, null, out Vector2 maskCenter);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRect, screenPoint, null, out Vector2 uiCenter);
        Vector2 size = target.sizeDelta + holePadding;

        _mat.DOVector(maskCenter, HoleCenter, duration).SetEase(Ease.OutQuad);
        _mat.DOVector(size, HoleSize, duration).SetEase(Ease.OutQuad);

        Vector2 textPos = GetSafeTextPosition(uiCenter, size);
        tutorialText.gameObject.SetActive(true);
        tutorialText.DOAnchorPos(textPos, duration);
        _textCG.DOFade(1, duration);

        highlightBorder.gameObject.SetActive(true);
        highlightBorder.DOAnchorPos(uiCenter, duration);
        highlightBorder.DOSizeDelta(size, duration);
        _borderCG.DOFade(1, duration);
    }

    private Vector2 GetSafeTextPosition(Vector2 holeCenter, Vector2 holeSize)
    {
        float hw = holeSize.x / 2f;
        float hh = holeSize.y / 2f;
        float m = 20f;
        float rightThird = screenWidth / 6f;

        float px = holeCenter.x > rightThird ? 1f : 0f;
        float py = holeCenter.y > 0 ? 1f : 0f;
        float ax = px == 0 ? holeCenter.x + hw + m : holeCenter.x - hw - m;
        float ay = py == 1 ? holeCenter.y + hh + m : holeCenter.y - hh - m;

        tutorialText.pivot = new Vector2(px, py);
        return new Vector2(ax, ay);
    }
}