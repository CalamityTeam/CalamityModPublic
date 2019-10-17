using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class SilencingSheath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silencing Sheath");
            Tooltip.SetDefault("+50 maximum stealth\n" +
                "10% increased stealth regeneration while standing still");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 34;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.rogueStealthMax += 0.5f;
            modPlayer.stealthGenStandstill += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DemoniteBar, 8);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.ShadowScale, 3);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CrimtaneBar, 8);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.TissueSample, 3);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
