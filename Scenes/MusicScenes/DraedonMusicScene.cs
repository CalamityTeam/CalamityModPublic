using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DraedonMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int NPCType => ModContent.NPCType<Draedon>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("ExoMechs");
        public override int VanillaMusic => MusicID.Boss3;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
    }
}
