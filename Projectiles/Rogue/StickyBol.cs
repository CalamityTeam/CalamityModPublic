using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class StickyBol : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sticky Bol");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 50;
        }

        public override void AI()
        {
			int Alpha = 175;
			Color newColor = new Color(0, 80, (int) byte.MaxValue, 100);
			if (Main.rand.NextBool(12))
			{
				Vector2 vector2 = projectile.velocity * (float) Main.rand.Next(6) / 6f;
				int num = 6;
				Dust.NewDust(projectile.position + Vector2.One * 6f, projectile.width - num * 2, projectile.height - num * 2, 4, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, Alpha, newColor, 1.2f);
			}
			if (projectile.localAI[1] == 1f)
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
			else
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
				projectile.ai[0] += 1f;
				if (projectile.ai[0] > 10f)
				{
					projectile.ai[0] = 10f;
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
            Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage &&
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
                                projectile.localAI[1] = 1f;
                                projectile.ai[1] = (float)i;
                                projectile.velocity = (Main.npc[i].Center - projectile.Center) * 0.75f;
                                projectile.netUpdate = true;
                                projectile.StatusNPC(i);
                                int num28 = 50;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 120);
        }
    }
}
