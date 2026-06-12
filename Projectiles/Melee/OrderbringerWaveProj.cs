using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class OrderbringerWaveProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/JudgementProj";
        public ref float time => ref Projectile.ai[0];
        public float hitboxSize = 10;
        public Color mainColor;
        public float fade = 1;
        public float fadeOut = 1;
        public override void SetDefaults()
        {
            Projectile.width = 336;
            Projectile.height = 274;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 450;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 22;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Vector2 topCorner = Projectile.Center + (Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(125f)) * Projectile.scale) * 190 * Projectile.ai[1];
            Vector2 bottomCorner = Projectile.Center + (Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-125f)) * Projectile.scale) * 190 * Projectile.ai[1];

            Player player = Main.player[Projectile.owner];
            mainColor = player.Calamity().lightRGB;

            if (time == 0)
            {
                Projectile.scale = 0.0875f;
                Projectile.velocity *= 1.25f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (time < 200)
            {
                Projectile.scale += 0.0026f;
                hitboxSize += 0.4525f;
                Projectile.velocity *= 0.995f;
            }
            else
            {
                Projectile.velocity *= 0.975f;
            }
            if (time > 300 && fade > 0)
                fade -= 0.0065f;

            if (time < 250 && time > 20)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust trailDust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(hitboxSize * 1.25f, hitboxSize * 1.25f)* Projectile.ai[1] - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 25 * Projectile.ai[1], ModContent.DustType<SquashDust>());
                    trailDust.scale = (Main.rand.NextFloat(1.6f, 1.95f) - (time < 150 ? 0 : time * 0.001f)) * Projectile.ai[1];
                    trailDust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.85f, 1.5f);
                    trailDust.color = mainColor;
                    trailDust.noGravity = true;
                    trailDust.fadeIn = 2f * Projectile.ai[1];
                }

                if (time % 9 == 0 && time < 200)
                {
                    Particle spark = new CustomSpark(topCorner, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-170f)), "CalamityMod/Particles/BloomCircle", false, 18, 0.35f * Projectile.ai[1], mainColor * 0.75f, new Vector2(1f, 2.5f), true, true, shrinkSpeed: 0.2f, glowOpacity: 0.7f);
                    GeneralParticleHandler.SpawnParticle(spark);
                    Particle spark2 = new CustomSpark(bottomCorner, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(170f)), "CalamityMod/Particles/BloomCircle", false, 18, 0.35f * Projectile.ai[1], mainColor * 0.75f, new Vector2(1f, 2.5f), true, true, shrinkSpeed: 0.2f, glowOpacity: 0.7f);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                if (Main.rand.NextBool(12))
                {
                    Particle orb3 = new SparkParticle(topCorner, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(170f)) * Main.rand.NextFloat(5f, 20f), false, 15, Main.rand.NextFloat(0.8f, 1.35f) * Projectile.ai[1], mainColor);
                    GeneralParticleHandler.SpawnParticle(orb3);
                    Particle orb4 = new SparkParticle(bottomCorner, Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-170f)) * Main.rand.NextFloat(5f, 20f), false, 15, Main.rand.NextFloat(0.8f, 1.35f) * Projectile.ai[1], mainColor);
                    GeneralParticleHandler.SpawnParticle(orb4);
                }
            }

            time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            float waveFade = (float)Math.Pow(Utils.GetLerpValue(0, 250, Projectile.timeLeft, true), 2);
            float waveFade2 = (float)Math.Pow(Utils.GetLerpValue(0, 80, Projectile.timeLeft, true), 2);
            for (int i = 1; i < 15; i++) // Weird for loop because of squash code
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 6f * i, null, mainColor with { A = 0 } * waveFade * (1 - 0.1f * i), Projectile.rotation, tex.Size() / 2f, new Vector2(1.1f * MathHelper.Lerp(waveFade2, 1, 0.3f), 0.6f + (1 - waveFade) * 0.8f) * Projectile.scale * 1.1f * Projectile.ai[1], SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 300);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, hitboxSize * Projectile.ai[1], targetHitbox);
    }
}
