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
        ///     How many natural frames have elapsed so far throughout the duration of this bolt's life. Extra updates do not affect this timer.
        /// </summary>
        public int Time
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        /// <summary>
        ///     Whether this bolt is fading out due to the a collision.
        /// </summary>
        public bool FadingOut
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
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
            if (FadingOut)
            {
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.24f);
                Projectile.velocity *= 0.81f;
                Projectile.tileCollide = false;
                if (Projectile.Opacity <= 0.1f)
                    Projectile.Kill();
            }
            else
                Projectile.velocity *= 1.004f;

            if (Projectile.FinalExtraUpdate())
                Time++;

            Projectile.scale = Utils.GetLerpValue(0f, 7f, Time, true);

            CreateMagicDust();

            // Emit light.
            Lighting.AddLight(Projectile.Center, Vector3.One * Projectile.Opacity * 0.45f);
        }

        private void CreateMagicDust()
        {
            if (Main.rand.NextBool(4))
            {
                Dust magic = Dust.NewDustPerfect(Projectile.Center, 264);
                magic.color = Color.White;
                magic.scale *= 0.56f;
                magic.noGravity = true;
            }
        }

        internal Color ColorFunction(float completionRatio)
        {
            float opacity = (1f - completionRatio) * Projectile.Opacity;
            return Color.White * opacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float tip = 1f - MathF.Pow(1f - Utils.GetLerpValue(0.05f, 0.2f, completionRatio, true), 2f);
            return tip * Projectile.Opacity * Projectile.scale * 22f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            MiscShaderData boltShader = GameShaders.Misc["CalamityMod:SylvestaffProjectile"];

            boltShader.SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/FabstaffStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f, smoothen: false, shader: boltShader), 80);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!FadingOut && Projectile.penetrate < 2)
            {
                FadingOut = true;
                Projectile.velocity *= 0.02f;
                Projectile.extraUpdates = 0;
                Projectile.netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!FadingOut)
            {
                FadingOut = true;
                Projectile.velocity = Vector2.Zero;
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
