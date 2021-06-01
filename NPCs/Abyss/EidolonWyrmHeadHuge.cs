using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class EidolonWyrmHeadHuge : ModNPC
    {
        public const int minLength = 40;
        public const int maxLength = 41;
        public float speed = 7.5f;
        public float turnSpeed = 0.15f;
        bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eidolon Wyrm");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 50f;
			npc.GetNPCDamage();
			npc.width = 254;
            npc.height = 138;
            npc.defense = 100;
			npc.DR_NERD(0.4f);
			CalamityGlobalNPC global = npc.Calamity();
			global.multDRReductions.Add(BuffID.CursedInferno, 0.9f);
			npc.LifeMaxNERB(1000000, 1150000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(10, 0, 0, 0);
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath6;
            npc.netAlways = true;
			npc.boss = true;
			music = CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ?? MusicID.Boss3;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			bool malice = CalamityWorld.malice;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive || malice;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive || malice;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive || malice;

			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Increase aggression if player is taking a long time to kill the boss
			if (lifeRatio > calamityGlobalNPC.killTimeRatio_IncreasedAggression)
				lifeRatio = calamityGlobalNPC.killTimeRatio_IncreasedAggression;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];
			Vector2 vectorCenter = npc.Center;

			//Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/EidolonWyrmRoarClose").WithVolume(2.5f), (int)npc.position.X, (int)npc.position.Y);

			if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

			if (!TailSpawned && npc.ai[0] == 0f)
			{
				if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, npc.Center) < 2800f)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Scare"),
						(int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
				}
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!TailSpawned && npc.ai[0] == 0f)
				{
					int Previous = npc.whoAmI;
					for (int num36 = 0; num36 < maxLength; num36++)
					{
						int lol;
						if (num36 >= 0 && num36 < minLength)
						{
							if (num36 % 2 == 0)
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<EidolonWyrmBodyHuge>(), npc.whoAmI);
							else
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<EidolonWyrmBodyAltHuge>(), npc.whoAmI);
						}
						else
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<EidolonWyrmTailHuge>(), npc.whoAmI);

						Main.npc[lol].realLife = npc.whoAmI;
						Main.npc[lol].ai[2] = (float)npc.whoAmI;
						Main.npc[lol].ai[1] = (float)Previous;
						Main.npc[Previous].ai[0] = (float)lol;
						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
						Previous = lol;
					}
					TailSpawned = true;
				}

				npc.localAI[0] += 1f;
				if (npc.localAI[0] >= 300f)
				{
					npc.localAI[0] = 0f;
					int damage = Main.expertMode ? 300 : 400;
					float xPos = Main.rand.NextBool(2) ? npc.position.X + 200f : npc.position.X - 200f;
					Vector2 vector2 = new Vector2(xPos, npc.position.Y + Main.rand.Next(-200, 201));
					int random = Main.rand.Next(3);
					if (random == 0)
					{
						Projectile.NewProjectile(vector2.X, vector2.Y, 0f, 0f, ProjectileID.CultistBossLightningOrb, damage, 0f, Main.myPlayer, 0f, 0f);
						Projectile.NewProjectile(-vector2.X, -vector2.Y, 0f, 0f, ProjectileID.CultistBossLightningOrb, damage, 0f, Main.myPlayer, 0f, 0f);
					}
					else if (random == 1)
					{
						Vector2 vec = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
						vec = Vector2.Normalize(Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 20f);
						if (vec.HasNaNs())
						{
							vec = new Vector2((float)npc.direction, 0f);
						}
						for (int n = 0; n < 1; n++)
						{
							Vector2 vector4 = vec * 4f;
							Projectile.NewProjectile(vector2.X, vector2.Y, vector4.X, vector4.Y, ProjectileID.CultistBossIceMist, damage, 0f, Main.myPlayer, 0f, 1f);
							Projectile.NewProjectile(-vector2.X, -vector2.Y, -vector4.X, -vector4.Y, ProjectileID.CultistBossIceMist, damage, 0f, Main.myPlayer, 0f, 1f);
						}
					}
					else
					{
						if (Math.Abs(Main.player[npc.target].velocity.X) > 0.1f || Math.Abs(Main.player[npc.target].velocity.Y) > 0.1f)
						{
							Main.PlaySound(SoundID.Item117, Main.player[npc.target].position);
							for (int num621 = 0; num621 < 20; num621++)
							{
								int num622 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X, Main.player[npc.target].position.Y),
									Main.player[npc.target].width, Main.player[npc.target].height, 185, 0f, 0f, 100, default, 2f);
								Main.dust[num622].velocity *= 0.6f;
								if (Main.rand.NextBool(2))
								{
									Main.dust[num622].scale = 0.5f;
									Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
								}
							}
							for (int num623 = 0; num623 < 30; num623++)
							{
								int num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X, Main.player[npc.target].position.Y),
									Main.player[npc.target].width, Main.player[npc.target].height, 185, 0f, 0f, 100, default, 3f);
								Main.dust[num624].noGravity = true;
								num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X, Main.player[npc.target].position.Y),
									Main.player[npc.target].width, Main.player[npc.target].height, 185, 0f, 0f, 100, default, 2f);
								Main.dust[num624].velocity *= 0.2f;
							}
							if (Math.Abs(Main.player[npc.target].velocity.X) > 0.1f)
							{
								Main.player[npc.target].velocity.X = -Main.player[npc.target].velocity.X * 2f;
							}
							if (Math.Abs(Main.player[npc.target].velocity.Y) > 0.1f)
							{
								Main.player[npc.target].velocity.Y = -Main.player[npc.target].velocity.Y * 2f;
							}
						}
					}
				}
			}

            if (npc.velocity.X < 0f)
                npc.spriteDirection = -1;
            else if (npc.velocity.X > 0f)
                npc.spriteDirection = 1;

            if (Main.player[npc.target].dead)
            {
                npc.TargetClosest(false);

				npc.velocity.Y += 3f;
				if (npc.position.Y > Main.worldSurface * 16.0)
					npc.velocity.Y += 3f;

				if (npc.position.Y > (Main.maxTilesY - 200) * 16.0)
				{
					for (int a = 0; a < Main.maxNPCs; a++)
					{
						if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<EidolonWyrmBodyAltHuge>() || Main.npc[a].type == ModContent.NPCType<EidolonWyrmBodyHuge>() || Main.npc[a].type == ModContent.NPCType<EidolonWyrmTailHuge>())
							Main.npc[a].active = false;
					}
				}
			}

            npc.alpha -= 42;
            if (npc.alpha < 0)
                npc.alpha = 0;

			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 6400f || !NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmTailHuge>()))
                npc.active = false;

            float num188 = speed;
            float num189 = turnSpeed;
			Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);

			float num191 = Main.player[npc.target].Center.X;
			float num192 = Main.player[npc.target].Center.Y;

            num188 = !Main.player[npc.target].wet ? 20f : 10f;
            num189 = !Main.player[npc.target].wet ? 0.25f : 0.175f;
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
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            float num196 = System.Math.Abs(num191);
            float num197 = System.Math.Abs(num192);
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
                if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
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
                if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
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
                        npc.velocity.X = npc.velocity.X + num189 * 1.1f;
                    }
                    else if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 1.1f;
                    }
                    if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
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
                    if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
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

            npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + MathHelper.PiOver2;

			if (calamityGlobalNPC.newAI[1] == 0f)
			{
				// Start a storm
				if (Main.netMode == NetmodeID.MultiplayerClient || (Main.netMode == NetmodeID.SinglePlayer && Main.gameMenu))
					return;

				CalamityUtils.StartRain(true, true);
				calamityGlobalNPC.newAI[1] = 1f;
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;

			Rectangle targetHitbox = target.Hitbox;

			float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist)
				minDist = dist2;
			if (dist3 < minDist)
				minDist = dist3;
			if (dist4 < minDist)
				minDist = dist4;

			return minDist <= 70f;
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			return !CalamityUtils.AntiButcher(npc, ref damage, 0.1f);
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;
			return null;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlowHuge").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlowHuge").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightYellow);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlowHuge"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<Voidstone>(), 80, 100);
            DropHelper.DropItem(npc, ModContent.ItemType<EidolicWail>());
            DropHelper.DropItem(npc, ModContent.ItemType<SoulEdge>());
            DropHelper.DropItem(npc, ModContent.ItemType<HalibutCannon>());

            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas, 1, 50, 108);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas && Main.expertMode, 2, 15, 27);
            DropHelper.DropItemCondition(npc, ItemID.Ectoplasm, NPC.downedPlantBoss, 1, 21, 32);
        }

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
				Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);

			if (npc.life <= 0)
			{
				for (int k = 0; k < 15; k++)
					Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/WyrmAdult"), 1f);
			}
		}

		public override bool CheckActive() => false;

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<CrushDepth>(), 600, true);
        }
    }
}
