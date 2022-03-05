using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class SuperballBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Superball Bullet");
            Tooltip.SetDefault("Bounces at extreme speeds");
        }

        public override void SetDefaults()
        {
            item.damage = 6;
            item.ranged = true;
            item.width = 8;
            item.height = 8;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.5f;
            item.value = Item.sellPrice(copper: 2);
            item.rare = ItemRarityID.Green;
            item.shoot = ModContent.ProjectileType<SuperballBulletProj>();
            item.shootSpeed = 1f;
            item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MeteorShot, 150);
            recipe.AddIngredient(ModContent.ItemType<VictoryShard>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 150);
            recipe.AddRecipe();
        }
    }
}
