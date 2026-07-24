using ReLogic.Content;
using System;
using System.Linq;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SiriusBeam : ModProjectile, ILocalizedModType
    {
        private static Asset<Texture2D> _cachedTexScarletDevilStreak;
        private static Asset<Texture2D> _cachedTexSylvestaffStreak;

        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 14;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.MaxUpdates = 3;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void AI()
        {
            NPC target = Projectile.Center.MinionHoming(5000f, Main.player[Projectile.owner]);
            // Move towards the target.
            if (target != null && Projectile.localNPCImmunity[target.whoAmI] <= 0)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * (Projectile.timeLeft < 300 ? 24f : 10f), 0.05f);
                Projectile.netUpdate = true;
                Projectile.Calamity().HomingTarget = target.whoAmI;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.damage <= 0)
            {
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
                Projectile.timeLeft = (int)MathHelper.Min(8,Projectile.timeLeft);
            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 12; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f) * 6f;
                float dustScale = Main.rand.NextFloat(3f, 5f);
                Color dustColor = Color.Lerp(Color.Yellow, Color.Gold, Main.rand.NextFloat(0.5f, 1f));

                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.TintableDustLighted, dustVelocity.X, dustVelocity.Y, 0, dustColor, dustScale);
                dust.noGravity = true;
                dust.noLight = false;
                dust.noLightEmittence = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End(out var ss);

            var device = Main.instance.GraphicsDevice;
            using var lease = RenderTargetPool.Shared.Rent(
                device,
                Main.screenWidth / 2,
                Main.screenHeight / 2,
                RenderTargetDescriptor.Default
            );

            using (lease.Scope(clearColor: Color.Transparent))
            {
                GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture((_cachedTexScarletDevilStreak ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak")));
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(FireWidthFunction, FireColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), Projectile.oldPos.Length + 32);

                Vector2[] fireCoreLength = Projectile.oldPos.Take(8).ToArray();
                GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture((_cachedTexSylvestaffStreak ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak")));
                PrimitiveRenderer.RenderTrail(fireCoreLength, new(FireCoreWidthFunction, FireCoreColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), fireCoreLength.Length + 24);
            }

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(lease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(ss);

            return false;
        }

        public float FireWidthFunction(float completion, Vector2 pos)
        {
            float width;
            float maxBodyWidth = 38f * Projectile.scale;
            float curveRatio = 0.2f;
            // Crop the tip of the trail into a conic shape.
            if (completion < curveRatio)
                width = MathF.Pow(completion / curveRatio, 0.5f) * maxBodyWidth;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            // Pulse inwards and outwards over time.
            float pulseInterpolant = MathF.Cos(MathHelper.Pi * completion - Main.GlobalTimeWrappedHourly * 20f) * 0.5f + 0.5f;
            float additionalPulseWidth = MathHelper.Lerp(0f, 12f, pulseInterpolant);
            return  (width + additionalPulseWidth ) * CalamityUtils.CountNonZeroVectors(Projectile.oldPos) / (float)ProjectileID.Sets.TrailCacheLength[Type] ;
        }

        public Color FireColorFunction(float completion, Vector2 pos)
        {
            Color mainColor = Color.DarkSlateBlue * 1.3f;
            Color endColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            return Color.Lerp(mainColor, endColor, completion) * Projectile.Opacity;
        }

        public float FireCoreWidthFunction(float completion, Vector2 pos)
        {
            float width;
            float maxBodyWidth = Projectile.scale * 16;
            float curveRatio = 0.25f;
            if (completion < curveRatio)
                width = MathF.Sin(completion / curveRatio * MathHelper.PiOver2) * maxBodyWidth + curveRatio;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);
            return  width * CalamityUtils.CountNonZeroVectors(Projectile.oldPos) / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireCoreColorFunction(float completion, Vector2 pos)
        {
            Color mainColor = Color.SkyBlue;
            Color tipColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            Color fullBodyColor = Color.Lerp(mainColor, tipColor, completion);
            return Color.Lerp(fullBodyColor, Color.White, 0.175f) * Projectile.Opacity;
        }
    }
}
