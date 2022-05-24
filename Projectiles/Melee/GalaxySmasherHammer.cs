using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = Lifetime;

            // Slightly ignores iframes so it can easily hit twice.
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            DrawOffsetX = -12;
            DrawOriginOffsetY = -5;
            DrawOriginOffsetX = 0;

            // Produces violet dust constantly while in flight. This lights the hammer.
            int numDust = 2;
            for (int i = 0; i < numDust; ++i)
            {
                int dustType = Main.rand.NextBool(6) ? 112 : 173;
                float scale = 0.8f + Main.rand.NextFloat(0.6f);
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Vector2.Zero;
                Main.dust[idx].scale = scale;
            }

            // The hammer makes sound while flying.
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            // ai[0] stores whether the hammer is returning. If 0, it isn't. If 1, it is.
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= ReboundTime)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.tileCollide = false;
                float returnSpeed = GalaxySmasherMelee.Speed;
                float acceleration = 3.2f;
                Player owner = Main.player[Projectile.owner];

                // Delete the hammer if it's excessively far away.
                Vector2 playerCenter = owner.Center;
                float xDist = playerCenter.X - Projectile.Center.X;
                float yDist = playerCenter.Y - Projectile.Center.Y;
                float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                if (dist > 3000f)
                    Projectile.Kill();

                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    if (Projectile.velocity.X < 0f && xDist > 0f)
                        Projectile.velocity.X += acceleration;
                }
                else if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    if (Projectile.velocity.X > 0f && xDist < 0f)
                        Projectile.velocity.X -= acceleration;
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    if (Projectile.velocity.Y < 0f && yDist > 0f)
                        Projectile.velocity.Y += acceleration;
                }
                else if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    if (Projectile.velocity.Y > 0f && yDist < 0f)
                        Projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == Projectile.owner)
                    if (Projectile.Hitbox.Intersects(owner.Hitbox))
                        Projectile.Kill();
            }

            // Rotate the hammer as it flies.
            Projectile.rotation += RotationIncrement;
            return;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
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
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // Three death lasers (aka "Nebula Shots") swarm the target.
            int laserID = ModContent.ProjectileType<NebulaShot>();
            int laserDamage = (int)(0.2f * Projectile.damage);
            float laserKB = 2.5f;
            int numLasers = 3;
            for (int i = 0; i < numLasers; ++i)
            {
                float startDist = Main.rand.NextFloat(260f, 270f);
                Vector2 startDir = Main.rand.NextVector2Unit();
                Vector2 startPoint = targetPos + startDir * startDist;

                float laserSpeed = Main.rand.NextFloat(15f, 18f);
                Vector2 velocity = startDir * -laserSpeed;

                if (Projectile.owner == Main.myPlayer)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), startPoint, velocity, laserID, laserDamage, laserKB, Projectile.owner);
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
