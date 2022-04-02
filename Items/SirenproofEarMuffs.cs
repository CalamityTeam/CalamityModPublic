using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items
{
    public class SirenproofEarMuffs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sirenproof Earmuffs");
            Tooltip.SetDefault("Favorite this item to prevent Anahita from spawning near you");
        }
        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 34;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
        }

        public override void UpdateInventory(Player player)
        {
            if (item.favorited)
                player.Calamity().disableAnahitaSpawns = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 2);
            recipe.AddIngredient(ItemID.Silk, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
