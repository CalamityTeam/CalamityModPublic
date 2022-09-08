using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AbyssLayer1Scene : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player) => player.Calamity().ZoneAbyssLayer1 || player.Calamity().ZoneSunkenSea; // Placeholder for the Sunken Sea

        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer1";
    }
}
