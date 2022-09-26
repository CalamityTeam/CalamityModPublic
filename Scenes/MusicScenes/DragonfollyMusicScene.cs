using CalamityMod.NPCs.Bumblebirb;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DragonfollyMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<Bumblefuck>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("Dragonfolly");
        public override int VanillaMusic => MusicID.Boss4;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
    }
}
