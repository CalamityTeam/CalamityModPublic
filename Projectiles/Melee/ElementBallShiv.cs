using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ElementBallShiv : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shiv");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 120;
            projectile.aiStyle = 27;
        }

        public override void AI()
        {
            int num250 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 66, (float)(projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
            Main.dust[num250].noGravity = true;
            Main.dust[num250].velocity *= 0f;

			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 600f, 36f, 20f);
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (projectile.timeLeft > 115)
				return false;

			return true;
		}

		public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 4; k++)
            {
                int num = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 66, (float)(projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                Main.dust[num].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 4; i++)
                {
					float xPos = i < 2 ? projectile.position.X + 800 : projectile.position.X - 800;
					Vector2 source = new Vector2(xPos, projectile.position.Y + Main.rand.Next(-800, 801));

					float xStart = xPos;
					Vector2 speed = target.position - source;
					float dir = speed.Length();
					dir = 10 / xStart;
					float random = (float)Main.rand.Next(1, 150);

                    speed.X *= dir * random;
                    speed.Y *= dir * random;
                    Projectile.NewProjectile(source, speed, ModContent.ProjectileType<SHIV>(), projectile.damage, 1f, projectile.owner);
                }
            }
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
        }
    }
}
