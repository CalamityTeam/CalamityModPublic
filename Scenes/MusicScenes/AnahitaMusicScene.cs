using CalamityMod.NPCs;
using CalamityMod.NPCs.Leviathan;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AnahitaMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<Anahita>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("Anahita");
        public override int VanillaMusic => MusicID.Boss3;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;

        public override bool AdditionalCheck() => CalamityGlobalNPC.LeviAndAna == -1;
    }
}
