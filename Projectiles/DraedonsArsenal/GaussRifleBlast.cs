using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class GaussRifleBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gauss Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 40;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Time++;
            projectile.tileCollide = Time > 3f;
            if (!Main.dedServ)
            {
                // Idle dust.
                for (int i = 0; i < 10; i++)
                {
                    float angle = i / 10f * MathHelper.TwoPi;
                    for (int j = 0; j < 4; j++)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center, 263);
                        dust.velocity = angle.ToRotationVector2().RotatedByRandom(0.25f) * Main.rand.NextFloat(6f, 8f);
                        dust.noGravity = true;
                        dust.scale = 1.6f;
                    }
                }
                // Ring dust.
                for (int i = 0; i < 4; i++)
                {
                    float angle = projectile.velocity.ToRotation() + (i / 4f * MathHelper.TwoPi) + Time / 24f;
                    float radius = (float)Math.Sin(Time / 7.5f) * 40f + 10f;
                    Dust dust = Dust.NewDustPerfect(projectile.Center + angle.ToRotationVector2() * radius, 226);
                    dust.velocity = Vector2.Zero;
                    dust.noGravity = true;
                    dust.scale = 1.5f;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == projectile.owner)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeMechGaussRifle"), projectile.Center);
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<GaussRifleExplosion>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
        }
    }
}
