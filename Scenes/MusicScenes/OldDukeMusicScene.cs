using CalamityMod.NPCs.OldDuke;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class OldDukeMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int NPCType => ModContent.NPCType<OldDuke>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("OldDuke");
        public override int VanillaMusic => MusicID.DukeFishron;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
    }
}
