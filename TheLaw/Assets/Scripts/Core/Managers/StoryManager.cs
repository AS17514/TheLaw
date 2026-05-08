using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class StorySegment
{
    // 章节名
    public string title;
    // 剧情结束后
    public string afterStory;
    // 看完剧情后能解锁的最高关卡
    public int unlockLevel;
    // 剧情结束后进入战斗的索引，没有填0
    public int nextBattle;
    // 所有页面
    public List<StoryPage> pages;
}
public class StoryPage
{
    // 页面数
    public int page;
    // 此页面文本段列表
    public List<StoryLine> lines;
}
public class StoryLine
{
    // 此句文本
    public string text;
    // 是否斜体
    public bool italic;
    // 透明度
    public float alpha;
    // 整体颜色
    public string color;
}

public class StoryManager : ManagerMonoBase<StoryManager>
{
    /// <summary>
    /// 当前故事段
    /// </summary>
    public int segment;
    /// <summary>
    /// 故事段
    /// </summary>
    public StorySegment storySegment;
    void Awake()
    {
        // 初始化字典
        storySegment = new StorySegment();
    }
    /// <summary>
    /// 把指定剧情段的内容加载到本类的storySegment中
    /// </summary>
    /// <param name="index">剧情段索引</param>
    public void LoadStorySegmentByIndex(int segmentIndex)
    {
        TextAsset data = Resources.Load<TextAsset>($"Story/Story{segmentIndex}");
        if (data == null)
        {
            Debug.LogWarning($"未找到剧情段{segmentIndex}文件，加载上次剧情文件");
            return;
        }
        Debug.Log($"读取剧情段{segmentIndex}");
        storySegment = JsonConvert.DeserializeObject<StorySegment>(data.text);
        segment = segmentIndex;
    }
}
