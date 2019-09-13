using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class CrystalDust : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dust");
		}

        public override void SetDefaults()
        {
            projectile.width = 46;
            projectile.height = 46;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 3;
            projectile.timeLeft = 100;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
	            int num307 = Main.rand.Next(3);
				if (num307 == 0)
				{
					num307 = 173;
				}
				else if (num307 == 1)
				{
					num307 = 57;
				}
				else
				{
					num307 = 58;
				}
				for (int num468 = 0; num468 < 5; num468++)
				{
					int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num307, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num469].noGravity = true;
					Main.dust[num469].velocity *= 0f;
				}
			}
			if (projectile.ai[0] == 1f)
			{
				if (projectile.owner == Main.myPlayer)
				{
					int num814 = 3;
					int num815 = (int)(projectile.position.X / 16f - (float)num814);
					int num816 = (int)(projectile.position.X / 16f + (float)num814);
					int num817 = (int)(projectile.position.Y / 16f - (float)num814);
					int num818 = (int)(projectile.position.Y / 16f + (float)num814);
					if (num815 < 0)
					{
						num815 = 0;
					}
					if (num816 > Main.maxTilesX)
					{
						num816 = Main.maxTilesX;
					}
					if (num817 < 0)
					{
						num817 = 0;
					}
					if (num818 > Main.maxTilesY)
					{
						num818 = Main.maxTilesY;
					}
					AchievementsHelper.CurrentlyMining = true;
					for (int num824 = num815; num824 <= num816; num824++)
					{
						for (int num825 = num817; num825 <= num818; num825++)
						{
							float num826 = Math.Abs((float)num824 - projectile.position.X / 16f);
							float num827 = Math.Abs((float)num825 - projectile.position.Y / 16f);
							double num828 = Math.Sqrt((double)(num826 * num826 + num827 * num827));
							if (num828 < (double)num814)
							{
								if (Main.tile[num824, num825] != null && Main.tile[num824, num825].active())
								{
									WorldGen.KillTile(num824, num825, false, false, false);
									if (!Main.tile[num824, num825].active() && Main.netMode != 0)
									{
										NetMessage.SendData(17, -1, -1, null, 0, (float)num824, (float)num825, 0f, 0, 0, 0);
									}
								}
							}
						}
					}
					AchievementsHelper.CurrentlyMining = false;
					if (Main.netMode != 0)
					{
						NetMessage.SendData(29, -1, -1, null, projectile.identity, (float)projectile.owner, 0f, 0f, 0, 0, 0);
					}
				}
			}
        }
    }
}
