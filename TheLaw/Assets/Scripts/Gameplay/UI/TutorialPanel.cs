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

    /// <summary>
    /// 外部调用。传入教程类型和目标控件（按步骤顺序）。
    /// </summary>
    public void ShowTutorial(E_TutorialType tutorialType, params RectTransform[] targets)
    {
        TutorialConfigData config = JsonConvert.DeserializeObject<TutorialConfigData>(
            Resources.Load<TextAsset>("Tutorial/TutorialSteps").text);
        _currentTutorial = config.tutorials.Find(t => t.type == tutorialType.ToString());
        if (_currentTutorial == null || _currentTutorial.steps.Count == 0)
        {
            Debug.LogWarning($"教程 {tutorialType} 无配置");
            return;
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
        _titleText.text = step.title;
        _contentText.text = step.content;

        if (_stepTargets != null && _currentStep < _stepTargets.Length)
            DoMoveHole(_stepTargets[_currentStep]);

        // 最后一步不闪，前面步骤闪
        bool isLast = _currentStep >= _currentTutorial.steps.Count - 1;
        _promptCG.gameObject.SetActive(!isLast);
        if (!isLast)
            StartPromptFlash();
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

    public void DoMoveHole(RectTransform target, float duration = 0.3f)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_maskRect, screenPoint, null, out Vector2 center);
        Vector2 size = target.sizeDelta + holePadding;

        _mat.DOVector(center, HoleCenter, duration).SetEase(Ease.OutQuad);
        _mat.DOVector(size, HoleSize, duration).SetEase(Ease.OutQuad);

        Vector2 textPos = GetSafeTextPosition(center, size);
        tutorialText.gameObject.SetActive(true);
        tutorialText.DOAnchorPos(textPos, duration);
        _textCG.DOFade(1, duration);

        highlightBorder.gameObject.SetActive(true);
        highlightBorder.DOAnchorPos(center, duration);
        highlightBorder.DOSizeDelta(size, duration);
        _borderCG.DOFade(1, duration);
    }

    private Vector2 GetSafeTextPosition(Vector2 holeCenter, Vector2 holeSize)
    {
        float halfW = holeSize.x / 2f;
        float halfH = holeSize.y / 2f;

        Vector2 holeTopLeft = holeCenter + new Vector2(-halfW, halfH);
        Vector2 holeTopRight = holeCenter + new Vector2(halfW, halfH);

        float textW = tutorialText.sizeDelta.x;
        float margin = 20f;

        Vector2 finalPos;
        float candidateXRight = holeTopRight.x + margin;
        float textRightEdge = candidateXRight + textW;

        if (textRightEdge <= screenWidth / 2f)
        {
            finalPos = new Vector2(candidateXRight, holeTopRight.y);
            tutorialText.pivot = new Vector2(0, 1);
        }
        else
        {
            float candidateXLeft = holeTopLeft.x - margin;
            finalPos = new Vector2(candidateXLeft, holeTopLeft.y);
            tutorialText.pivot = new Vector2(1, 1);
        }

        return finalPos;
    }
}