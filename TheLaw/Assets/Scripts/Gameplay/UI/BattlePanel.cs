using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 布豪，终于要开始写这一坨了吗
/// </summary>
enum E_UI_Dice
{
    // 选中的骰子
    // 正常骰子
}

public class BattlePanel : PanelBase
{
    // 资源加载
    Dictionary<string, GameObject> resources = new Dictionary<string, GameObject>();
    void LoadAllResources()
    {
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/UI/Battle");
        foreach (GameObject gameObject in gameObjects)
        {
            if (!resources.ContainsKey(gameObject.name))
            {
                resources.Add(gameObject.name, gameObject);
            }
        }
    }
    // 更新选中骰
    void UpdateSelectedDice(List<DiceBase> selectedDice)
    {
        Transform content = GetControl<ScrollRect>("Scroll View_SelectedDice").content;
        // 清除ui上所有选中骰
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
        // 重新生成一遍，时间和百搭不生成
        foreach (DiceBase item in selectedDice)
        {
            switch (item.type)
            {
                // 生成、初始化数据、同时添加删除对象的监听
                case E_DiceType.Action:
                    Button buttonAction = Instantiate(resources["SelectedActionDice"], content).GetComponentInChildren<Button>();
                    buttonAction.GetComponentInChildren<TextMeshProUGUI>().text = item.value.ToString();
                    buttonAction.onClick.AddListener(() => { Destroy(buttonAction.transform.parent.gameObject); });
                    DiceMark actBase = buttonAction.AddComponent<DiceMark>();
                    actBase.mark = item;
                    buttonAction.onClick.AddListener(() =>
                    {
                        foreach (Toggle actDie in GetControl<ScrollRect>("Scroll View_ActionDice").content.GetComponentsInChildren<Toggle>())
                        {
                            if (actDie.GetComponent<DiceMark>().mark == item)
                            {
                                actDie.isOn = false;
                            }
                        }
                    });
                    break;
                case E_DiceType.Mind:
                    Button buttonMind = Instantiate(resources["SelectedMindDice"], content).GetComponentInChildren<Button>();
                    buttonMind.GetComponentInChildren<TextMeshProUGUI>().text = item.value.ToString();
                    buttonMind.onClick.AddListener(() => { Destroy(buttonMind.transform.parent.gameObject); });
                    Transform mindContent = GetControl<ScrollRect>($"Scroll View_MindDice").content;
                    DiceMark mindBase = buttonMind.AddComponent<DiceMark>();
                    mindBase.mark = item;
                    buttonMind.onClick.AddListener(() =>
                    {
                        foreach (Toggle mindDie in GetControl<ScrollRect>("Scroll View_MindDice").content.GetComponentsInChildren<Toggle>())
                        {

                            if (mindDie.GetComponent<DiceMark>().mark == item)
                            {
                                mindDie.isOn = false;
                            }
                        }
                    });
                    break;
            }
        }
    }
    // 更新行动/思维骰
    void UpdateDice<T>(List<DiceBase> Dice) where T : DiceBase, new()
    {
        List<DiceBase> selectedList = DiceManager.Instance.selectedDice;
        Transform content = GetControl<ScrollRect>($"Scroll View_{typeof(T).Name}").content;
        // 清除ui上所有骰
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
        // 重新生成一遍
        foreach (T item in Dice)
        {
            T currentItem = item;
            Toggle toggle = Instantiate(resources[typeof(T).Name], content).GetComponentInChildren<Toggle>();
            DiceMark toggleDie = toggle.AddComponent<DiceMark>();
            toggleDie.mark = item;
            toggle.GetComponentInChildren<TextMeshProUGUI>().text = currentItem.value.ToString();
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    if (!selectedList.Contains(currentItem))
                    {
                        selectedList.Add(currentItem);
                    }
                    DiceManager.Instance.SortSelectedByValue();
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice, selectedList);
                }
                else
                {
                    for (int i = selectedList.Count - 1; i >= 0; i--)
                    {
                        if (selectedList[i] == currentItem)
                        {
                            selectedList.RemoveAt(i);
                            DiceManager.Instance.SortSelectedByValue();
                            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice, selectedList);
                            break;
                        }
                    }
                }
            });
        }
    }
    // 时间骰拥有与选择个数
    void UpdateTimeDiceCount(E_DiceType e_DiceType, int num)
    {
        switch (e_DiceType)
        {
            case E_DiceType.Time1:
                GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice1Count").text = num.ToString();
                break;
            case E_DiceType.Time2:
                GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice2Count").text = num.ToString();
                break;
            case E_DiceType.Time3:
                GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice3Count").text = num.ToString();
                break;
            case E_DiceType.Time4:
                GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice4Count").text = num.ToString();
                break;
            default:
                return;
        }
    }
    void UpdateTimeDiceSelectedCount(E_DiceType e_DiceType, int num)
    {
        switch (e_DiceType)
        {
            case E_DiceType.Time1:
                GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice1SelectedCount").text = num.ToString();
                break;
            case E_DiceType.Time2:
                GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice2SelectedCount").text = num.ToString();
                break;
            case E_DiceType.Time3:
                GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice3SelectedCount").text = num.ToString();
                break;
            case E_DiceType.Time4:
                GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice4SelectedCount").text = num.ToString();
                break;
            default:
                return;
        }
    }
    // 百搭骰个数
    void UpdateWildDice(int num)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_WildDiceCount").text = num.ToString();
    }
    // 公共骰盘情况
    void UpdateEntityDice(List<EntityDice> entityDice)
    {
        Transform content = GetControl<ScrollRect>("Scroll View_PublicDice").content;
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
        foreach (EntityDice entityDie in entityDice)
        {
            TextMeshProUGUI tmp = Instantiate<GameObject>(resources[$"EntityDice"], content).GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = entityDie.value.ToString();
        }
    }

    void InitEvents()
    {
        EventCenter eventCenter = EventCenter.Instance;
        #region 选中、行动、思维
        // 更新选中骰
        eventCenter.AddEventListener(E_EventType.UI_Update_SelectedDice, (obj) =>
        {
            UpdateSelectedDice((List<DiceBase>)obj);
        });
        // 更新行动骰
        eventCenter.AddEventListener(E_EventType.UI_Update_ActionDice, (obj) =>
        {
            UpdateDice<ActionDice>((List<DiceBase>)obj);
        });
        // 更新思维骰
        eventCenter.AddEventListener(E_EventType.UI_Update_MindDice, (obj) =>
        {
            UpdateDice<MindDice>((List<DiceBase>)obj);
        });
        #endregion
        #region 时间
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDice1Count, (obj) =>
        {
            UpdateTimeDiceCount(E_DiceType.Time1, (int)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDice2Count, (obj) =>
        {
            UpdateTimeDiceCount(E_DiceType.Time2, (int)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDice3Count, (obj) =>
        {
            UpdateTimeDiceCount(E_DiceType.Time3, (int)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDice4Count, (obj) =>
        {
            UpdateTimeDiceCount(E_DiceType.Time4, (int)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDice1SelectedCount, (obj) =>
        {
            UpdateTimeDiceSelectedCount(E_DiceType.Time1, (int)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDice2SelectedCount, (obj) =>
        {
            UpdateTimeDiceSelectedCount(E_DiceType.Time2, (int)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDice3SelectedCount, (obj) =>
        {
            UpdateTimeDiceSelectedCount(E_DiceType.Time3, (int)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDice4SelectedCount, (obj) =>
        {
            UpdateTimeDiceSelectedCount(E_DiceType.Time4, (int)obj);
        });
        #endregion
        // 百搭
        eventCenter.AddEventListener(E_EventType.UI_Update_WildDiceCount, (obj) =>
        {
            UpdateWildDice((int)obj);
        });
        // 公共
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityDice, (obj) =>
        {
            UpdateEntityDice((List<EntityDice>)obj);
        });
    }
    void Init()
    {
        #region 进度
        // 时间段，似乎需要+1
        GetControl<TextMeshProUGUI>("Text (TMP)_Phase").text = (ProgressManager.Instance.phase + 1).ToString();
        // 时间进度
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeProgress").text = ProgressManager.Instance.timeProgress.ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_MaxTimeProgress").text = ProgressManager.Instance.initialTimeProgress.ToString();
        Slider Slider_TimeProgress = GetControl<Slider>("Slider_TimeProgress");
        Slider_TimeProgress.maxValue = ProgressManager.Instance.initialTimeProgress;
        Slider_TimeProgress.value = ProgressManager.Instance.timeProgress;
        // 时间骰
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDicePerPhase").text = ProgressManager.Instance.phaseDice.ToString();
        #endregion
        #region 玩家骰面板
        UpdateDice<ActionDice>(DiceManager.Instance.dicePool[E_DiceType.Action]);
        UpdateDice<MindDice>(DiceManager.Instance.dicePool[E_DiceType.Mind]);
        UpdateEntityDice(DiceManager.Instance.entityDicePool);
        #endregion
    }

    protected override void Awake()
    {
        #region 假装往管理器里塞了东西
        ProgressManager.Instance.intoNewLevel(1);
        DiceManager.Instance.ClearPool();
        DiceManager.Instance.ClearSelected();
        DiceManager.Instance.AddTimeDice(4);
        DiceManager.Instance.AddDice(E_DiceType.Action, new ActionDice());
        DiceManager.Instance.AddDice(E_DiceType.Action, new ActionDice());
        DiceManager.Instance.AddDice(E_DiceType.Mind, new MindDice());
        DiceManager.Instance.ClearEntityPool();
        DiceManager.Instance.AddEntityDice();
        DiceManager.Instance.AddEntityDice();
        #endregion
        base.Awake();
        // 初始化所有东西
        LoadAllResources();
        Init();
        InitEvents();
    }
}