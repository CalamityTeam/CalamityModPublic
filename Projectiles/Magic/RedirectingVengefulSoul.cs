using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class RedirectingVengefulSoul : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public ref float BurstIntensity => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.scale = 0.8f;
            Projectile.width = Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI() => DoSoulAI(Projectile, ref Time, 2f);

        public static void DoSoulAI(Projectile projectile, ref float time, float soulPower)
        {
            if (time >= 20f)
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(1250f, false);
                if (potentialTarget != null)
                    HomeInOnTarget(projectile, potentialTarget, time);

                float accelerationFactor = MathHelper.SmoothStep(1.03f, 1.015f, Utils.GetLerpValue(6f, 24f, projectile.velocity.Length(), true));
                if (projectile.velocity.Length() < 21f + soulPower * 2f)
                    projectile.velocity *= accelerationFactor;
            }
            projectile.Opacity = Utils.GetLerpValue(0f, 15f, time, true);
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
            float delayFactor = Utils.GetLerpValue(20f, 35f, time, true);
            float homeSpeed = MathHelper.Lerp(0f, 0.075f, delayFactor);

            projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(target.Center) * 16f, homeSpeed);
            projectile.velocity = (projectile.velocity + projectile.SafeDirectionTo(target.Center) * 3f).SafeNormalize(Vector2.UnitY) * oldSpeed;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color baseColor = Color.Lerp(Color.IndianRed, Color.DarkViolet, Projectile.identity % 5f / 5f * 0.5f);
            Color color = Color.Lerp(baseColor * 1.5f, baseColor, BurstIntensity * 0.5f + (float)Math.Cos(Main.GlobalTimeWrappedHourly * 2.7f) * 0.04f);
            color.A = 0;
            return color * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < Projectile.oldPos.Length / 2; j++)
                {
                    float fade = (float)Math.Pow(1f - Utils.GetLerpValue(0f, Projectile.oldPos.Length / 2, j, true), 2D);
                    Color drawColor = Color.Lerp(Projectile.GetAlpha(lightColor), Color.White * Projectile.Opacity, j / Projectile.oldPos.Length) * fade;
                    Vector2 drawPosition = Projectile.oldPos[j] + Projectile.Size * 0.5f + (MathHelper.TwoPi * i / 4f).ToRotationVector2() * 1.5f - Main.screenPosition;
                    float rotation = Projectile.oldRot[j];

                    Main.EntitySpriteDraw(texture, drawPosition, frame, drawColor, rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
                }
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // Play a wraith death sound at max intensity and a dungeon spirit hit sound otherwise.
            SoundEngine.PlaySound(BurstIntensity >= 1f ? SoundID.NPCDeath52 : SoundID.NPCHit35, Projectile.Center);

            // Make a ghost sound and explode into ectoplasmic energy.
            if (Main.dedServ)
                return;

            for (int i = 0; i < 45; i++)
            {
                Dust ectoplasm = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(100f, 100f) * (float)Math.Pow(BurstIntensity, 2D), 264);
                ectoplasm.velocity = Main.rand.NextVector2Circular(2f, 2f);
                ectoplasm.color = Projectile.GetAlpha(Color.White);
                ectoplasm.scale = MathHelper.Lerp(1.2f, 1.9f, BurstIntensity);
                ectoplasm.noGravity = true;
                ectoplasm.noLight = true;

                ectoplasm = Dust.NewDustPerfect(Projectile.Center, 264);
                ectoplasm.velocity = (MathHelper.TwoPi * i / 45f).ToRotationVector2() * 6f;
                ectoplasm.color = Projectile.GetAlpha(Color.White);
                ectoplasm.scale = MathHelper.Lerp(1.2f, 1.9f, BurstIntensity);
                ectoplasm.noGravity = true;
                ectoplasm.noLight = true;
            }
        }
    }
}
