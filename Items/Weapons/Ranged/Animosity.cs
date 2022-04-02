using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Animosity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Animosity");
            Tooltip.SetDefault(@"50% chance to not consume ammo
Fires a powerful sniper round
Right click to fire a burst of bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 25;
            item.ranged = true;
            item.width = 70;
            item.height = 18;
            item.useTime = 4;
            item.reuseDelay = 15;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item31;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 11f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useTime = 4;
                item.reuseDelay = 13;
                item.useAnimation = 12;
                item.UseSound = SoundID.Item31;
                item.shootSpeed = 10f;
            }
            else
            {
                item.useTime = 35;
                item.reuseDelay = 0;
                item.useAnimation = 35;
                item.UseSound = SoundID.Item40;
                item.shootSpeed = 15f;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-10, 11) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                return false;
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ProjectileID.BulletHighVelocity, (int)(damage * 5.8f), knockBack, player.whoAmI, 0f, 0f);
                return false;
            }
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 50)
                return false;
            return true;
        }
    }
}
