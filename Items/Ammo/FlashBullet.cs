using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class FlashBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flash Round");
            Tooltip.SetDefault("Gives off a concussive blast that confuses enemies in a large area for a short time");
        }

        public override void SetDefaults()
        {
            item.damage = 7;
            item.ranged = true;
            item.width = 12;
            item.height = 18;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.15f;
            item.value = Item.sellPrice(copper: 2);
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<FlashBulletProj>();
            item.shootSpeed = 12f;
            item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Grenade);
            recipe.AddIngredient(ItemID.Glass, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 10);
            recipe.AddRecipe();
        }
    }
}
