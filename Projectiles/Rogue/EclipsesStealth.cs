using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class EclipsesStealth : ModProjectile
	{
		private int deadinside = 0; //used to know when to drop spears from the sky

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eclipse's Stealth");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

		public override void SetDefaults()
		{
			projectile.width = 25;
			projectile.height = 25;
            projectile.friendly = true;
			projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 30;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}

		public override void AI()
		{
			deadinside++; //congrats Pinkie
			if (deadinside >= 5) //every 5 ticks
			{
				deadinside = 0;
				if (Main.rand.Next(2) == 0)
				{
					int spearAmt = Main.rand.Next(1,4); //1 to 3 spears
					for (int n = 0; n < spearAmt; n++)
					{
						float x = projectile.position.X + (float)Main.rand.Next(-400, 400);
						float y = projectile.position.Y - (float)Main.rand.Next(500, 800);
						Vector2 vector = new Vector2(x, y);
						float num13 = projectile.position.X + (float)(projectile.width / 2) - vector.X;
						float num14 = projectile.position.Y + (float)(projectile.height / 2) - vector.Y;
							num13 += (float)Main.rand.Next(-100, 101);
						int num15 = 29;
						float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
						num16 = (float)num15 / num16;
						num13 *= num16;
						num14 *= num16;
						Projectile.NewProjectile(x, y, num13, num14, mod.ProjectileType("EclipsesSmol"), (int)((double)projectile.damage * 0.1 * Main.rand.Next(7, 10)), (int)((double)projectile.knockBack * 0.1 * Main.rand.Next(7, 10)), projectile.owner, 0f, 0f);
					} //very complicated and painful
				}
			}
            if (projectile.ai[0] == 0f)
            {
				projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
				if (Main.rand.Next(5) == 0) //dust
				{
					Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 138, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
				}
            }
            if (projectile.ai[0] == 1f) //sticking to enemies
            {
                int num988 = 15;
                bool flag54 = false;
                bool flag55 = false;
                float[] var_2_2CB4E_cp_0 = projectile.localAI;
                int var_2_2CB4E_cp_1 = 0;
                float num73 = var_2_2CB4E_cp_0[var_2_2CB4E_cp_1];
                var_2_2CB4E_cp_0[var_2_2CB4E_cp_1] = num73 + 1f;
                if (projectile.localAI[0] % 30f == 0f)
                {
                    flag55 = true;
                }
                int num989 = (int)projectile.ai[1];
                if (projectile.localAI[0] >= (float)(60 * num988))
                {
                    flag54 = true;
                }
                else if (num989 < 0 || num989 >= 200)
                {
                    flag54 = true;
                }
                else if (Main.npc[num989].active && !Main.npc[num989].dontTakeDamage)
                {
                    projectile.Center = Main.npc[num989].Center - projectile.velocity * 2f;
                    projectile.gfxOffY = Main.npc[num989].gfxOffY;
                    if (flag55)
                    {
                        Main.npc[num989].HitEffect(0, 1.0);
                    }
                }
                else
                {
                    flag54 = true;
                }
                if (flag54)
                {
                    projectile.Kill();
                }
            }
        }

		#region SpearBarrage
		public void SpearBarrage(Player player)
		{
		}
		#endregion

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (((Main.npc[i].active && !Main.npc[i].dontTakeDamage)) &&
                        ((projectile.friendly && (!Main.npc[i].friendly || projectile.type == 318 || (Main.npc[i].type == 22 && projectile.owner < 255 && Main.player[projectile.owner].killGuide) || (Main.npc[i].type == 54 && projectile.owner < 255 && Main.player[projectile.owner].killClothier))) ||
                        (projectile.hostile && Main.npc[i].friendly && !Main.npc[i].dontTakeDamageFromHostiles)) && (projectile.owner < 0 || Main.npc[i].immune[projectile.owner] == 0 || projectile.maxPenetrate == 1))
                    {
                        if (Main.npc[i].noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(Main.npc[i]))
                        {
                            bool flag3;
                            if (Main.npc[i].type == 414)
                            {
                                Rectangle rect = Main.npc[i].getRect();
                                int num5 = 8;
                                rect.X -= num5;
                                rect.Y -= num5;
                                rect.Width += num5 * 2;
                                rect.Height += num5 * 2;
                                flag3 = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                flag3 = projectile.Colliding(myRect, Main.npc[i].getRect());
                            }
                            if (flag3)
                            {
                                if (Main.npc[i].reflectingProjectiles && projectile.CanReflect())
                                {
                                    Main.npc[i].ReflectProjectile(projectile.whoAmI);
                                    return;
                                }
                                projectile.ai[0] = 1f;
								projectile.timeLeft = 900; //increase lifetime if sticking to something
                                projectile.ai[1] = (float)i;
                                projectile.velocity = (Main.npc[i].Center - projectile.Center) * 0.75f;
                                projectile.netUpdate = true;
                                projectile.StatusNPC(i);
                                //projectile.damage = 0; //aaaaaaa
                                int num28 = 1;
                                Point[] array2 = new Point[num28];
                                int num29 = 0;
                                for (int l = 0; l < 1000; l++)
                                {
                                    if (l != projectile.whoAmI && Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == projectile.type && Main.projectile[l].ai[0] == 1f && Main.projectile[l].ai[1] == (float)i)
                                    {
                                        array2[num29++] = new Point(l, Main.projectile[l].timeLeft);
                                        if (num29 >= array2.Length)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (num29 >= array2.Length)
                                {
                                    int num30 = 0;
                                    for (int m = 1; m < array2.Length; m++)
                                    {
                                        if (array2[m].Y < array2[num30].Y)
                                        {
                                            num30 = m;
                                        }
                                    }
                                    Main.projectile[array2[num30].X].Kill();
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}
	}
}
