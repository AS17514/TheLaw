using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryPanel : PanelBase
{
    StorySegment storySegment;
    GameObject storyLineObj;
    Transform content;
    Dictionary<int, int> pageProgress;
    int currentLineIndex;
    int maxCurrentLineIndex;
    int maxLastLineIndex;
    int currentPage;
    int maxPage;

    void Start()
    {
        // 加载文本预制体
        storyLineObj = Resources.Load<GameObject>("Prefabs/UI/Story/StoryLine");
        // 得到滑动框
        content = GetControl<ScrollRect>("Scroll View_Story").content;
        // 得到故事段
        storySegment = StoryManager.Instance.storySegment;
        // 生成进度字典
        pageProgress = new Dictionary<int, int>();
        currentLineIndex = -1;
        // 故事段最后一页最后句段的索引
        maxLastLineIndex = storySegment.pages[storySegment.pages.Count - 1].lines.Count - 1;
        // 初始化为第一页最后一段的索引
        maxCurrentLineIndex = storySegment.pages[0].lines.Count - 1;
        currentPage = 0;
        // 故事段最后一页的索引
        maxPage = storySegment.pages.Count - 1;
        Init();
    }
    void Update()
    {
        // 按空格继续
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }
    void OnDestroy()
    {
        // 如果不是插入剧情，就记录剧情进度
        if (storySegment.afterStory != "BackToBattle")
        {
            // 看完剧情记录当前看过的剧情进度，解锁设定的最大关卡进度（取最大值）
            JsonManager.Instance.AdjustSaveDataByType(E_SaveDataType.StoryProgress, StoryManager.Instance.segment);
            if (JsonManager.Instance.LoadDataByType(E_SaveDataType.LevelProgress) < storySegment.unlockLevel)
            {
                JsonManager.Instance.AdjustSaveDataByType(E_SaveDataType.LevelProgress, storySegment.unlockLevel);
            }
        }
    }
    void Init()
    {
        // 标题
        GetControl<TextMeshProUGUI>("Text (TMP)_Titel").text = storySegment.title;
        // 页码
        GetControl<TextMeshProUGUI>("Text (TMP)_CurrentPage").text = "1";
        GetControl<TextMeshProUGUI>("Text (TMP)_MaxPage").text = storySegment.pages.Count.ToString();
        // 初始图
    }
    void CreatStoryLine(int pageIndex, int lineIndex)
    {
        GameObject LineObj = GameObject.Instantiate<GameObject>(storyLineObj, content);
        StoryLine line = storySegment.pages[pageIndex].lines[lineIndex];
        TextMeshProUGUI tmp = LineObj.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = line.text;
        tmp.alpha = line.alpha;
        tmp.fontStyle = line.italic ? FontStyles.Italic : FontStyles.Normal;
        ColorUtility.TryParseHtmlString(line.color, out Color color);
        tmp.color = color;
    }
    void NextLine()
    {
        // 如果看到最后一页最后一句，就显示继续按钮
        if (currentPage == maxPage && currentLineIndex == maxLastLineIndex)
        {
            ShowContinueButton();
            return;
        }
        if (currentLineIndex == maxCurrentLineIndex)
        {
            if (!pageProgress.ContainsKey(currentPage + 1))
            {
                pageProgress.Add(currentPage + 1, 0);
            }
            NextPage();

        }
        else
        {
            // 没有此页进度，在字典生成此页进度
            if (!pageProgress.ContainsKey(currentPage))
            {
                pageProgress.Add(currentPage, 0);
            }
            currentLineIndex++;
            pageProgress[currentPage] = currentLineIndex;
            CreatStoryLine(currentPage, currentLineIndex);
        }
    }
    void NextPage()
    {
        // 没有看过下一页或者本身已经是最后一页时不进行操作
        if (!pageProgress.ContainsKey(currentPage + 1) || currentPage == maxPage)
        {
            return;
        }
        else
        {
            currentPage++;
            maxCurrentLineIndex = storySegment.pages[currentPage].lines.Count - 1;
            print(maxCurrentLineIndex);
            // 改变页码
            GetControl<TextMeshProUGUI>("Text (TMP)_CurrentPage").text = (currentPage + 1).ToString();
            // 清空容器
            foreach (Transform item in content)
            {
                Destroy(item.gameObject);
            }
            // 根据看过的进度重新填充容器
            for (int i = 0; i <= pageProgress[currentPage]; i++)
            {
                CreatStoryLine(currentPage, i);
            }
            currentLineIndex = pageProgress[currentPage];
            // 设置按钮按下后不是选中状态，防止和推进剧情抢按键
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    void PreviousPage()
    {
        // 如果已经是第一页
        if (currentPage == 0)
        {
            return;
        }
        else
        {
            currentPage--;
            maxCurrentLineIndex = storySegment.pages[currentPage].lines.Count - 1;
            print(maxCurrentLineIndex);
            GetControl<TextMeshProUGUI>("Text (TMP)_CurrentPage").text = (currentPage + 1).ToString();
            foreach (Transform item in content)
            {
                Destroy(item.gameObject);
            }
            // 因为看完上一页了，所以进行完全填充
            for (int i = 0; i <= maxCurrentLineIndex; i++)
            {
                CreatStoryLine(currentPage, i);
            }
            // 设置当前段数为段数上限
            currentLineIndex = maxCurrentLineIndex;
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    void ShowContinueButton()
    {
        CanvasGroup canvasGroup = GetControl<Button>("Button_Continue").GetComponent<CanvasGroup>();

        canvasGroup.DOFade(1f, 0.3f);

        // 渐变结束后允许点击
        DOVirtual.DelayedCall(0.3f, () =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
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
                if (storySegment.afterStory != "BackToBattle")
                {
                    UIManager.Instance.ChangePanel<StoryPanel, StartMenuPanel>();
                }
                else
                {
                    UIManager.Instance.ShakePanel<StoryPanel>();
                }
                break;
            case "Button_BackToSelectLevel":
                if (storySegment.afterStory != "BackToBattle")
                {
                    UIManager.Instance.ChangePanel<StoryPanel, LevelSelectPanel>();
                }
                else
                {
                    UIManager.Instance.ShakePanel<StoryPanel>();
                }
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
                    case "BackToBattle":
                        UIManager.Instance.RemovePanel<StoryPanel>();
                        break;
                    case "End":
                        UIManager.Instance.ChangePanel<StoryPanel, EndingPanel>();
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
