using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AbyssLayer4Scene : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player) => player.Calamity().ZoneAbyssLayer4;

        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer4";
    }
}
