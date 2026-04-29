using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum E_UILayer
{
    Loading,
    Top,
    Middle,
    Bottom
}

public class UIManager : ManagerBase<UIManager>
{

    private Camera uiCamera;
    private Canvas uiCanvas;
    private EventSystem uiEventSystem;
    #region 层级
    private Transform loadingLayer;
    private Transform topLayer;
    private Transform middleLayer;
    private Transform bottomLayer;
    #endregion
    /// <summary>
    /// // 初始化必要对象及组件
    /// </summary>
    private UIManager()
    {
        // 初始化对象 们
        uiCamera = Object.Instantiate(Resources.Load<Camera>("Prefabs/UI/UI Camera")).GetComponent<Camera>();
        uiCanvas = Object.Instantiate(Resources.Load<Canvas>("Prefabs/UI/Canvas")).GetComponent<Canvas>();
        uiCanvas.worldCamera = uiCamera;
        uiEventSystem = Object.Instantiate(Resources.Load<EventSystem>("Prefabs/UI/EventSystem")).GetComponent<EventSystem>();
        // 设置不移除
        Object.DontDestroyOnLoad(uiCamera);
        Object.DontDestroyOnLoad(uiCanvas);
        Object.DontDestroyOnLoad(uiEventSystem);
        // 添加层级
        loadingLayer = uiCanvas.transform.Find("Loading");
        topLayer = uiCanvas.transform.Find("Top");
        middleLayer = uiCanvas.transform.Find("Middle");
        bottomLayer = uiCanvas.transform.Find("Bottom");
    }
    // 存面板的字典
    public Dictionary<string, PanelBase> panels = new Dictionary<string, PanelBase>();

    /// <summary>
    /// 向字典加入面板
    /// </summary>
    /// <param name="panel">面板对象</param>
    /// <typeparam name="T">面板类型</typeparam>
    public void AddPanel<T>(PanelBase panel) where T : PanelBase
    {
        panels.Add(typeof(T).Name, panel);
    }

    /// <summary>
    /// 创建指定类型的面板、将其加入字典、设置父对象（移动到对应层级）、最后显示面板
    /// </summary>
    /// <param name="layer"></param>
    /// <typeparam name="T"></typeparam>
    public void CreatPanel<T>(E_UILayer layer = E_UILayer.Bottom) where T : PanelBase
    {
        string name = typeof(T).Name;
        if (!panels.ContainsKey(name))
        {
            GameObject panel = Object.Instantiate(Resources.Load<GameObject>($"Prefabs/UI/Panels/{name}"), GetUILayer(layer), false);
            panel.AddComponent<T>();
            T panelComponent = panel.GetComponent<T>();
            panels.Add(name, panelComponent);
        }
        panels[name].ShowSelf();
    }

    /// <summary>
    /// 隐藏指定类型的面板，然后将其删除并从字典中移除
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public void RemovePanel<T>() where T : PanelBase
    {
        string name = typeof(T).Name;
        if (panels.ContainsKey(name))
        {
            T panel = panels[name] as T;
            panel.HideSelf(() => { Object.Destroy(panel.gameObject); });
            panels.Remove(name);
        }
    }
    /// <summary>
    /// 获取面板层级对象的transform组件
    /// </summary>
    /// <param name="layer">层级枚举</param>
    /// <returns>对应层级的transform</returns>
    public Transform GetUILayer(E_UILayer layer)
    {
        switch (layer)
        {
            case E_UILayer.Loading:
                return loadingLayer;
            case E_UILayer.Top:
                return topLayer;
            case E_UILayer.Middle:
                return middleLayer;
            case E_UILayer.Bottom:
                return bottomLayer;
            default:
                return null;
        }
    }
    /// <summary>
    /// 返回对应类型的面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns>有类型返回对应面板，没有返回空</returns>
    public PanelBase GetPanel<T>() where T : PanelBase
    {
        if (panels.ContainsKey(typeof(T).Name))
        {
            return panels[typeof(T).Name] as T;
        }
        return null;
    }
}
