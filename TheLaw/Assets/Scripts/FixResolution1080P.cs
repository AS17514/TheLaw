using UnityEngine;

public class FixResolution1080P : MonoBehaviour
{
    void Awake()
    {
        // 强制固定分辨率 1920*1080（标准16:9）
        Screen.SetResolution(1920, 1080, true);
        // 强制横屏
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        // 禁止自动旋转
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }
}