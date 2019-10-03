using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class CoinofDeceit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coin of Deceit");
            Tooltip.SetDefault("6% increased rogue crit chance");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.accessory = true;
			item.rare = 1;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
            player.GetCalamityPlayer().throwingCrit += 6;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GoldBar, 4);
            recipe.AddIngredient(ItemID.CopperBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GoldBar, 4);
            recipe.AddIngredient(ItemID.TinBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.PlatinumBar, 4);
            recipe.AddIngredient(ItemID.CopperBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.PlatinumBar, 4);
            recipe.AddIngredient(ItemID.TinBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
