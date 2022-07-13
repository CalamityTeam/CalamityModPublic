using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Ranged
{
    public class TitaniumRailgunShot : BaseLaserbeamProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/YharimsCrystalBeam";
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd", AssetRequestMode.ImmediateLoad).Value;
        public override float MaxScale => 1.5f * ChargePercent;
        public override float Lifetime => 15f;
        public override float MaxLaserLength => 2200f;
        public ref float ChargePercent => ref Projectile.ai[1];
        public override Color LaserOverlayColor => Color.White;
        public override Color LightCastColor => LaserOverlayColor;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titanium Decimator");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.scale = MaxScale;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 15;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            base.AI();
        }

        public override void ExtraBehavior()
        {
            if (Projectile.timeLeft == 15)
            {
                
                Vector2 beamVector = Projectile.velocity;
                float beamLenght = DetermineLaserLength_CollideWithTiles(12);

                //Rapid dust
                int dustCount = Main.rand.Next(10, 30);
                for (int i = 0; i < dustCount; i++)
                {
                    float dustProgressAlongBeam = beamLenght * Main.rand.NextFloat(0f, 0.8f);
                    Vector2 dustPosition = Projectile.Center + dustProgressAlongBeam * beamVector + beamVector.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-6f, 6f) * Projectile.scale;

                    Dust dust = Dust.NewDustPerfect(dustPosition, 187, beamVector * Main.rand.NextFloat(5f, 26f), 0, Color.White, 2.2f);
                    dust.noGravity = true;
                }

                //Put a titanium shell into the impact tile (if it collided with one)
                if (beamLenght < MaxLaserLength)
                {
                    Vector2 endPoint = beamLenght * beamVector + Projectile.Center + beamVector * 8.5f;
                    Point anchorPos = new Point((int)endPoint.X / 16, (int)endPoint.Y / 16);

                    Color burnColor = Main.rand.NextBool(4) ? Color.PaleGreen : Main.rand.NextBool(4) ? Color.PaleTurquoise : Color.OrangeRed;
                    Particle shell = new TitaniumRailgunShell(endPoint, anchorPos, Projectile.rotation + MathHelper.PiOver2, burnColor);
                    GeneralParticleHandler.SpawnParticle(shell);
                }
            }
        }

        public override void DetermineScale() => Projectile.scale = Projectile.timeLeft / Lifetime * MaxScale;

        public override float DetermineLaserLength() => DetermineLaserLength_CollideWithTiles(5);

        public override bool ShouldUpdatePosition() => false;

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * LaserLength, Projectile.width + 16, DelegateMethods.CutTiles);
        }
    }
}
