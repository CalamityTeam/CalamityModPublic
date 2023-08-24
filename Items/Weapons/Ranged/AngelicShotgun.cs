using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class AngelicShotgun : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        private static int BaseDamage = 96;
        private static float BulletSpeed = 12f;

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.width = 86;
            Item.height = 38;

            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.UseSound = SoundID.Item38;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int NumBullets = Main.rand.Next(5, 8);
            Vector2 baseVelocity = velocity.SafeNormalize(Vector2.Zero) * BulletSpeed;

            // Fire a shotgun spread of bullets.
            for (int i = 0; i < NumBullets; ++i)
            {
                float dx = Main.rand.NextFloat(-1.3f, 1.3f);
                float dy = Main.rand.NextFloat(-1.3f, 1.3f);
                Vector2 randomVelocity = baseVelocity + new Vector2(dx, dy);

                if (type == ProjectileID.Bullet)
                    Projectile.NewProjectile(source, position, randomVelocity, ModContent.ProjectileType<IlluminatedBullet>(), damage, knockback, player.whoAmI);
                else
                    Projectile.NewProjectile(source, position, randomVelocity, type, damage, knockback, player.whoAmI);
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
            Projectile.NewProjectile(source, rrp, new Vector2(mouseDX, mouseDY + Main.rand.NextFloat(-0.8f, 0.8f)), ModContent.ProjectileType<AngelicBeam>(), laserDamage, laserKB, player.whoAmI);

            // Play the sound of the laser beam
            SoundEngine.PlaySound(SoundID.Item72, player.Center);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SunplateBlock, 75).
                AddIngredient<UelibloomBar>(10).
                AddIngredient<DivineGeode>(15).
                AddIngredient<CoreofSunlight>(7).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

    }
}
