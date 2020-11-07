using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class CryoBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 35;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 4;
            projectile.timeLeft = 600;
			projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (projectile.scale <= 3.6f)
            {
                projectile.scale *= 1.01f;
				projectile.position = projectile.Center;
                projectile.width = (int)(35f * projectile.scale);
                projectile.height = (int)(35f * projectile.scale);
				projectile.Center = projectile.position;
            }

			if (projectile.timeLeft < 53)
				projectile.alpha += 5;

            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0.5f);

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
				int ice = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, 0f, 0f, 100, default, projectile.scale * 0.5f);
				Main.dust[ice].noGravity = true;
				Main.dust[ice].velocity *= 0f;
				int snow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 185, 0f, 0f, 100, default, projectile.scale * 0.5f);
				Main.dust[snow].noGravity = true;
				Main.dust[snow].velocity *= 0f;
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.timeLeft > 599)
				return false;

			Texture2D texture = Main.projectileTexture[projectile.type];
			Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
			Vector2 origin = rectangle.Size() / 2f;

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), rectangle, lightColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);
			return false;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 360);
            target.AddBuff(BuffID.Frostburn, 360);
        }
    }
}
