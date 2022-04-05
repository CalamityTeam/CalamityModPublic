using CalamityMod.Backgrounds;
using CalamityMod.Systems;
using CalamityMod.Waters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class BrimstoneCragsBiome : ModBiome
    {
        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("Crag") ?? MusicID.Eerie;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Crags");
        }

        public override bool IsBiomeActive(Player player)
        {
            return BiomeTileCounterSystem.BrimstoneCragTiles > 100;
        }
    }
}
