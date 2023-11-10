using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class BloodBoilerFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private bool playedSound = false;
        public int Time = 0;
        public float particleSize = 15;
        public Vector2 bloodCloudReturn;
        public bool improvedHeal = false;
        public bool setHomingVelocity = false;
        public float HomingVelocity = 0.18f;
        public override void SetDefaults()
        {
            Projectile.width = 43;
            Projectile.height = 43;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            if (Projectile.timeLeft > 220) // Main visual size changes
                particleSize += 0.5f;
            else
                particleSize -= 1f;

            Time++;

            if (Projectile.timeLeft == 295) // Blood produced from the muzzle when firing the weapon
            {
                int bloodLifetime = Main.rand.Next(22, 36);
                float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                Color bloodColor = Main.rand.NextBool() ? Color.Firebrick : Color.Red;

                float randomSpeedMultiplier = Main.rand.NextFloat(0.8f, 1.55f);
                Vector2 bloodVelocity = Projectile.velocity.RotatedByRandom(0.5) * randomSpeedMultiplier + new Vector2 (0, -3);
                BloodParticle blood = new BloodParticle(Projectile.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                GeneralParticleHandler.SpawnParticle(blood);
            }
            if (Projectile.timeLeft < 296) // The main visual of the projectile and the blood mist it produces before turning around
            {
                BloodBoilerMetaball2.SpawnParticle(Projectile.Center, particleSize * 2.3f);
                BloodBoilerMetaball.SpawnParticle(Projectile.Center, particleSize * 1.7f);

                if (Main.rand.NextBool(6) && Projectile.ai[1] == 0f)
                {
                    Color smokeColor = Main.rand.NextBool() ? Color.Red : Color.Firebrick;
                    Vector2 smokePosition = Projectile.Center + Main.rand.NextVector2Circular(5 + Time * 0.3f, 5 + Time * 0.3f);
                    float smokeScale = Main.rand.NextFloat(0.5f, 1.6f);
                    float smokeOpacity = 170 + -Time * 0.6f;
                    Particle smoke = new MediumMistParticle(smokePosition, Vector2.Zero, smokeColor, Color.Black, smokeScale, smokeOpacity, Main.rand.NextFloat(0.2f, -0.2f));
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
            }
            
            if (Projectile.ai[1] == 0f) // Blood pulses on the projectile before it turns around
            {
                if (Main.rand.NextBool(9))
                {
                    DirectionalPulseRing pulse = new DirectionalPulseRing(Projectile.Center + Main.rand.NextVector2Circular(7 + Time * 0.4f, 7 + Time * 0.4f), Vector2.Zero, Main.rand.NextBool(3) ? Color.Red : Color.Firebrick, new Vector2(1, 1), 0, Main.rand.NextFloat(0.02f, 0.07f) + Time * 0.0006f, 0f, 35);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
            }
            if (!playedSound)
            {
                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
                playedSound = true;
            }

            Lighting.AddLight(Projectile.Center, 1f, 0f, 0f);

            // A bunch of small things mostly for visuals when the projectile starts turning around before it homes on the player
            if (Projectile.timeLeft == 235)
                bloodCloudReturn = Projectile.velocity;
            if (Projectile.timeLeft <= 235 && Projectile.ai[1] == 0f)
                Projectile.velocity *= 0.98f;
            if (Projectile.timeLeft == 220)
            {
                Projectile.velocity = -bloodCloudReturn.RotatedBy(Projectile.ai[2] == 5 ? -0.7f : 0.7f) * 1.2f;
                for (int i = 0; i <= 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * 2 + Main.rand.NextVector2Circular(35, 35), Main.rand.NextBool(3) ? 60 : 296, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(0.5f)) * Main.rand.NextFloat(1.7f, 3.2f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.2f, 2f);
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * 2 + Main.rand.NextVector2Circular(35, 35), Main.rand.NextBool(3) ? 60 : 296, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.7f));
                    dust2.noGravity = true;
                    dust2.scale = Main.rand.NextFloat(1.2f, 2f);
                }
            }
            if (Projectile.timeLeft == 209) // Begin returning to player
            {
                Projectile.ai[1] = 1f;
            }

            if (Projectile.ai[1] == 1f) // Dusts as the projectile homes back in on the player
            {
                if (!setHomingVelocity)
                {
                    HomingVelocity = Main.rand.NextFloat(0.29f, 0.32f);
                    setHomingVelocity = true;
                }
                Projectile.extraUpdates = 5;
                bool dustEffect = Main.rand.NextBool(3) ? false : true;
                int dustColor = dustEffect ? 296 : 60;
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(dustEffect ? 0 : 5, dustEffect ? 0 : 5), dustColor);
                dust.scale = dustEffect ? Main.rand.NextFloat(1.1f, 1.45f) : Main.rand.NextFloat(0.9f, 1.2f);
                dust.velocity = dustEffect ? Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f) : Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(0.7f, 1.2f);
                dust.alpha = 100;
                dust.noLight = true;
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
                    Projectile.velocity.X = Projectile.velocity.X + HomingVelocity;
                    if (Projectile.velocity.X < 0f && xDist > 0f)
                        Projectile.velocity.X += HomingVelocity;
                }
                else if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - HomingVelocity;
                    if (Projectile.velocity.X > 0f && xDist < 0f)
                        Projectile.velocity.X -= HomingVelocity;
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + HomingVelocity;
                    if (Projectile.velocity.Y < 0f && yDist > 0f)
                        Projectile.velocity.Y += HomingVelocity;
                }
                else if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - HomingVelocity;
                    if (Projectile.velocity.Y > 0f && yDist < 0f)
                        Projectile.velocity.Y -= HomingVelocity;
                }
                if (Projectile.timeLeft == 5) // Absolutely make sure the player gets the chance to heal
                    Projectile.Center = playerCenter;
                // Delete the projectile if it touches its owner. Has a chance to heal the player again
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.Hitbox.Intersects(player.Hitbox))
                    {
                        if (Main.rand.NextBool(4) && !Main.player[Projectile.owner].moonLeech)
                        {
                            int bonusHeal = Main.rand.NextBool(3) ? 3 : 2;
                            player.statLife += improvedHeal ? bonusHeal : 1;
                            player.HealEffect(improvedHeal ? bonusHeal : 1);
                        }
                        Projectile.Kill();
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 600);
            improvedHeal = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.damage = (int)(Projectile.damage * 0.88f); // 12% damage nerf for every enemy hit
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
    }
}
