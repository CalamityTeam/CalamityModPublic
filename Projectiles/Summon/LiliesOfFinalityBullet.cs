using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class LiliesOfFinalityBullet : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
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
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = LiliesOfFinality.Elster_BulletMaxUpdates;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            // If on a dedicated server, don't bother running the visuals and sounds to save resources.
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
            // Since we don't want the trail to instantly disappear on hit, we'll make it stop and reduce its time alive to near death.
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 10;
        }

        private float PrimitiveWidthFunction(float completionRatio)
        {
            float trailPoint = 0.1f;
            return completionRatio > trailPoint ? Utils.Remap(completionRatio, trailPoint, 1f, 16f, 0f) : Utils.Remap(completionRatio, trailPoint, 0f, 16f, 0f);
        }

        private Color PrimitiveColorFunction(float completionRatio) => Color.Lerp(Color.DarkGoldenrod, Color.LightGoldenrodYellow, completionRatio);

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, PixelationPrimitiveLayer layer)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_) => Projectile.Size * 0.5f, pixelate: true, shader: GameShaders.Misc["CalamityMod:TrailStreak"]));
        }
    }
}
