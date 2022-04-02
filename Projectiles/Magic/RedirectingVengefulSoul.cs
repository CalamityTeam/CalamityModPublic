using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class RedirectingVengefulSoul : ModProjectile
    {
        public ref float BurstIntensity => ref projectile.ai[0];
        public ref float Time => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.scale = 0.8f;
            projectile.width = projectile.height = 46;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI() => DoSoulAI(projectile, ref Time, 2f);

        public static void DoSoulAI(Projectile projectile, ref float time, float soulPower)
        {
            if (time >= 20f)
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(1250f, false);
                if (potentialTarget != null)
                    HomeInOnTarget(projectile, potentialTarget, time);

                float accelerationFactor = MathHelper.SmoothStep(1.03f, 1.015f, Utils.InverseLerp(6f, 24f, projectile.velocity.Length(), true));
                if (projectile.velocity.Length() < 21f + soulPower * 2f)
                    projectile.velocity *= accelerationFactor;
            }
            projectile.Opacity = Utils.InverseLerp(0f, 15f, time, true);
            projectile.frameCounter++;
            if (projectile.frameCounter % 5f == 4f)
                projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];

            projectile.rotation = projectile.velocity.ToRotation();
            projectile.spriteDirection = (projectile.velocity.X > 0f).ToDirectionInt();
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;

            time++;
        }

        public static void HomeInOnTarget(Projectile projectile, NPC target, float time)
        {
            float oldSpeed = projectile.velocity.Length();
            float delayFactor = Utils.InverseLerp(20f, 35f, time, true);
            float homeSpeed = MathHelper.Lerp(0f, 0.075f, delayFactor);

            projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(target.Center) * 16f, homeSpeed);
            projectile.velocity = (projectile.velocity + projectile.SafeDirectionTo(target.Center) * 3f).SafeNormalize(Vector2.UnitY) * oldSpeed;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color baseColor = Color.Lerp(Color.IndianRed, Color.DarkViolet, projectile.identity % 5f / 5f * 0.5f);
            Color color = Color.Lerp(baseColor * 1.5f, baseColor, BurstIntensity * 0.5f + (float)Math.Cos(Main.GlobalTime * 2.7f) * 0.04f);
            color.A = 0;
            return color * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects direction = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < projectile.oldPos.Length / 2; j++)
                {
                    float fade = (float)Math.Pow(1f - Utils.InverseLerp(0f, projectile.oldPos.Length / 2, j, true), 2D);
                    Color drawColor = Color.Lerp(projectile.GetAlpha(lightColor), Color.White * projectile.Opacity, j / projectile.oldPos.Length) * fade;
                    Vector2 drawPosition = projectile.oldPos[j] + projectile.Size * 0.5f + (MathHelper.TwoPi * i / 4f).ToRotationVector2() * 1.5f - Main.screenPosition;
                    float rotation = projectile.oldRot[j];

                    spriteBatch.Draw(texture, drawPosition, frame, drawColor, rotation, frame.Size() * 0.5f, projectile.scale, direction, 0f);
                }
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            // Play a wraith death sound at max intensity and a dungeon spirit hit sound otherwise.
            Main.PlaySound(BurstIntensity >= 1f ? SoundID.NPCDeath52 : SoundID.NPCHit35, projectile.Center);

            // Make a ghost sound and explode into ectoplasmic energy.
            if (Main.dedServ)
                return;

            for (int i = 0; i < 45; i++)
            {
                Dust ectoplasm = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(100f, 100f) * (float)Math.Pow(BurstIntensity, 2D), 264);
                ectoplasm.velocity = Main.rand.NextVector2Circular(2f, 2f);
                ectoplasm.color = projectile.GetAlpha(Color.White);
                ectoplasm.scale = MathHelper.Lerp(1.2f, 1.9f, BurstIntensity);
                ectoplasm.noGravity = true;
                ectoplasm.noLight = true;

                ectoplasm = Dust.NewDustPerfect(projectile.Center, 264);
                ectoplasm.velocity = (MathHelper.TwoPi * i / 45f).ToRotationVector2() * 6f;
                ectoplasm.color = projectile.GetAlpha(Color.White);
                ectoplasm.scale = MathHelper.Lerp(1.2f, 1.9f, BurstIntensity);
                ectoplasm.noGravity = true;
                ectoplasm.noLight = true;
            }
        }
    }
}
