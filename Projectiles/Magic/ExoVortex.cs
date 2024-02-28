﻿using CalamityMod.Buffs.DamageOverTime;
using System.Collections.Generic;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Magic
{
    public class ExoVortex : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        public float Hue => Projectile.ai[0];

        public ref float Time => ref Projectile.ai[1];

        public const float HueShiftAcrossAfterimages = 0.2f;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.alpha = 255;
            Projectile.scale = 0.01f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = Projectile.MaxUpdates * 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 18;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Time++;
            
            // Move sharply towards nearby targets.
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(SubsumingVortex.SmallVortexTargetRange);
            if (potentialTarget != null)
            {
                float flySpeed = 40f / Projectile.MaxUpdates;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(potentialTarget.Center) * flySpeed, 0.02f);
            }

            // Rotate.
            Projectile.rotation += Projectile.velocity.X * 0.04f;

            // Emit light.
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.9f);

            // Re-determine the hitbox size.
            Projectile.Opacity = Utils.GetLerpValue(0f, 20f, Time, true);
            Projectile.scale = Utils.Remap(Time, 0f, Projectile.MaxUpdates * 15f, 0.01f, 1.5f) * Utils.GetLerpValue(0f, Projectile.MaxUpdates * 16f, Projectile.timeLeft, true);
            Projectile.ExpandHitboxBy((int)(Projectile.scale * 62f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        // Draw these vortices behind other projectiles to ensure that they do not obstruct SCal's projectiles.
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public float PrimitiveWidthFunction(float completionRatio)
        {
            float width = Projectile.width * 0.6f;
            width *= MathHelper.SmoothStep(0.6f, 1f, Utils.GetLerpValue(0f, 0.3f, completionRatio, true));
            return width;
        }

        public Color PrimitiveTrailColor(float completionRatio)
        {
            float hue = Hue % 1f + HueShiftAcrossAfterimages;
            if (hue >= 0.99f)
                hue = 0.99f;

            float velocityOpacityFadeout = Utils.GetLerpValue(2f, 5f, Projectile.velocity.Length(), true);
            Color c = CalamityUtils.MulticolorLerp(hue, CalamityUtils.ExoPalette) * Projectile.Opacity * (1f - completionRatio);
            return c * Utils.GetLerpValue(0.04f, 0.2f, completionRatio, true) * velocityOpacityFadeout;
        }

        public Vector2 PrimitiveOffsetFunction(float completionRatio) => Projectile.Size * 0.5f + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.scale * 2f;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
            Texture2D worleyNoise = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/BlobbyNoise").Value;
            float spinRotation = Main.GlobalTimeWrappedHourly * 5.2f;

            Main.spriteBatch.EnterShaderRegion();

            // Draw the streak trail.
            GameShaders.Misc["CalamityMod:SideStreakTrail"].UseImage1("Images/Misc/Perlin");
            PrimitiveSet.Prepare(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveTrailColor, PrimitiveOffsetFunction, shader: GameShaders.Misc["CalamityMod:SideStreakTrail"]), 51);
            Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
            
            GameShaders.Misc["CalamityMod:ExoVortex"].Apply();

            // Draw the vortex, along with some afterimages.
            for (int i = 0; i < 5; i++)
            {
                float hue = Hue % 1f + i / 4f * HueShiftAcrossAfterimages;
                Vector2 scale = MathHelper.Lerp(1f, 0.6f, i / 4f) * Projectile.Size / worleyNoise.Size() * 2f;
                Vector2 drawOffset = Vector2.UnitY * Projectile.scale * 6f;
                Color c = CalamityUtils.MulticolorLerp(hue, CalamityUtils.ExoPalette) * Projectile.Opacity;
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Main.spriteBatch.Draw(worleyNoise, drawPosition - drawOffset, null, c, -spinRotation, worleyNoise.Size() * 0.5f, scale, 0, 0f);
                Main.spriteBatch.Draw(worleyNoise, drawPosition + drawOffset, null, c, spinRotation, worleyNoise.Size() * 0.5f, scale, 0, 0f);
            }

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
