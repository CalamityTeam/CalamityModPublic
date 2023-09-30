using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class UltimaBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public float Bounces
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float DefaultHue => ((float)Math.Sin(Main.GlobalTimeWrappedHourly * 2.5f) + 1) * 0.5f;
        public const int MaxBounces = 1;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public Color GetColorFromHue(float hue) => Main.hslToRgb(hue, 1f, 0.725f) * Projectile.Opacity;
        public override Color? GetAlpha(Color lightColor) => GetColorFromHue(DefaultHue);

        public void ProduceExplosionDust(int dustCount)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < dustCount; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 267);
                    dust.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.8f);
                    dust.scale = Main.rand.NextFloat(1f, 1.25f);
                    dust.velocity = Main.rand.NextVector2Circular(7f, 7f);
                    dust.noGravity = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Bounces >= MaxBounces)
            {
                Projectile.Kill();
                return false;
            }

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            Bounces++;
            ProduceExplosionDust(24);

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (i > 0)
                {
                    CalamityUtils.DistanceClamp(ref Projectile.oldPos[i], ref Projectile.oldPos[i - 1], 8f); // Ensure the max distance between the old positions isn't too great. It looks weird if it is.
                }
                float completionRatio = i / (float)Projectile.oldPos.Length;
                Color color = GetColorFromHue((DefaultHue + completionRatio * 0.5f) % 1f) * (float)Math.Pow(1f - completionRatio, 2);
                Main.EntitySpriteDraw(texture,
                                 Projectile.oldPos[i] + texture.Size() * 0.5f - Main.screenPosition,
                                 null,
                                 color,
                                 Projectile.rotation,
                                 texture.Size() * 0.5f,
                                 Projectile.scale,
                                 SpriteEffects.None,
                                 0);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(90, 90);
            Projectile.Damage();
            ProduceExplosionDust(32);
        }
    }
}
