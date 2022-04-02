using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class AntiCystOintment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anti-Cyst Ointment");
            Tooltip.SetDefault("Favorite this item to prevent perforator cysts from spawning near you");
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
                player.Calamity().disablePerfCystSpawns = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrimtaneBar, 5);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
