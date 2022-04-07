using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Ammo
{
    public class AccelerationBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acceleration Round");
            Tooltip.SetDefault("Gains speed over time");
        }

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 1.25f;
            Item.value = Item.sellPrice(copper: 2);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<AccelerationBulletProj>();
            Item.shootSpeed = 1f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(150).
                AddIngredient(ItemID.MusketBall, 150).
                AddIngredient<VictoryShard>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
