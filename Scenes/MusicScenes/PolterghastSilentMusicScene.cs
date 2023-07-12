using CalamityMod.NPCs.Polterghast;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class PolterghastSilentMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public int SilentMusicSlot => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Silence");

        public override int NPCType => ModContent.NPCType<Polterghast>();
        public override int? MusicModMusic => SilentMusicSlot;
        public override int VanillaMusic => SilentMusicSlot;
        public override int OtherworldMusic => SilentMusicSlot;

        public override bool AdditionalCheck() => Main.zenithWorld;
    }
}
