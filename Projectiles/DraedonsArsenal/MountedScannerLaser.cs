using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class MountedScannerLaser : BaseLaserbeamProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int OwnerIndex
        {
            get => Projectile.GetByUUID(projectile.owner, projectile.ai[1]);
            set => projectile.ai[1] = value;
        }
        public override float MaxScale => 0.5f + (float)Math.Cos(Main.GlobalTime * 10f) * 0.07f;
        public override float MaxLaserLength => 900f;
        public override float Lifetime => 70f;
        public override Color LaserOverlayColor => Color.Red;
        public override Color LightCastColor => LaserOverlayColor;
        public override Texture2D LaserBeginTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayStart");
        public override Texture2D LaserMiddleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayMid");
        public override Texture2D LaserEndTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd");
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deathray");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 17;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.localNPCHitCooldown = 15;
            projectile.usesLocalNPCImmunity = true;
        }
        public override float DetermineLaserLength()
        {
            float[] samples = new float[4];
            Collision.LaserScan(projectile.Center, projectile.velocity, projectile.width * projectile.scale, MaxLaserLength, samples);
            return samples.Average();
        }
        public override void UpdateLaserMotion()
        {
            if (OwnerIndex == -1)
            {
                projectile.Kill();
                return;
            }
            projectile.velocity = Vector2.Lerp(projectile.velocity, Main.projectile[OwnerIndex].rotation.ToRotationVector2().RotatedBy(Math.Cos(Time / 25) * 0.05f), 0.125f);
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }
        public override void AttachToSomething()
        {
            if (OwnerIndex == -1)
            {
                projectile.Kill();
                return;
            }
            projectile.Center = Main.projectile[OwnerIndex].Center + Main.projectile[OwnerIndex].rotation.ToRotationVector2() * 18f;

            // Kill the projectile if the owner isn't targeting anything anymore.
            if (Main.projectile[OwnerIndex].localAI[0] == 0f)
                projectile.Kill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override void ExtraBehavior()
        {
            // Spawn dust at the end of the laser.
            if (!Main.dedServ)
            {
                Vector2 laserSpawnPosition = projectile.Center + projectile.velocity * (LaserLength - 14f);
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(laserSpawnPosition + Main.rand.NextVector2Circular(8f, 8f), 261);
                    dust.velocity = Main.rand.NextVector2CircularEdge(9f, 9f) * Main.rand.NextFloat(0.3f, 1f);
                    dust.color = Color.Lerp(Color.Crimson, Color.Red, Main.rand.NextFloat());
                    dust.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			projectile.damage = (int)(projectile.damage * 0.6);
		}

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			projectile.damage = (int)(projectile.damage * 0.6);
		}
    }
}
