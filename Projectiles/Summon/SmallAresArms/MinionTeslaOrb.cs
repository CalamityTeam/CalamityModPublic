using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class MinionTeslaOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public ref float Identity => ref Projectile.ai[0];

        public ref float Time => ref Projectile.ai[1];

        public override string Texture => "CalamityMod/Projectiles/Boss/AresTeslaOrb";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 420;
            Projectile.Opacity = 0f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 7;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            // Accelerate.
            if (Projectile.velocity.Length() < 21f)
                Projectile.velocity *= 1.012f;

            // Handle fade effects.
            Projectile.Opacity = Utils.GetLerpValue(0f, 4f, Time, true) * Utils.GetLerpValue(0f, 15f, Projectile.timeLeft, true);

            // Emit electric light.
            Lighting.AddLight(Projectile.Center, 0.1f * Projectile.Opacity, 0.25f * Projectile.Opacity, 0.25f * Projectile.Opacity);

            // Handle frames.
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Projectile.type];

            Time++;
        }

        public Projectile GetOrbToAttachTo()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type != Projectile.type || Main.projectile[i].ai[0] != Identity + 1f || !Main.projectile[i].active)
                    continue;

                if (!Main.projectile[i].WithinRange(Projectile.Center, AresExoskeleton.TeslaOrbDetatchDistance))
                    continue;

                return Main.projectile[i];
            }

            return null;
        }

        internal float WidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(0.75f, 1.85f, (float)Math.Sin(MathHelper.Pi * completionRatio)) * Projectile.scale;
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeToWhite = MathHelper.Lerp(0f, 0.65f, (float)Math.Sin(MathHelper.TwoPi * completionRatio + Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f);
            Color baseColor = Color.Lerp(Color.Cyan, Color.White, fadeToWhite);
            return Color.Lerp(baseColor, Color.LightBlue, ((float)Math.Sin(MathHelper.Pi * completionRatio + Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f) * 0.8f) * Projectile.Opacity;
        }

        internal float BackgroundWidthFunction(float completionRatio) => WidthFunction(completionRatio) * 4f;

        internal Color BackgroundColorFunction(float completionRatio)
        {
            Color color = Color.CornflowerBlue * Projectile.Opacity * 0.4f;
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile orbToAttachTo = GetOrbToAttachTo();
            if (orbToAttachTo != null)
            {
                List<Vector2> arcPoints = AresTeslaOrb.DetermineElectricArcPoints(Projectile.Center, orbToAttachTo.Center, 117);
                PrimitiveSet.Prepare(arcPoints, new(BackgroundWidthFunction, BackgroundColorFunction, smoothen: false), 90);
                PrimitiveSet.Prepare(arcPoints, new(WidthFunction, ColorFunction, smoothen: false), 90);
            }

            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float _ = 0f;
            Projectile orbToAttachTo = GetOrbToAttachTo();
            if (orbToAttachTo != null && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, orbToAttachTo.Center, 8f, ref _))
                return true;

            return false;
        }
    }
}
