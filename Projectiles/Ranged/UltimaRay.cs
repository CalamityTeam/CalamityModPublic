using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class UltimaRay : BaseLaserbeamProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float HueOffset
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        public override float MaxScale => 0.7f;
        public override float MaxLaserLength => 2400f;
        public override float Lifetime => 50f;
        public override Color LaserOverlayColor => Main.hslToRgb((float)Math.Sin(Main.GlobalTime * 2.3f + HueOffset) * 0.5f + 0.5f, 1f, 0.775f) * Utils.InverseLerp(Lifetime, 0f, Time, true);
        public override Color LightCastColor => LaserOverlayColor;
        public override Texture2D LaserBeginTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayStart");
        public override Texture2D LaserMiddleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayMid");
        public override Texture2D LaserEndTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultima Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 22;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.localNPCHitCooldown = 10;
            projectile.usesLocalNPCImmunity = true;
        }

        public override bool PreAI()
        {
            // Initialization. Using the AI hook would override the base laser's code, and we don't want that.
            if (projectile.localAI[0] == 0f)
            {
                HueOffset = Main.rand.NextFloat(MathHelper.Pi / 5f);
                projectile.localAI[0] = 1f;
            }
            return true;
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
