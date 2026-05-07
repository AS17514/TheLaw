using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryPanel : PanelBase
{
    StorySegment storySegment;
    GameObject storyLineObj;
    Transform content;
    int currentLineIndex;
    int currentPage;

    void Start()
    {
        // 加载文本预制体
        storyLineObj = Resources.Load<GameObject>("Prefabs/Story/StoryLine");
        // 得到滑动框
        content = GetControl<ScrollRect>("Scroll View_Story").content;
        // 得到故事段
        storySegment = StoryManager.Instance.storySegment;
        currentLineIndex = 0;
        currentPage = 0;
    }
    void NextLine()
    {

    }
    void NextPage()
    {

    }
    void PreviousPage()
    {

    }
    protected override void ButtonOnClick(string buttonName)
    {
        switch (buttonName)
        {
            case "Button_PreviousPage":
                PreviousPage();
                break;
            case "Button_NextPage":
                NextPage();
                break;
            case "Button_BackToStartMenu":
                UIManager.Instance.ChangePanel<StoryPanel, StartMenuPanel>();
                break;
            case "Button_BackToSelectLevel":
                UIManager.Instance.ChangePanel<StoryPanel, LevelSelectPanel>();
                break;
            case "Button_Skip":
            case "Button_Continue":
                // 根据剧情段文件设置的下一步执行
                switch (storySegment.afterStory)
                {
                    case "Battle":
                        if (storySegment.nextBattle == 0)
                        {
                            Debug.LogWarning("剧情结束应该进入战斗，但是未填写需要进入的关卡数据，将返回主页");
                            UIManager.Instance.ChangePanel<StoryPanel, StartMenuPanel>();
                        }
                        else if (storySegment.nextBattle < 0 || storySegment.nextBattle > 5)
                        {
                            Debug.LogWarning($"关卡数据{storySegment.nextBattle}填写超出范围，将返回主页");
                            UIManager.Instance.ChangePanel<StoryPanel, StartMenuPanel>();
                        }
                        else
                        {
                            ProgressManager.Instance.intoNewLevel(storySegment.nextBattle);
                            UIManager.Instance.ChangePanel<StoryPanel, BattlePanel>();
                        }
                        break;
                    case "SelectLevel":
                        UIManager.Instance.ChangePanel<StoryPanel, LevelSelectPanel>();
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }
}
