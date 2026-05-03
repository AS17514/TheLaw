using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInitializer
{
    public static void Init()
    {
        _ = EventCenter.Instance;
        _ = EventManager.Instance;
        _ = StateManager.Instance;
        _ = BuffManager.Instance;
        _ = DiceManager.Instance;
        _ = ProgressManager.Instance;
        SkillManager.InitSkills();
    }
}
