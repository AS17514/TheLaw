using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 怎么，打不中吗（指找不着bug）
/// </summary>

public class PlayerTips
{
    // 固有
    public List<List<string>> inherentActions;
    // 律
    public List<List<string>> laws;
    // 许愿
    public List<List<string>> wishes;
    // buff
    public List<List<List<string>>> buffs;
}
public class EntityTips
{
    public string name;
    // 状态
    public List<List<string>> states;
    // buff
    public List<List<string>> buffs;
    // 事件
    public List<List<string>> events;
}
public class EntityActionTips
{
    // 行动
    public List<List<string>> actions;
}
public class EntityDesireTips
{
    // 许愿
    public List<List<string>> wishes;
}
public class EntityPartTips
{
    // 部位
    public List<List<string>> parts;
}
public class BattlePanel : PanelBase
{
    public override E_BGM? BGMType => E_BGM.Battle;

    //  记录一下选中骰列表
    List<DiceBase> selectedDiceList = DiceManager.Instance.selectedDice;
    RectTransform rectTransform;
    // 记录对应关卡文本
    PlayerTips playerTips;
    EntityTips entityTips;
    EntityActionTips entityActionTips;
    EntityDesireTips entityDesireTips;
    EntityPartTips entityPartTips;
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
                if (i == 0) continue;
                if (GetControl<Toggle>($"Toggle_Point{i}").isOn) return i;
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
                    if (level == 4 && i >= 1) return i - 1;
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
            if (GetControl<Toggle>("Toggle_Action").isOn) return E_DiceType.Action;
            if (GetControl<Toggle>("Toggle_Mind").isOn) return E_DiceType.Mind;
            return E_DiceType.Wild;
        }
    }

    // 资源加载
    Dictionary<string, GameObject> resources = new Dictionary<string, GameObject>();
    Dictionary<string, Sprite> spriteResources = new Dictionary<string, Sprite>();

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        level = ProgressManager.Instance.level;
        LoadAllResources();
        InitEvents();
        Init();
        InitTipsRegist();
    }

    void LoadAllResources()
    {
        UnityEngine.Object[] gameObjects = Resources.LoadAll<UnityEngine.Object>("Prefabs/UI/Battle");
        foreach (UnityEngine.Object obj in gameObjects)
        {
            if (obj is GameObject go)
            {
                if (!resources.ContainsKey(go.name))
                    resources.Add(go.name, go);
            }
        }
        // 加载Buff图片
        UnityEngine.Object[] buffSprites = Resources.LoadAll<UnityEngine.Object>("UI/BattlePanel");
        foreach (UnityEngine.Object obj in buffSprites)
        {
            if (obj is Sprite sprite)
            {
                if (!spriteResources.ContainsKey(sprite.name))
                    spriteResources.Add(sprite.name, sprite);
            }
        }
        // 加载默认精灵
        Sprite defaultSprite = Resources.Load<Sprite>("UI/Default");
        if (defaultSprite != null && !spriteResources.ContainsKey("Default"))
            spriteResources.Add("Default", defaultSprite);
        // 加载文本描述
        playerTips = JsonConvert.DeserializeObject<PlayerTips>(Resources.Load<TextAsset>($"TipsText/PlayerTips").text);
        entityTips = JsonConvert.DeserializeObject<EntityTips>(Resources.Load<TextAsset>($"TipsText/Entity{level}Tips").text);
        entityActionTips = JsonConvert.DeserializeObject<EntityActionTips>(Resources.Load<TextAsset>($"TipsText/EntityActionTips").text);
        entityDesireTips = JsonConvert.DeserializeObject<EntityDesireTips>(Resources.Load<TextAsset>($"TipsText/EntityDesireTips").text);
        entityPartTips = JsonConvert.DeserializeObject<EntityPartTips>(Resources.Load<TextAsset>($"TipsText/Entity{level}PartTips").text);
    }

    #region Dice
    // 更新选中骰
    void UpdateSelectedDice(object obj = null)
    {
        Transform content = GetControl<ScrollRect>("Scroll View_SelectedDice").content;
        if (selectedDiceList == null) { Debug.Log("选中骰列表为空"); return; }

        // 清除ui上所有选中骰
        foreach (Transform item in content) Destroy(item.gameObject);

        // 重新生成一遍，时间和百搭不生成
        foreach (DiceBase item in selectedDiceList)
        {
            switch (item.type)
            {
                // 生成、初始化数据、同时添加删除对象的监听
                case E_DiceType.Action:
                    Button buttonAction = Instantiate(resources["SelectedActionDice"], content).GetComponentInChildren<Button>();
                    buttonAction.GetComponentInChildren<TextMeshProUGUI>().text = item.value.ToString();
                    buttonAction.onClick.AddListener(() => Destroy(buttonAction.transform.parent.gameObject));
                    DiceMark actMark = buttonAction.AddComponent<DiceMark>();
                    actMark.mark = item;
                    buttonAction.onClick.AddListener(() =>
                    {
                        foreach (Toggle actDie in GetControl<ScrollRect>("Scroll View_ActionDice").content.GetComponentsInChildren<Toggle>())
                        {
                            if (actDie.GetComponent<DiceMark>().mark == item) actDie.isOn = false;
                        }
                    });
                    break;
                case E_DiceType.Mind:
                    Button buttonMind = Instantiate(resources["SelectedMindDice"], content).GetComponentInChildren<Button>();
                    buttonMind.GetComponentInChildren<TextMeshProUGUI>().text = item.value.ToString();
                    buttonMind.onClick.AddListener(() => Destroy(buttonMind.transform.parent.gameObject));
                    DiceMark mindMark = buttonMind.AddComponent<DiceMark>();
                    mindMark.mark = item;
                    buttonMind.onClick.AddListener(() =>
                    {
                        foreach (Toggle mindDie in GetControl<ScrollRect>("Scroll View_MindDice").content.GetComponentsInChildren<Toggle>())
                        {
                            if (mindDie.GetComponent<DiceMark>().mark == item) mindDie.isOn = false;
                        }
                    });
                    break;
            }
        }
        // 取消已不在选中列表中的骰子的 toggle
        foreach (DiceMark item in GetControl<ScrollRect>("Scroll View_MindDice").content.GetComponentsInChildren<DiceMark>())
        {
            bool isSelected = false;
            foreach (DiceMark diceMark in content.GetComponentsInChildren<DiceMark>())
            {
                if (diceMark.mark == item.mark) { isSelected = true; break; }
            }
            if (!isSelected) item.GetComponentInParent<Toggle>().isOn = false;
        }
    }

    // 设置现有骰子为未选中状态
    void SetDiceSelectedFalse(object obj = null)
    {
        foreach (Transform item in GetControl<ScrollRect>("Scroll View_ActionDice").content)
            item.GetComponentInChildren<Toggle>().isOn = false;
        foreach (Transform item in GetControl<ScrollRect>("Scroll View_MindDice").content)
            item.GetComponentInChildren<Toggle>().isOn = false;
    }

    // 更新行动/思维骰
    void UpdateDice<T>() where T : DiceBase, new()
    {
        List<DiceBase> Dice;
        if (typeof(T) == typeof(ActionDice))
            Dice = DiceManager.Instance.dicePool[E_DiceType.Action];
        else if (typeof(T) == typeof(MindDice))
            Dice = DiceManager.Instance.dicePool[E_DiceType.Mind];
        else { Debug.Log("更新骰列表为空"); return; }

        Transform content = GetControl<ScrollRect>($"Scroll View_{typeof(T).Name}").content;
        // 清除ui上所有骰
        foreach (Transform item in content) Destroy(item.gameObject);

        // 重新生成一遍
        foreach (T item in Dice)
        {
            T currentItem = item;
            Toggle toggle = Instantiate(resources[typeof(T).Name], content).GetComponent<Toggle>();
            DiceMark toggleDie = toggle.AddComponent<DiceMark>();
            toggleDie.mark = item;
            toggle.GetComponentInChildren<TextMeshProUGUI>().text = currentItem.value.ToString();
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    if (!selectedDiceList.Contains(currentItem)) selectedDiceList.Add(currentItem);
                    EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                        new object[] { E_SFX.SelectDice, false });
                    DiceManager.Instance.SortSelectedByValue();
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice);
                }
                else
                {
                    for (int i = selectedDiceList.Count - 1; i >= 0; i--)
                    {
                        if (selectedDiceList[i] == currentItem)
                        {
                            selectedDiceList.RemoveAt(i);
                            DiceManager.Instance.SortSelectedByValue();
                            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice);
                            break;
                        }
                    }
                }
            });
        }
    }

    // 时间骰拥有与选择个数
    void UpdateTimeDiceCount(object obj = null)
    {
        Dictionary<E_DiceType, List<DiceBase>> pool = DiceManager.Instance.dicePool;
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice1Count").text = pool[E_DiceType.Time1].Count.ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice2Count").text = pool[E_DiceType.Time2].Count.ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice3Count").text = pool[E_DiceType.Time3].Count.ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice4Count").text = pool[E_DiceType.Time4].Count.ToString();
    }

    void UpdateTimeDiceSelectedCount(object obj = null)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice1SelectedCount").text = DiceManager.Instance.GetSelectedTime1DiceCount().ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice2SelectedCount").text = DiceManager.Instance.GetSelectedTime2DiceCount().ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice3SelectedCount").text = DiceManager.Instance.GetSelectedTime3DiceCount().ToString();
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDice4SelectedCount").text = DiceManager.Instance.GetSelectedTime4DiceCount().ToString();
    }

    // 百搭骰个数
    void UpdateWildDiceCount(object obj = null)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_WildDiceCount").text = DiceManager.Instance.dicePool[E_DiceType.Wild].Count.ToString();
    }

    void UpdateWildDiceSelectedCount(object obj = null)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_WildDiceSelectedCount").text = DiceManager.Instance.GetSelectedWildDiceCount().ToString();
    }

    // 公共骰盘情况
    void UpdateEntityDice(object obj = null)
    {
        List<EntityDice> entityDice = DiceManager.Instance.entityDicePool;
        Transform content = GetControl<ScrollRect>("Scroll View_PublicDice").content;
        if (entityDice == null) { Debug.Log("公共骰列表为空"); return; }

        foreach (Transform item in content) Destroy(item.gameObject);
        foreach (EntityDice entityDie in entityDice)
        {
            TextMeshProUGUI tmp = Instantiate(resources["EntityDice"], content).GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = entityDie.value.ToString();
        }
    }
    #endregion

    #region Player
    void UpdatePlayerHP(object obj = null)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_PlayerHP").text = BuffManager.Instance.player.hp.ToString();
        GetControl<Slider>("Slider_PlayerHP").value = BuffManager.Instance.player.hp;
    }

    Sprite GetBuffSprite(E_BuffType buffType)
    {
        string key = $"Buff_{buffType}";
        if (spriteResources.ContainsKey(key)) return spriteResources[key];
        if (spriteResources.ContainsKey("Default")) return spriteResources["Default"];
        return null;
    }

    void UpdatePlayerBuff(object obj = null)
    {
        Dictionary<E_BuffType, int> keyValuePairs = BuffManager.Instance.player.UI_buffs;
        Transform content = GetControl<ScrollRect>("Scroll View_PlayerBuff").content;
        foreach (Transform item in content) Destroy(item.gameObject);

        foreach (KeyValuePair<E_BuffType, int> item in keyValuePairs)
        {
            if (item.Value != 0)
            {
                GameObject buff = Instantiate(resources["Buff"], content);
                buff.GetComponentInChildren<Image>().sprite = GetBuffSprite(item.Key);
                buff.GetComponentInChildren<TextMeshProUGUI>().text = item.Value.ToString();
                RegisterTooltip<Image>(buff.GetComponentInChildren<Image>(), playerTips.buffs[level - 1][(int)item.Key][0], playerTips.buffs[level - 1][(int)item.Key][1]);
            }
        }
    }
    #endregion

    #region Entity
    void UpdateEntityAction(object obj = null)
    {
        E_IntentType actionType = StateManager.Instance.UI_currentExecutableAction;
        TextMeshProUGUI action = GetControl<TextMeshProUGUI>("Text (TMP)_EntityAction");
        // 设置行动名，注册光标覆盖事件
        action.text = entityActionTips.actions[(int)actionType][0];
        RegisterTooltip<TextMeshProUGUI>(action, entityActionTips.actions[(int)actionType][0], entityActionTips.actions[(int)actionType][1]);
    }

    void UpdateEntityWish(object obj = null)
    {
        E_DesireType wishType = StateManager.Instance.UI_currentExecutableDesire;
        TextMeshProUGUI wish = GetControl<TextMeshProUGUI>("Text (TMP)_EntityWish");
        // 设置许愿名，注册光标覆盖事件
        wish.text = entityDesireTips.wishes[(int)wishType][0];
        RegisterTooltip<TextMeshProUGUI>(wish, entityDesireTips.wishes[(int)wishType][0], entityDesireTips.wishes[(int)wishType][1]);
    }

    void UpdateEntityState(object obj = null)
    {
        Enum state = StateManager.Instance.currentState;
        TextMeshProUGUI entityState = GetControl<TextMeshProUGUI>("Text (TMP)_EntityState");
        // 设置状态名，注册光标覆盖事件

        switch (level)
        {
            case 1:
                entityState.text = entityTips.states[(int)(E_StateType_1)state][0];
                RegisterTooltip<TextMeshProUGUI>(entityState, entityTips.states[(int)(E_StateType_1)state][0], entityTips.states[(int)(E_StateType_1)state][1]);
                break;
            case 2:
                entityState.text = entityTips.states[(int)(E_StateType_2)state][0];
                RegisterTooltip<TextMeshProUGUI>(entityState, entityTips.states[(int)(E_StateType_2)state][0], entityTips.states[(int)(E_StateType_2)state][1]);
                break;
            case 3:
                entityState.text = entityTips.states[(int)(E_StateType_3)state][0];
                RegisterTooltip<TextMeshProUGUI>(entityState, entityTips.states[(int)(E_StateType_3)state][0], entityTips.states[(int)(E_StateType_3)state][1]);
                break;
            case 4:
                entityState.text = entityTips.states[(int)(E_StateType_4)state][0];
                RegisterTooltip<TextMeshProUGUI>(entityState, entityTips.states[(int)(E_StateType_4)state][0], entityTips.states[(int)(E_StateType_4)state][1]);
                break;
            case 5:
                entityState.text = entityTips.states[(int)(E_StateType_5)state][0];
                RegisterTooltip<TextMeshProUGUI>(entityState, entityTips.states[(int)(E_StateType_5)state][0], entityTips.states[(int)(E_StateType_5)state][1]);
                break;
        }
    }

    void UpdateEntityBuff(object obj = null)
    {
        Dictionary<E_BuffType, int> keyValuePairs = BuffManager.Instance.entity.UI_buffs;
        Transform content = GetControl<ScrollRect>("Scroll View_EntityBuff").content;
        foreach (Transform item in content) Destroy(item.gameObject);

        foreach (KeyValuePair<E_BuffType, int> item in keyValuePairs)
        {
            if (item.Value != 0)
            {
                GameObject buff = Instantiate(resources["Buff"], content);
                buff.GetComponentInChildren<Image>().sprite = GetBuffSprite(item.Key);
                buff.GetComponentInChildren<TextMeshProUGUI>().text = item.Value.ToString();
                RegisterTooltip<Image>(buff.GetComponentInChildren<Image>(), entityTips.buffs[(int)item.Key][0], entityTips.buffs[(int)item.Key][1]);
            }
        }
    }

    void UpdateEntityPart(object obj = null)
    {
        for (int index = 1; index < 5; index++)
            UpdateSinglePartUI(index);
    }

    void UpdateSinglePartUI(int index)
    {
        if (level == 4)
        {
            if (index == 1)
            {
                // 槽位1：读取 Entity4 本体
                Entity4 entity = ProgressManager.Instance.nowEntities[0] as Entity4;
                if (entity != null)
                {
                    // 用 SetPartValues 风格显示实体数据
                    // 但实体没有 partName / IsVisible / isDestroyed
                    // 需要直接操作 UI 控件
                    GetControl<Toggle>($"Toggle_EntityPart1").interactable = true;
                    GetControl<TextMeshProUGUI>("Text (TMP)_EntityPart1Name").text = entityTips.name;
                    GetControl<TextMeshProUGUI>("Text (TMP)_EntityPart1HP").text = entity.hp.ToString();
                    GetControl<TextMeshProUGUI>("Text (TMP)_EntityPart1MaxHP").text = entity.maxHp.ToString();
                    Slider slider = GetControl<Slider>("Slider_EntityPart1HP");
                    slider.maxValue = entity.maxHp;
                    slider.value = entity.hp;
                }
                return;
            }
            else
            {
                // 槽位2-4：映射到 nowEntities[1-3]（即 Part4_1/2/3）
                // index=2 → nowEntities[1], index=3 → nowEntities[2], index=4 → nowEntities[3]
                int mappedIndex = index - 1;
                Part part_ = (Part)ProgressManager.Instance.nowEntities[mappedIndex];
                if (part_ == null)
                {
                    SetPartUnknown(index);
                    return;
                }
                // tooltip 下标：Part4_1(mappedIndex=1) → parts[0]，所以用 mappedIndex-1
                int tipIndex = mappedIndex - 1;

                // ─── 状态A：已发现 + 未破坏 + 可攻击 ───
                if (part_.IsVisible && !part_.isDestroyed && part_.IsCouldBeAttacked())
                {
                    SetPartValues(index, part_, true);
                    RegisterTooltip<Toggle>($"Toggle_EntityPart{index}",
                        entityPartTips.parts[tipIndex][0],
                        entityPartTips.parts[tipIndex][1]);
                }
                // ─── 状态B：已发现 + 未破坏 + 不可攻击 ───
                else if (!part_.isDestroyed && !part_.IsCouldBeAttacked())
                {
                    SetPartValues(index, part_, false);
                    RegisterTooltip<Toggle>($"Toggle_EntityPart{index}",
                        entityPartTips.parts[tipIndex][0],
                        entityPartTips.parts[tipIndex][1]);
                }
                // ─── 状态C：已发现 + 已破坏 ───
                else if (part_.IsVisible && part_.isDestroyed)
                {
                    SetPartValues(index, part_, false, " (已破坏)");
                    RegisterTooltip<Toggle>($"Toggle_EntityPart{index}",
                        entityPartTips.parts[tipIndex][0],
                        entityPartTips.parts[tipIndex][1]);
                }
                // ─── 兜底：未发现 ───
                else
                {
                    SetPartUnknown(index);
                }
                return;
            }
        }
        Part part = (Part)ProgressManager.Instance.nowEntities[index];
        if (part == null)
        {
            // 没发现部位
            SetPartUnknown(index);
            return;
        }
        if (part.IsVisible && !part.isDestroyed && part.IsCouldBeAttacked())
        {
            // 发现部位，部位可攻击且没被破坏
            SetPartValues(index, part, true);
            RegisterTooltip<Toggle>($"Toggle_EntityPart{index}", entityPartTips.parts[index - 1][0], entityPartTips.parts[index - 1][1]);
        }
        else if (!part.isDestroyed && !part.IsCouldBeAttacked())
        {
            // 发现部位，部位没被破坏但是不可攻击
            SetPartValues(index, part, false);
            RegisterTooltip<Toggle>($"Toggle_EntityPart{index}", entityPartTips.parts[index - 1][0], entityPartTips.parts[index - 1][1]);
        }
        else if (part.IsVisible && part.isDestroyed)
        {
            // 发现部位，已经被破坏
            SetPartValues(index, part, false, " (已破坏)");
            RegisterTooltip<Toggle>($"Toggle_EntityPart{index}", entityPartTips.parts[index - 1][0], entityPartTips.parts[index - 1][1]);
        }
        else
        {
            // 默认情况
            SetPartUnknown(index);
        }
    }

    void SetPartUnknown(int index)
    {
        GetControl<Toggle>($"Toggle_EntityPart{index}").interactable = false;
        GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}Name").text = "???";
        GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}HP").text = "??";
        GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}MaxHP").text = "??";
        Slider slider = GetControl<Slider>($"Slider_EntityPart{index}HP");
        slider.maxValue = 1;
        slider.value = 0;
    }

    void SetPartValues(int index, Part part, bool interactable, string nameSuffix = "")
    {
        GetControl<Toggle>($"Toggle_EntityPart{index}").interactable = interactable;
        GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}Name").text = part.partName + nameSuffix;
        GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}HP").text = part.hp.ToString();
        GetControl<TextMeshProUGUI>($"Text (TMP)_EntityPart{index}MaxHP").text = part.maxHp.ToString();
        Slider slider = GetControl<Slider>($"Slider_EntityPart{index}HP");
        slider.maxValue = part.maxHp;
        slider.value = part.hp;
    }

    void UpdateEntityHP(object obj = null)
    {
        if (level == 4)
        {
            // HP 已移到部位1，刷新部位槽位1而不是本体区
            UpdateSinglePartUI(1);
            return;
        }
        GetControl<TextMeshProUGUI>("Text (TMP)_EntityHP").text = ProgressManager.Instance.nowEntities[0].hp.ToString();
        GetControl<Slider>("Slider_EntityHP").value = ProgressManager.Instance.nowEntities[0].hp;
    }
    #endregion

    #region Event
    void UpdateEvents(object obj = null)
    {
        Dictionary<E_OptionType, OptionBase[]> eventDic = EventManager.Instance.optionPool;
        Transform eventContent = GetControl<ScrollRect>("Scroll View_Event").content;
        // 删除原事件的对象
        foreach (Transform item in eventContent) Destroy(item.gameObject);
        // 标记索引为0
        int index = 0;
        // 遍历对应关卡事件列表
        foreach (OptionBase option in eventDic[(E_OptionType)(level + 2)])
        {
            // 避免闭包问题，在里面赋值拿到当前index的值
            int eventIndex = index;
            if (!option.IsVisible) { index++; continue; }
            Debug.Log($"当前index = {index}, 事件名 = {option.OptionName}, 是否显示 = {option.IsVisible}");
            // 创建事件框
            GameObject eventObj = Instantiate(resources["Event"], eventContent);
            // 设置事件的名称和描述
            foreach (TextMeshProUGUI item in eventObj.GetComponentsInChildren<TextMeshProUGUI>())
            {
                switch (item.gameObject.name)
                {
                    case "EventName": item.text = entityTips.events[index][0]; break;
                    case "Description": item.text = entityTips.events[index][4]; break;
                    case "FlavorText": item.text = entityTips.events[index][1]; break;
                }
            }
            // 设置光标移上去显示的需求
            RegisterTooltip<Button>(eventObj.GetComponentInChildren<Button>(), "", entityTips.events[index][2]);
            // 拿到骰子滑动列表，准备装骰子
            Transform content = eventObj.GetComponentInChildren<ScrollRect>().content;
            // 如果有需求的骰子列表
            if (option.DiceCost != null)
            {
                // 添加需求骰子
                foreach (DiceCondition item in option.DiceCost)
                {
                    GameObject dieObj = Instantiate(resources[$"Event_{item.type}Dice_{item.mode}"], content);
                    // 不是any的骰子需要改点数
                    if (item.mode != E_CompareType.Any)
                        dieObj.GetComponentInChildren<TextMeshProUGUI>().text = item.value.ToString();
                }
            }
            // 如果需求骰子是combo类型，添加对应骰子
            else if (option.IsUseDiceCombo)
            {
                switch (option.ComboType)
                {
                    case E_ComboType.MindActionPair: Instantiate(resources["Event_MindActionPairDice"], content); break;
                    case E_ComboType.DoubleMind: Instantiate(resources["Event_DoubleMindDice"], content); break;
                    case E_ComboType.DoubleAction: Instantiate(resources["Event_DoubleActionDice"], content); break;
                    case E_ComboType.TripleMind: Instantiate(resources["Event_TripleMindDice"], content); break;
                    case E_ComboType.Quadruple: Instantiate(resources["Event_QuadrupleDice"], content); break;
                    case E_ComboType.SingleWild: Instantiate(resources["Event_SingleWildDice"], content); break;
                }
            }
            // 没有骰子需求，删除滑动框省空间
            else
            {
                Destroy(eventObj.GetComponentInChildren<ScrollRect>().gameObject);
            }
            // 给事件加点击委托
            Button eventButton = eventObj.GetComponentInChildren<Button>();
            eventButton.onClick.RemoveAllListeners();
            eventButton.onClick.AddListener(() =>
            {
                // 特殊传参
                OptionContext context = null;
                if ((E_OptionType)(level + 2) == E_OptionType.Level3_Option)
                {
                    if (option.OptionID == 1 || option.OptionID == 3 || option.OptionID == 4 || option.OptionID == 5 || option.OptionID == 6)
                        context = new EntityEvent_3_01_OptionContext() { index = PartIndex };
                }
                // 正常添加
                Debug.Log($"执行{eventIndex + 1}号事件({option.OptionName})");
                EventManager.Instance.ExcuteOption((E_OptionType)(level + 2), eventIndex, context);

                // 若有闪动文字，则创建闪动提示
                if (entityTips.events[eventIndex].Count > 3 && !string.IsNullOrEmpty(entityTips.events[eventIndex][3]))
                {
                    GameObject flashTip = Instantiate(resources["FlashTip"], transform);
                    flashTip.GetComponentInChildren<TextMeshProUGUI>().text = entityTips.events[eventIndex][3];

                    CanvasGroup cg = flashTip.GetComponent<CanvasGroup>();
                    if (cg == null) cg = flashTip.AddComponent<CanvasGroup>();
                    cg.alpha = 0;

                    // 渐入 → 闪动3次 → 渐出 → 销毁
                    DG.Tweening.Sequence seq = DOTween.Sequence();
                    seq.Append(cg.DOFade(1, 1f));
                    seq.Append(cg.DOFade(0.2f, 0.8f).SetLoops(4, LoopType.Yoyo));
                    seq.Append(cg.DOFade(0, 1f));
                    seq.OnComplete(() => Destroy(flashTip));
                }
                // 清除选中
                EventSystem.current.SetSelectedGameObject(null);
            });
            index++;
        }
    }
    #endregion

    #region Wish
    void UnlockedWish(object obj = null)
    {
        switch (level)
        {
            case 5:
                Button btn4 = GetControl<Button>("Button_Wish_SmoothAndSteady");
                btn4.interactable = true;
                btn4.GetComponentInChildren<TextMeshProUGUI>().text = playerTips.wishes[3][0];
                btn4.GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;
                RegisterTooltip<Button>("Button_Wish_SmoothAndSteady", playerTips.wishes[3][0], playerTips.wishes[3][1]);
                goto case 4;
            case 4:
                Button btn3 = GetControl<Button>("Button_Wish_Infinite");
                btn3.interactable = true;
                btn3.GetComponentInChildren<TextMeshProUGUI>().text = playerTips.wishes[2][0];
                btn3.GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;
                RegisterTooltip<Button>("Button_Wish_Infinite", playerTips.wishes[2][0], playerTips.wishes[2][1]);
                goto case 3;
            case 3:
                Button btn2 = GetControl<Button>("Button_Wish_Vibrancy");
                btn2.interactable = true;
                btn2.GetComponentInChildren<TextMeshProUGUI>().text = playerTips.wishes[1][0];
                btn2.GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;
                RegisterTooltip<Button>("Button_Wish_Vibrancy", playerTips.wishes[1][0], playerTips.wishes[1][1]);
                goto case 2;
            case 2:
                Button btn1 = GetControl<Button>("Button_Wish_Abundance");
                btn1.interactable = true;
                btn1.GetComponentInChildren<TextMeshProUGUI>().text = playerTips.wishes[0][0];
                btn1.GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;
                RegisterTooltip<Button>("Button_Wish_Abundance", playerTips.wishes[0][0], playerTips.wishes[0][1]);
                break;
        }
    }

    void LockWish(object obj = null)
    {
        Button[] buttons = GetControl<Image>("Wish").GetComponentsInChildren<Button>();
        foreach (Button item in buttons)
        {
            item.interactable = false;
            item.GetComponentInChildren<TextMeshProUGUI>().alpha = 0.5f;
        }
    }
    #endregion

    #region Time/Phase
    void UpdatePhase(object obj = null)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_Phase").text = (ProgressManager.Instance.phase + 1).ToString();
    }

    void UpdateTimeProgress(object obj = null)
    {
        int num = ProgressManager.Instance.timeProgress;
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeProgress").text = num.ToString();
        Slider Slider_TimeProgress = GetControl<Slider>("Slider_TimeProgress");
        Slider_TimeProgress.value = num;
    }

    void UpdateMaxTimeProgress(object obj)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_MaxTimeProgress").text = ((int)obj).ToString();
        Slider Slider_TimeProgress = GetControl<Slider>("Slider_TimeProgress");
        Slider_TimeProgress.maxValue = (int)obj;
    }

    void UpdateTimeDicePerPhase(object obj)
    {
        GetControl<TextMeshProUGUI>("Text (TMP)_TimeDicePerPhase").text = ((int)obj).ToString();
    }
    #endregion

    #region 注册事件专用方法
    void OnUpdateActionDice(object obj) => UpdateDice<ActionDice>();
    void OnUpdateMindDice(object obj) => UpdateDice<MindDice>();
    void OnUpdateTimeDice(object obj) { UpdateTimeDiceCount(); UpdateTimeDiceSelectedCount(); }

    void OnUpdatePlayerDied(object obj)
    {
        AudioManager.Instance.StopAllSFX();
        AudioManager.Instance.PlaySFX(E_SFX.PlayerDie, false);
        UIManager.Instance.ChangePanel<BattlePanel, DiePanel>(showLoading: false);
    }

    void OnUpdateEntityDied(object obj)
    {
        Debug.Log("触发胜利");
        // 当前关卡大于存档的最大关卡时才刷新存档
        if (level >= JsonManager.Instance.LoadDataByType(E_SaveDataType.LevelProgress))
            JsonManager.Instance.AdjustSaveDataByType(E_SaveDataType.LevelProgress, level + 1);
        // 加载战后剧情
        StoryManager.Instance.LoadStorySegmentByIndex(level * 2 + 1);
        UIManager.Instance.ChangePanel<BattlePanel, StoryPanel>();
    }

    void OnUpdateIsConditionNotMet(object obj)
    {
        UpdateSelectedDice();
        UpdateTimeDiceCount();
        UpdateTimeDiceSelectedCount();
        // 面板振动效果
        rectTransform.DOShakeAnchorPos(0.3f, 10).OnComplete(() =>
            {
                // 震完回到000
                rectTransform.DOAnchorPos(Vector2.zero, 0.1f);
            });
        Debug.LogWarning("判定未通过");
    }
    #endregion

    #region 图片UI点击方法封装
    /// <summary>
    /// 时间骰/百搭骰的左右键点击
    /// </summary>
    void HandleTimeDiceClick(E_DiceType diceType, bool isLeftClick)
    {
        int selectedNum = diceType switch
        {
            E_DiceType.Time1 => DiceManager.Instance.GetSelectedTime1DiceCount(),
            E_DiceType.Time2 => DiceManager.Instance.GetSelectedTime2DiceCount(),
            E_DiceType.Time3 => DiceManager.Instance.GetSelectedTime3DiceCount(),
            E_DiceType.Time4 => DiceManager.Instance.GetSelectedTime4DiceCount(),
            E_DiceType.Wild => DiceManager.Instance.GetSelectedWildDiceCount(),
            _ => -1
        };
        if (selectedNum < 0) return;

        List<DiceBase> diceList = DiceManager.Instance.dicePool[diceType];

        if (isLeftClick)
        {
            if (selectedNum >= diceList.Count) return;
            foreach (DiceBase item in selectedDiceList.ToList())
                if (item.type == diceType) selectedDiceList.Remove(item);
            for (int i = diceList.Count - 1; i >= diceList.Count - 1 - selectedNum; i--)
                selectedDiceList.Add(diceList[i]);
        }
        else
        {
            if (selectedNum <= 0) return;
            foreach (DiceBase item in selectedDiceList)
            {
                if (item.type == diceType) { selectedDiceList.Remove(item); break; }
            }
        }

        if (diceType == E_DiceType.Wild)
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WildDiceSelectedCount);
        else
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDiceSelectedCount);
    }
    #endregion

    // 注册事件
    void InitEvents()
    {
        EventCenter eventCenter = EventCenter.Instance;

        #region Progress
        eventCenter.AddEventListener(E_EventType.UI_Update_Phase, UpdatePhase);
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeProgress, UpdateTimeProgress);
        eventCenter.AddEventListener(E_EventType.UI_Update_MaxTimeProgress, UpdateMaxTimeProgress);
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDicePerPhase, UpdateTimeDicePerPhase);
        #endregion

        #region Dice
        eventCenter.AddEventListener(E_EventType.UI_Update_SelectedDice, UpdateSelectedDice);
        eventCenter.AddEventListener(E_EventType.UI_Update_SetDiceSelectedFalse, SetDiceSelectedFalse);
        eventCenter.AddEventListener(E_EventType.UI_Update_ActionDice, OnUpdateActionDice);
        eventCenter.AddEventListener(E_EventType.UI_Update_MindDice, OnUpdateMindDice);
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDiceCount, UpdateTimeDiceCount);
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDiceSelectedCount, UpdateTimeDiceSelectedCount);
        eventCenter.AddEventListener(E_EventType.UI_Update_TimeDice, OnUpdateTimeDice);
        eventCenter.AddEventListener(E_EventType.UI_Update_WildDiceCount, UpdateWildDiceCount);
        eventCenter.AddEventListener(E_EventType.UI_Update_WildDiceSelectedCount, UpdateWildDiceSelectedCount);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityDice, UpdateEntityDice);
        #endregion

        #region Player
        eventCenter.AddEventListener(E_EventType.UI_Update_PlayerHP, UpdatePlayerHP);
        eventCenter.AddEventListener(E_EventType.UI_Update_PlayerBuff, UpdatePlayerBuff);
        eventCenter.AddEventListener(E_EventType.UI_Update_PlayerDied, OnUpdatePlayerDied);
        #endregion

        #region Entity
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityState, UpdateEntityState);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityAction, UpdateEntityAction);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityWish, UpdateEntityWish);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityBuff, UpdateEntityBuff);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityPart, UpdateEntityPart);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityHP, UpdateEntityHP);
        eventCenter.AddEventListener(E_EventType.UI_Update_EntityDied, OnUpdateEntityDied);
        #endregion

        eventCenter.AddEventListener(E_EventType.UI_Update_Events, UpdateEvents);

        #region Wish
        eventCenter.AddEventListener(E_EventType.UI_Update_WishToAvailable, UnlockedWish);
        eventCenter.AddEventListener(E_EventType.UI_Update_WishToUnavailable, LockWish);
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
        LockWish();
        UnlockedWish();
        #endregion
        #region 玩家/骰面板
        UpdateDice<ActionDice>();
        UpdateDice<MindDice>();
        UpdateTimeDiceCount();
        UpdateTimeDiceSelectedCount();
        UpdateWildDiceCount();
        UpdateWildDiceSelectedCount();
        UpdatePlayerHP();
        UpdatePlayerBuff();
        UpdateTimeDiceCount();
        #endregion
        #region Entity
        // hp
        if (level == 5)
        {
            // 第五关本体特殊处理
            GetControl<Toggle>("Toggle_EntityPart0").interactable = false;
            GetControl<TextMeshProUGUI>("Text (TMP)_EntityHP").text = "∞";
            GetControl<TextMeshProUGUI>("Text (TMP)_EntityMaxHP").text = "∞";
            GetControl<TextMeshProUGUI>("Text (TMP)_EntityPart0Name").text = "<color=grey>本体</color>";
            Slider slider = GetControl<Slider>("Slider_EntityHP");
            slider.maxValue = 1;
            slider.value = 1;

        }
        else
        {
            GetControl<TextMeshProUGUI>("Text (TMP)_EntityMaxHP").text = ProgressManager.Instance.nowEntities[0].maxHp.ToString();
            GetControl<Slider>("Slider_EntityHP").maxValue = ProgressManager.Instance.nowEntities[0].maxHp;
            UpdateEntityHP();
        }
        // other
        GetControl<TextMeshProUGUI>("Text (TMP)_EntityName").text = entityTips.name;
        if (level == 4)
        {
            GetControl<Toggle>("Toggle_EntityPart0").interactable = false;
            GetControl<TextMeshProUGUI>("Text (TMP)_EntityPart0Name").text = "<color=grey>本体</color>";
            GetControl<TextMeshProUGUI>("Text (TMP)_EntityName").text = "陈列室";
            Slider slider = GetControl<Slider>("Slider_EntityHP");
            slider.maxValue = 1;
            slider.value = 0;
            GetControl<TextMeshProUGUI>("Text (TMP)_EntityHP").text = "null";
            GetControl<TextMeshProUGUI>("Text (TMP)_EntityMaxHP").text = "null";
        }
        UpdateEntityState();
        UpdateEntityAction();
        UpdateEntityWish();
        UpdateEntityPart();
        UpdateEntityBuff();
        #endregion
        // event
        UpdateEvents();
    }

    // 面板移除时同时移除监听
    void OnDestroy()
    {
        EventCenter eventCenter = EventCenter.Instance;

        #region Progress
        eventCenter.RemoveEventListener(E_EventType.UI_Update_Phase, UpdatePhase);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_TimeProgress, UpdateTimeProgress);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_MaxTimeProgress, UpdateMaxTimeProgress);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_TimeDicePerPhase, UpdateTimeDicePerPhase);
        #endregion

        #region Dice
        eventCenter.RemoveEventListener(E_EventType.UI_Update_SelectedDice, UpdateSelectedDice);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_SetDiceSelectedFalse, SetDiceSelectedFalse);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_ActionDice, OnUpdateActionDice);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_MindDice, OnUpdateMindDice);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_TimeDiceCount, UpdateTimeDiceCount);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_TimeDiceSelectedCount, UpdateTimeDiceSelectedCount);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_TimeDice, OnUpdateTimeDice);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_WildDiceCount, UpdateWildDiceCount);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_WildDiceSelectedCount, UpdateWildDiceSelectedCount);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityDice, UpdateEntityDice);
        #endregion

        #region Player
        eventCenter.RemoveEventListener(E_EventType.UI_Update_PlayerHP, UpdatePlayerHP);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_PlayerBuff, UpdatePlayerBuff);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_PlayerDied, OnUpdatePlayerDied);
        #endregion

        #region Entity
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityState, UpdateEntityState);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityAction, UpdateEntityAction);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityWish, UpdateEntityWish);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityBuff, UpdateEntityBuff);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityPart, UpdateEntityPart);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityHP, UpdateEntityHP);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_EntityDied, OnUpdateEntityDied);
        #endregion

        eventCenter.RemoveEventListener(E_EventType.UI_Update_Events, UpdateEvents);

        #region Wish
        eventCenter.RemoveEventListener(E_EventType.UI_Update_WishToAvailable, UnlockedWish);
        eventCenter.RemoveEventListener(E_EventType.UI_Update_WishToUnavailable, LockWish);
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
                if (ActOrMind == E_DiceType.Wild) { EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet); break; }
                SkillManager.ExcuteSkills(0, new PrepareOptionContext { diceType = ActOrMind });
                break;
            case "Button_InherentAction_Adjust":
                if (!(point == -1 || point == 1)) { EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet); break; }
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
                if (isSelectedWildDice || partIndex == -1) { EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet); break; }
                SkillManager.ExcuteSkills(3, new AtkOptionContext { index = partIndex });
                break;
            #endregion
            #region 律
            case "Button_Law_ChantingLaw": SkillManager.ExcuteSkills(4); break;
            case "Button_Law_GunArt3": SkillManager.ExcuteSkills(5); break;
            case "Button_Law_ShatteredStars": SkillManager.ExcuteSkills(6); break;
            #endregion
            #region 许愿
            case "Button_Wish_Abundance": SkillManager.ExcuteSkills(7); break;
            case "Button_Wish_Vibrancy":
                if (point > 0 && point <= 6)
                    SkillManager.ExcuteSkills(8, new ColorfullOptionContext { i = point });
                else { Debug.LogWarning("玩家没有选择目标点数！"); EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet); }
                break;
            case "Button_Wish_Infinite": SkillManager.ExcuteSkills(9); break;
            case "Button_Wish_SmoothAndSteady": SkillManager.ExcuteSkills(10); break;
            #endregion
            #region Other
            case "Button_Explanation":
                UIManager.Instance.CreatPanel<TutorialPanel>(E_UILayer.Top);
                UIManager.Instance.GetPanel<TutorialPanel>().ShowTutorial(
                    E_TutorialType.None,
                    GetControl<Image>("None1").rectTransform,
                    GetControl<Image>("None2").rectTransform,
                    null
                );
                break;
            case "Button_Settings": UIManager.Instance.CreatPanel<SettingsPanel>(E_UILayer.Top); break;
            case "Button_ReplayLevel":
                ProgressManager.Instance.intoNewLevel(level);
                UIManager.Instance.ChangePanel<BattlePanel, BattlePanel>();
                break;
            case "Button_ExitLevel": UIManager.Instance.ChangePanel<BattlePanel, StartMenuPanel>(); break;
            case "Button_BackToStartMenu": UIManager.Instance.ChangePanel<BattlePanel, StartMenuPanel>(); break;
            case "Button_BackToSelectLevel": UIManager.Instance.ChangePanel<BattlePanel, LevelSelectPanel>(); break;
            default: break;
            #endregion
        }
    }

    protected override void ImageOnLeftClick(string imageName)
    {
        switch (imageName)
        {
            case "Image_Time1Dice": HandleTimeDiceClick(E_DiceType.Time1, true); break;
            case "Image_Time2Dice": HandleTimeDiceClick(E_DiceType.Time2, true); break;
            case "Image_Time3Dice": HandleTimeDiceClick(E_DiceType.Time3, true); break;
            case "Image_Time4Dice": HandleTimeDiceClick(E_DiceType.Time4, true); break;
            case "Image_WildDice": HandleTimeDiceClick(E_DiceType.Wild, true); break;
        }
    }

    protected override void ImageOnRightClick(string imageName)
    {
        switch (imageName)
        {
            case "Image_Time1Dice": HandleTimeDiceClick(E_DiceType.Time1, false); break;
            case "Image_Time2Dice": HandleTimeDiceClick(E_DiceType.Time2, false); break;
            case "Image_Time3Dice": HandleTimeDiceClick(E_DiceType.Time3, false); break;
            case "Image_Time4Dice": HandleTimeDiceClick(E_DiceType.Time4, false); break;
            case "Image_WildDice": HandleTimeDiceClick(E_DiceType.Wild, false); break;
        }
    }

    void InitTipsRegist()
    {
        #region 固有行动
        RegisterTooltip<Button>("Button_InherentAction_Prepare", playerTips.inherentActions[0][0], playerTips.inherentActions[0][1]);
        RegisterTooltip<Button>("Button_InherentAction_Adjust", playerTips.inherentActions[1][0], playerTips.inherentActions[1][1]);
        RegisterTooltip<Button>("Button_InherentAction_Overturn", playerTips.inherentActions[2][0], playerTips.inherentActions[2][1]);
        RegisterTooltip<Button>("Button_InherentAction_Atk", playerTips.inherentActions[3][0], playerTips.inherentActions[3][1]);
        #endregion
        #region 律
        RegisterTooltip<Button>("Button_Law_ChantingLaw", playerTips.laws[0][0], playerTips.laws[0][1]);
        RegisterTooltip<Button>("Button_Law_GunArt3", playerTips.laws[1][0], playerTips.laws[1][1]);
        RegisterTooltip<Button>("Button_Law_ShatteredStars", playerTips.laws[2][0], playerTips.laws[2][1]);
        #endregion
        #region Other
        RegisterTooltip<TextMeshProUGUI>("Text (TMP)_PhaseText", "时间段", "所有时间骰被消耗时，结束本时间段\n每时间段最多使用一次许愿\n时间段结束时，实体进行相应许愿");
        RegisterTooltip<TextMeshProUGUI>("Text (TMP)_TimeProgressText", "时间进度", "消耗时间骰使时间进度增加对应点数\n时间进度到达上限时，实体进行相应行动");
        RegisterTooltip<TextMeshProUGUI>("Text (TMP)_TimeDicePerPhaseText", "时间骰", "每时间段开始时获得此数值个点数为1的时间骰");
        #endregion
    }
}
