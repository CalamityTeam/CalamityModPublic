using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Rogue
{
    public class ProfanedPartisanspear : ModProjectile
    {
		public int timer = 0;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Profaned Spear");
		}

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.timeLeft = 600;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			if (projectile.ai[1] != 1f)
			{
				if (projectile.velocity.X != oldVelocity.X)
				{
					projectile.velocity.X = -oldVelocity.X;
				}
				if (projectile.velocity.Y != oldVelocity.Y)
				{
					projectile.velocity.Y = -oldVelocity.Y;
				}
				projectile.ai[1] = 1f;
				projectile.ai[0] = 1f;
				projectile.extraUpdates = 2;
				if (projectile.timeLeft > 280)
					projectile.timeLeft = 280;
			}
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
			if (projectile.ai[1] != 1f)
			{
				projectile.velocity.X = -projectile.velocity.X;
				projectile.velocity.Y = -projectile.velocity.Y;
				projectile.ai[1] = 1f;
				projectile.ai[0] = 1f;
				projectile.extraUpdates = 2;
				if (projectile.timeLeft > 280)
					projectile.timeLeft = 280;
			}
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
			if (projectile.ai[1] != 1f)
			{
				projectile.velocity.X = -projectile.velocity.X;
				projectile.velocity.Y = -projectile.velocity.Y;
				projectile.ai[1] = 1f;
				projectile.ai[0] = 1f;
				projectile.extraUpdates = 2;
				if (projectile.timeLeft > 280)
					projectile.timeLeft = 280;
			}
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

			if (projectile.ai[0] == 1f)
				timer++;
			if (timer >= 5)
				projectile.penetrate = 1;
			float pcx = projectile.Center.X;
			float pcy = projectile.Center.Y;
			if (timer >= 10)
			{
				float var1 = 800f;
				bool flag = false;
				for (int npcvar = 0; npcvar < 200; npcvar++)
				{
					if (Main.npc[npcvar].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[npcvar].Center, 1, 1))
					{
						float var2 = Main.npc[npcvar].position.X + (Main.npc[npcvar].width / 2);
						float var3 = Main.npc[npcvar].position.Y + (Main.npc[npcvar].height / 2);
						float var4 = Math.Abs(projectile.position.X + (projectile.width / 2) - var2) + Math.Abs(projectile.position.Y + (projectile.height / 2) - var3);
						if (var4 < var1)
						{
							var1 = var4;
							pcx = var2;
							pcy = var3;
							flag = true;
						}
					}
				}
				if (flag)
				{
					float homingstrenght = 7f;
					Vector2 vector1 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
					float var6 = pcx - vector1.X;
					float var7 = pcy - vector1.Y;
					float var8 = (float)Math.Sqrt(var6 * var6 + var7 * var7);
					var8 = homingstrenght / var8;
					var6 *= var8;
					var7 *= var8;
					projectile.velocity.X = (projectile.velocity.X * 20f + var6) / 21f;
					projectile.velocity.Y = (projectile.velocity.Y * 20f + var7) / 21f;
				}
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
