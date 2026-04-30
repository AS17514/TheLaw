using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    // 选中骰情况
    List<DiceBase> selectedDice = new List<DiceBase>();
    // 行动与思维骰拥有情况
    List<ActionDice> actionDice = new List<ActionDice>();
    List<MindDice> mindDice = new List<MindDice>();
    // 时间骰拥有与选择个数
    // 百搭骰个数
    // 公共骰盘情况
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
        #endregion
        base.Awake();
        // 初始化所有东西
        Init();
    }
    void Init()
    {
        #region 初始化
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

        #endregion
        #endregion
        #region 添加监听
        #endregion
    }
}
