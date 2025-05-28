using System;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SylvBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        /// <summary>
        ///     The color hue interpolant for this bolt.
        /// </summary>
        public ref float HueInterpolant => ref Projectile.ai[0];

        /// <summary>
        ///     How many natural frames have elapsed so far throughout the duration of this bolt's life. Extra updates do not affect this timer.
        /// </summary>
        public int Time
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        /// <summary>
        ///     Whether this bolt is vanishing due to the a collision.
        /// </summary>
        public bool Vanishing
        {
            get => Projectile.ai[2] == 1f;
            set => Projectile.ai[2] = value.ToInt();
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
            Projectile.timeLeft = Projectile.extraUpdates * 90;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.004f;
            if (Vanishing)
                Vanish();

            if (Projectile.FinalExtraUpdate())
                Time++;

            Projectile.scale = Utils.GetLerpValue(0f, 7f, Time, true);

            CreateMagicDust();

            // Emit light.
            Lighting.AddLight(Projectile.Center, Vector3.One * Projectile.Opacity * 0.45f);
        }

        /// <summary>
        ///     Makes this bolt vanish.
        /// </summary>
        private void Vanish()
        {
            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.24f);
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
            if (Projectile.Opacity <= 0.1f)
                Projectile.Kill();
        }

        /// <summary>
        ///     Idly emits magic dust off this bolt.
        /// </summary>
        private void CreateMagicDust()
        {
            if (Main.rand.NextBool(4))
            {
                Dust magic = Dust.NewDustPerfect(Projectile.Center, 264);
                magic.color = ColorFunction(0f);
                magic.scale *= 0.56f;
                magic.noGravity = true;
            }
        }

        /// <summary>
        ///     The function responsible for dictating the color of this bolt.
        /// </summary>
        internal Color ColorFunction(float completionRatio)
        {
            float opacity = (1f - completionRatio) * Projectile.Opacity;
            return CalamityUtils.MulticolorLerp(HueInterpolant, new Color(255, 193, 255), Color.White, new Color(127, 242, 255)) * opacity;
        }

        /// <summary>
        ///     The function responsible for dictating the width of this bolt.
        /// </summary>
        internal float WidthFunction(float completionRatio)
        {
            float tip = 1f - MathF.Pow(1f - Utils.GetLerpValue(0.05f, 0.2f, completionRatio * Projectile.Opacity, true), 2f);
            return tip * Projectile.Opacity * Projectile.scale * 22f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            MiscShaderData boltShader = GameShaders.Misc["CalamityMod:SylvestaffProjectile"];

            boltShader.SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f, smoothen: false, shader: boltShader), 80);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Vanishing && Projectile.penetrate < 2)
            {
                Vanishing = true;
                Projectile.velocity *= 0.02f;
                Projectile.extraUpdates = 0;
                Projectile.netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!Vanishing)
            {
                Vanishing = true;
                Projectile.velocity = Vector2.Zero;
                Projectile.extraUpdates = 0;
                Projectile.netUpdate = true;
            }
            return false;
        }

        public override bool? CanDamage() => !Vanishing;
    }
}
