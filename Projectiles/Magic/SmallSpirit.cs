using CalamityMod.Graphics.Metaballs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SmallSpirit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public Player Owner => Main.player[Projectile.owner];
        public Projectile ProjectileOwner
        {
            get
            {
                int spiritType = ModContent.ProjectileType<SpiritCongregation>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type != spiritType || Main.projectile[i].identity != Projectile.ai[0] ||
                        Main.projectile[i].owner != Projectile.owner)
                    {
                        continue;
                    }

                    return Main.projectile[i];
                }
                return null;
            }
        }

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            float radius = MathHelper.SmoothStep(67f, 32f, (float)Math.Sqrt(1f - Projectile.timeLeft / 360f));

            // Handle frames.
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Projectile.type];

            float maxOpacity = 1f;

            // Fly around towards a target.
            Entity target = Owner;
            float flySpeed = 8f;
            float flyInertia = 54f;

            Projectile.hostile = true;
            Projectile.friendly = false;
            if (ProjectileOwner != null && (ProjectileOwner.ModProjectile<SpiritCongregation>().CurrentPower > 0.97f || Projectile.timeLeft < 95))
            {
                Projectile.hostile = false;

                radius += 36f;

                target = ProjectileOwner;
                flySpeed = 29f;
                flyInertia = 5f;

                // Die if close to the owner projectile, effectively being absorbed.
                Projectile.Center = Projectile.Center.MoveTowards(target.Center, 8.5f);
                if (Projectile.WithinRange(target.Center, 80f))
                    Projectile.Kill();

                // Become translucent when returning to the projectile owner.
                maxOpacity = 0.4f;
            }

            // Die if no valid target exists or the projectile owner is absent.
            if (target is null || ProjectileOwner is null || !ProjectileOwner.active)
            {
                Projectile.Kill();
                return;
            }

            // If not close to the target, redirect towards them.
            if (!Projectile.WithinRange(target.Center, 260f))
            {
                Vector2 idealVelocity = Projectile.SafeDirectionTo(target.Center) * flySpeed;
                Projectile.velocity = (Projectile.velocity * (flyInertia - 1f) + idealVelocity) / flyInertia;
            }

            // Otherwise accelerate.
            else
                Projectile.velocity *= 1.01f;

            // Rapidly fade in.
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.075f, 0f, maxOpacity);

            // Decide rotation.
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            // Emit particles.
            Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(5f, 5f) * radius / 130f;
            GruesomeMetaball.SpawnParticle(spawnPosition, Main.rand.NextVector2Circular(6f, 6f), radius);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust polterplasm = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12f, 12f), 261);
                polterplasm.color = Color.Lerp(Color.LightPink, Color.Red, Main.rand.NextFloat(0.67f));
                polterplasm.scale = 1.2f;
                polterplasm.fadeIn = 0.55f;
                polterplasm.noGravity = true;
            }
        }
    }
}
