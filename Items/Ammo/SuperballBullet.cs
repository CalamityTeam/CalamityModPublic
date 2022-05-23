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
            SacrificeTotal = 99;
            DisplayName.SetDefault("Superball Bullet");
            Tooltip.SetDefault("Bounces at extreme speeds");
        }

        public override void SetDefaults()
        {
            Item.damage = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 1.5f;
            Item.value = Item.sellPrice(copper: 2);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<SuperballBulletProj>();
            Item.shootSpeed = 1f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            CreateRecipe(150).
                AddIngredient(ItemID.MeteorShot, 150).
                AddIngredient<VictoryShard>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
