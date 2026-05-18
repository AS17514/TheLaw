using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
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
        // 监听插入面板事件
        EventCenter.Instance.AddEventListener(E_EventType.UI_Insert_StoryPanelByInsertIndex, InsertStoryPanelByInsertIndex);
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
    public void RemovePanel<T>(UnityAction onDestroyed = null, bool animate = true) where T : PanelBase
    {
        string name = typeof(T).Name;
        if (panels.ContainsKey(name))
        {
            T panel = panels[name] as T;
            if (animate)
            {
                panel.HideSelf(() =>
                {
                    Destroy(panel.gameObject);
                    panels.Remove(name);
                    onDestroyed?.Invoke();
                });
            }
            else
            {
                Destroy(panel.gameObject);
                panels.Remove(name);
                onDestroyed?.Invoke();
            }
        }
    }
    /// <summary>
    /// 删除指定面板，添加指定新面板，加入Loading面板过渡，默认新生成在中层，让前一个面板完全销毁后新建面板
    /// </summary>
    /// <param name="layer">新生成面板的层级</param>
    /// <typeparam name="T">删除的面板</typeparam>
    /// <typeparam name="K">生成的面板</typeparam>
    /// <returns></returns>
    public void ChangePanel<T, K>(E_UILayer layer = E_UILayer.Middle, bool showLoading = true) where T : PanelBase where K : PanelBase
    {
        if (showLoading)
            CreatPanel<LoadingPanel>(E_UILayer.Loading);

        StartCoroutine(DoChangePanelCoroutine<T, K>(layer, showLoading));
    }

    private IEnumerator DoChangePanelCoroutine<T, K>(E_UILayer layer, bool showLoading)
    where T : PanelBase where K : PanelBase
    {
        if (showLoading)
            yield return new WaitForSecondsRealtime(0.6f);

        E_BGM? oldBGM = GetPanel<T>()?.BGMType;

        bool removed = false;
        RemovePanel<T>(onDestroyed: () => removed = true, animate: false);
        while (!removed)
            yield return null;
        CreatPanel<K>(layer);

        E_BGM? newBGM = GetPanel<K>()?.BGMType;

        if (oldBGM != null && oldBGM == newBGM)
            AudioManager.Instance.FadeOutThenPauseBGM();
        else
            AudioManager.Instance.StopBGM();

        if (showLoading)
        {
            yield return new WaitForSecondsRealtime(3f);
            RemovePanel<LoadingPanel>();
        }

        if (newBGM != null)
        {
            if (oldBGM == newBGM)
                AudioManager.Instance.FadeInResumeBGM();
            else
                AudioManager.Instance.PlayBGM(newBGM.Value);
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
    public T GetPanel<T>() where T : PanelBase
    {
        if (panels.ContainsKey(typeof(T).Name))
        {
            return panels[typeof(T).Name] as T;
        }
        return null;
    }
    /// <summary>
    /// 在任何时候插入插入剧情面板
    /// </summary>
    /// <param name="index">插入剧情段的序号，不是索引</param>
    public void InsertStoryPanelByInsertIndex(object obj)
    {
        int index = (int)obj;
        TextAsset data = Resources.Load<TextAsset>($"Story/StoryInsert{index}");
        if (data == null)
        {
            Debug.LogWarning($"未找到插入剧情段{index}文件，加载上次剧情文件");
            return;
        }
        Debug.Log($"读取插入剧情段{index}");
        StoryManager.Instance.storySegment = JsonConvert.DeserializeObject<StorySegment>(data.text);
        // 插入故事就不设置故事段值了
        AudioManager.Instance.PauseBGM();
        CreatPanel<StoryPanel>(E_UILayer.Top);
        AudioManager.Instance.PlayBGM(E_BGM.Story);
    }
    /// <summary>
    /// 随时能用的震屏效果
    /// </summary>
    public void ShakePanel<T>() where T : PanelBase
    {
        T panel = GetPanel<T>();
        if (panel == null) return;
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        // 面板振动效果
        rectTransform.DOShakeAnchorPos(0.3f, 10).OnComplete(() =>
            {
                // 震完回到000
                rectTransform.DOAnchorPos(Vector2.zero, 0.1f);
            });
    }
}
