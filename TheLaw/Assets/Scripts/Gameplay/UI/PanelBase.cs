using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

// 面板基类
public abstract class PanelBase : MonoBehaviour
{
    protected virtual void Awake()
    {
        // 初始化面板字典，把组件塞进去
        // 组合组件先进入
        SetControlsByType<Button>();
        SetControlsByType<Toggle>();
        SetControlsByType<Slider>();
        SetControlsByType<ScrollRect>();
        SetControlsByType<Dropdown>();
        // 普通组件后进入
        SetControlsByType<TextMeshProUGUI>();
        SetControlsByType<Image>();
    }
    #region 存储子对象组件并添加监听
    // 存储面板上所有组件的字典，按组件种类分类
    public Dictionary<string, UIBehaviour> controls = new Dictionary<string, UIBehaviour>();

    // 组件默认名，用来筛选默认名组件，默认名组件不会进入字典
    static List<string> defaultNames = new List<string>()
    {
        "Image",
        "RawImage",
        "Text (TMP)",
        "Text (Legacy)",
        "Label",
        "Background",
        "Checkmark",
        "Arrow",
        "Placeholder",
        "Fill",
        "Handle",
        "Viewport",
        "Scrollbar Horizontal",
        "Scrollbar Vertical",
    };

    /// <summary>
    /// 寻找子对象组件，塞入字典并按类型设置对应监听
    /// </summary>
    /// <typeparam name="T">需要加入字典的组件种类</typeparam>
    void SetControlsByType<T>() where T : UIBehaviour
    {
        // 得到所有子对象组件
        T[] childrenControls = GetComponentsInChildren<T>(true);
        // 遍历填充
        for (int i = 0; i < childrenControls.Length; i++)
        {
            string currentName = childrenControls[i].gameObject.name;
            // 筛选默认名
            if (!defaultNames.Contains(currentName))
            {
                // 如果字典没有同名组件，才进入字典，防报错
                // 加入字典前拼接组件类型，防止重复添加，缺点是不能直接用键调值，只能通过方法获取组件
                string keyName = $"{typeof(T)}_{currentName}";
                if (!controls.ContainsKey(keyName))
                {
                    controls.Add(keyName, childrenControls[i]);
                }
                else
                {
                    print(keyName + " 重复添加");
                }
            }
            // 设置监听
            // 因为不经过事件中心所以会自己销毁
            if (childrenControls[i] is Button)
            {
                (childrenControls[i] as Button).onClick.AddListener(() =>
                {
                    ButtonOnClick(currentName);
                });
            }
            else if (childrenControls[i] is Slider)
            {
                (childrenControls[i] as Slider).onValueChanged.AddListener((value) =>
                {
                    SliderOnValueChanged(currentName, value);
                });
            }
            else if (childrenControls[i] is Toggle)
            {
                (childrenControls[i] as Toggle).onValueChanged.AddListener((value) =>
                {
                    ToggleOnValueChanged(currentName, value);
                });
            }
            // 只对时间骰和百搭骰添加这个监听，不然会出现无响应bug
            else if (childrenControls[i] is Image && (currentName.StartsWith("Image_Time") || currentName.StartsWith("Image_Wild")))
            {
                ImageOnClick(childrenControls[i] as Image, currentName);
            }
        }
    }
    // 事件触发器的方法
    void AddEvent(EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> cb)
    {
        // 先检查是否已存在同类型事件，避免重复添加
        foreach (var entry in trigger.triggers)
        {
            if (entry.eventID == type)
            {
                entry.callback.AddListener(cb);
                return;
            }
        }

        // 不存在就新建
        EventTrigger.Entry newEntry = new EventTrigger.Entry();
        newEntry.eventID = type;
        newEntry.callback.AddListener(cb);
        trigger.triggers.Add(newEntry);
    }
    // 图片点击事件
    private void ImageOnClick(Image image, string imageName)
    {
        EventTrigger eventTrigger = image.GetComponent<EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = image.AddComponent<EventTrigger>();
        AddEvent(eventTrigger, EventTriggerType.PointerClick, (data) =>
        {
            PointerEventData pointer = data as PointerEventData;
            switch (pointer.button)
            {
                case PointerEventData.InputButton.Left:
                    ImageOnLeftClick(imageName);
                    break;
                case PointerEventData.InputButton.Right:
                    ImageOnRightClick(imageName);
                    break;
            }
        });
    }
    // 注册鼠标悬浮的提示
    protected void RegisterTooltip<T>(string controlName, string title, string desc) where T : UIBehaviour
    {
        T control = GetControl<T>(controlName);
        if (control == null) return;

        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null) trigger = control.gameObject.AddComponent<EventTrigger>();

