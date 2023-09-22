using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AtlasMunitionsLaserOverdrive : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.MaxUpdates = 3;
            Projectile.Opacity = 0f;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * Projectile.Opacity * 0.75f);

            Projectile.Opacity = Utils.GetLerpValue(240f, 235f, Projectile.timeLeft, true);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
                Projectile.frameCounter = 0;
            }

            // Accelerate.
            if (Projectile.velocity.Length() < 19f)
                Projectile.velocity *= 1.026f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int dustCount = 32;
            bool sideways = Main.rand.NextBool(8);
            float generalAngularOffset = Main.rand.NextFloatDirection() * MathHelper.Pi / 24f;
            if (sideways)
                dustCount += 20;

            for (int i = 0; i < dustCount; i++)
            {
                float offsetAngle = MathHelper.TwoPi * i / dustCount;

                // Parametric equations for an asteroid.
                float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 4.2f;
                if (sideways)
                    puffDustVelocity = puffDustVelocity.RotatedBy(MathHelper.PiOver4) * 2f;
                else
                    puffDustVelocity = puffDustVelocity.RotatedBy(generalAngularOffset);

                Dust magic = Dust.NewDustPerfect(target.Center, 182, puffDustVelocity);
                magic.scale = 1.8f;
                magic.fadeIn = 0.5f;
                magic.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, 0, 0);
            return false;
        }
    }
}
