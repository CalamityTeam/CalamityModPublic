using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class NebulaElementalBeam : BaseLaserbeamProjectile
    {
        public override float MaxScale => 1.4f;
        public override float MaxLaserLength => 1000f;
        public override float Lifetime => 30f;
        public override Color LightCastColor => Color.White;
        public override Texture2D LaserBeginTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayStart");
        public override Texture2D LaserMiddleTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayMid");
        public override Texture2D LaserEndTexture => ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd");

        public const float UniversalAngularSpeed = MathHelper.Pi / 400f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 20;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 13;
            projectile.tileCollide = false;
            projectile.timeLeft = (int)Lifetime;
        }

        public override void ExtraBehavior()
        {
            RotationalSpeed = UniversalAngularSpeed;
            // Generate a burst of bubble-like nebula dust.
            if (!Main.dedServ && Time == 5f)
            {
                int totalBubbles = 24;
                for (int i = 0; i < totalBubbles; i++)
                {
                    Dust nebulaBubble = Dust.NewDustPerfect(projectile.Center, 242);
                    nebulaBubble.velocity = Main.rand.NextVector2Circular(6f, 6f);
                    nebulaBubble.scale = Main.rand.NextFloat(2f, 3f);
                    nebulaBubble.noGravity = true;
                }
            }
        }

        public override void DetermineScale() => projectile.scale = projectile.timeLeft / Lifetime * MaxScale;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawBeamWithColor(spriteBatch, Color.Lerp(Color.Lerp(Color.Purple, Color.White, 0.26f) * 1.1f, Color.Transparent, 0.25f), projectile.scale);
            DrawBeamWithColor(spriteBatch, Color.Lerp(Color.Fuchsia * 1.1f, Color.Transparent, 0.25f), projectile.scale * 0.5f);
            return false;
        }
    }
}
