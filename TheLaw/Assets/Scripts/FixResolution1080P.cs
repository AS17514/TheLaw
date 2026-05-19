using UnityEngine;

public class FixResolutionMobile : MonoBehaviour
{
    void Awake()
    {
        if (Application.isMobilePlatform)
        {
            // 强制固定 1920x1080 16:9
            Screen.SetResolution(1920, 1080, true);

            // 强制横屏
            Screen.orientation = ScreenOrientation.LandscapeLeft;

            // 禁止旋转
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }
    }
}