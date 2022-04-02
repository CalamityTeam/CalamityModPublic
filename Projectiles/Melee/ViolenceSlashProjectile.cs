using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ViolenceSlashProjectile : ModProjectile
    {
        internal PrimitiveTrail SliceAfterimageDrawer = null;
        internal Player Owner => Main.player[projectile.owner];
        internal ref float Time => ref projectile.ai[0];
        internal float SwingSine => (float)Math.Sin(MathHelper.TwoPi * Time / 50f);
        public override string Texture => "CalamityMod/Items/Weapons/Melee/Violence";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Violence");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 36;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 142;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 90000;
            projectile.ignoreWater = true;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            // Fade in.
            projectile.Opacity = Utils.InverseLerp(0f, 15f, Time, true);

            if (!Owner.channel)
            {
                projectile.Kill();
                return;
            }
                
            ManipulatePlayerFields();
            DoMovement();

            // Create some demonic light at the tip of the spear.
            Lighting.AddLight(projectile.Center + (projectile.rotation + MathHelper.PiOver4).ToRotationVector2() * projectile.height * 0.45f, Color.Red.ToVector3() * 0.4f);
            Time++;
        }

        internal void DoMovement()
        {
            projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter) + (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * projectile.height * 0.45f;
            projectile.rotation = SwingSine * MathHelper.ToRadians(87f);

            if (Main.myPlayer == projectile.owner && !projectile.WithinRange(Main.MouseWorld, projectile.height + 15f))
            {
                projectile.velocity = projectile.SafeDirectionTo(Main.MouseWorld);
                projectile.netSpam = 0;
                projectile.netUpdate = true;
            }

            projectile.rotation += projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        internal void ManipulatePlayerFields()
        {
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.ChangeDir((Math.Cos(projectile.rotation - MathHelper.PiOver4) > 0f).ToDirectionInt());
        }

        internal Color PrimitiveColorFunction(float completionRatio)
        {
            float averageRotation = projectile.oldRot.Average(angle => MathHelper.WrapAngle(angle) + MathHelper.Pi);
            float opacity = projectile.Opacity * Utils.InverseLerp(0.75f, 0.45f, completionRatio, true) * 0.5f;
            opacity *= Utils.InverseLerp(0.125f, 0.15f, Math.Abs(SwingSine), true);

            float rotationAdjusted = MathHelper.WrapAngle(projectile.rotation) + MathHelper.Pi;
            float oldRotationAdjusted = MathHelper.WrapAngle(projectile.oldRot[1]) + MathHelper.Pi;

            return Color.Lerp(Color.Red * 1.1f, Color.DarkRed, Utils.InverseLerp(0f, 0.5f, completionRatio, true)) * opacity;
        }

        internal float PrimitiveWidthFunction(float completionRatio) => projectile.height * 0.48f;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (SliceAfterimageDrawer is null)
                SliceAfterimageDrawer = new PrimitiveTrail(PrimitiveWidthFunction, PrimitiveColorFunction, specialShader: GameShaders.Misc["CalamityMod:PhaseslayerRipEffect"]);

            GameShaders.Misc["CalamityMod:PhaseslayerRipEffect"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/SwordSlashTexture"));

            Texture2D spearProjectile = Main.projectileTexture[projectile.type];

            Player player = Main.player[projectile.owner];
            List<Vector2> positions = new List<Vector2>();

            for (int i = 0; i < 16; i++)
            {
                Vector2 position = projectile.position + (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (PrimitiveWidthFunction(0f) - 30f) * projectile.scale * 0.5f;
                float angleOffset = MathHelper.Pi * 0.25f * -Math.Sign(SwingSine) * i / 16f;
                position += (projectile.rotation - MathHelper.PiOver4 + MathHelper.PiOver2).ToRotationVector2().RotatedBy(angleOffset) * -SwingSine * i * 12f;
                positions.Add(position);
            }

            SliceAfterimageDrawer.Draw(positions, projectile.Size * 0.5f - Main.screenPosition, 50);
            return true;
        }
    }
}
