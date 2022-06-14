using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class ArsenalLabBiome : ModBiome
    {
        public override string BestiaryIcon => "BiomeManagers/LaboratoryIcon";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arsenal Lab");
        }

        //Does this even need proper detection, this file solely exists for a few critter entries and the detection code is done elsewhere
        /*public override bool IsBiomeActive(Player player)
        {
            return BiomeTileCounterSystem.ArsenalLabTiles > 150;
        }*/
    }
}
