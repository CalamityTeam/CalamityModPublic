using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class YharonMinionFireball : ModProjectile
    {
        public ref float InitialSpeed => ref projectile.ai[0];
        public override string Texture => "CalamityMod/Projectiles/Boss/YharonFireball";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Fireball");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.minionSlots = 0f;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            projectile.frame = projectile.frameCounter / 5 % Main.projFrames[projectile.type];
            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            projectile.Opacity = MathHelper.Clamp(projectile.Opacity + 0.075f, 0f, 1f);

            if (InitialSpeed == 0f)
            {
                InitialSpeed = projectile.velocity.Length();
                projectile.netUpdate = true;
                return;
            }

            NPC potentialTarget = projectile.Center.MinionHoming(600f, Main.player[projectile.owner]);
            if (potentialTarget != null)
                projectile.velocity = Vector2.Lerp(projectile.velocity, projectile.SafeDirectionTo(potentialTarget.Center) * InitialSpeed, 0.1f);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            SpriteEffects direction = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                direction = SpriteEffects.FlipHorizontally;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 0; i < projectile.oldPos.Length; i++)
                {
                    Vector2 afterimageDrawPosition = projectile.oldPos[i] + projectile.Size / 2f - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                    Color afterimageColor = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                    spriteBatch.Draw(texture, afterimageDrawPosition, frame, afterimageColor, projectile.rotation, frame.Size() * 0.5f, projectile.scale, direction, 0f);
                }
            }

            Vector2 drawPosition = projectile.Center - Main.screenPosition - Vector2.UnitY * projectile.gfxOffY;
            spriteBatch.Draw(texture, drawPosition, frame, projectile.GetAlpha(lightColor), projectile.rotation, frame.Size() * 0.5f, projectile.scale, direction, 0f);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item74, projectile.Center);

            for (int d = 0; d < 2; d++)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 50, default, 1.5f);

            for (int d = 0; d < 20; d++)
            {
                Dust fire = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 0, default, 2.2f);
                fire.noGravity = true;
                fire.velocity *= 4f;

                fire = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 50, default, 1.5f);
                fire.velocity *= 5f;
                fire.noGravity = true;
            }
        }
    }
}
