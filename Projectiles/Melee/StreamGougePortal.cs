using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class StreamGougePortal : ModProjectile
    {
        public Vector2 SpearCenter => Vector2.Lerp(projectile.Center + projectile.velocity + projectile.velocity.SafeNormalize(Vector2.Zero) * 42f, projectile.Center, 1f - projectile.scale);
        public ref float Time => ref projectile.ai[0];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Portal");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.timeLeft = StreamGouge.PortalLifetime;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.MaxUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            // Play a slice sound when spawning.
            if (projectile.localAI[0] == 0f)
            {
                Main.PlayTrackedSound(CalamityUtils.GetTrackableSound("Sounds/Custom/SwiftSlice"), projectile.Center);
                projectile.localAI[0] = 1f;
            }

            float spearOutwardness = MathHelper.Lerp(4f, 52f, Utils.InverseLerp(0f, 18f, Time, true));
            projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * spearOutwardness;
            projectile.Opacity = Utils.InverseLerp(0f, 8f, Time, true) * Utils.InverseLerp(0f, 10f, projectile.timeLeft, true);
            projectile.scale = projectile.Opacity;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Time++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/StreamGouge");
            Vector2 spearDrawPosition = SpearCenter - Main.screenPosition;
            Vector2 spearOrigin = texture.Size() * 0.5f;
            Color spearColor = Main.hslToRgb(projectile.identity % 10f / 40f + 0.67f, 1f, 0.5f);
            spearColor.A = 0;

            for (int i = 0; i < 2; i++)
                spriteBatch.Draw(texture, spearDrawPosition, null, spearColor * projectile.Opacity, projectile.rotation, spearOrigin, projectile.scale, 0, 0);

            Texture2D portalTexture = ModContent.GetTexture("CalamityMod/Projectiles/Melee/StreamGougePortal");
            Vector2 origin = portalTexture.Size() * 0.5f;
            Color baseColor = Color.White;
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            float rotation = Main.GlobalTime * 6f + projectile.identity * 1.45f;

            // Black portal.
            Color color = Color.Lerp(baseColor, Color.Black, 0.55f).MultiplyRGB(Color.DarkGray) * projectile.Opacity;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, rotation, origin, projectile.scale * 1.2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(portalTexture, drawPosition, null, color, -rotation, origin, projectile.scale * 1.2f, SpriteEffects.None, 0f);

            spriteBatch.SetBlendState(BlendState.Additive);

            // Cyan portal.
            color = Color.Lerp(baseColor, Color.Cyan, 0.55f) * projectile.Opacity * 1.4f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, rotation * 0.6f, origin, projectile.scale * 1.2f, SpriteEffects.None, 0f);

            // Magenta portal.
            color = Color.Lerp(baseColor, Color.Fuchsia, 0.55f) * projectile.Opacity * 1.4f;
            spriteBatch.Draw(portalTexture, drawPosition, null, color, rotation * -0.7f, origin, projectile.scale * 1.2f, SpriteEffects.None, 0f);
            spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }

        // Create impact and cosmic parallax particles when hitting enemies.
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.Item74, target.Center);
            if (Main.netMode == NetmodeID.Server)
                return;

            Color impactColor = Color.Lerp(Color.Cyan, Color.White, Main.rand.NextFloat(0.3f, 0.64f));
            Vector2 impactPoint = Vector2.Lerp(projectile.Center, target.Center, 0.65f);
            ImpactParticle impactParticle = new ImpactParticle(impactPoint, 0.1f, 20, Main.rand.NextFloat(0.4f, 0.5f), impactColor);
            GeneralParticleHandler.SpawnParticle(impactParticle);

            for (int i = 0; i < 20; i++)
            {
                Vector2 spawnPosition = target.Center + Main.rand.NextVector2Circular(30f, 30f);
                FusableParticleManager.GetParticleSetByType<StreamGougeParticleSet>()?.SpawnParticle(spawnPosition, 60f);

                float scale = MathHelper.Lerp(24f, 64f, CalamityUtils.Convert01To010(i / 19f));
                spawnPosition = target.Center + projectile.velocity.SafeNormalize(Vector2.UnitY) * MathHelper.Lerp(-40f, 90f, i / 19f);
                FusableParticleManager.GetParticleSetByType<StreamGougeParticleSet>()?.SpawnParticle(spawnPosition, scale);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            Vector2 start = SpearCenter - projectile.velocity.SafeNormalize(Vector2.UnitY) * projectile.scale * 50f;
            Vector2 end = SpearCenter + projectile.velocity.SafeNormalize(Vector2.UnitY) * projectile.scale * 50f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f, ref _);
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
