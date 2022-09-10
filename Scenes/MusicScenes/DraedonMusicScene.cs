using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DraedonMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<Draedon>();
        public override int? MusicModMusic => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/DraedonAmbience");
        public override int VanillaMusic => -1;
        public override int OtherworldMusic => -1;

        public override bool AdditionalCheck() => CalamityGlobalNPC.draedonAmbience != -1;
    }
}
