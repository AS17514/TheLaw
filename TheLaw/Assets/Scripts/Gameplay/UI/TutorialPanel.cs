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

    // 检查这个文本框在指定位置和 Pivot 下，是否会超出屏幕
    bool TextFits(Vector2 anchor, Vector2 pivot, Vector2 size)
    {
        float hsw = screenWidth / 2f;
        float hsh = screenHeight / 2f;

        // 根据锚点和 Pivot 算出文本在 UI 坐标系下的四维绝对边界
        float L = anchor.x - pivot.x * size.x;
        float R = L + size.x;
        float T = anchor.y + (1f - pivot.y) * size.y;
        float B = T - size.y;

        // 确保全都在屏幕可见范围内
        return L >= -hsw && R <= hsw && T <= hsh && B >= -hsh;
    }

    private Vector2 GetSafeTextPosition(Vector2 holeCenter, Vector2 holeSize)
    {
        float hw = holeSize.x / 2f;
        float hh = holeSize.y / 2f;
        float m = 30f; // 稍微拉开一点间距，视觉效果更好

        // 核心：强制触发 TMP 和 Layout 立即重建，确保拿到的 sizeDelta 是当前文本的最准尺寸
        _titleText.ForceMeshUpdate();
        _contentText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tutorialText);
        Vector2 sz = tutorialText.sizeDelta;

        // 策略：预设 4 个最佳放置位置（上下左右）
        // 每个策略包含：文本框的 Pivot, 应当处于的 Anchor 坐标
        var strategies = new List<(Vector2 pivot, Vector2 anchor)>()
        {
            // 1. 优先放上方 (靠左或靠右，取决于洞在哪侧，防止挡住内容)
            (new Vector2(holeCenter.x > 0 ? 1f : 0f, 0f), new Vector2(holeCenter.x > 0 ? holeCenter.x + hw : holeCenter.x - hw, holeCenter.y + hh + m)),
            // 2. 其次放下方
            (new Vector2(holeCenter.x > 0 ? 1f : 0f, 1f), new Vector2(holeCenter.x > 0 ? holeCenter.x + hw : holeCenter.x - hw, holeCenter.y - hh - m)),
            // 3. 放左侧 (右对齐)
            (new Vector2(1f, 0.5f), new Vector2(holeCenter.x - hw - m, holeCenter.y)),
            // 4. 放右侧 (左对齐)
            (new Vector2(0f, 0.5f), new Vector2(holeCenter.x + hw + m, holeCenter.y))
        };

        // 轮询策略，哪个能完全塞进屏幕就用哪个
        foreach (var strategy in strategies)
        {
            if (TextFits(strategy.anchor, strategy.pivot, sz))
            {
                tutorialText.pivot = strategy.pivot;
                return strategy.anchor;
            }
        }

        // =================【极端情况兜底】=================
        // 如果上下左右都塞不下（比如高亮框极大），说明屏幕空间不够了。
        // 强制把文本塞进屏幕四角空余最大的地方，并做严格的 Clamp 限制，绝不穿帮。
        Vector2 backupPivot = new Vector2(holeCenter.x > 0 ? 1f : 0f, holeCenter.y > 0 ? 1f : 0f);
        tutorialText.pivot = backupPivot;

        Vector2 fallbackPos;
        // 如果洞偏右，文本放左边；洞偏上，文本放下边
        fallbackPos.x = holeCenter.x > 0 ? holeCenter.x - hw - m : holeCenter.x + hw + m;
        fallbackPos.y = holeCenter.y > 0 ? holeCenter.y - hh - m : holeCenter.y + hh + m;

        float hsw = screenWidth / 2f;
        float hsh = screenHeight / 2f;

        // 严格根据当前 Pivot 计算 Clamp 范围，防止瞎偏移
        if (backupPivot.x == 0f) // 左对齐
            fallbackPos.x = Mathf.Clamp(fallbackPos.x, -hsw, hsw - sz.x);
        else // 右对齐
            fallbackPos.x = Mathf.Clamp(fallbackPos.x, -hsw + sz.x, hsw);

        if (backupPivot.y == 0f) // 下对齐
            fallbackPos.y = Mathf.Clamp(fallbackPos.y, -hsh, hsh - sz.y);
        else // 上对齐
            fallbackPos.y = Mathf.Clamp(fallbackPos.y, -hsh + sz.y, hsh);

        return fallbackPos;
    }
}