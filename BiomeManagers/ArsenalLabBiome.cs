using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class ArsenalLabBiome : ModBiome
    {
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/LaboratoryIcon";
		// Could use its own unique background if someone even bothers
        public override string BackgroundPath => "Terraria/Images/MapBG32";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arsenal Labs");
        }

        //Does this even need proper detection, this file solely exists for a few critter entries and the detection code is done elsewhere
        /*public override bool IsBiomeActive(Player player)
        {
            return BiomeTileCounterSystem.ArsenalLabTiles > 150;
        }*/
    }
}
