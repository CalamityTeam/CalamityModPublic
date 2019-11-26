using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class GodsParanoiaProj : ModProjectile
    {
		public int kunaiStabbing = 12;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God's Paranoia");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 15;
            projectile.height = 15;
            projectile.friendly = true;
			projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }

		public override void AI()
		{
            Lighting.AddLight(projectile.Center, 0.35f, 0f, 0.25f);
            if (Main.rand.NextBool(2))
            {
                int num137 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), 1, 1, Main.rand.NextBool(3) ? 56 : 242, 0f, 0f, 0, default, 0.5f);
                Main.dust[num137].alpha = projectile.alpha;
                Main.dust[num137].velocity *= 0f;
                Main.dust[num137].noGravity = true;
            }

            if (projectile.ai[0] == 1f)
            {
				if (projectile.Calamity().stealthStrike)
				{
					kunaiStabbing += 2;
				}
				else
				{
					kunaiStabbing++;
				}
				if (kunaiStabbing >= 20)
				{
					kunaiStabbing = 0;
					float startOffsetX = Main.rand.NextFloat(100f, 200f) * (Main.rand.NextBool() ? -1f : 1f);
					float startOffsetY = Main.rand.NextFloat(100f, 200f) * (Main.rand.NextBool() ? -1f : 1f);
					Vector2 startPos = new Vector2(projectile.position.X + startOffsetX, projectile.position.Y + startOffsetY);
					float dx = projectile.position.X - startPos.X;
					float dy = projectile.position.Y - startPos.Y;

					// Add some randomness / inaccuracy
					dx += Main.rand.NextFloat(-5f, 5f);
					dy += Main.rand.NextFloat(-5f, 5f);
					float speed = Main.rand.NextFloat(20f, 25f);
					float dist = (float)Math.Sqrt((double)(dx * dx + dy * dy));
					dist = speed / dist;
					dx *= dist;
					dy *= dist;
					Vector2 kunaiSp = new Vector2(dx, dy);
					float angle = Main.rand.NextFloat(MathHelper.TwoPi);
					if (projectile.owner == Main.myPlayer)
					{
						for (int i = 0; i < 3; i++)
						{
							int idx = Projectile.NewProjectile(startPos, kunaiSp, ModContent.ProjectileType<GodsParanoiaDart>(), projectile.damage, 1f, projectile.owner, 0f, 0f);
							Main.projectile[idx].rotation = angle;
						}
					}
				}
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
            }
			else
			{
				projectile.rotation += 0.2f * (float)projectile.direction;
				float pcx = projectile.Center.X;
				float pcy = projectile.Center.Y;
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
					float homingstrenght = (projectile.Calamity().stealthStrike ? 14f : 7f);
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

            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
			if (modPlayer.killSpikyBalls == true)
			{
				projectile.active = false;
				projectile.netUpdate = true;
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
                                projectile.ai[0] = 1f;
                                projectile.ai[1] = (float)i;
                                projectile.velocity = (Main.npc[i].Center - projectile.Center) * 0.75f;
                                projectile.netUpdate = true;
                                projectile.StatusNPC(i);
                                int num28 = 10;
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }
    }
}
