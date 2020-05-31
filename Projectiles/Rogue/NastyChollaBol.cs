using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class NastyChollaBol : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nasty Cholla");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 200;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 157, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}
            //Sticky Behaviour
            CalamityUtils.StickyProjAI(projectile, 15);
			if (projectile.ai[0] != 1f)
			{
				try
				{
					int num223 = (int)(projectile.position.X / 16f) - 1;
					int num224 = (int)((projectile.position.X + (float)projectile.width) / 16f) + 2;
					int num225 = (int)(projectile.position.Y / 16f) - 1;
					int num226 = (int)((projectile.position.Y + (float)projectile.height) / 16f) + 2;
					if (num223 < 0)
					{
						num223 = 0;
					}
					if (num224 > Main.maxTilesX)
					{
						num224 = Main.maxTilesX;
					}
					if (num225 < 0)
					{
						num225 = 0;
					}
					if (num226 > Main.maxTilesY)
					{
						num226 = Main.maxTilesY;
					}
					for (int num227 = num223; num227 < num224; num227++)
					{
						for (int num228 = num225; num228 < num226; num228++)
						{
							if (Main.tile[num227, num228] != null && Main.tile[num227, num228].nactive() && (Main.tileSolid[(int)Main.tile[num227, num228].type] || (Main.tileSolidTop[(int)Main.tile[num227, num228].type] && Main.tile[num227, num228].frameY == 0)))
							{
								Vector2 vector19;
								vector19.X = (float)(num227 * 16);
								vector19.Y = (float)(num228 * 16);
								if (projectile.position.X + (float)projectile.width - 4f > vector19.X && projectile.position.X + 4f < vector19.X + 16f && projectile.position.Y + (float)projectile.height - 4f > vector19.Y && projectile.position.Y + 4f < vector19.Y + 16f)
								{
									projectile.velocity.X = 0f;
									projectile.velocity.Y = -0.2f;
								}
							}
						}
					}
				} catch
				{
				}
				projectile.localAI[1] += 1f;
				if (projectile.localAI[1] > 10f)
				{
					projectile.localAI[1] = 10f;
					if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
					{
						projectile.velocity.X = projectile.velocity.X * 0.97f;
						if ((double)projectile.velocity.X > -0.01 && (double)projectile.velocity.X < 0.01)
						{
							projectile.velocity.X = 0f;
							projectile.netUpdate = true;
						}
					}
					projectile.velocity.Y = projectile.velocity.Y + 0.2f;
				}
				projectile.rotation += projectile.velocity.X * 0.1f;
			}				
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            CalamityUtils.ModifyHitNPCSticky(projectile, 20, false);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

		//So you can stick a bol up the Guide's ass
        public override bool? CanHitNPC(NPC target)
		{
			if (target.townNPC)
			{
				return true;
			}
			return null;
		}

        public override void Kill(int timeLeft)
        {
			Player player = Main.player[projectile.owner];
			int num251 = Main.rand.Next(2, 4);
			if (projectile.owner == Main.myPlayer)
			{
				for (int num252 = 0; num252 < num251; num252++)
				{
					Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					while (value15.X == 0f && value15.Y == 0f)
					{
						value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					}
					value15.Normalize();
					value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
					int shard = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<NastyChollaNeedle>(), (int)((NastyCholla.BaseDamage/4) * (player.allDamage + player.meleeDamage - 1f)), 0f, projectile.owner, 0f, 0f);
				}
			}
		}
    }
}
