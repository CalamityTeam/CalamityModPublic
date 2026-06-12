using CalamityMod.CustomRecipes;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class CrimsonEffigy : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";

        public static float DamageBoost = 0.1f;
        public static int DefenseBoost = 5;
        public static float MaxHealthLossPercent = 0.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent(), DefenseBoost, MaxHealthLossPercent.ToPercent());

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.CrimsonEffigy>());
            Item.value = Item.buyPrice(gold: 10); // Sold by Shady Salesman
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundCrimsonEffigy)
            {
                RecipeUnlockHandler.HasFoundCrimsonEffigy = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CorruptionEffigy>().
                AddTile(TileID.TinkerersWorkbench).
                AddCondition(Condition.InGraveyard).
                Register()
                .DisableDecraft();
        }
    }
}
