using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HellbornProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            //Animation
            if (Projectile.frame == 0 || Projectile.frame == 3)
                Projectile.frameCounter++;
            else
                Projectile.frameCounter += 2;
            if (Projectile.frameCounter > 20)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 5)
            {
                Projectile.frame = 0;
            }
            if (Projectile.velocity.Length() < 18)
                Projectile.velocity *= 1.035f;

            //Rotation
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());

            if (targetDist < 1400f)
            {
                // Spawn in a helix-style pattern
                float sine = (float)Math.Sin(Projectile.timeLeft * 0.575f / MathHelper.Pi);

                Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 16f;
                if (Main.rand.NextBool(2))
                {
                    //Particle orb = new GlowOrbParticle(Projectile.Center + offset, -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.8f), false, Main.rand.Next(20, 28 + 1), Main.rand.NextFloat(0.6f, 1.3f), Main.rand.NextBool() ? Color.Orange : Color.OrangeRed);
                    //GeneralParticleHandler.SpawnParticle(orb);

                    Dust dust5 = Dust.NewDustPerfect(Projectile.Center + offset, ModContent.DustType<SquashDust>(), -Projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(3f, 5f));
                    dust5.noGravity = true;
                    dust5.scale = Main.rand.NextFloat(2.3f, 2.7f);
                    dust5.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f));
                    dust5.fadeIn = 2.5f;
                }
                if (Main.rand.NextBool(2))
                {
                    //Particle orb2 = new GlowOrbParticle(Projectile.Center - offset, -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.8f), false, Main.rand.Next(20, 28 + 1), Main.rand.NextFloat(0.6f, 1.3f), Main.rand.NextBool() ? Color.Orange : Color.OrangeRed);
                    //GeneralParticleHandler.SpawnParticle(orb2);

                    Dust dust5 = Dust.NewDustPerfect(Projectile.Center - offset, ModContent.DustType<SquashDust>(), -Projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(3f, 5f));
                    dust5.noGravity = true;
                    dust5.scale = Main.rand.NextFloat(2.3f, 2.7f);
                    dust5.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f));
                    dust5.fadeIn = 2.5f;
                }
            }
            Vector2 dustPos = Projectile.Center + Main.rand.NextVector2Circular(30, 30);
            Dust dust = Dust.NewDustPerfect(dustPos, ModContent.DustType<DiamondDust>(), -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.4f));
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.62f, 0.82f);
            dust.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f));
            dust.fadeIn = 1;
            dust.noLight = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Owner = Main.player[Projectile.owner];

            // Launch
            Vector2 launchVel = Utils.DirectionTo(Projectile.Center, target.Center);
            target.MoveNPC(launchVel, 6, true, Owner);

            MakeBlast(target.whoAmI, true);
        }
        public override void OnKill(int timeLeft)
        {
            for (int l = 0; l < 17; l++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, new Vector2(8, 8).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 0.8f));
                dust.noGravity = false;
                dust.scale = Main.rand.NextFloat(0.62f, 0.82f);
                dust.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f));

                Dust dust5 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SquashDust>(), new Vector2(10, 10).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 0.8f));
                dust5.noGravity = true;
                dust5.scale = Main.rand.NextFloat(2.1f, 2.4f);
                dust5.color = Color.Lerp(Color.Orange, Color.Red, Main.rand.NextFloat(0f, 1f));
                dust5.fadeIn = 1.7f;
            }

            Particle blastRing = new CustomPulse(Projectile.Center, Vector2.Zero, Color.OrangeRed * 0.7f, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 1.2f, 1.7f, 16, true);
            GeneralParticleHandler.SpawnParticle(blastRing);
            Particle blastRing2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White * 0.7f, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(-10, 10), 0.7f, 0.9f, 16, true);
            GeneralParticleHandler.SpawnParticle(blastRing2);

            SoundStyle bigShot = new("CalamityMod/Sounds/Item/HellbornImpact");
            SoundEngine.PlaySound(bigShot with { PitchVariance = 0.15f, Volume = 0.8f }, Projectile.Center);
            if (Projectile.numHits == 0)
            {
                MakeBlast(0, false);
            }
        }
        public void MakeBlast(int target, bool hitTarget)
        {
            float blastSize = 70;
            float minMultiplier = 0.25f;
            int hitsToMinMult = 5;
            int blastDamage = (int)(Projectile.damage * 0.33f);
            if (hitTarget)
            {
                Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurstExclusive>(), blastDamage, Projectile.knockBack, Projectile.owner, blastSize, minMultiplier, hitsToMinMult);
                blast.timeLeft = 2;
                blast.DamageType = DamageClass.Ranged;
                blast.localAI[0] = target;
            }
            else
            {
                Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), blastDamage, Projectile.knockBack, Projectile.owner, blastSize, minMultiplier, hitsToMinMult);
                blast.timeLeft = 2;
                blast.DamageType = DamageClass.Ranged;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/HellbornProj").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;

            Rectangle frame = texture.Frame(1, 6, 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            Texture2D rechargeTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            // Glow Orb
            float randSize = Main.rand.NextFloat(0.8f, 1.2f);
            Main.EntitySpriteDraw(rechargeTexture, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 }, Projectile.rotation, rechargeTexture.Size() * 0.5f, 0.5f * randSize, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(rechargeTexture, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.75f, Projectile.rotation, rechargeTexture.Size() * 0.5f, 0.35f * randSize, SpriteEffects.None, 0);

            for (int i = 0; i < 1; i++)
                Main.EntitySpriteDraw(texture, drawPosition, frame, Color.White, 0, origin, Projectile.scale * (i == 0 ? 1.15f : 1), SpriteEffects.None, 0);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 35, targetHitbox);
    }
}
