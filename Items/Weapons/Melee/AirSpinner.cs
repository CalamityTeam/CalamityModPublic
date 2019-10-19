using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AirSpinner : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Air Spinner");
            Tooltip.SetDefault("Fires feathers when enemies are near");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.Valor);
            item.damage = 20;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 4;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<AirSpinnerProjectile>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 6);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
