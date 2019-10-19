using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PerfectDark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Perfect Dark");
            Tooltip.SetDefault("Fires a vile ball that sticks to tiles and explodes");
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.damage = 22;
            item.melee = true;
            item.useAnimation = 25;
            item.useStyle = 1;
            item.useTime = 25;
            item.useTurn = true;
            item.knockBack = 4.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 50;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<DarkBall>();
            item.shootSpeed = 5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RottenChunk, 5);
            recipe.AddIngredient(ItemID.DemoniteBar, 5);
            recipe.AddIngredient(ModContent.ItemType<TrueShadowScale>(), 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
