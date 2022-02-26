using CalamityMod.Events;
using CalamityMod.Items.Materials;
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
            Tooltip.SetDefault("Causes an acidic downpour in the Sulphurous Sea\n" +
                "Not consumable");

        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.maxStack = 1;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !CalamityWorld.rainingAcid;
        }

        public override bool UseItem(Player player)
        {
            CalamityNetcode.SyncWorld();
            AcidRainEvent.TryStartEvent(true);
            return true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SulfuricScale>(), 5);
            recipe.needWater = true;
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
