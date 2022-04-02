using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items
{
    public class BleachBall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bleach Ball");
            Tooltip.SetDefault("Favorite this item to prevent the Aquatic Scourge from naturally spawning near you");
        }
        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 46;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
        }

        public override void UpdateInventory(Player player)
        {
            if (item.favorited)
                player.Calamity().disableNaturalScourgeSpawns = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<EbonianGel>(), 5);
            recipe.AddIngredient(ItemID.CalmingPotion);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
