using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class HypothermiaShard : ModProjectile
    {
		private float counter = 0f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Shard");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.scale = 1f;
            projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.alpha = 50;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
			projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.1f, 0f, 0.5f);
            projectile.rotation += projectile.velocity.X * 0.2f;
            counter += 0.25f;
            if (Main.rand.NextBool(10))
            {
                int num300 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 191, 0f, 0f, 0, default, 0.8f);
                Main.dust[num300].noGravity = true;
                Main.dust[num300].velocity *= 0.2f;
            }
            projectile.velocity *= 0.996f;
            if (counter > 100f)
            {
                projectile.scale -= 0.05f;
                if ((double)projectile.scale <= 0.2)
                {
                    projectile.scale = 0.2f;
                    projectile.Kill();
                }
                projectile.width = (int)(6f * projectile.scale);
                projectile.height = (int)(6f * projectile.scale);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y;
			}
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 191, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 0.8f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Changes the texture of the projectile
            Texture2D texture = Main.projectileTexture[projectile.type];
            if (projectile.ai[0] == 1f)
            {
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/HypothermiaShard2");
            }
            if (projectile.ai[0] == 2f)
            {
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/HypothermiaShard3");
            }
            if (projectile.ai[0] == 3f)
            {
                texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/HypothermiaShard4");
            }
			Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
			return false;
        }
    }
}
