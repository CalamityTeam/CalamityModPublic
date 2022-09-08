using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class SulphurousSeaScene : ModSceneEffect
    {
        public override bool IsSceneEffectActive(Player player) => player.Calamity().ZoneSulphur;

        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/SulphurBG";
    }
}
