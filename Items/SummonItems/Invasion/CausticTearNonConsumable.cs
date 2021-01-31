using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems.Invasion
{
    public class CausticTearNonConsumable : ModItem
    {
        public override string Texture => "CalamityMod/Items/SummonItems/Invasion/CausticTear";

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
            item.rare = ItemRarityID.LightPurple;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool UseItem(Player player)
        {
            if (!CalamityWorld.rainingAcid)
            {
                AcidRainEvent.TryStartEvent(true);
            }
            else
            {
                CalamityWorld.acidRainPoints = 0;
                CalamityWorld.triedToSummonOldDuke = false;
                AcidRainEvent.UpdateInvasion(false);
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CorrodedFossil>(), 10);
            recipe.AddIngredient(ModContent.ItemType<CausticTear>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
