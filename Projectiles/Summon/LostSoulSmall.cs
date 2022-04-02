using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class LostSoulSmall : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 38;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        public override void AI() => LostSoulGiant.DoSoulAI(projectile, ref Time);

        public override Color? GetAlpha(Color lightColor)
        {
            Color baseColor = Color.Lerp(Color.Orange, Color.IndianRed, projectile.identity % 5f / 5f * 0.425f);
            Color color = Color.Lerp(baseColor * 1.5f, baseColor, (float)Math.Cos(Main.GlobalTime * 2.7f) * 0.04f + 0.45f);
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
                    Vector2 drawPosition = projectile.oldPos[j] + projectile.Size * 0.5f + (MathHelper.TwoPi * i / 4f).ToRotationVector2() * 0.8f - Main.screenPosition;
                    float rotation = projectile.oldRot[j];

                    spriteBatch.Draw(texture, drawPosition, frame, drawColor, rotation, frame.Size() * 0.5f, projectile.scale, direction, 0f);
                }
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            // Play a wraith death sound.
            Main.PlaySound(SoundID.NPCDeath52, projectile.Center);

            if (Main.dedServ)
                return;

            for (int i = 0; i < 45; i++)
            {
                Dust ectoplasm = Dust.NewDustPerfect(projectile.Center + Main.rand.NextVector2Circular(42f, 42f), 264);
                ectoplasm.velocity = Main.rand.NextVector2Circular(1.75f, 1.75f);
                ectoplasm.color = projectile.GetAlpha(Color.White);
                ectoplasm.scale = 1.45f;
                ectoplasm.noGravity = true;
                ectoplasm.noLight = true;
            }
        }
    }
}
