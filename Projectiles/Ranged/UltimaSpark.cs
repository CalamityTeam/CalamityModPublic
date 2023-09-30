using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class UltimaSpark : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public const int DustType = 261;
        public const float MaxHomingDistance = 1200f;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (!Main.dedServ && Time > 5f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPosition = Vector2.Lerp(Projectile.oldPosition, Projectile.position, i / 3f);
                    Dust dust = Dust.NewDustPerfect(spawnPosition, DustType);
                    dust.color = Main.hslToRgb((Main.rand.NextFloat(-0.04f, 0.04f) + Time / 80f) % 1f, 0.8f, 0.6f);
                    dust.scale = 2.3f;
                    dust.fadeIn = 1f;
                    dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = true;
                }
            }

            NPC potentialTarget = Projectile.Center.ClosestNPCAt(MaxHomingDistance);
            if (potentialTarget != null)
            {
                Projectile.velocity = (Projectile.velocity * 8f + Projectile.SafeDirectionTo(potentialTarget.Center) * 18f) / 9f;
                return;
            }

            if (Time > 30f)
            {
                float updatedTime = Time - 30f;
                // Make a complete 90 degree turn in 30 frames.
                if (updatedTime % 120f > 90f)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2 / 20f);
                }
                // Arc around quickly for 60 frames.
                else if (updatedTime % 120f > 30f)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy((float)Math.Sin((updatedTime - 30f) % 60f / 60f * MathHelper.TwoPi) * MathHelper.ToRadians(15f));
                }
            }

            Time++;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(60, 60);
            Projectile.Damage();
            if (!Main.dedServ)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType);
                    dust.color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f);
                    dust.scale = Main.rand.NextFloat(0.9f, 1.25f);
                    dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
                    dust.noGravity = true;
                }
            }
        }
    }
}
