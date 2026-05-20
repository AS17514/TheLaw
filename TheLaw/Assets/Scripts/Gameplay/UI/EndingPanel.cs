using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndingPanel : PanelBase
{
    float _delay = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        Image ciallo = GetControl<Image>("Image_Ciallo");
        CanvasGroup cg = ciallo.GetComponent<CanvasGroup>();
        cg.DOFade(0.15f, 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    void Update()
    {
        if (_delay > 0) { _delay -= Time.deltaTime; return; }
        if (Input.anyKeyDown)
            UIManager.Instance.ChangePanel<EndingPanel, StartMenuPanel>();
    }
}
