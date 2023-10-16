using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Ranged
{
    public class BloodBoilerFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Ranged/BloodFlames";

        private bool playedSound = false;
        public int Time = 0;
        public Vector2 bloodCloudReturn;
        public override void SetDefaults()
        {
            Projectile.width = 43;
            Projectile.height = 43;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
                Time++;
            else
                Time -= 3;
            Projectile.rotation += 3 * Projectile.direction;
            if (Projectile.timeLeft == 295)
            {
                Projectile.scale = 0.04f;
                if (Main.rand.NextBool())
                {
                    int bloodLifetime = Main.rand.Next(22, 36);
                    float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                    Color bloodColor = Main.rand.NextBool() ? Color.Firebrick : Color.Red;

                    float randomSpeedMultiplier = Main.rand.NextFloat(0.8f, 1.55f);
                    Vector2 bloodVelocity = Projectile.velocity.RotatedByRandom(0.5) * randomSpeedMultiplier;
                    BloodParticle blood = new BloodParticle(Projectile.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                    GeneralParticleHandler.SpawnParticle(blood);
                }
            }
            if (Projectile.timeLeft == 296)
            {
                Projectile.alpha = 80;
            }
            if (Projectile.timeLeft < 296 && Main.rand.NextBool(5))
            {
                Color smokeColor = Main.rand.NextBool() ? Color.Red : Color.Firebrick;
                Vector2 smokePosition = Projectile.Center + Main.rand.NextVector2Circular(5 + Time * 0.3f, 5 + Time * 0.3f);
                float smokeScale = Projectile.ai[1] == 1 ? Main.rand.NextFloat(0.1f, 0.35f) + Time * 0.01f : Main.rand.NextFloat(0.5f, 1.6f);
                float smokeOpacity = Projectile.ai[1] == 1 ? 120 + Time * 0.9f : 170 + -Time * 0.7f;
                Particle smoke = new MediumMistParticle(smokePosition, Projectile.ai[1] == 1 ? Projectile.velocity * 0.5f : Vector2.Zero, smokeColor, Color.Black, smokeScale, smokeOpacity, Main.rand.NextFloat(0.2f, -0.2f));
                GeneralParticleHandler.SpawnParticle(smoke);
            }
            if (Projectile.ai[1] == 0f)
            {
                if (Main.rand.NextBool(7))
                {
                    DirectionalPulseRing pulse = new DirectionalPulseRing(Projectile.Center + Main.rand.NextVector2Circular(7 + Time * 0.4f, 7 + Time * 0.4f), Vector2.Zero, Main.rand.NextBool(3) ? Color.Red : Color.Firebrick, new Vector2(1, 1), 0, Main.rand.NextFloat(0.02f, 0.07f) + Time * 0.0006f, 0f, 35);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
                Projectile.scale += 0.006f;
            }
            else if (Projectile.scale > 0.01)
                Projectile.scale -= 0.01f;
            if (!playedSound)
            {
                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
                playedSound = true;
            }

            Lighting.AddLight(Projectile.Center, 1f, 0f, 0f);
            if (Projectile.timeLeft == 235)
                bloodCloudReturn = Projectile.velocity;
            if (Projectile.timeLeft <= 235 && Projectile.ai[1] == 0f)
                Projectile.velocity *= 0.98f;
            if (Projectile.timeLeft == 220)
            {
                Projectile.velocity = -bloodCloudReturn.RotatedBy(Projectile.ai[2] == 5 ? -0.7f : 0.7f) * 1.2f;
                for (int i = 0; i <= 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(40, 40), Main.rand.NextBool(3) ? 60 : 90, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(2f)) * Main.rand.NextFloat(0.3f, 2.2f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.2f, 2f);
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(40, 40), Main.rand.NextBool(3) ? 60 : 90, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.2f));
                    dust2.noGravity = true;
                    dust2.scale = Main.rand.NextFloat(1.2f, 2f);
                }
            }
            if (Projectile.timeLeft == 209)
            {
                Projectile.ai[1] = 1f;
            }

            if (Projectile.ai[1] == 1f)
            {
                Projectile.extraUpdates = 3;
                Projectile.alpha = 160;
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Main.rand.NextBool(3) ? 60 : 5);
                dust.scale = Main.rand.NextFloat(0.4f, 0.7f);
                dust.noGravity = true;

                Player player = Main.player[Projectile.owner];

                // Delete the projectile if it's excessively far away.
                Vector2 playerCenter = player.Center;
                float xDist = playerCenter.X - Projectile.Center.X;
                float yDist = playerCenter.Y - Projectile.Center.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                if (dist > 3000f)
                    Projectile.Kill();

                dist = 20f / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + 0.1f;
                    if (Projectile.velocity.X < 0f && xDist > 0f)
                        Projectile.velocity.X += 0.1f;
                }
                else if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - 0.1f;
                    if (Projectile.velocity.X > 0f && xDist < 0f)
                        Projectile.velocity.X -= 0.1f;
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.1f;
                    if (Projectile.velocity.Y < 0f && yDist > 0f)
                        Projectile.velocity.Y += 0.1f;
                }
                else if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - 0.1f;
                    if (Projectile.velocity.Y > 0f && yDist < 0f)
                        Projectile.velocity.Y -= 0.1f;
                }

                // Delete the projectile if it touches its owner. Has a chance to heal the player again
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.Hitbox.Intersects(player.Hitbox))
                    {
                        if (Main.rand.NextBool(4) && !Main.player[Projectile.owner].moonLeech)
                        {
                            player.statLife += 1;
                            player.HealEffect(1);
                        }
                        Projectile.Kill();
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 240);

            if (!target.canGhostHeal || Main.player[Projectile.owner].moonLeech)
                return;

            Player player = Main.player[Projectile.owner];
            if (Main.rand.NextBool(3))
            {
                int healAmt = Main.rand.Next(1, 4);
                player.statLife += healAmt;
                player.HealEffect(healAmt);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/BloodFlames").Value);
            return false;
        }
    }
}
