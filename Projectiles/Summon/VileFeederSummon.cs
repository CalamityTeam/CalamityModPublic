using CalamityMod.CalPlayer;
using CalamityMod.Buffs.Summon;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class VileFeederSummon : ModProjectile
    {
        private bool spawnDust = true;
		private float attackCooldown = 0f;
		private int eaterCooldown = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eater of Souls");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (spawnDust)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 7, vector7.X * 1.75f, vector7.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
                spawnDust = false;
            }

            bool correctMinion = projectile.type == ModContent.ProjectileType<VileFeederSummon>();
            player.AddBuff(ModContent.BuffType<VileFeederBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.vileFeeder = false;
                }
                if (modPlayer.vileFeeder)
                {
                    projectile.timeLeft = 2;
                }
            }

			if (eaterCooldown < 0)
				eaterCooldown = 0;

			if (projectile.ai[0] != 3f)
			{
				if (eaterCooldown > 0)
					eaterCooldown--;
                projectile.damage = (int)(VileFeeder.BaseDamage * player.MinionDamage());
				if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
				{
					int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
						projectile.Calamity().spawnedPlayerMinionDamageValue *
						player.MinionDamage());
					projectile.damage = damage2;
				}
				float antiStickyFloat = 0.05f;
				for (int index = 0; index < Main.maxProjectiles; index++)
				{
					Projectile proj = Main.projectile[index];
					bool flag23 = proj.type == ModContent.ProjectileType<VileFeederSummon>();
					if (index != projectile.whoAmI && proj.active && proj.owner == projectile.owner && flag23 && Math.Abs(projectile.position.X - proj.position.X) + Math.Abs(projectile.position.Y - proj.position.Y) < (float)projectile.width)
					{
						if (projectile.position.X < proj.position.X)
						{
							projectile.velocity.X = projectile.velocity.X - antiStickyFloat;
						}
						else
						{
							projectile.velocity.X = projectile.velocity.X + antiStickyFloat;
						}
						if (projectile.position.Y < proj.position.Y)
						{
							projectile.velocity.Y = projectile.velocity.Y - antiStickyFloat;
						}
						else
						{
							projectile.velocity.Y = projectile.velocity.Y + antiStickyFloat;
						}
					}
				}
				projectile.frameCounter++;
				if (projectile.frameCounter > 3)
				{
					projectile.frame++;
					projectile.frameCounter = 0;
				}
				if (projectile.frame >= 4)
				{
					projectile.frame = 0;
				}
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(270);

				bool flag24 = false;
				if (projectile.ai[0] == 2f)
				{
					attackCooldown += 1f;
					projectile.extraUpdates = 1;
					if (attackCooldown > 40f)
					{
						attackCooldown = 1f;
						projectile.ai[0] = 0f;
						projectile.extraUpdates = 0;
						projectile.numUpdates = 0;
						projectile.netUpdate = true;
					}
					else
					{
						flag24 = true;
					}
				}
				if (flag24)
				{
					return;
				}

				Vector2 vector46 = projectile.position;
				float num633 = 800f; //50 block detection range
				bool targetFound = false;
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(projectile, false))
					{
						float npcDist = Vector2.Distance(npc.Center, projectile.Center);
						if (npcDist < num633 && !targetFound && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
						{
							num633 = npcDist;
							vector46 = npc.Center;
							targetFound = true;
						}
					}
				}
				else
				{
					for (int num645 = 0; num645 < Main.maxNPCs; num645++)
					{
						NPC nPC2 = Main.npc[num645];
						if (nPC2.CanBeChasedBy(projectile, false))
						{
							float npcDist = Vector2.Distance(nPC2.Center, projectile.Center);
							if (npcDist < num633 && !targetFound && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, nPC2.position, nPC2.width, nPC2.height))
							{
								num633 = npcDist;
								vector46 = nPC2.Center;
								targetFound = true;
							}
						}
					}
				}
				float maxPlayerRange = 1100f;
				if (targetFound)
				{
					maxPlayerRange = 2400f;
				}
				if (Vector2.Distance(player.Center, projectile.Center) > maxPlayerRange)
				{
					projectile.ai[0] = 1f;
					projectile.netUpdate = true;
				}
				if (targetFound && projectile.ai[0] == 0f)
				{
					Vector2 vector47 = vector46 - projectile.Center;
					float num648 = vector47.Length();
					vector47.Normalize();
					if (num648 > 200f)
					{
						float scaleFactor2 = 8f;
						vector47 *= scaleFactor2;
						projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
					}
					else
					{
						float num649 = 4f;
						vector47 *= -num649;
						projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
					}
				}
				else
				{
					bool flag26 = false;
					if (!flag26)
					{
						flag26 = projectile.ai[0] == 1f;
					}
					float num650 = 6f;
					if (flag26)
					{
						num650 = 15f;
					}
					Vector2 center2 = projectile.Center;
					Vector2 vector48 = player.Center - center2 + new Vector2(0f, -60f);
					float playerDist = vector48.Length();
					if (playerDist > 200f && num650 < 8f)
					{
						num650 = 8f;
					}
					if (playerDist < 150f && flag26 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
					{
						projectile.ai[0] = 0f;
						projectile.netUpdate = true;
					}
					if (playerDist > 2000f)
					{
						projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
						projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
						projectile.netUpdate = true;
					}
					if (playerDist > 70f)
					{
						vector48.Normalize();
						vector48 *= num650;
						projectile.velocity = (projectile.velocity * 40f + vector48) / 41f;
					}
					else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
					{
						projectile.velocity.X = -0.15f;
						projectile.velocity.Y = -0.05f;
					}
				}
				if (attackCooldown > 0f)
				{
					attackCooldown += (float)Main.rand.Next(1, 4);
				}
				if (attackCooldown > 40f)
				{
					attackCooldown = 0f;
					projectile.netUpdate = true;
				}
				if (projectile.ai[0] == 0f)
				{
					if (attackCooldown == 0f && targetFound && num633 < 500f)
					{
						attackCooldown += 1f;
						if (Main.myPlayer == projectile.owner)
						{
							projectile.ai[0] = 2f;
							Vector2 value20 = vector46 - projectile.Center;
							value20.Normalize();
							projectile.velocity = value20 * 8f;
							projectile.netUpdate = true;
						}
					}
				}
			}
			else
			{
				projectile.frame = 0;
                projectile.extraUpdates = 0;
                int num988 = 10;
                bool flag54 = false;
                bool flag55 = false;
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] % 30f == 0f)
                {
                    flag55 = true;
                }
                int npcIndex = (int)projectile.ai[1];
                if (projectile.localAI[0] >= (float)(60000 * num988)) //tryna make it stay on there "forever" without glitching
                {
                    flag54 = true;
                }
                else if (npcIndex < 0 || npcIndex >= Main.maxNPCs)
                {
                    flag54 = true;
                }
                else if (Main.npc[npcIndex].active && !Main.npc[npcIndex].dontTakeDamage && Main.npc[npcIndex].defense < 9999)
                {
                    projectile.Center = Main.npc[npcIndex].Center - projectile.velocity * 2f;
                    projectile.gfxOffY = Main.npc[npcIndex].gfxOffY;
                    if (flag55)
                    {
                        Main.npc[npcIndex].HitEffect(0, 1.0);
                    }
                }
                else
                {
                    flag54 = true;
                }
                if (flag54)
                {
                    projectile.ai[0] = 0f;
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = 0f;
                }
				if (projectile.owner == Main.myPlayer)
				{
					if (eaterCooldown > 0)
						eaterCooldown -= Main.rand.Next(1,4);

					if (eaterCooldown <= 0)
					{
						int projNumber = Main.rand.Next(1,4);
						for (int index2 = 0; index2 < projNumber; index2++)
						{
							float xVector = (float)Main.rand.Next(-35, 36) * 0.02f;
							float yVector = (float)Main.rand.Next(-35, 36) * 0.02f;
							xVector *= 10f;
							yVector *= 10f;
							Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, xVector, yVector, ModContent.ProjectileType<VileFeederProjectile>(), (int)(VileFeeder.BaseDamage * player.MinionDamage() * 2f), projectile.knockBack, projectile.owner, 0f, 0f);
						}
						eaterCooldown = 60;
					}
				}
			}
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			Player player = Main.player[projectile.owner];
            Rectangle myRect = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
					NPC npc = Main.npc[i];
                    if (npc.active && !npc.dontTakeDamage && npc.defense < 9999 &&
                        ((projectile.friendly && (!npc.friendly || (npc.type == 22 && projectile.owner < Main.maxPlayers && player.killGuide) || (npc.type == 54 && projectile.owner < Main.maxPlayers && player.killClothier))) && (projectile.owner < 0 || npc.immune[projectile.owner] == 0 || projectile.maxPenetrate == 1)))
                    {
                        if (npc.noTileCollide || !projectile.ownerHitCheck || projectile.CanHit(npc))
                        {
                            bool flag3;
                            if (npc.type == 414)
                            {
                                Rectangle rect = npc.getRect();
                                int num5 = 8;
                                rect.X -= num5;
                                rect.Y -= num5;
                                rect.Width += num5 * 2;
                                rect.Height += num5 * 2;
                                flag3 = projectile.Colliding(myRect, rect);
                            }
                            else
                            {
                                flag3 = projectile.Colliding(myRect, npc.getRect());
                            }
                            if (flag3)
                            {
                                if (npc.reflectingProjectiles && projectile.CanReflect())
                                {
                                    npc.ReflectProjectile(projectile.whoAmI);
                                    return;
                                }
                                projectile.ai[0] = 3f;
                                projectile.ai[1] = (float)i;
                                projectile.velocity = (npc.Center - projectile.Center) * 0.75f;
                                projectile.netUpdate = true;
                                projectile.StatusNPC(i);
                                projectile.damage = 0;
                                int num28 = 10;
                                Point[] array2 = new Point[num28];
                                int num29 = 0;
                                for (int l = 0; l < Main.maxProjectiles; l++)
                                {
									Projectile proj = Main.projectile[l];
                                    if (l != projectile.whoAmI && proj.active && proj.owner == Main.myPlayer && proj.type == projectile.type && proj.ai[0] == 3f && proj.ai[1] == (float)i)
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
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = Main.projectileTexture[projectile.type];
            int num214 = texture.Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)num214 / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 0;
        }
    }
}
