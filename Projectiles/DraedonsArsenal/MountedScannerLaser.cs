using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class MountedScannerLaser : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int OwnerIndex
        {
            get => Projectile.GetByUUID(Projectile.owner, Projectile.ai[1]);
            set => Projectile.ai[1] = value;
        }
        public override float MaxScale => 0.5f + (float)Math.Cos(Main.GlobalTimeWrappedHourly * 10f) * 0.07f;
        public override float MaxLaserLength => 900f;
        public override float Lifetime => 70f;
        public override Color LaserOverlayColor => Color.Red * 0.6f;
        public override Color LightCastColor => LaserOverlayColor;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd", AssetRequestMode.ImmediateLoad).Value;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 17;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.localNPCHitCooldown = 15;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override float DetermineLaserLength()
        {
            float[] samples = new float[4];
            Collision.LaserScan(Projectile.Center, Projectile.velocity, Projectile.width * Projectile.scale, MaxLaserLength, samples);
            return samples.Average();
        }

        public override void UpdateLaserMotion()
        {
            if (OwnerIndex == -1)
            {
                Projectile.Kill();
                return;
            }
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Main.projectile[OwnerIndex].rotation.ToRotationVector2().RotatedBy(Math.Cos(Time / 25) * 0.05f), 0.125f);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void AttachToSomething()
        {
            if (OwnerIndex == -1)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = Main.projectile[OwnerIndex].Center + Main.projectile[OwnerIndex].rotation.ToRotationVector2() * 18f;

            // Kill the projectile if the owner isn't targeting anything anymore.
            if (Main.projectile[OwnerIndex].localAI[0] == 0f)
                Projectile.Kill();
        }
        
        public override bool ShouldUpdatePosition() => false;

        public override void ExtraBehavior()
        {
            // Spawn dust at the end of the laser.
            if (!Main.dedServ)
            {
                Vector2 laserSpawnPosition = Projectile.Center + Projectile.velocity * (LaserLength - 14f);
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(laserSpawnPosition + Main.rand.NextVector2Circular(8f, 8f), 261);
                    dust.velocity = Main.rand.NextVector2CircularEdge(9f, 9f) * Main.rand.NextFloat(0.3f, 1f);
                    dust.color = Color.Lerp(Color.Crimson, Color.Red, Main.rand.NextFloat());
                    dust.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.6);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.damage = (int)(Projectile.damage * 0.6);
        }
    }
}
