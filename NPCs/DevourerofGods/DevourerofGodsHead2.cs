using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    public class DevourerofGodsHead2 : ModNPC
    {
        private bool tail = false;
        private const int minLength = 10;
        private const int maxLength = 11;
        private int invinceTime = 180;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Guardian");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
            npc.width = 64;
            npc.height = 76;
            npc.defense = 40;
            npc.lifeMax = 50000;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
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
            writer.Write(invinceTime);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            invinceTime = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
			// Target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			Player player = Main.player[npc.target];

            if (invinceTime > 0)
            {
                invinceTime--;
                npc.damage = 0;
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.damage = npc.defDamage;
                npc.dontTakeDamage = false;
            }

            Vector2 vector = npc.Center;

			bool increaseSpeed = Vector2.Distance(player.Center, vector) > CalamityGlobalNPC.CatchUpDistance200Tiles;
			bool increaseSpeedMore = Vector2.Distance(player.Center, vector) > CalamityGlobalNPC.CatchUpDistance350Tiles;

			Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

            if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

            if (npc.alpha != 0)
            {
                int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                Main.dust[num935].noGravity = true;
                Main.dust[num935].noLight = true;
            }

            npc.alpha -= 12;
            if (npc.alpha < 0)
                npc.alpha = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tail && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int segmentSpawn = 0; segmentSpawn < maxLength; segmentSpawn++)
                    {
                        int segment;
                        if (segmentSpawn >= 0 && segmentSpawn < minLength)
                        {
                            segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DevourerofGodsBody2>(), npc.whoAmI);
                        }
                        else
                        {
                            segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DevourerofGodsTail2>(), npc.whoAmI);
                        }
                        Main.npc[segment].realLife = npc.whoAmI;
                        Main.npc[segment].ai[2] = npc.whoAmI;
                        Main.npc[segment].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = segment;
                        npc.netUpdate = true;
                        Previous = segment;
                    }
                    tail = true;
                }
                if (!npc.active && Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
            }

            if (player.dead || CalamityGlobalNPC.DoGHead < 0 || !Main.npc[CalamityGlobalNPC.DoGHead].active)
            {
                npc.TargetClosest(false);
                npc.velocity.Y -= 3f;
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    npc.velocity.Y -= 3f;
                }
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<DevourerofGodsBody2>() || Main.npc[a].type == ModContent.NPCType<DevourerofGodsTail2>())
                        {
                            Main.npc[a].active = false;
                        }
                    }
                }
            }

			Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
			float num191 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2);
			float num192 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2);
			float num188 = CalamityWorld.malice ? 18f : CalamityWorld.revenge ? 16f : 14f;
			float num189 = CalamityWorld.malice ? 0.17f : CalamityWorld.revenge ? 0.15f : 0.13f;

			if (increaseSpeedMore)
				num189 *= 6f;
			else if (increaseSpeed)
				num189 *= 3f;

			for (int num52 = 0; num52 < Main.maxNPCs; num52++)
			{
				if (Main.npc[num52].active && Main.npc[num52].type == npc.type && num52 != npc.whoAmI)
				{
					Vector2 vector4 = Main.npc[num52].Center - npc.Center;
					if (vector4.Length() < 60f)
					{
						vector4.Normalize();
						vector4 *= 200f;
						num191 -= vector4.X;
						num192 -= vector4.Y;
					}
				}
			}

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
			npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;
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
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead2Glow");
			Color color37 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead2Glow2");
			color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override void NPCLoot()
		{
			if (!CalamityWorld.revenge)
			{
				int heartAmt = Main.rand.Next(3) + 3;
				for (int i = 0; i < heartAmt; i++)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DoT"), 1f);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 15; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 30; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180, true);
        }
    }
}
