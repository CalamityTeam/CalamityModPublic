using CalamityMod.Items.Potions;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Scenes.MusicScenes
{
    public class TheConcoctionMusicScene : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player)
        {
            TheConcoctionPlayer concoctionPlayer = Main.LocalPlayer.GetModPlayer<TheConcoctionPlayer>();
            return (concoctionPlayer.swinesWrathCounter <= 600 && concoctionPlayer.swinesWrathCounter != -1);
        }
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Silence");

        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
    }
}
