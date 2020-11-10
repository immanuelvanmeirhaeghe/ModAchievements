using UnityEngine;

namespace ModAchievements
{
    class PlayerExtended : Player
    {
        protected override void Start()
        {
            base.Start();
            new GameObject($"__{nameof(ModAchievements)}__").AddComponent<ModAchievements>();
        }
    }
}
