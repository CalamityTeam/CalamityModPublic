//using CalamityMod.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AcidRainBiome : ModBiome
    {
        public override string BestiaryIcon => "BiomeManagers/AcidRainIcon";
        public override string BackgroundPath => "Backgrounds/MapBackgrounds/SulphurBG"; //Probably needs a unique bg
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow; //Sulphurous Sea overrides it

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Rain");
        }

        //Does this even need proper detection, the detection code is done elsewhere
        /*public override bool IsBiomeActive(Player player)
        {
            return AcidRainEvent.AcidRainEventIsOngoing && !DownedBossSystem.downedAquaticScourge && !DownedBossSystem.downedPolterghast;
        }*/
    }
}
