using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class HolyFlare : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Flare");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 600;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            projectile.velocity.Y = projectile.velocity.Y + 0.02f;
            if (projectile.velocity.Y > 1f)
            {
                projectile.velocity.Y = 1f;
            }
            if (projectile.position.X + (float)projectile.width < player.position.X)
            {
                if (projectile.velocity.X < 0f)
                {
                    projectile.velocity.X = projectile.velocity.X * 0.98f;
                }
                projectile.velocity.X = projectile.velocity.X + 0.025f;
            }
            else if (projectile.position.X > player.position.X + (float)player.width)
            {
                if (projectile.velocity.X > 0f)
                {
                    projectile.velocity.X = projectile.velocity.X * 0.98f;
                }
                projectile.velocity.X = projectile.velocity.X - 0.025f;
            }
            if (projectile.velocity.X > 5f || projectile.velocity.X < -5f)
            {
                projectile.velocity.X = projectile.velocity.X * 0.97f;
            }
            projectile.rotation = projectile.velocity.X * 0.05f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 150, 0, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 50;
            projectile.height = 50;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 10; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
