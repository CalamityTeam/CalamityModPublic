using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class LiliesOfEternityBullet : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            if (Main.dedServ)
                return;

            Dust trailDust = Dust.NewDustPerfect(
                Projectile.Center + Main.rand.NextVector2Circular(8f, 8f),
                64,
                Projectile.velocity * Main.rand.NextFloat(0.01f, 0.05f),
                Scale: Main.rand.NextFloat(1f, 1.2f));
            trailDust.noGravity = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 10;
        }

        private float PrimitiveWidthFunction(float completionRatio)
        {
            float trailPoint = 0.1f;
            return completionRatio > trailPoint ? Utils.Remap(completionRatio, trailPoint, 1f, 12f, 0f) : Utils.Remap(completionRatio, trailPoint, 0f, 12f, 0f);
        }

        private Color PrimitiveColorFunction(float completionRatio) => Color.Lerp(Color.DarkGoldenrod, Color.LightGoldenrodYellow, completionRatio);

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, PixelationPrimitiveLayer layer)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/FabstaffStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_) => Projectile.Size * 0.5f, pixelate: true, shader: GameShaders.Misc["CalamityMod:TrailStreak"]));
        }
    }
}
