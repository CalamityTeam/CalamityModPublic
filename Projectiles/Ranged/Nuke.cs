using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class Nuke : ModProjectile
    {
        public int flarePowderTimer = 12;
        public static Item FalseLauncher = null;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuke");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 125;
            projectile.ranged = true;
        }

        private static void DefineFalseLauncher()
        {
            int rocketID = ItemID.RocketLauncher;
            FalseLauncher = new Item();
            FalseLauncher.SetDefaults(rocketID, true);
        }

        public override void AI()
        {
            //Animation
            projectile.frameCounter++;
            if (projectile.frameCounter > 7)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 2)
            {
                projectile.frame = 0;
            }

            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * projectile.direction;


            flarePowderTimer--;
            if (flarePowderTimer <= 0)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<DragonDust>(), (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                }
                flarePowderTimer = 12;
            }
            if (Math.Abs(projectile.velocity.X) >= 8f || Math.Abs(projectile.velocity.Y) >= 8f)
            {
                float num247 = projectile.velocity.X * 0.5f;
                float num248 = projectile.velocity.Y * 0.5f;
                int num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 244, 0f, 0f, 100, default, 1f);
                Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                Main.dust[num249].velocity *= 0.2f;
                Main.dust[num249].noGravity = true;
                num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 244, 0f, 0f, 100, default, 0.5f);
                Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num249].velocity *= 0.05f;

            }
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 192);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int i = 0; i < 40; i++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 60; i++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 10);

            // Construct a fake item to use with vanilla code for the sake of picking ammo.
            if (FalseLauncher is null)
                DefineFalseLauncher();
            Player player = Main.player[projectile.owner];
            int projID = ProjectileID.RocketI;
            float shootSpeed = 0f;
            bool canShoot = true;
            int damage = 0;
            float kb = 0f;
            player.PickAmmo(FalseLauncher, ref projID, ref shootSpeed, ref canShoot, ref damage, ref kb, true);
            int blastRadius = 0;
            if (projID == ProjectileID.RocketII)
                blastRadius = 6;
            else if (projID == ProjectileID.RocketIV)
                blastRadius = 12;

            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 22);

            if (projectile.owner == Main.myPlayer && blastRadius > 0)
            {
                CalamityUtils.ExplodeandDestroyTiles(projectile, blastRadius, true, new List<int>() { }, new List<int>() { });
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
