using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 怎么，打不中吗（指找不着bug）
/// </summary>

public class BattlePanel : PanelBase
{
    // 记录一下这是第几关
    int level;
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
    #region Dice
    // 更新选中骰
    void UpdateSelectedDice(List<DiceBase> selectedDice)
    {
        Transform content = GetControl<ScrollRect>("Scroll View_SelectedDice").content;
        if (selectedDice == null)
        {
            Debug.Log("选中骰列表为空");
            return;
        }
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
        if (Dice == null)
        {
            Debug.Log("更新骰列表为空");
            return;
        }
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
        if (entityDice == null)
        {
            Debug.Log("公共骰列表为空");
            return;
        }
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
    #endregion
    #region Player
    void UpdatePlayerHP(int num)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_PlayerHP").text = num.ToString();
        GetControl<Slider>("Slider_PlayerHP").value = num;
    }
    void UpdatePlayerBuff(Dictionary<E_BuffType, int> keyValuePairs)
    {
        Transform content = GetControl<ScrollRect>("Scroll View_PlayerBuff").content;
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
        foreach (KeyValuePair<E_BuffType, int> item in keyValuePairs)
        {
            if (item.Value != 0)
            {
                GameObject buff = Instantiate<GameObject>(resources[$"Buff_{item.Key}"], content);
                buff.GetComponentInChildren<TextMeshProUGUI>().text = item.Value.ToString();
            }
        }
    }
    #endregion
    #region Entity
    void UpdateEntityState(Enum state)
    {
        TextMeshProUGUI entityState = GetControl<TextMeshProUGUI>("Text (TMP)_EntityState");
        if (state is E_StateType_1)
        {
            switch ((E_StateType_1)state)
            {
                case E_StateType_1.normal:
                    entityState.text = "正常";
                    break;
                case E_StateType_1.exhausted:
                    entityState.text = "疲惫";
                    break;
                default:
                    return;
            }
        }
    }
    void UpdateEntityAction(E_IntentType actionType)
    {
        TextMeshProUGUI action = GetControl<TextMeshProUGUI>("Text (TMP)_EntityAction");
        switch (actionType)
        {
            case E_IntentType.Entity1_Atk:
                action.text = "攻击";
                break;
            case E_IntentType.Entity1_Eat:
                action.text = "吃";
                break;
            default:
                return;
        }
    }
    void UpdateEntityWish(E_DesireType wishType)
    {
        TextMeshProUGUI wish = GetControl<TextMeshProUGUI>("Text (TMP)_EntityWish");
        switch (wishType)
        {
            case E_DesireType.Entity1_FilledWithFood:
                wish.text = "装满食物";
                break;
            case E_DesireType.Entity1_Urgent:
                wish.text = "迫切";
                break;
            case E_DesireType.Entity1_Feed:
                wish.text = "进食";
                break;
            default:
                return;
        }
    }
    void UpdateEntityBuff(Dictionary<E_BuffType, int> keyValuePairs)
    {
        Transform content = GetControl<ScrollRect>("Scroll View_EntityBuff").content;
        foreach (Transform item in content)
        {
            Destroy(item.gameObject);
        }
        foreach (KeyValuePair<E_BuffType, int> item in keyValuePairs)
        {
            if (item.Value != 0)
            {
                GameObject buff = Instantiate<GameObject>(resources[$"Buff_{item.Key}"], content);
                buff.GetComponentInChildren<TextMeshProUGUI>().text = item.Value.ToString();
            }
        }
    }
    void UpdateEntityPart(CharacterBase[] Parts)
    {
        for (int index = 1; index < 5; index++)
        {
            Part part = (Part)Parts[index];
            if (part == null)
            {
                ;
                GetControl<Toggle>($"Toggle_EntityPart{index}").interactable = false;
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}Name").text = "???";
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}HP").text = "??";
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}MaxHP").text = "??";
                Slider slider = GetControl<Slider>($"Slider_EntityPart{index}HP");
                slider.maxValue = 1;
                slider.value = 0;
                continue;
            }
            if (part.IsVisible)
            {
                GetControl<Toggle>($"Toggle_EntityPart{index}").interactable = true;
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}Name").text = part.name;
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}HP").text = part.hp.ToString();
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}MaxHP").text = part.maxHp.ToString();
                Slider slider = GetControl<Slider>($"Slider_EntityPart{index}HP");
                slider.maxValue = part.maxHp;
                slider.value = part.hp;
            }
            else if (part.isDestroyed)
            {
                GetControl<Toggle>($"Toggle_EntityPart{index}").interactable = false;
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}Name").text = $"{part.name} (已破坏)";
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}HP").text = part.hp.ToString();
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}MaxHP").text = part.maxHp.ToString();
                Slider slider = GetControl<Slider>($"Slider_EntityPart{index}HP");
                slider.maxValue = part.maxHp;
                slider.value = part.hp;
            }
            else if (!part.IsVisible)
            {
                GetControl<Toggle>($"Toggle_EntityPart{index}").interactable = false;
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}Name").text = "???";
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}HP").text = "??";
                GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}MaxHP").text = "??";
                Slider slider = GetControl<Slider>($"Slider_EntityPart{index}HP");
                slider.maxValue = 1;
                slider.value = 0;
            }
        }
    }
    void UpdateEntityHP(int num)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_EntityHP").text = num.ToString();
        GetControl<Slider>("Slider_EntityHP").value = num;
    }
    #endregion
    // Events
    void UpdateEvents(Dictionary<E_OptionType, OptionBase[]> eventDic)
    {
        Transform grid = GetControl<ScrollRect>("Scroll View_Event").content.GetChild(0);
        ToggleGroup toggleGroup = grid.GetComponent<ToggleGroup>();
        foreach (Transform item in grid)
        {
            Destroy(item.gameObject);
        }
        foreach (OptionBase option in eventDic[(E_OptionType)(level + 2)])
        {
            if (option.IsVisible)
            {
                GameObject eventObj = Instantiate<GameObject>(resources["Event"], grid);
                eventObj.GetComponentInChildren<Toggle>().group = toggleGroup;
                foreach (TextMeshProUGUI item in eventObj.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    switch (item.gameObject.name)
                    {
                        case "Name":
                            item.text = option.OptionName;
                            break;
                        case "Description":
                            item.text = option.OptionDescription;
                            break;
                        default:
                            return;
                    }
                }
                Transform content = eventObj.GetComponentInChildren<ScrollRect>().content;
                foreach (DiceCondition item in option.DiceCost)
                {
                    GameObject dieObj = Instantiate<GameObject>(resources[$"Event_{item.type}Dice_{item.mode}"], content);
                    if (!(item.mode == E_CompareType.Any))
                    {
                        dieObj.GetComponentInChildren<TextMeshProUGUI>().text = item.value.ToString();
                    }
                }
            }
        }
    }
    // 许愿
    void UnlockedWish()
    {
        switch (level)
        {
            case 5:
                Button btn4 = GetControl<Button>("Button_Wish_Null4");
                btn4.interactable = true;
                btn4.GetComponentInChildren<TextMeshProUGUI>().text = "null4";
                goto case 4;
            case 4:
                Button btn3 = GetControl<Button>("Button_Wish_Null3");
                btn3.interactable = true;
                btn3.GetComponentInChildren<TextMeshProUGUI>().text = "null3";
                goto case 3;
            case 3:
                Button btn2 = GetControl<Button>("Button_Wish_Vibrancy");
                btn2.interactable = true;
                btn2.GetComponentInChildren<TextMeshProUGUI>().text = "鲜艳";
                goto case 2;
            case 2:
                Button btn1 = GetControl<Button>("Button_Wish_Abundance");
                btn1.interactable = true;
                btn1.GetComponentInChildren<TextMeshProUGUI>().text = "富足";
                break;
            default:
                return;
        }
    }
    void LockWish()
    {
        Button[] buttons = GetControl<TextMeshProUGUI>("Text (TMP)_Wish").GetComponentsInChildren<Button>();
        foreach (Button item in buttons)
        {
            item.interactable = false;
        }
    }

    void InitEvents()
    {
        EventCenter eventCenter = EventCenter.Instance;
        #region Progress
        // 时间段，似乎需要+1
        eventCenter.AddEventListener(E_EventType.UI_Update_Phase, (obj) =>
        {
            GetControl<TextMeshProUGUI>("Text (TMP)_Phase").text = ((int)obj + 1).ToString();
        });
        // 时间进度
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeProgress, (obj) =>
        {
            GetControl<TextMeshProUGUI>("Text (TMP)_TimeProgress").text = ((int)obj).ToString();
            Slider Slider_TimeProgress = GetControl<Slider>("Slider_TimeProgress");
            Slider_TimeProgress.value = (int)obj;
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_MaxTimeProgress, (obj) =>
        {
            GetControl<TextMeshProUGUI>("Text (TMP)_MaxTimeProgress").text = ((int)obj).ToString();
            Slider Slider_TimeProgress = GetControl<Slider>("Slider_TimeProgress");
            Slider_TimeProgress.maxValue = (int)obj;
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDicePerPhase, (obj) =>
        {
            GetControl<TextMeshProUGUI>("Text (TMP)_TimeDicePerPhase").text = ((int)obj).ToString();
        });
        // 时间骰

        #endregion
        #region Dice
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
        #endregion
        #region Player
        eventCenter.AddEventListener(E_EventType.UI_Update_PlayerHP, (obj) =>
        {
            UpdatePlayerHP((int)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_PlayerBuff, (obj) =>
        {
            UpdatePlayerBuff((Dictionary<E_BuffType, int>)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_PlayerDied, (obj) =>
        {
            UIManager.Instance.CreatPanel<DiePanel>(E_UILayer.Top);
        });
        #endregion
        #region Entity
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityState, (obj) =>
        {
            UpdateEntityState((Enum)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityAction, (obj) =>
        {
            UpdateEntityAction((E_IntentType)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityWish, (obj) =>
        {
            UpdateEntityWish((E_DesireType)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityBuff, (obj) =>
        {
            UpdateEntityBuff((Dictionary<E_BuffType, int>)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityPart, (obj) =>
        {
            UpdateEntityPart((CharacterBase[])obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityHP, (obj) =>
        {
            UpdateEntityHP((int)obj);
        });
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityDied, (obj) =>
        {
            UIManager.Instance.ChangePanel<BattlePanel, StartMenuPanel>();
        });
        #endregion
        // Events
        eventCenter.AddEventListener(E_EventType.UI_Update_Events, (obj) =>
        {
            UpdateEvents((Dictionary<E_OptionType, OptionBase[]>)obj);
        });
        #region Wish
        eventCenter.AddEventListener(E_EventType.UI_Update_WishToAvailable, (obj) =>
            {
                UnlockedWish();
            });
        eventCenter.AddEventListener(E_EventType.UI_Update_WishToUnavailable, (obj) =>
        {
            LockWish();
        });
        #endregion
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
        #region 许愿
        UnlockedWish();
        #endregion
        #region 玩家/骰面板
        UpdateDice<ActionDice>(DiceManager.Instance.dicePool[E_DiceType.Action]);
        UpdateDice<MindDice>(DiceManager.Instance.dicePool[E_DiceType.Mind]);
        UpdatePlayerHP(BuffManager.Instance.player.hp);
        #endregion
        #region Entity
        // hp
        GetControl<TextMeshProUGUI>("Text (TMP)_EntityMaxHP").text = ProgressManager.Instance.nowEntities[0].maxHp.ToString();
        GetControl<Slider>("Slider_EntityHP").maxValue = ProgressManager.Instance.nowEntities[0].maxHp;
        UpdateEntityHP(ProgressManager.Instance.nowEntities[0].hp);
        // other
        UpdateEntityState(StateManager.Instance.UI_currentState);
        UpdateEntityAction(StateManager.Instance.UI_currentExecutableAction);
        UpdateEntityWish(StateManager.Instance.UI_currentExecutableDesire);
        UpdateEntityPart(ProgressManager.Instance.nowEntities);
        UpdateEntityBuff(BuffManager.Instance.entity.UI_buffs);
        #endregion
        // event
        UpdateEvents(EventManager.Instance.optionPool);
    }
    // 面板移除时同时移除监听
    protected override void AfterRemove()
    {
        EventCenter eventCenter = EventCenter.Instance;

        #region Progress
        eventCenter.ClearEventListeners(E_EventType.UI_Update_Phase);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_TimeProgress);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_MaxTimeProgress);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_TimeDicePerPhase);
        #endregion

        #region Dice
        #region 选中、行动、思维
        eventCenter.ClearEventListeners(E_EventType.UI_Update_SelectedDice);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_ActionDice);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_MindDice);
        #endregion

        #region 时间
        eventCenter.ClearEventListeners(E_EventType.UI_Update_TimeDice1Count);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_TimeDice2Count);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_TimeDice3Count);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_TimeDice4Count);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_TimeDice1SelectedCount);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_TimeDice2SelectedCount);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_TimeDice3SelectedCount);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_TimeDice4SelectedCount);
        #endregion

        eventCenter.ClearEventListeners(E_EventType.UI_Update_WildDiceCount);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_EntityDice);
        #endregion

        #region Player
        eventCenter.ClearEventListeners(E_EventType.UI_Update_PlayerHP);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_PlayerBuff);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_PlayerDied);
        #endregion

        #region Entity
        eventCenter.ClearEventListeners(E_EventType.UI_Update_EntityState);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_EntityAction);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_EntityWish);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_EntityBuff);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_EntityPart);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_EntityHP);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_EntityDied);
        #endregion

        eventCenter.ClearEventListeners(E_EventType.UI_Update_Events);

        #region Wish
        eventCenter.ClearEventListeners(E_EventType.UI_Update_WishToAvailable);
        eventCenter.ClearEventListeners(E_EventType.UI_Update_WishToUnavailable);
        #endregion
    }
    protected override void ButtonOnClick(string buttonName)
    {
        switch (buttonName)
        {
            // Other
            case "Button_Explanation":
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
                break;
            case "Button_Settings":
                UIManager.Instance.CreatPanel<SettingsPanel>(E_UILayer.Top);
                break;
            case "Button_ReplayLevel":
                ProgressManager.Instance.intoNewLevel(level);
                UIManager.Instance.ChangePanel<BattlePanel, BattlePanel>();
                break;
            case "Button_ExitLevel":
                UIManager.Instance.ChangePanel<BattlePanel, StartMenuPanel>();
                break;
            default:
                return;
        }
    }
    public void Start()
    {
        #region 假装往管理器里塞了东西
        DiceManager.Instance.AddTimeDice(4);
        DiceManager.Instance.AddDice(E_DiceType.Action, new ActionDice());
        DiceManager.Instance.AddDice(E_DiceType.Action, new ActionDice());
        DiceManager.Instance.AddDice(E_DiceType.Mind, new MindDice());
        #endregion
        // 初始化所有东西
        level = ProgressManager.Instance.level;
        LoadAllResources();
        InitEvents();
        Init();
    }
}