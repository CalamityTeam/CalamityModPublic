using System;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class IonBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public ref float time => ref Projectile.ai[0];
        public bool chargeShot => Projectile.ai[2] == 5;
        public bool onSpawn = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 7;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 280;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (chargeShot)
            {
                if (onSpawn)
                {
                    Projectile.penetrate = -1;
                    Projectile.extraUpdates = 6;
                    Projectile.timeLeft = 190;
                    Projectile.tileCollide = false;
                    onSpawn = false;
                }
                Projectile.velocity *= 0.975f;

                if (Projectile.timeLeft > 45 && time > 3)
                {
                    Particle trail = new SparkParticle(Projectile.Center, -Projectile.velocity * 0.01f, false, 15, 1f, Color.Crimson * 0.6f);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }
            else
            {
                if (Projectile.ai[1] != 0 && time > 18)
                    Projectile.velocity = Projectile.velocity.RotatedBy(0.003f * -Projectile.ai[1]);

                if (Projectile.velocity.Length() < 17)
                    Projectile.velocity *= 1.005f;

                Vector2 vel = -Projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(0.5f, 4.5f);
                
                if (time % 3 == 0)
                {
                    Particle trail = new CustomSpark(Projectile.Center + Main.rand.NextVector2Circular(6, 6), vel, "CalamityMod/Particles/GlowSquareParticle", false, Main.rand.Next(7, 12), Main.rand.NextFloat(0.5f, 0.8f), Main.rand.NextBool() ? Color.Lerp(Color.Crimson, Color.White, 0.5f) : Color.Crimson, new Vector2(1f, 1f), extraRotation: MathHelper.PiOver4);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }

            Lighting.AddLight(Projectile.Center, Color.Crimson.ToVector3() * 0.5f);
            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.25f;
            int hitsToMinMult = 8;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override void OnKill(int timeLeft)
        {
            if (!chargeShot)
            {
                SoundEngine.PlaySound(SoundID.Item92 with { Volume = 0.4f }, Projectile.Center);

                Particle blastfx = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Crimson, "CalamityMod/Particles/BloomRingThinLarge", Vector2.One, 0, 0.02f, 0.053f, 17);
                GeneralParticleHandler.SpawnParticle(blastfx);
                Particle blastfx2 = new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(Color.Crimson, Color.White, 0.5f), "CalamityMod/Particles/BloomRingThinLarge", Vector2.One, 0, 0.02f, 0.06f, 15);
                GeneralParticleHandler.SpawnParticle(blastfx2);

                for (int i = 0; i < 15; i++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), Vector2.One.RotatedByRandom(Math.PI) * Main.rand.NextFloat(3, 7));
                    dust2.scale = Main.rand.NextFloat(0.85f, 1f);
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool() ? Color.Lerp(Color.Crimson, Color.White, 0.5f) : Color.Crimson;
                }

                // Create Blast
                float blastSize = 60;
                float minMultiplier = 0.3f;
                int hitsToMinMult = 7;
                Projectile blast = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BasicBurst>(), (int)(Projectile.damage * 0.5f), 0, Projectile.owner, blastSize, minMultiplier, hitsToMinMult);
                blast.timeLeft = 10;
                blast.DamageType = DamageClass.Magic;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player Owner = Main.player[Projectile.owner];
            if (time <= 1)
            {
                float _ = float.NaN;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Owner.Center, Projectile.width, ref _);
            }
            else
                return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowSpark").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation;
            float timeleftFade = Utils.GetLerpValue(0, 60, Projectile.timeLeft, true);
            if (chargeShot)
            {
                for (int i = 0; i < 5; i++)
                {
                    float iMult = (1 - 0.1f * i);
                    Main.EntitySpriteDraw(tex2, drawPosition, null, Color.Lerp(Color.Crimson, Color.White, i * 0.1f) with { A = 0 } * iMult * timeleftFade, drawRotation, tex2.Size() * 0.5f, new Vector2(0.2f + 2 * iMult, (1.2f - 1f * iMult) * timeleftFade) * iMult * 0.03f, SpriteEffects.None);
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                    Main.EntitySpriteDraw(tex, drawPosition, null, Color.Lerp(Color.Crimson, Color.White, i * 0.1f) with { A = 0 } * (1 - 0.1f * i), drawRotation, tex.Size() * 0.5f, (1 - 0.1f * i), SpriteEffects.None);
            }
            return false;
        }
    }
}
