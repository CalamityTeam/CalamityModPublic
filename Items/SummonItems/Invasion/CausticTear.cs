using CalamityMod.Events;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems.Invasion
{
    public class CausticTear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Caustic Tear");
            Tooltip.SetDefault("Toggles the acid rain in the Sulphurous Sea");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.maxStack = 1;
            item.rare = 6;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.consumable = false;
        }

        public override bool UseItem(Player player)
        {
            if (!CalamityWorld.rainingAcid)
            {
                AcidRainEvent.TryStartEvent();
            }
            else
            {
                CalamityWorld.acidRainPoints = 0;
                CalamityWorld.triedToSummonOldDuke = false;
                AcidRainEvent.UpdateInvasion(false);
            }
            return true;
        }
    }
}
