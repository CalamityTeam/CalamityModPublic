using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class RealmRavager : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Realm Ravager");
            Tooltip.SetDefault("Shoots a burst of 3 to 4 bullets\n" +
                "Converts musket balls into explosive bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 50;
            item.ranged = true;
            item.width = 76;
            item.height = 32;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item38;
            item.autoReuse = true;
            item.shootSpeed = 30f;
            item.shoot = ModContent.ProjectileType<RealmRavagerBullet>();
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numBullets = Main.rand.NextBool() ? 4 : 3;
            for (int index = 0; index < numBullets; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-75, 76) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-75, 76) * 0.05f;

                if (type == ProjectileID.Bullet)
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<RealmRavagerBullet>(), damage, knockBack, player.whoAmI);
                else
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
