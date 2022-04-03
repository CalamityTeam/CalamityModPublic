using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class AngelicShotgun : ModItem
    {
        private static int BaseDamage = 96;
        private static float BulletSpeed = 12f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angelic Shotgun");
            Tooltip.SetDefault(@"Each shot casts a radiant beam of holy light from the sky
Converts musket balls into illuminated bullets
Fighting 'til the war's won");
        }

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.width = 44;
            Item.height = 7;

            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.UseSound = SoundID.Item38;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;

            Item.shootSpeed = BulletSpeed;
            Item.shoot = ModContent.ProjectileType<IlluminatedBullet>();
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-17, -3);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int NumBullets = Main.rand.Next(5, 8);
            Vector2 baseVelocity = new Vector2(speedX, speedY).SafeNormalize(Vector2.Zero) * BulletSpeed;

            // Fire a shotgun spread of bullets.
            for (int i = 0; i < NumBullets; ++i)
            {
                float dx = Main.rand.NextFloat(-1.3f, 1.3f);
                float dy = Main.rand.NextFloat(-1.3f, 1.3f);
                Vector2 randomVelocity = baseVelocity + new Vector2(dx, dy);

                if (type == ProjectileID.Bullet)
                    Projectile.NewProjectile(position, randomVelocity, ModContent.ProjectileType<IlluminatedBullet>(), damage, knockBack, player.whoAmI);
                else
                    Projectile.NewProjectile(position, randomVelocity, type, damage, knockBack, player.whoAmI);
            }

            // Spawn a beam from the sky ala Deathhail Staff or Lunar Flare
            float laserSpeed = 8f;
            int laserDamage = 3 * damage;
            float laserKB = 5f;

            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseDX = (float)Main.mouseX - Main.screenPosition.X - rrp.X;
            float mouseDY = (float)Main.mouseY - Main.screenPosition.Y - rrp.Y;

            // Correct for grav potion
            if (player.gravDir == -1f)
                mouseDY = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - rrp.Y;

            // Unnecessary NaN checks
            float mouseDist = (float)Math.Sqrt((double)(mouseDX * mouseDX + mouseDY * mouseDY));
            if ((float.IsNaN(mouseDX) && float.IsNaN(mouseDY)) || (mouseDX == 0f && mouseDY == 0f))
            {
                mouseDX = (float)player.direction;
            }
            else
            {
                mouseDist = laserSpeed / mouseDist;
            }

            rrp = new Vector2(player.Center.X + (Main.rand.NextFloat(200f) * -player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
            rrp.X = (rrp.X + player.Center.X) / 2f + Main.rand.NextFloat(-200f, 200f);
            rrp.Y -= 100f;
            mouseDX = (float)Main.mouseX + Main.screenPosition.X - rrp.X;
            mouseDY = (float)Main.mouseY + Main.screenPosition.Y - rrp.Y;
            if (mouseDY < 0f)
            {
                mouseDY *= -1f;
            }
            if (mouseDY < 20f)
            {
                mouseDY = 20f;
            }
            mouseDist = (float)Math.Sqrt((double)(mouseDX * mouseDX + mouseDY * mouseDY));
            mouseDist = laserSpeed / mouseDist;
            mouseDX *= mouseDist;
            mouseDY *= mouseDist;
            Projectile.NewProjectile(rrp, new Vector2(mouseDX, mouseDY + Main.rand.NextFloat(-0.8f, 0.8f)), ModContent.ProjectileType<AngelicBeam>(), laserDamage, laserKB, player.whoAmI);

            // Play the sound of the laser beam
            SoundEngine.PlaySound(SoundID.Item72, player.Center);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.SunplateBlock, 75).AddIngredient(ModContent.ItemType<UeliaceBar>(), 10).AddIngredient(ModContent.ItemType<DivineGeode>(), 15).AddIngredient(ModContent.ItemType<CoreofCinder>(), 7).AddTile(TileID.LunarCraftingStation).Register();
        }

    }
}
