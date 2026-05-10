using UnityEngine;
using TMPro;
using System.Collections;

public class LoadingPanel : PanelBase
{
    private TextMeshProUGUI _loadingText;
    private Coroutine _dotsCoroutine;
    private string _baseText = "Loading"; // 基础文本

    protected override void Awake()
    {
        base.Awake();
        // 假设你的加载文本名字叫 Text_Loading
        _loadingText = GetControl<TextMeshProUGUI>("Text (TMP)_LoadingText");
    }

    // 当面板显示时开始动画
    public override void ShowSelf()
    {
        base.ShowSelf();
        if (_dotsCoroutine != null) StopCoroutine(_dotsCoroutine);
        _dotsCoroutine = StartCoroutine(AnimateDots());
    }

    // 当面板隐藏时停止动画，节省性能
    public override void HideSelf(UnityEngine.Events.UnityAction callBack)
    {
        base.HideSelf(callBack);
        if (_dotsCoroutine != null)
        {
            StopCoroutine(_dotsCoroutine);
            _dotsCoroutine = null;
        }
    }

    private IEnumerator AnimateDots()
    {
        int dotCount = 0;
        while (true)
        {
            dotCount = (dotCount + 1) % 4; // 在 0, 1, 2, 3 之间循环
            string dots = new string('.', dotCount);

            if (_loadingText != null)
            {
                _loadingText.text = _baseText + dots;
            }

            // 每隔 0.5 秒切换一次状态
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}