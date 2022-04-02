using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class SulphuricAcidBubbleFriendly : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Enemy/SulphuricAcidBubble";

        private bool fromArmour = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Bubble");
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.scale = 0.1f;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.timeLeft = 360;
            projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 1f)
            {
                projectile.ai[0] = 0f;
                projectile.scale = 1f;
                fromArmour = true;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 6)
            {
                projectile.frame = 0;
            }
            if (projectile.localAI[1] < 1f)
            {
                projectile.localAI[1] += 0.01f;
                if (projectile.scale < 1f || (fromArmour && projectile.scale < 1.8f))
                    projectile.scale += 0.02f;
                projectile.width = (int)(30f * projectile.scale);
                projectile.height = (int)(30f * projectile.scale);
            }
            else
            {
                projectile.width = fromArmour ? projectile.width : 30;
                projectile.height = fromArmour ? projectile.height : 30;
                projectile.tileCollide = true;
            }
            if (projectile.localAI[0] > 2f)
            {
                projectile.alpha -= 20;
                if (projectile.alpha < 100)
                {
                    projectile.alpha = 100;
                }
            }
            else
            {
                projectile.localAI[0] += 1f;
            }
            if (projectile.ai[1] > 30f)
            {
                if (projectile.velocity.Y > -2f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - 0.05f;
                }
            }
            else
            {
                projectile.ai[1] += 1f;
            }
            if (projectile.wet)
            {
                if (projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y * 0.98f;
                }
                if (projectile.velocity.Y > -1f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - 0.2f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)      
        {
            if (projectile.localAI[1] < 1f)
            {
                return;
            }
            target.AddBuff(BuffID.Venom, fromArmour ? 150 : 120);
            projectile.Kill();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.localAI[1] < 1f)
            {
                return;
            }
            target.AddBuff(BuffID.Venom, fromArmour ? 150 : 120);
            projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item54, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 60;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            int num3;
            for (int num246 = 0; num246 < 25; num246 = num3 + 1)
            {
                int num247 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 0, default, 1f);
                Main.dust[num247].position = (Main.dust[num247].position + projectile.position) / 2f;
                Main.dust[num247].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[num247].velocity.Normalize();
                Dust dust = Main.dust[num247];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[num247].alpha = projectile.alpha;
                num3 = num246;
            }
        }
    }
}
