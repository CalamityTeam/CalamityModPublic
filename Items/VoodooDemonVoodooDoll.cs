using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class VoodooDemonVoodooDoll : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Voodoo Demon Voodoo Doll");
            Tooltip.SetDefault("Favorite this item to prevent voodoo demons from spawning near you");
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
                player.Calamity().disableVoodooSpawns = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HellstoneBar, 5);
            recipe.AddIngredient(ItemID.Silk, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
