using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class IcyBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icy Bullet");
            Tooltip.SetDefault("Can hit up to three times\nBreaks into ice shards on last impact");
        }
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.consumable = true;
            Item.width = 14;
            Item.height = 20;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 0, 0, 80);
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<IcyBulletProj>();
            Item.shootSpeed = 5f;
            Item.ammo = AmmoID.Bullet;
            Item.maxStack = 999;
        }
    }
}
