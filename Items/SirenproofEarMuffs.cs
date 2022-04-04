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
            Item.width = 44;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateInventory(Player player)
        {
            if (Item.favorited)
                player.Calamity().disableAnahitaSpawns = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LivingShard>(2)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
