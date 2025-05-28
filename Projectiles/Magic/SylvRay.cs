using System;
using System.IO;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SylvRay : ModProjectile, ILocalizedModType
    {
        /// <summary>
        ///     The position from which this ray's glow effect appears.
        /// </summary>
        public Vector2 GlowCenter
        {
            get;
            set;
        }

        /// <summary>
        ///     How many natural frames have elapsed so far throughout the duration of this ray's life. Extra updates do not affect this timer.
        /// </summary>
        public int Time
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 54;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 70;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 60 * Projectile.extraUpdates;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.WriteVector2(GlowCenter);

        public override void ReceiveExtraAI(BinaryReader reader) => GlowCenter = reader.ReadVector2();

        public override void AI()
        {
            if (Projectile.FinalExtraUpdate())
                Time++;

            Projectile.Opacity = Utils.GetLerpValue(0f, Projectile.MaxUpdates * 10f, Projectile.timeLeft, true);
            Projectile.scale = Utils.GetLerpValue(1f, 9.5f, Time, true) * Projectile.Opacity;

            if (GlowCenter == Vector2.Zero)
                GlowCenter = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 74f;

            HandleBoltFiring();
            CreateGlowyDust();

            Lighting.AddLight(Projectile.Center, Vector3.One * Projectile.Opacity * 0.7f);
        }

        /// <summary>
        ///     Handles the firing of bolts across this ray.
        /// </summary>
        private void HandleBoltFiring()
        {
            int shootRate = Sylvestaff.RayBoltShootRate;
            if (Time % shootRate == 0 && Projectile.FinalExtraUpdate())
            {
                int trailSearchPositions = 13;
                for (int i = 0; i < trailSearchPositions; i += 3)
                    TryToFireBolt(Projectile.oldPos[i] + Projectile.Size * 0.5f, i / (float)trailSearchPositions);
            }
        }

        /// <summary>
        ///     Attempts to fire a given bolt projectile from a given position at the closest enemy to said position, assuming one exists.
        /// </summary>
        private void TryToFireBolt(Vector2 searchPosition, float hue)
        {
            NPC potentialTarget = searchPosition.ClosestNPCAt(Sylvestaff.RayBoltTargetingRange, false);
            if (potentialTarget is null)
                return;

            SoundEngine.PlaySound(Sylvestaff.BounceSound with { MaxInstances = 12, Volume = 0.4f, Pitch = 0.4f }, Projectile.Center);

            float shootSpeed = Projectile.velocity.Length();
            Vector2 shootVelocity = (potentialTarget.Center - searchPosition).SafeNormalize(Vector2.UnitY) * shootSpeed;
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), searchPosition, shootVelocity, ModContent.ProjectileType<SylvBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner, hue);

            float burstSpeed = Main.rand.NextFloat(1f, 3f);
            for (int i = 0; i < 4; i++)
            {
                Dust magicBurst = Dust.NewDustPerfect(searchPosition, 264);
                magicBurst.velocity = Projectile.velocity.RotatedBy(MathHelper.TwoPi * i / 4f + MathHelper.PiOver4).SafeNormalize(Vector2.Zero) * burstSpeed;
                magicBurst.color = Color.White;
                magicBurst.noLight = true;
                magicBurst.noGravity = true;
            }
        }

        /// <summary>
        ///     Emits dust particles at the glow center of this ray, to help sell the notion that it's appearing from a concentrated ball of queer magic energy.
        /// </summary>
        private void CreateGlowyDust()
        {
            if (Time <= 9 && Main.rand.NextBool())
            {
                Color colorAccent = Main.rand.NextBool() ? Color.HotPink : Color.Aqua;

                Dust transMagic = Dust.NewDustPerfect(GlowCenter, 264);
                transMagic.velocity = Main.rand.NextVector2Circular(4.6f, 4.6f) + Projectile.velocity * 0.25f;
                transMagic.color = Color.Lerp(Color.White, colorAccent, Main.rand.NextFloat(0.23f));
                transMagic.scale *= 1.1f;
                transMagic.fadeIn = 0.75f;
                transMagic.noGravity = true;
            }
        }

        /// <summary>
        ///     The function responsible for dictating the color of this ray.
        /// </summary>
        internal Color ColorFunction(float completionRatio)
        {
            float opacity = MathF.Pow(Utils.GetLerpValue(1f, 0.64f, completionRatio, true), 3f) * Projectile.Opacity;
            float colorInterpolant = MathF.Cos(MathHelper.Pi * completionRatio - Main.GlobalTimeWrappedHourly * 7.2f) * 0.5f + 0.5f;

            Color pink = new Color(255, 147, 255);
            Color blue = new Color(109, 224, 255);
            Color baseColor = CalamityUtils.MulticolorLerp(colorInterpolant, pink, Color.White, blue, Color.White);

            return baseColor * opacity;
        }

        /// <summary>
        ///     The function responsible for dictating the width of this ray.
        /// </summary>
        internal float WidthFunction(float completionRatio)
        {
            float expansionCompletion = 1f - MathF.Pow(1f - Utils.GetLerpValue(0f, 0.3f, completionRatio, true), 2f);
            float undulation = MathF.Cos(MathHelper.Pi * completionRatio * 5f - Main.GlobalTimeWrappedHourly * 23f) * 2.4f;
            float maxWidth = undulation + 32f;

            return MathHelper.Lerp(0f, Projectile.scale * maxWidth, expansionCompletion);
        }

        /// <summary>
        ///     The function responsible for dictating the render offset of this ray.
        /// </summary>
        internal Vector2 OffsetFunction(float completionRatio) => Projectile.Size * 0.5f;

        /// <summary>
        ///     Renders the front-glow for this ray, to help make it look like it has a defined origin of concentrated magic.
        /// </summary>
        private void RenderFrontGlow()
        {
            float glowAnimationProgress = Utils.GetLerpValue(0f, 9.5f, Time, true);
            float glowBump = CalamityUtils.Convert01To010(glowAnimationProgress);
            float glowRotation = Projectile.velocity.ToRotation();
            Vector2 glowScale = new Vector2(1f + glowBump * 0.8f, 1f) * glowBump;

            Vector2 startingPosition = GlowCenter - Main.screenPosition;
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BloomCirclePinpoint").Value;
            Main.spriteBatch.Draw(lightTexture, startingPosition, null, ColorFunction(0f) with { A = 0 }, glowRotation, lightTexture.Size() * 0.5f, glowScale, 0, 0f);
            Main.spriteBatch.Draw(lightTexture, startingPosition, null, ColorFunction(0f) with { A = 0 }, glowRotation, lightTexture.Size() * 0.5f, glowScale * 0.4f, 0, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            RenderFrontGlow();

            MiscShaderData rayShader = GameShaders.Misc["CalamityMod:SylvestaffProjectile"];
            rayShader.SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));

            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new PrimitiveSettings(WidthFunction, ColorFunction, OffsetFunction, pixelate: false, shader: rayShader), 32);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                SoundEngine.PlaySound(Sylvestaff.BounceSound, Projectile.Center);

                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
    }
}
