using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class FleshTotemSoul : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/Rogue/PhantasmalSoulBlue";
        public ref float Time => ref Projectile.ai[0];
        public float dustRotation = 0;
        public Player Owner => Main.player[Projectile.owner];
        public bool visuals => Owner.Calamity().fleshTotemVisual; // Enables/disables visuals and sounds based on accessory visibility

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            Time++;
            // Handle animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;
            if (Time >= 45f)
            {
                CalamityUtils.HomeInOnNPC(Projectile, true, 1200f, 30f, 40f);
            }
            //Welcome back to Sunny tries to smash 5 different visual effects together in the effort to get something to look good 
            float squash = Utils.GetLerpValue(1, 3, Projectile.velocity.Length(), true);
            if (squash > 0.15f)
            {
                Particle fadeInfx = new CustomSpark(Projectile.Center, Projectile.velocity * 0.01f, "CalamityMod/Particles/BloomCircle", false, 15, 0.5f * Projectile.scale, Color.SkyBlue * 0.4f * squash, new Vector2(1 - 0.15f * squash, 1f), true, false, shrinkSpeed: 0.1f * squash);
                GeneralParticleHandler.SpawnParticle(fadeInfx);
            }
            Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * Main.rand.NextFloat(-0.2f, -0.6f), Color.SkyBlue, 3, Main.rand.NextFloat(0.25f, 0.3f), 1f, Main.rand.NextFloat(-0.2f, 0.2f), false);
            GeneralParticleHandler.SpawnParticle(smoke);
            if (Time > 5 && visuals)
            {
                for (int i = 0; i < 3; i++)
                {
                    dustRotation += 0.12f;
                    Vector2 dustPos = Projectile.Center + (MathHelper.Pi + dustRotation + MathHelper.PiOver2).ToRotationVector2() * 10f * Projectile.scale;
                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.SpectreStaff, (MathHelper.Pi + dustRotation * Math.Sign(Projectile.velocity.Length())).ToRotationVector2() * 2);
                    dust.noGravity = false;
                    dust.scale = Main.rand.NextFloat(0.75f, 1.2f);
                    dust.alpha = Main.rand.Next(100, 170 + 1);
                    dust.velocity = dust.velocity.RotatedByRandom(0f);
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + -MathHelper.PiOver2;
            // Blue light
            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.7f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = Texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPosition;
            Vector2 origin = frame.Size() * 0.5f;

            drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(Texture, drawPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage() => Time >= 15f;

        public override void OnKill(int timeLeft)
        {
            if (visuals)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 sparkVel = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(MathHelper.ToRadians(24f)) * Main.rand.NextFloat(6f, 10f);
                    Color color = Main.hslToRgb(Main.rand.NextFloat(0.3f, 0.5f), 1f, 0.8f);
                    SparkParticle spark = new(Projectile.Center, sparkVel, false, 30, 1.3f, color);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }
    }
}
