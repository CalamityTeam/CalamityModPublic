using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class FabBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public bool FadingOut
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value.ToInt();
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 23;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 12;
            Projectile.timeLeft = 90 * Projectile.extraUpdates;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            if (FadingOut)
            {
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.27f);
                if (Projectile.Opacity <= 0.05f)
                    Projectile.Kill();
            }
            else
                Projectile.velocity *= 1.004f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Emit light.
            Lighting.AddLight(Projectile.Center, Vector3.One * Projectile.Opacity * 0.45f);
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeToEnd = MathHelper.Lerp(0.25f, 0.5f, (float)Math.Cos(-Main.GlobalTimeWrappedHourly * 3f) * 0.5f + 0.5f);
            fadeToEnd *= 1f - Utils.GetLerpValue(0.35f, 0f, completionRatio, true);
            Color endColor = Color.Lerp(Color.Cyan, Color.HotPink, Projectile.identity % 2);
            return Color.Lerp(Color.White, endColor, fadeToEnd) * Projectile.Opacity * 0.7f;
        }

        internal float WidthFunction(float completionRatio)
        {
            float expansionCompletion = 1f - (float)Math.Pow(1f - Utils.GetLerpValue(0f, 0.2f, completionRatio, true), 2D);
            return MathHelper.Lerp(0f, 22f, expansionCompletion) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/FabstaffStreak"));
            PrimitiveSet.Prepare(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f, smoothen: false, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 80);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!FadingOut && Projectile.penetrate < 2)
            {
                FadingOut = true;
                Projectile.velocity *= 0.1f;
                Projectile.extraUpdates = 0;
                Projectile.netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!FadingOut)
            {
                FadingOut = true;
                Projectile.velocity *= 0.1f;
                Projectile.extraUpdates = 0;
                Projectile.netUpdate = true;
            }
            return false;
        }

        public override bool? CanDamage()
        {
            if (FadingOut)
                return false;
            return null;
        }
    }
}
