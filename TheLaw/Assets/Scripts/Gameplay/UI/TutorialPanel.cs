using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : PanelBase
{
    Material _mat;
    static readonly int HoleCenter = Shader.PropertyToID("_HoleCenter");
    static readonly int HoleSize = Shader.PropertyToID("_HoleSize");
    RectTransform target;

    // 提示文本
    public RectTransform tutorialText;
    private CanvasGroup _textCG;
    public RectTransform highlightBorder; // 新加：边框对象
    private CanvasGroup _borderCG; // 新加：边框的CanvasGroup

    private float screenWidth = 1920;
    private float screenHeight = 1080; // 你自己填真实高度

    void Start()
    {
        _mat = GetControl<Image>("Image_Mask").material;
        target = GetControl<Button>("Button_test").GetComponent<RectTransform>();

        _textCG = tutorialText.GetComponent<CanvasGroup>();
        _borderCG = highlightBorder.GetComponent<CanvasGroup>();
        _borderCG.alpha = 0;
        _textCG.alpha = 0;
        HideHole();
    }

    void HideHole()
    {
        _mat.SetVector(HoleCenter, Vector2.zero);
        _mat.SetVector(HoleSize, Vector2.zero);
        _borderCG.DOFade(0, 0.2f);
        _textCG.DOFade(0, 0.2f);


    }

    // 移动高亮洞 + 自动计算文本安全位置
    public void DoMoveHole(RectTransform target, float duration = 0.3f)
    {
        Vector2 center = target.anchoredPosition;
        Vector2 size = target.sizeDelta + Vector2.one * 20;

        // 高亮洞动画（原有逻辑）
        _mat.DOVector(center, HoleCenter, duration).SetEase(Ease.OutQuad);
        _mat.DOVector(size, HoleSize, duration).SetEase(Ease.OutQuad);

        // 文本同步动画（原有逻辑）
        Vector2 textPos = GetSafeTextPosition(center, size);
        tutorialText.gameObject.SetActive(true);
        tutorialText.DOAnchorPos(textPos, duration);
        _textCG.DOFade(1, duration);

        // ==============================================
        // 边框同步动画（关键代码）
        // ==============================================
        highlightBorder.gameObject.SetActive(true);
        highlightBorder.DOAnchorPos(center, duration); // 位置和洞中心完全同步
        highlightBorder.DOSizeDelta(size, duration);  // 大小和洞完全同步
        _borderCG.DOFade(1, duration);                // 淡入同步
    }

    // ==============================================
    // 🔥 核心：自动计算不超屏的文本位置
    // ==============================================
    private Vector2 GetSafeTextPosition(Vector2 holeCenter, Vector2 holeSize)
    {
        // 高亮框半宽、半高
        float halfW = holeSize.x / 2f;
        float halfH = holeSize.y / 2f;

        // 高亮框 左上角 & 右上角（anchoredPosition 坐标系，中心为原点）
        Vector2 holeTopLeft = holeCenter + new Vector2(-halfW, halfH);
        Vector2 holeTopRight = holeCenter + new Vector2(halfW, halfH);

        // 文本自身尺寸
        float textW = tutorialText.sizeDelta.x;
        float textH = tutorialText.sizeDelta.y;

        // 边距
        float margin = 20f;

        // 最终位置
        Vector2 finalPos;

        // ======================
        // 吸附规则：
        // 文本 左上角 → 吸附 高亮框 右上角
        // 文本 右上角 → 吸附 高亮框 左上角
        // ======================

        // 先尝试：文本左上角 吸附 高亮框右上角
        float candidateXRight = holeTopRight.x + margin;
        float textRightEdge = candidateXRight + textW;

        // 如果右边不超屏 → 放右边
        if (textRightEdge <= screenWidth / 2f)
        {
            finalPos = new Vector2(candidateXRight, holeTopRight.y);
            // 文本轴心设为 左上角 (0,1)
            tutorialText.pivot = new Vector2(0, 1);
        }
        else
        {
            // 放不下 → 文本右上角 吸附 高亮框左上角
            float candidateXLeft = holeTopLeft.x - margin;
            finalPos = new Vector2(candidateXLeft, holeTopLeft.y);
            // 文本轴心设为 右上角 (1,1)
            tutorialText.pivot = new Vector2(1, 1);
        }

        // 垂直位置保持和高亮框顶部平齐
        return finalPos;
    }

    protected override void ButtonOnClick(string buttonName)
    {
        if (buttonName == "Button_test")
        {
            DoMoveHole(target);
        }
    }
}