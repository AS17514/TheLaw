using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 面板基类
public abstract class PanelBase : MonoBehaviour
{
    protected virtual void Awake()
    {
        // 淡入淡出，创建canvasgroup组件
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
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

    protected virtual void Update()
    {
        // 淡入淡出
        if (isShow && canvasGroup.alpha != 1)
        {
            canvasGroup.alpha += alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
            }
        }
        else if (!isShow && canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                // 完全隐藏后执行销毁委托
                hideCallBack?.Invoke();
            }
        }
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
        }
    }
    // 不同组件添加监听的虚方法
    protected virtual void ButtonOnClick(string buttonName) { }
    protected virtual void SliderOnValueChanged(string sliderName, float value) { }
    protected virtual void ToggleOnValueChanged(string toggleName, bool value) { }
    #endregion
    #region 面板显隐时执行的方法
    // 淡入淡出
    public bool isShow = false;
    CanvasGroup canvasGroup;
    int alphaSpeed = 2;
    // 隐藏后让管理器销毁自己
    UnityAction hideCallBack;
    protected virtual void AfterRemove()
    {

    }
    /// <summary>
    /// 淡入
    /// </summary>
    public virtual void ShowSelf()
    {
        isShow = true;
        canvasGroup.alpha = 0;
    }
    /// <summary>
    /// 淡出
    /// </summary>
    /// <param name="callBack">淡出完成后要执行的委托</param>
    public virtual void HideSelf(UnityAction callBack)
    {
        isShow = false;
        // 没有淡入一半的情况关掉的（根本没协程），所以还是设置一下alpha为1
        canvasGroup.alpha = 1;
        hideCallBack = callBack;
        hideCallBack += AfterRemove;
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
                Debug.LogError($"请求的{name}组件存在，但不是请求所需的{typeof(T)}类型");
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
