using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Shimmerspark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shimmerspark");
            Tooltip.SetDefault("Fires stars when enemies are near");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.Chik);
            item.damage = 36;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ShimmersparkProjectile>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VerstaltiteBar", 6);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
