using CalamityMod.CustomRecipes;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class CorruptionEffigy : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";

        public static float MoveSpeedBoost = 0.1f;
        public static int CritBoost = 10; // Both 10% so we only need just one in the tooltip
        public static float DamageReductionLoss = 0.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBoost, DamageReductionLoss.ToPercent());

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.CorruptionEffigy>());
            Item.value = Item.buyPrice(gold: 10); // Sold by Shady Salesman
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundCorruptionEffigy)
            {
                RecipeUnlockHandler.HasFoundCorruptionEffigy = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrimsonEffigy>().
                AddTile(TileID.TinkerersWorkbench).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();
        }
    }
}
