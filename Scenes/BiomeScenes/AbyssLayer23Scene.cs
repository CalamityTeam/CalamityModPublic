using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AbyssLayer23Scene : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player) => player.Calamity().ZoneAbyssLayer2 || player.Calamity().ZoneAbyssLayer3;

        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer23";
    }
}
