using System;
using System.Collections.Generic;
using CalamityMod.Dusts;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    [PierceResistException]
    public class LeviAmberDash : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public Color effectsColor = Color.White;
        public float fxFade = 0;
        public bool isAlive = true;
        public bool onSpawn = true;
        public Vector2 aimVel;
        public bool visuals => Owner.Calamity().lAmbergrisVisual;
        private static float ExplosionRadius = 85f;
        public static readonly SoundStyle Slap = new("CalamityMod/Sounds/Custom/WetSlap", 4) { Volume = 0.8f, PitchVariance = 0.3f };
        public override void SetDefaults()
        {
            Projectile.width = (int)ExplosionRadius;
            Projectile.height = (int)ExplosionRadius;
            Projectile.friendly = true;
            Projectile.DamageType = AverageDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
        }
        public override void AI()
        {
            if (onSpawn && visuals)
            {
                Particle pulse = new CustomSpark(Owner.Center, Owner.velocity.SafeNormalize(Vector2.UnitX) * 9, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 23, 0.095f, Color.MediumTurquoise, new Vector2(1f, 1.7f), shrinkSpeed: -0.2f);
                GeneralParticleHandler.SpawnParticle(pulse);
                Particle pulse2 = new CustomSpark(Owner.Center, Owner.velocity.SafeNormalize(Vector2.UnitX) * 12, "CalamityMod/Particles/HighResHollowCircleHardEdgeAlt", false, 20, 0.065f, Color.DeepSkyBlue, new Vector2(1f, 1.7f), shrinkSpeed: -0.23f);
                GeneralParticleHandler.SpawnParticle(pulse2);
                onSpawn = false;
            }

            if (Owner.dashDelay != -1)
                isAlive = false;
            if (isAlive)
                Projectile.timeLeft++;
            Projectile.Center = Owner.Center;

            if (effectsColor == Color.White)
                aimVel = Owner.velocity;
            else
            {
                aimVel = Vector2.Lerp(aimVel, Owner.velocity, 0.2f);
            }

            if (isAlive)
            {
                float goalSize = Utils.GetLerpValue(5, 15, Math.Abs(aimVel.X), true);
                if (goalSize < fxFade)
                    fxFade = MathHelper.Lerp(fxFade, goalSize, 0.03f);
                else
                    fxFade = goalSize;
            }
            else
            {
                fxFade = MathHelper.Lerp(fxFade, 0, 0.13f);
            }


            float rate = Main.GlobalTimeWrappedHourly * 22;
            List<Color> eColors = new List<Color>()
            {
                Color.DarkTurquoise,
                Color.DeepSkyBlue,
                Color.MediumTurquoise
            };

            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            effectsColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

            if (visuals)
            {
                int dir = MathF.Sign(aimVel.X);
                Vector2 safeVel = aimVel.SafeNormalize(Vector2.UnitX);
                float sparkscale2 = 0.35f * Math.Max(fxFade, 0.5f);
                for (int i = -1; i <= 1; i += 2)
                {
                    Vector2 sparkVelocity = safeVel.RotatedBy(dir * 1.5f * i) - safeVel * 3;
                    Vector2 sparkPlace = Owner.Center + (((safeVel * 15).RotatedBy(2f * dir * i) * 1.5f) + safeVel.RotatedBy(0.5f * i * dir) * 30) * fxFade;
                    Particle spark = new CustomSpark(sparkPlace, sparkVelocity.RotatedByRandom(0.1f), "CalamityMod/Particles/BloomCircle", false, 6, sparkscale2, effectsColor * fxFade, new Vector2(0.8f, 2f), shrinkSpeed: 1.1f);
                    GeneralParticleHandler.SpawnParticle(spark);

                    if (isAlive)
                    {
                        int dustStyle = ModContent.DustType<SquashDustHollow>();
                        Dust dust2 = Dust.NewDustPerfect(sparkPlace, dustStyle, sparkVelocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(3, 5));
                        dust2.scale = Main.rand.NextFloat(0.9f, 1.3f);
                        dust2.color = effectsColor;
                        dust2.noGravity = true;
                        dust2.fadeIn = Main.rand.NextFloat(0f, 0.7f);
                    }
                }
            }

            if (Owner.dead || (!isAlive && fxFade < 0.1f) || Owner.velocity == Vector2.Zero)
                Projectile.Kill();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundStyle sound = new("CalamityMod/Sounds/Item/WaterSplash", 2);
            SoundEngine.PlaySound(sound with { Volume = 0.2f, Pitch = Main.rand.NextFloat(0.35f, 0.5f), MaxInstances = -1 }, Projectile.Center);

            target.AddBuff(BuffID.Wet, 300);
            target.AddBuff(ModContent.BuffType<Buffs.DamageOverTime.RiptideDebuff>(), 180);

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
            target.MoveNPC(launchVel, 20, true, Owner);

            for (int i = 0; i <= 17; i++)
            {
                float variance = Main.rand.NextFloat(-0.4f, 0.4f);
                int dustStyle = ModContent.DustType<SquashDust>();
                Dust dust2 = Dust.NewDustPerfect(target.Center, dustStyle);
                dust2.scale = (Main.rand.NextFloat(1.2f, 1.4f) - Math.Abs(variance)) * 1.5f;
                dust2.velocity = (aimVel.SafeNormalize(Vector2.UnitX).RotatedBy(variance) - Vector2.UnitY * 0.3f) * Main.rand.NextFloat(15, 35f) * (1 - Math.Abs(variance) * 1.3f);
                dust2.color = Main.rand.NextBool() ? Color.MediumTurquoise : Color.DeepSkyBlue;
                dust2.noGravity = false;
                dust2.fadeIn = Main.rand.NextFloat(0f, 1.2f);
            }
            Projectile.damage = (int)(Projectile.damage * 0.67f);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, ExplosionRadius, targetHitbox);
        public override bool? CanDamage() => (isAlive ? null : false);
        public override bool PreDraw(ref Color lightColor)
        {
            if (effectsColor == Color.White || !visuals)
                return false;

            float sine = MathHelper.Lerp(Math.Abs((float)Math.Sin(Main.GlobalTimeWrappedHourly * 50f / MathHelper.Pi)), 0.8f, 0.7f);
            Texture2D bTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/VerticalSmearRagged").Value;

            for (int i = 0; i < 10; i++)
            {
                float bScale2 = 0.75f;
                Vector2 scale = new Vector2((1 - i * 0.13f) * sine, (1 + i * 0.012f) + fxFade * 0.2f) * (bScale2 + i * 0.08f) * fxFade * 0.27f;
                Main.EntitySpriteDraw(bTexture, Owner.Center - Main.screenPosition + aimVel.SafeNormalize(Vector2.UnitX) * -15, null, Color.Lerp(Color.Aquamarine, effectsColor, i * 0.15f) with { A = 0 } * fxFade * 1f, aimVel.ToRotation() + MathHelper.PiOver2, bTexture.Size() * 0.5f, scale, i % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            return false;
        }
        public override bool? CanCutTiles() => false;
    }
}
