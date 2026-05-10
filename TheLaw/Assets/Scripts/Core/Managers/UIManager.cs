using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

public class UIManager : ManagerMonoBase<UIManager>
{
    public Camera mainCamera;
    public Camera uiCamera;
    Canvas uiCanvas;
    EventSystem uiEventSystem;
    #region 层级
    Transform loadingLayer;
    Transform topLayer;
    Transform middleLayer;
    Transform bottomLayer;
    #endregion
    /// <summary>
    /// // 初始化必要对象及组件
    /// </summary>
    void Awake()
    {
        // 找一下并记录主相机
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        // 初始化对象 们
        uiCamera = Instantiate(Resources.Load<Camera>("Prefabs/UI/UI Camera")).GetComponent<Camera>();
        uiCanvas = Instantiate(Resources.Load<Canvas>("Prefabs/UI/Canvas")).GetComponent<Canvas>();
        uiCanvas.worldCamera = uiCamera;
        uiEventSystem = Instantiate(Resources.Load<EventSystem>("Prefabs/UI/EventSystem")).GetComponent<EventSystem>();
        // 设置不移除
        DontDestroyOnLoad(uiCamera);
        DontDestroyOnLoad(uiCanvas);
        DontDestroyOnLoad(uiEventSystem);
        // 添加层级
        loadingLayer = uiCanvas.transform.Find("Loading");
        topLayer = uiCanvas.transform.Find("Top");
        middleLayer = uiCanvas.transform.Find("Middle");
        bottomLayer = uiCanvas.transform.Find("Bottom");
        // 初始化DOTween
        DOTween.Init();
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
            GameObject panel = Instantiate(Resources.Load<GameObject>($"Prefabs/UI/Panels/{name}"), GetUILayer(layer), false);
            if (panel.GetComponent<T>() == null)
            {
                panel.AddComponent<T>();
            }
            T panelComponent = panel.GetComponent<T>();
            panels.Add(name, panelComponent);
        }
        panels[name].ShowSelf();
    }

    /// <summary>
    /// 隐藏指定类型的面板，然后将其删除并从字典中移除
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public void RemovePanel<T>(UnityAction onDestroyed = null) where T : PanelBase
    {
        string name = typeof(T).Name;
        if (panels.ContainsKey(name))
        {
            T panel = panels[name] as T;
            panel.HideSelf(() =>
            {
                Destroy(panel.gameObject);
                panels.Remove(name);
                onDestroyed?.Invoke();
            });
        }
    }
    /// <summary>
    /// 删除指定面板，添加指定新面板，加入Loading面板过渡，默认新生成在中层，让前一个面板完全销毁后新建面板
    /// </summary>
    /// <param name="layer">新生成面板的层级</param>
    /// <typeparam name="T">删除的面板</typeparam>
    /// <typeparam name="K">生成的面板</typeparam>
    /// <returns></returns>
    // UIManager.cs 内部

    public void ChangePanel<T, K>(E_UILayer layer = E_UILayer.Middle) where T : PanelBase where K : PanelBase
    {
        // 1. 开启 Loading
        CreatPanel<LoadingPanel>(E_UILayer.Loading);

        // 开启协程处理后续逻辑
        StartCoroutine(DoChangePanelCoroutine<T, K>(layer));
    }

    private IEnumerator DoChangePanelCoroutine<T, K>(E_UILayer layer)
    where T : PanelBase where K : PanelBase
    {
        // 1. Loading 淡入
        yield return new WaitForSecondsRealtime(0.6f);

        // 2. 销毁旧的，实例化新的 (这里会卡一下，但 Loading 协程在后台跑)
        // 注意：Instantiate 是同步的，执行时连 Loading 动画都会停一下
        // 这是正常的，代表程序正在全力加载
        RemovePanel<T>();
        CreatPanel<K>(layer);

        // 3. 强制让 Loading 多展示一会儿（比如 1 秒），让玩家看清楚动画
        yield return new WaitForSecondsRealtime(2f);

        // 4. 移除 Loading
        RemovePanel<LoadingPanel>();
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
    public T GetPanel<T>() where T : PanelBase
    {
        if (panels.ContainsKey(typeof(T).Name))
        {
            return panels[typeof(T).Name] as T;
        }
        return null;
    }
}
