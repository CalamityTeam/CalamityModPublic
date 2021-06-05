using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class SCalWormHead : ModNPC
    {
        private const int minLength = 51;
        private const int maxLength = 52;
        private float passedVar = 0f;
        private bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sepulcher");
        }

        public override void SetDefaults()
        {
            npc.damage = 0; //150
            npc.npcSlots = 5f;
            npc.width = 62; //324
            npc.height = 78; //216
            npc.defense = 0;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
			npc.lifeMax = CalamityWorld.revenge ? 345000 : 300000;
			npc.aiStyle = -1; //new
            aiType = -1; //new
            npc.knockBackResist = 0f;
            npc.scale = 1.2f;
            if (Main.expertMode)
            {
                npc.scale = 1.35f;
            }
            npc.alpha = 255;
            npc.chaseable = false;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
			writer.Write(npc.alpha);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
			npc.alpha = reader.ReadInt32();
		}

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            CalamityGlobalNPC.SCalWorm = npc.whoAmI;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			if (npc.ai[2] > 0f)
			{
				npc.realLife = (int)npc.ai[2];
			}

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int num36 = 0; num36 < maxLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < minLength && num36 % 2 == 0)
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<SCalWormBodyWeak>(), npc.whoAmI);
                            Main.npc[lol].localAI[0] += passedVar;
                            passedVar += 36f;
                        }
                        else if (num36 >= 0 && num36 < minLength)
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<SCalWormBody>(), npc.whoAmI);
                            if (npc.localAI[0] % 2 == 0)
                            {
                                Main.npc[lol].localAI[3] = 1f;
                                npc.localAI[0] = 1f;
                            }
                            else
                            {
                                npc.localAI[0] = 2f;
                            }
                        }
                        else
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<SCalWormTail>(), npc.whoAmI);
                        }
                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[2] = npc.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
                if (!npc.active && Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
            }

            if (Main.player[npc.target].dead || !NPC.AnyNPCs(ModContent.NPCType<SCalWormHeart>()) || CalamityGlobalNPC.SCal < 0 || !Main.npc[CalamityGlobalNPC.SCal].active)
            {
                npc.TargetClosest(false);
                npc.alpha += 5;
                if (npc.alpha >= 255)
                {
                    npc.alpha = 255;
					for (int i = 0; i < Main.maxNPCs; i++)
					{
						if (Main.npc[i].type == npc.type || Main.npc[i].type == ModContent.NPCType<SCalWormBody>() || Main.npc[i].type == ModContent.NPCType<SCalWormBodyWeak>() || Main.npc[i].type == ModContent.NPCType<SCalWormTail>())
						{
							Main.npc[i].active = false;
						}
					}
				}
            }
            else
            {
                npc.alpha -= 42;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
            }

			Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
			float num191 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2);
			float num192 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2);
			float num188 = CalamityWorld.malice ? 12.5f : 10f;
			float num189 = CalamityWorld.malice ? 0.125f : 0.1f;

			float num48 = num188 * 1.3f;
			float num49 = num188 * 0.7f;
			float num50 = npc.velocity.Length();
			if (num50 > 0f)
			{
				if (num50 > num48)
				{
					npc.velocity.Normalize();
					npc.velocity *= num48;
				}
				else if (num50 < num49)
				{
					npc.velocity.Normalize();
					npc.velocity *= num49;
				}
			}

			num191 = (int)(num191 / 16f) * 16;
			num192 = (int)(num192 / 16f) * 16;
			vector18.X = (int)(vector18.X / 16f) * 16;
			vector18.Y = (int)(vector18.Y / 16f) * 16;
			num191 -= vector18.X;
			num192 -= vector18.Y;
			float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
			float num196 = Math.Abs(num191);
			float num197 = Math.Abs(num192);
			float num198 = num188 / num193;
			num191 *= num198;
			num192 *= num198;
			if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
			{
				if (npc.velocity.X < num191)
				{
					npc.velocity.X = npc.velocity.X + num189;
				}
				else
				{
					if (npc.velocity.X > num191)
					{
						npc.velocity.X = npc.velocity.X - num189;
					}
				}
				if (npc.velocity.Y < num192)
				{
					npc.velocity.Y = npc.velocity.Y + num189;
				}
				else
				{
					if (npc.velocity.Y > num192)
					{
						npc.velocity.Y = npc.velocity.Y - num189;
					}
				}
				if (Math.Abs(num192) < num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
				{
					if (npc.velocity.Y > 0f)
					{
						npc.velocity.Y = npc.velocity.Y + num189 * 2f;
					}
					else
					{
						npc.velocity.Y = npc.velocity.Y - num189 * 2f;
					}
				}
				if (Math.Abs(num191) < num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
				{
					if (npc.velocity.X > 0f)
					{
						npc.velocity.X = npc.velocity.X + num189 * 2f; //changed from 2
					}
					else
					{
						npc.velocity.X = npc.velocity.X - num189 * 2f; //changed from 2
					}
				}
			}
			else
			{
				if (num196 > num197)
				{
					if (npc.velocity.X < num191)
					{
						npc.velocity.X = npc.velocity.X + num189 * 1.1f; //changed from 1.1
					}
					else if (npc.velocity.X > num191)
					{
						npc.velocity.X = npc.velocity.X - num189 * 1.1f; //changed from 1.1
					}
					if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
					{
						if (npc.velocity.Y > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num189;
						}
						else
						{
							npc.velocity.Y = npc.velocity.Y - num189;
						}
					}
				}
				else
				{
					if (npc.velocity.Y < num192)
					{
						npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
					}
					else if (npc.velocity.Y > num192)
					{
						npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
					}
					if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
					{
						if (npc.velocity.X > 0f)
						{
							npc.velocity.X = npc.velocity.X + num189;
						}
						else
						{
							npc.velocity.X = npc.velocity.X - num189;
						}
					}
				}
			}
			npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / 2));

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/SupremeCalamitas/SCalWormHeadGlow");
			Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
			if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => projectile.type != x))
			{
				if (projectile.penetrate == -1 && !projectile.minion)
				{
					projectile.penetrate = 1;
				}
				else if (projectile.penetrate >= 1)
				{
					projectile.penetrate = 1;
				}
			}
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
