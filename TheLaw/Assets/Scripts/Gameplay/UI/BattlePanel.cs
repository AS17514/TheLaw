using System;
using System.Collections.Generic;
using System.Linq;
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
    //  记录一下选中骰列表
    List<DiceBase> selectedDiceList = DiceManager.Instance.selectedDice;
    // 记录一下这是第几关
    int level;
    // 记录玩家修改界面上的参数
    bool isSelectedWildDice;
    int Point
    {
        get
        {
            for (int i = -1; i <= 6; i++)
            {
                if (i == 0)
                {
                    continue;
                }
                if (GetControl<Toggle>($"Toggle_Point{i}").isOn)
                {
                    return i;
                }
            }
            return 0;
        }
    }
    int PartIndex
    {
        get
        {
            for (int i = 0; i <= 4; i++)
            {
                if (GetControl<Toggle>($"Toggle_EntityPart{i}").isOn)
                {
                    return i;
                }
            }
            return -1;
        }
    }
    E_DiceType ActOrMind
    {
        get
        {
            if (GetControl<Toggle>("Toggle_Action").isOn)
            {
                return E_DiceType.Action;
            }
            else if (GetControl<Toggle>("Toggle_Mind").isOn)
            {
                return E_DiceType.Mind;
            }
            else
            {
                return E_DiceType.Wild;
            }
        }
    }
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
    void UpdateSelectedDice()
    {
        Dictionary<E_DiceType, List<DiceBase>> dice = DiceManager.Instance.dicePool;
        Transform content = GetControl<ScrollRect>("Scroll View_SelectedDice").content;
        if (selectedDiceList == null)
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
        foreach (DiceBase item in selectedDiceList)
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
        // 遍历拥有骰子池，若选中骰子池没有对应骰子，将自己的isOn设置为false
        foreach (DiceMark item in GetControl<ScrollRect>("Scroll View_ActionDice").content.GetComponentsInChildren<DiceMark>())
        {
            bool isSelected = false;
            foreach (DiceMark diceMark in content.GetComponentsInChildren<DiceMark>())
            {
                if (diceMark.mark == item.mark)
                {
                    isSelected = true;
                    break;
                }
            }
            if (!isSelected)
            {
                item.GetComponentInParent<Toggle>().isOn = false;
            }
        }
        foreach (DiceMark item in GetControl<ScrollRect>("Scroll View_MindDice").content.GetComponentsInChildren<DiceMark>())
        {
            bool isSelected = false;
            foreach (DiceMark diceMark in content.GetComponentsInChildren<DiceMark>())
            {
                if (diceMark.mark == item.mark)
                {
                    isSelected = true;
                    break;
                }
            }
            if (!isSelected)
            {
                item.GetComponentInParent<Toggle>().isOn = false;
            }
        }
    }
    // 更新行动/思维骰
    void UpdateDice<T>(List<DiceBase> Dice) where T : DiceBase, new()
    {
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
                    if (!selectedDiceList.Contains(currentItem))
                    {
                        selectedDiceList.Add(currentItem);
                    }
                    DiceManager.Instance.SortSelectedByValue();
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice, selectedDiceList);
                }
                else
                {
                    for (int i = selectedDiceList.Count - 1; i >= 0; i--)
                    {
                        if (selectedDiceList[i] == currentItem)
                        {
                            selectedDiceList.RemoveAt(i);
                            DiceManager.Instance.SortSelectedByValue();
                            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice, selectedDiceList);
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
    void UpdateTimeDiceCount()
    {
        Dictionary<E_DiceType, List<DiceBase>> pool = DiceManager.Instance.dicePool;
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice1Count").text = pool[E_DiceType.Time1].Count.ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice2Count").text = pool[E_DiceType.Time2].Count.ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice3Count").text = pool[E_DiceType.Time3].Count.ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice4Count").text = pool[E_DiceType.Time4].Count.ToString();
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
    void UpdateTimeDiceSelectedCount()
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice1SelectedCount").text = DiceManager.Instance.GetSelectedTime1DiceCount().ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice2SelectedCount").text = DiceManager.Instance.GetSelectedTime2DiceCount().ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice3SelectedCount").text = DiceManager.Instance.GetSelectedTime3DiceCount().ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice4SelectedCount").text = DiceManager.Instance.GetSelectedTime4DiceCount().ToString();
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
            else if (!part.IsCouldBeAttacked())
            {
                GetControl<Toggle>($"Toggle_EntityPart{index}").interactable = false;
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
    void UpdateEvents()
    {
        Dictionary<E_OptionType, OptionBase[]> eventDic = EventManager.Instance.optionPool;
        Transform grid = GetControl<ScrollRect>("Scroll View_Event").content.GetChild(0);
        foreach (Transform item in grid)
        {
            Destroy(item.gameObject);
        }
        int index = 0;
        foreach (OptionBase option in eventDic[(E_OptionType)(level + 2)])
        {
            int eventIndex = index;
            if (option.IsVisible)
            {
                GameObject eventObj = Instantiate<GameObject>(resources["Event"], grid);
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
                if (option.DiceCost.Count() == 0)
                {
                    continue;
                }
                foreach (DiceCondition item in option.DiceCost)
                {
                    GameObject dieObj = Instantiate<GameObject>(resources[$"Event_{item.type}Dice_{item.mode}"], content);
                    if (!(item.mode == E_CompareType.Any))
                    {
                        dieObj.GetComponentInChildren<TextMeshProUGUI>().text = item.value.ToString();
                    }
                }
                eventObj.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    Debug.Log($"执行{eventIndex}号事件");
                    EventManager.Instance.ExcuteOption((E_OptionType)(level + 2), eventIndex);
                });
            }
            index++;
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

    #region 注册事件专用有名方法
    void OnUpdatePhase(object obj)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_Phase").text = ((int)obj + 1).ToString();
    }
    void OnUpdateTimeProgress(object obj)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeProgress").text = ((int)obj).ToString();
        Slider Slider_TimeProgress = GetControl<Slider>("Slider_TimeProgress");
        Slider_TimeProgress.value = (int)obj;
    }
    void OnUpdateMaxTimeProgress(object obj)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_MaxTimeProgress").text = ((int)obj).ToString();
        Slider Slider_TimeProgress = GetControl<Slider>("Slider_TimeProgress");
        Slider_TimeProgress.maxValue = (int)obj;
    }
    void OnUpdateTimeDicePerPhase(object obj)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDicePerPhase").text = ((int)obj).ToString();
    }
    void OnUpdateSelectedDice(object obj)
    {
        UpdateSelectedDice();
    }
    void OnUpdateActionDice(object obj)
    {
        UpdateDice<ActionDice>((List<DiceBase>)obj);
    }
    void OnUpdateMindDice(object obj)
    {
        UpdateDice<MindDice>((List<DiceBase>)obj);
    }
    void OnUpdateTimeDiceCount(object obj)
    {
        UpdateTimeDiceCount();
    }
    void OnUpdateTimeDiceSelectedCount(object obj)
    {
        UpdateTimeDiceSelectedCount();
    }
    void OnUpdateTimeDice(object obj)
    {
        UpdateTimeDiceCount();
        UpdateTimeDiceSelectedCount();
    }
    void OnUpdateTimeDice1Count(object obj)
    {
        UpdateTimeDiceCount(E_DiceType.Time1, (int)obj);
    }
    void OnUpdateTimeDice2Count(object obj)
    {
        UpdateTimeDiceCount(E_DiceType.Time2, (int)obj);
    }
    void OnUpdateTimeDice3Count(object obj)
    {
        UpdateTimeDiceCount(E_DiceType.Time3, (int)obj);
    }
    void OnUpdateTimeDice4Count(object obj)
    {
        UpdateTimeDiceCount(E_DiceType.Time4, (int)obj);
    }
    void OnUpdateTimeDice1SelectedCount(object obj)
    {
        UpdateTimeDiceSelectedCount(E_DiceType.Time1, (int)obj);
    }
    void OnUpdateTimeDice2SelectedCount(object obj)
    {
        UpdateTimeDiceSelectedCount(E_DiceType.Time2, (int)obj);
    }
    void OnUpdateTimeDice3SelectedCount(object obj)
    {
        UpdateTimeDiceSelectedCount(E_DiceType.Time3, (int)obj);
    }
    void OnUpdateTimeDice4SelectedCount(object obj)
    {
        UpdateTimeDiceSelectedCount(E_DiceType.Time4, (int)obj);
    }
    void OnUpdateWildDiceCount(object obj)
    {
        UpdateWildDice((int)obj);
    }
    void OnUpdateEntityDice(object obj)
    {
        UpdateEntityDice((List<EntityDice>)obj);
    }
    void OnUpdatePlayerHP(object obj)
    {
        UpdatePlayerHP((int)obj);
    }
    void OnUpdatePlayerBuff(object obj)
    {
        UpdatePlayerBuff((Dictionary<E_BuffType, int>)obj);
    }
    void OnUpdatePlayerDied(object obj)
    {
        UIManager.Instance.CreatPanel<DiePanel>(E_UILayer.Top);
    }
    void OnUpdateEntityState(object obj)
    {
        UpdateEntityState((Enum)obj);
    }
    void OnUpdateEntityAction(object obj)
    {
        UpdateEntityAction((E_IntentType)obj);
    }
    void OnUpdateEntityWish(object obj)
    {
        UpdateEntityWish((E_DesireType)obj);
    }
    void OnUpdateEntityBuff(object obj)
    {
        UpdateEntityBuff((Dictionary<E_BuffType, int>)obj);
    }
    void OnUpdateEntityPart(object obj)
    {
        UpdateEntityPart((CharacterBase[])obj);
    }
    void OnUpdateEntityHP(object obj)
    {
        UpdateEntityHP((int)obj);
    }
    void OnUpdateEntityDied(object obj)
    {
        UIManager.Instance.ChangePanel<BattlePanel, StartMenuPanel>();
    }
    void OnUpdateEvents(object obj)
    {
        UpdateEvents();
    }
    void OnUpdateWishToAvailable(object obj)
    {
        UnlockedWish();
    }
    void OnUpdateWishToUnavailable(object obj)
    {
        LockWish();
    }
    void OnUpdateIsConditionNotMet(object obj)
    {
        Debug.LogWarning("判定未通过");
    }
    #endregion
    // 注册事件
    void InitEvents()
    {
        EventCenter eventCenter = EventCenter.Instance;

        #region Progress
        eventCenter.AddEventListener(E_EventType.UI_Update_Phase, OnUpdatePhase);
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeProgress, OnUpdateTimeProgress);
        eventCenter.AddEventListener(E_EventType.UI_Update_MaxTimeProgress, OnUpdateMaxTimeProgress);
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDicePerPhase, OnUpdateTimeDicePerPhase);
        #endregion

        #region Dice
        eventCenter.AddEventListener(E_EventType.UI_Update_SelectedDice, OnUpdateSelectedDice);
        eventCenter.AddEventListener(E_EventType.UI_Update_ActionDice, OnUpdateActionDice);
        eventCenter.AddEventListener(E_EventType.UI_Update_MindDice, OnUpdateMindDice);
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDiceCount, OnUpdateTimeDiceCount);
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDiceSelectedCount, OnUpdateTimeDiceSelectedCount);
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDice, OnUpdateTimeDice);
        eventCenter.AddEventListener(E_EventType.UI_Update_WildDiceCount, OnUpdateWildDiceCount);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityDice, OnUpdateEntityDice);
        #endregion

        #region Player
        eventCenter.AddEventListener(E_EventType.UI_Update_PlayerHP, OnUpdatePlayerHP);
        eventCenter.AddEventListener(E_EventType.UI_Update_PlayerBuff, OnUpdatePlayerBuff);
        eventCenter.AddEventListener(E_EventType.UI_Update_PlayerDied, OnUpdatePlayerDied);
        #endregion

        #region Entity
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityState, OnUpdateEntityState);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityAction, OnUpdateEntityAction);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityWish, OnUpdateEntityWish);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityBuff, OnUpdateEntityBuff);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityPart, OnUpdateEntityPart);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityHP, OnUpdateEntityHP);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityDied, OnUpdateEntityDied);
        #endregion

        eventCenter.AddEventListener(E_EventType.UI_Update_Events, OnUpdateEvents);

        #region Wish
        eventCenter.AddEventListener(E_EventType.UI_Update_WishToAvailable, OnUpdateWishToAvailable);
        eventCenter.AddEventListener(E_EventType.UI_Update_WishToUnavailable, OnUpdateWishToUnavailable);
        #endregion

        eventCenter.AddEventListener(E_EventType.UI_Update_IsConditionNotMet, OnUpdateIsConditionNotMet);
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
        UpdateTimeDiceCount(E_DiceType.Time1, DiceManager.Instance.dicePool[E_DiceType.Time1].Count);
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
        UpdateEvents();
    }
    // 面板移除时同时移除监听
    void OnDestroy()
    {
        EventCenter eventCenter = EventCenter.Instance;

        #region Progress
        eventCenter.RemoveEventListener(E_EventType.UI_Update_Phase, OnUpdatePhase);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_TimeProgress, OnUpdateTimeProgress);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_MaxTimeProgress, OnUpdateMaxTimeProgress);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_TimeDicePerPhase, OnUpdateTimeDicePerPhase);
        #endregion

        #region Dice
        eventCenter.RemoveEventListener(E_EventType.UI_Update_SelectedDice, OnUpdateSelectedDice);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_ActionDice, OnUpdateActionDice);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_MindDice, OnUpdateMindDice);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_TimeDiceCount, OnUpdateTimeDiceCount);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_TimeDiceSelectedCount, OnUpdateTimeDiceSelectedCount);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_TimeDice, OnUpdateTimeDice);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_WildDiceCount, OnUpdateWildDiceCount);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityDice, OnUpdateEntityDice);
        #endregion

        #region Player
        eventCenter.RemoveEventListener(E_EventType.UI_Update_PlayerHP, OnUpdatePlayerHP);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_PlayerBuff, OnUpdatePlayerBuff);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_PlayerDied, OnUpdatePlayerDied);
        #endregion

        #region Entity
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityState, OnUpdateEntityState);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityAction, OnUpdateEntityAction);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityWish, OnUpdateEntityWish);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityBuff, OnUpdateEntityBuff);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityPart, OnUpdateEntityPart);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityHP, OnUpdateEntityHP);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityDied, OnUpdateEntityDied);
        #endregion

        eventCenter.RemoveEventListener(E_EventType.UI_Update_Events, OnUpdateEvents);

        #region Wish
        eventCenter.RemoveEventListener(E_EventType.UI_Update_WishToAvailable, OnUpdateWishToAvailable);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_WishToUnavailable, OnUpdateWishToUnavailable);
        #endregion

        eventCenter.RemoveEventListener(E_EventType.UI_Update_IsConditionNotMet, OnUpdateIsConditionNotMet);
    }
    protected override void ButtonOnClick(string buttonName)
    {
        int point = Point;
        int partIndex = PartIndex;
        switch (buttonName)
        {
            #region 固有行动
            case "Button_InherentAction_Prepare":
                if (ActOrMind == E_DiceType.Wild)
                {
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                    break;
                }
                SkillManager.ExcuteSkills(0, new PrepareOptionContext { diceType = ActOrMind });
                break;
            case "Button_InherentAction_Adjust":
                if (point == -1 || point == 1)
                {
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                    break;
                }
                SkillManager.ExcuteSkills(1, new AdjustOptionContext { change = point });
                break;
            case "Button_InherentAction_Overturn":
                foreach (DiceBase item in selectedDiceList)
                {
                    if (item.type == E_DiceType.Time4)
                    {
                        UpdateSelectedDice();
                        UpdateTimeDiceSelectedCount();
                        Debug.LogWarning("推翻操作选择的时间骰点数不能为4");
                        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        break;
                    }
                }
                SkillManager.ExcuteSkills(2);
                break;
            case "Button_InherentAction_Atk":
                if (isSelectedWildDice || partIndex == -1)
                {
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                    break;
                }
                SkillManager.ExcuteSkills(3, new AtkOptionContext { index = partIndex });
                break;
            #endregion
            #region 律
            case "Button_Law_ChantingLaw":
                SkillManager.ExcuteSkills(0);
                break;
            case "Button_Law_GunArt3":
                SkillManager.ExcuteSkills(1);
                break;
            case "Button_Law_ShatteredStars":
                SkillManager.ExcuteSkills(2);
                break;
            #endregion
            #region 许愿
            case "Button_Wish_Abundance":
                SkillManager.ExcuteSkills(0);
                break;
            case "Button_Wish_Vibrancy":
                SkillManager.ExcuteSkills(1);
                break;
            case "Button_Wish_Null3":
                SkillManager.ExcuteSkills(2);
                break;
            case "Button_Wish_Null4":
                SkillManager.ExcuteSkills(3);
                break;
            #endregion
            #region Other
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
            #endregion
        }
    }
    protected override void ImageOnLeftClick(string imageName)
    {
        switch (imageName)
        {
            case "Image_Time1Dice":
                int selectedTime1Num = DiceManager.Instance.GetSelectedTime1DiceCount();
                List<DiceBase> Time1List = DiceManager.Instance.dicePool[E_DiceType.Time1];
                if (selectedTime1Num >= Time1List.Count)
                {
                    return;
                }
                foreach (DiceBase item in selectedDiceList.ToList())
                {
                    if (item.type == E_DiceType.Time1)
                    {
                        selectedDiceList.Remove(item);
                    }
                }
                for (int i = Time1List.Count - 1; i >= Time1List.Count - 1 - selectedTime1Num; i--)
                {
                    selectedDiceList.Add(Time1List[i]);
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDiceSelectedCount);
                break;
            case "Image_Time2Dice":
                int selectedTime2Num = DiceManager.Instance.GetSelectedTime2DiceCount();
                List<DiceBase> Time2List = DiceManager.Instance.dicePool[E_DiceType.Time2];
                if (selectedTime2Num >= Time2List.Count)
                {
                    return;
                }
                foreach (DiceBase item in selectedDiceList.ToList())
                {
                    if (item.type == E_DiceType.Time2)
                    {
                        selectedDiceList.Remove(item);
                    }
                }
                for (int i = Time2List.Count - 1; i >= Time2List.Count - 1 - selectedTime2Num; i--)
                {
                    selectedDiceList.Add(Time2List[i]);
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDiceSelectedCount);
                break;
            case "Image_Time3Dice":
                int selectedTime3Num = DiceManager.Instance.GetSelectedTime3DiceCount();
                List<DiceBase> Time3List = DiceManager.Instance.dicePool[E_DiceType.Time3];
                if (selectedTime3Num >= Time3List.Count)
                {
                    return;
                }
                foreach (DiceBase item in selectedDiceList.ToList())
                {
                    if (item.type == E_DiceType.Time3)
                    {
                        selectedDiceList.Remove(item);
                    }
                }
                for (int i = Time3List.Count - 1; i >= Time3List.Count - 1 - selectedTime3Num; i--)
                {
                    selectedDiceList.Add(Time3List[i]);
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDiceSelectedCount);
                break;
            case "Image_Time4Dice":
                int selectedTime4Num = DiceManager.Instance.GetSelectedTime4DiceCount();
                List<DiceBase> Time4List = DiceManager.Instance.dicePool[E_DiceType.Time4];
                if (selectedTime4Num >= Time4List.Count)
                {
                    return;
                }
                foreach (DiceBase item in selectedDiceList.ToList())
                {
                    if (item.type == E_DiceType.Time4)
                    {
                        selectedDiceList.Remove(item);
                    }
                }
                for (int i = Time4List.Count - 1; i >= Time4List.Count - 1 - selectedTime4Num; i--)
                {
                    selectedDiceList.Add(Time4List[i]);
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDiceSelectedCount);
                break;
            default:
                return;
        }

    }
    protected override void ImageOnRightClick(string imageName)
    {
        switch (imageName)
        {
            case "Image_Time1Dice":
                if (DiceManager.Instance.GetSelectedTime1DiceCount() <= 0)
                {
                    return;
                }
                foreach (DiceBase item in selectedDiceList)
                {
                    if (item.type == E_DiceType.Time1)
                    {
                        selectedDiceList.Remove(item);
                        break;
                    }
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDiceSelectedCount);
                break;
            case "Image_Time2Dice":
                if (DiceManager.Instance.GetSelectedTime2DiceCount() <= 0)
                {
                    return;
                }
                foreach (DiceBase item in selectedDiceList)
                {
                    if (item.type == E_DiceType.Time2)
                    {
                        selectedDiceList.Remove(item);
                        break;
                    }
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDiceSelectedCount);
                break;
            case "Image_Time3Dice":
                if (DiceManager.Instance.GetSelectedTime3DiceCount() <= 0)
                {
                    return;
                }
                foreach (DiceBase item in selectedDiceList)
                {
                    if (item.type == E_DiceType.Time3)
                    {
                        selectedDiceList.Remove(item);
                        break;
                    }
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDiceSelectedCount);
                break;
            case "Image_Time4Dice":
                if (DiceManager.Instance.GetSelectedTime4DiceCount() <= 0)
                {
                    return;
                }
                foreach (DiceBase item in selectedDiceList)
                {
                    if (item.type == E_DiceType.Time4)
                    {
                        selectedDiceList.Remove(item);
                        break;
                    }
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDiceSelectedCount);
                break;
            default:
                return;
        }
    }
    protected override void ToggleOnValueChanged(string toggleName, bool value)
    {

        switch (toggleName)
        {
            case "Toggle_WildDice":
                isSelectedWildDice = value;
                if (value)
                {
                    foreach (WildDice item in DiceManager.Instance.dicePool[E_DiceType.Wild])
                    {
                        selectedDiceList.Add(item);
                    }
                }
                else
                {
                    foreach (DiceBase item in selectedDiceList)
                    {
                        if (item.type == E_DiceType.Wild)
                        {
                            selectedDiceList.Remove(item);
                        }
                    }
                }
                break;
            default:
                return;
        }
    }
    void Start()
    {
        level = ProgressManager.Instance.level;
        LoadAllResources();
        InitEvents();
        Init();
    }
}