using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class GalaxySmasherHammer : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/GalaxySmasherMelee";

        private static float RotationIncrement = 0.22f;
        private static int Lifetime = 240;
        private static float ReboundTime = 26f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galaxy Smasher");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 62;
            projectile.height = 62;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = Lifetime;

            // Slightly ignores iframes so it can easily hit twice.
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            drawOffsetX = -12;
            drawOriginOffsetY = -5;
            drawOriginOffsetX = 0;

            // Produces violet dust constantly while in flight. This lights the hammer.
            int numDust = 2;
            for (int i = 0; i < numDust; ++i)
            {
                int dustType = Main.rand.NextBool(6) ? 112 : 173;
                float scale = 0.8f + Main.rand.NextFloat(0.6f);
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Vector2.Zero;
                Main.dust[idx].scale = scale;
            }

            // The hammer makes sound while flying.
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, (int)projectile.position.X, (int)projectile.position.Y);
            }

            // ai[0] stores whether the hammer is returning. If 0, it isn't. If 1, it is.
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= ReboundTime)
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                projectile.tileCollide = false;
                float returnSpeed = GalaxySmasherMelee.Speed;
                float acceleration = 3.2f;
                Player owner = Main.player[projectile.owner];

                // Delete the hammer if it's excessively far away.
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - projectile.Center.X;
                float yDist = playerCenter.Y - projectile.Center.Y;
                float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                if (dist > 3000f)
                    projectile.Kill();

                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (projectile.velocity.X < xDist)
                {
                    projectile.velocity.X = projectile.velocity.X + acceleration;
                    if (projectile.velocity.X < 0f && xDist > 0f)
                        projectile.velocity.X += acceleration;
                }
                else if (projectile.velocity.X > xDist)
                {
                    projectile.velocity.X = projectile.velocity.X - acceleration;
                    if (projectile.velocity.X > 0f && xDist < 0f)
                        projectile.velocity.X -= acceleration;
                }
                if (projectile.velocity.Y < yDist)
                {
                    projectile.velocity.Y = projectile.velocity.Y + acceleration;
                    if (projectile.velocity.Y < 0f && yDist > 0f)
                        projectile.velocity.Y += acceleration;
                }
                else if (projectile.velocity.Y > yDist)
                {
                    projectile.velocity.Y = projectile.velocity.Y - acceleration;
                    if (projectile.velocity.Y > 0f && yDist < 0f)
                        projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner)
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        projectile.Kill();
            }

            // Rotate the hammer as it flies.
            projectile.rotation += RotationIncrement;
            return;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Applies God Slayer Inferno on contact.
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 240);
            OnHitEffect(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            // Applies God Slayer Inferno on contact.
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 240);
            OnHitEffect(target.Center);
        }

        private void OnHitEffect(Vector2 targetPos)
        {
            // Some dust gets produced on impact.
            int dustSets = Main.rand.Next(5, 8);
            int dustRadius = 6;
            Vector2 corner = new Vector2(targetPos.X - dustRadius, targetPos.Y - dustRadius);
            for (int i = 0; i < dustSets; ++i)
            {
                // Bigger, flying orb dust
                float scaleOrb = 1.2f + Main.rand.NextFloat(1f);
                int orb = Dust.NewDust(corner, 2 * dustRadius, 2 * dustRadius, 112);
                Main.dust[orb].noGravity = true;
                Main.dust[orb].velocity *= 4f;
                Main.dust[orb].scale = scaleOrb;

                // Add six sparkles per flying orb
                for (int j = 0; j < 6; ++j)
                {
                    float scaleSparkle = 0.8f + Main.rand.NextFloat(1.1f);
                    int sparkle = Dust.NewDust(corner, 2 * dustRadius, 2 * dustRadius, 173);
                    Main.dust[sparkle].noGravity = true;
                    float dustSpeed = Main.rand.NextFloat(10f, 18f);
                    Main.dust[sparkle].velocity = Main.rand.NextVector2Unit() * dustSpeed;
                    Main.dust[sparkle].scale = scaleSparkle;
                }
            }

            // Makes an explosion sound.
            Main.PlaySound(SoundID.Item14, projectile.position);

            // Three death lasers (aka "Nebula Shots") swarm the target.
            int laserID = ModContent.ProjectileType<NebulaShot>();
            int laserDamage = (int)(0.2f * projectile.damage);
            float laserKB = 2.5f;
            int numLasers = 3;
            for (int i = 0; i < numLasers; ++i)
            {
                float startDist = Main.rand.NextFloat(260f, 270f);
                Vector2 startDir = Main.rand.NextVector2Unit();
                Vector2 startPoint = targetPos + startDir * startDist;

                float laserSpeed = Main.rand.NextFloat(15f, 18f);
                Vector2 velocity = startDir * -laserSpeed;

                if (projectile.owner == Main.myPlayer)
                {
                    int proj = Projectile.NewProjectile(startPoint, velocity, laserID, laserDamage, laserKB, projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].Calamity().forceMelee = true;
                        Main.projectile[proj].tileCollide = false;
                        Main.projectile[proj].timeLeft = 30;
                    }
                }
            }
        }
    }
}
