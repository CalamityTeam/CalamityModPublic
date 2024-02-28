using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ViolenceSlashProjectile : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Violence>();
        internal Player Owner => Main.player[Projectile.owner];
        internal ref float Time => ref Projectile.ai[0];
        internal float SwingSine => (float)Math.Sin(MathHelper.TwoPi * Time / 50f);
        public override string Texture => "CalamityMod/Items/Weapons/Melee/Violence";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 36;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 142;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90000;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            // Fade in.
            Projectile.Opacity = Utils.GetLerpValue(0f, 15f, Time, true);

            if (!Owner.channel)
            {
                Projectile.Kill();
                return;
            }

            ManipulatePlayerFields();
            DoMovement();

            // Create some demonic light at the tip of the spear.
            Lighting.AddLight(Projectile.Center + (Projectile.rotation + MathHelper.PiOver4).ToRotationVector2() * Projectile.height * 0.45f, Color.Red.ToVector3() * 0.4f);
            Time++;
        }

        internal void DoMovement()
        {
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter) + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * Projectile.height * 0.45f;
            Projectile.rotation = SwingSine * MathHelper.ToRadians(87f);

            if (Main.myPlayer == Projectile.owner && !Projectile.WithinRange(Main.MouseWorld, Projectile.height + 15f))
            {
                Projectile.velocity = Projectile.SafeDirectionTo(Main.MouseWorld);
                Projectile.netSpam = 0;
                Projectile.netUpdate = true;
            }

            Projectile.rotation += Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        internal void ManipulatePlayerFields()
        {
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir((Math.Cos(Projectile.rotation - MathHelper.PiOver4) > 0f).ToDirectionInt());
        }

        internal Color PrimitiveColorFunction(float completionRatio)
        {
            float averageRotation = Projectile.oldRot.Average(angle => MathHelper.WrapAngle(angle) + MathHelper.Pi);
            float opacity = Projectile.Opacity * Utils.GetLerpValue(0.75f, 0.45f, completionRatio, true) * 0.5f;
            opacity *= Utils.GetLerpValue(0.125f, 0.15f, Math.Abs(SwingSine), true);

            float rotationAdjusted = MathHelper.WrapAngle(Projectile.rotation) + MathHelper.Pi;
            float oldRotationAdjusted = MathHelper.WrapAngle(Projectile.oldRot[1]) + MathHelper.Pi;

            return Color.Lerp(Color.Red * 1.1f, Color.DarkRed, Utils.GetLerpValue(0f, 0.5f, completionRatio, true)) * opacity;
        }

        internal float PrimitiveWidthFunction(float completionRatio) => Projectile.height * 0.48f;

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:PhaseslayerRipEffect"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SwordSlashTexture"));

            Texture2D spearProjectile = ModContent.Request<Texture2D>(Texture).Value;

            Player player = Main.player[Projectile.owner];
            List<Vector2> positions = new List<Vector2>();

            for (int i = 0; i < 16; i++)
            {
                Vector2 position = Projectile.position + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (PrimitiveWidthFunction(0f) - 30f) * Projectile.scale * 0.5f;
                float angleOffset = MathHelper.Pi * 0.25f * -Math.Sign(SwingSine) * i / 16f;
                position += (Projectile.rotation - MathHelper.PiOver4 + MathHelper.PiOver2).ToRotationVector2().RotatedBy(angleOffset) * -SwingSine * i * 12f;
                positions.Add(position);
            }

            PrimitiveSet.Prepare(positions, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:PhaseslayerRipEffect"]), 50);
            return true;
        }
    }
}
