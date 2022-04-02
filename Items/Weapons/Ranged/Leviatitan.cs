using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Leviatitan : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leviatitan");
            Tooltip.SetDefault("Converts musket balls into toxic and normal water blasts");
        }

        public override void SetDefaults()
        {
            item.damage = 80;
            item.ranged = true;
            item.width = 82;
            item.height = 28;
            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item92;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AquaBlast>();
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + Main.rand.Next(-10, 11) * 0.05f;
            float SpeedY = speedY + Main.rand.Next(-10, 11) * 0.05f;

            if (type == ProjectileID.Bullet)
            {
                if (Main.rand.NextBool(3))
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<AquaBlastToxic>(), (int)(damage * 1.5), knockBack, player.whoAmI);
                else
                    Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<AquaBlast>(), damage, knockBack, player.whoAmI);
            }
            else
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);

            return false;
        }
    }
}
