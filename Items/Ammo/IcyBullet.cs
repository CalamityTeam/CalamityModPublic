using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
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
            item.damage = 10;
            item.ranged = true;
            item.consumable = true;
            item.width = 18;
            item.height = 16;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 0, 0, 15);
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<Projectiles.IcyBullet>();
            item.shootSpeed = 5f;
            item.ammo = AmmoID.Bullet;
            item.maxStack = 999;
        }
    }
}
