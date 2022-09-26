using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override float MaxScale => 0.7f;
        public override float MaxLaserLength => 2400f;
        public override float Lifetime => 50f;
        public override Color LaserOverlayColor => Main.hslToRgb((float)Math.Sin(Main.GlobalTimeWrappedHourly * 2.3f + HueOffset) * 0.5f + 0.5f, 1f, 0.775f) * Utils.GetLerpValue(Lifetime, 0f, Time, true);
        public override Color LightCastColor => LaserOverlayColor;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd", AssetRequestMode.ImmediateLoad).Value;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultima Ray");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.localNPCHitCooldown = 10;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool PreAI()
        {
            // Initialization. Using the AI hook would override the base laser's code, and we don't want that.
            if (Projectile.localAI[0] == 0f)
            {
                HueOffset = Main.rand.NextFloat(MathHelper.Pi / 5f);
                Projectile.localAI[0] = 1f;
            }
            return true;
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
