using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class FrostsparkBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frostspark Bullet");
            Tooltip.SetDefault("Has a chance to freeze enemies and explode into electricity\n" +
                "Enemies that are immune to being frozen take more damage from these bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 8;
            item.ranged = true;
            item.width = 8;
            item.height = 8;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.25f;
            item.value = 600;
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<FrostsparkBulletProj>();
            item.shootSpeed = 14f;
            item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MusketBall, 150);
            recipe.AddIngredient(ModContent.ItemType<CryoBar>());
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this, 150);
            recipe.AddRecipe();
        }
    }
}