        // 鼠标进入 → 显示双文本
        AddEvent(trigger, EventTriggerType.PointerEnter, (d) =>
        {
            TipPanel.Instance.ShowTooltip(title, desc);
        });

        // 鼠标离开 → 隐藏
        AddEvent(trigger, EventTriggerType.PointerExit, (d) =>
        {
            TipPanel.Instance.HideTooltip();
        });
    }
    protected void RegisterTooltip<T>(T control, string title, string desc) where T : UIBehaviour
    {
        if (control == null) return;

        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null) trigger = control.gameObject.AddComponent<EventTrigger>();

        // 鼠标进入 → 显示双文本
        AddEvent(trigger, EventTriggerType.PointerEnter, (d) =>
        {
            TipPanel.Instance.ShowTooltip(title, desc);
        });

        // 鼠标离开 → 隐藏
        AddEvent(trigger, EventTriggerType.PointerExit, (d) =>
        {
            TipPanel.Instance.HideTooltip();
        });
    }

    // 不同组件添加监听的虚方法
    protected virtual void ButtonOnClick(string buttonName) { }
    protected virtual void SliderOnValueChanged(string sliderName, float value) { }
    protected virtual void ToggleOnValueChanged(string toggleName, bool value) { }
    protected virtual void ImageOnLeftClick(string imageName) { }
    protected virtual void ImageOnRightClick(string imageName) { }
    #endregion
    #region 面板显隐时执行的方法
    // 淡入淡出
    private CanvasGroup _canvasGroup;
    protected CanvasGroup canvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
            return _canvasGroup;
        }
    }
    // 淡入淡出速度
    protected float fadeDuration = 0.5f;
    // 隐藏后让管理器销毁自己
    UnityAction hideCallBack;
    protected virtual void AfterRemove() { }
    /// <summary>
    /// 淡入（DOTween实现）
    /// </summary>
    public virtual void ShowSelf()
    {
        this.gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        // 停止当前可能存在的动画，避免叠加
        canvasGroup.DOKill();
        // DOTween淡入到alpha=1
        canvasGroup.DOFade(1, fadeDuration);
    }
    /// <summary>
    /// 淡出
    /// </summary>
    /// <param name="callBack">淡出完成后要执行的委托</param>
    public virtual void HideSelf(UnityAction callBack)
    {
        canvasGroup.alpha = 1;
        hideCallBack = callBack;
        // 停止当前可能存在的动画，避免叠加
        canvasGroup.DOKill();
        // DOTween淡出到alpha=0，完成后执行回调
        canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        {
            hideCallBack?.Invoke();
        });
    }
    #endregion
    #region 其他方法
    /// <summary>
    /// 得到特定名称与特定类型的组件
    /// </summary>
    /// <param name="name">组件名</param>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>特定种类的组件对象，没有返回空</returns>
    public T GetControl<T>(string name) where T : UIBehaviour
    {
        string keyName = $"{typeof(T)}_{name}";
        if (controls.ContainsKey(keyName))
        {
            T control = controls[keyName] as T;
            if (control == null)
            {
                Debug.LogError($"请求的{name}组件存在，但不是请求所需的{typeof(T)}类型，或是面板被移除后事件未完全注销");
            }
            return control;
        }
        else
        {
            Debug.LogError($"请求的{name}组件不存在");
            return null;
        }
    }
    #endregion
}