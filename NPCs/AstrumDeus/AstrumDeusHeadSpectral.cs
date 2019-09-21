using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.Utilities;

namespace CalamityMod.NPCs.AstrumDeus
{
    [AutoloadBossHead]
	public class AstrumDeusHeadSpectral : ModNPC
	{
		private bool colorChange = false;
		private const int minLength = 12;
		private const int maxLength = 13;
		private float speed = 10f;
		private float turnSpeed = 0.2f;
		private bool tailSpawned = false;
		private int deusHeadCount = 0;
		private int astrumDeusTotalHPMax = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrum Deus");
		}

		public override void SetDefaults()
		{
			npc.damage = 130; //150
			npc.npcSlots = 5f;
			npc.width = 56; //324
			npc.height = 56; //216
			npc.defense = 40;
			npc.lifeMax = CalamityWorld.revenge ? 53800 : 37500;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 80700;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 1500000 : 1300000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.aiStyle = -1; //new
			aiType = -1; //new
			npc.knockBackResist = 0f;
			npc.scale = 1.2f;
			if (Main.expertMode)
			{
				npc.scale = 1.35f;
			}
			NPCID.Sets.TrailCacheLength[npc.type] = 8;
			NPCID.Sets.TrailingMode[npc.type] = 1;
			npc.boss = true;
			npc.value = Item.buyPrice(0, 20, 0, 0);
			npc.alpha = 255;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.netAlways = true;
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
			if (calamityModMusic != null)
				music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/AstrumDeus");
			else
				music = MusicID.Boss3;
			bossBag = mod.ItemType("AstrumDeusBag");
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(deusHeadCount);
			writer.Write(astrumDeusTotalHPMax);
			writer.Write(colorChange);
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.chaseable);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			deusHeadCount = reader.ReadInt32();
			astrumDeusTotalHPMax = reader.ReadInt32();
			colorChange = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.chaseable = reader.ReadBoolean();
		}

		public override void AI()
		{
			CalamityGlobalNPC.astrumDeusHeadMain = npc.whoAmI;
			colorChange = NPC.AnyNPCs(mod.NPCType("AstrumDeusHead"));
			int astrumDeusTotalHP = 0;
			double astrumDeusHPRatio = 0;
			double mainDeusHPRatio = (double)npc.life / (double)npc.lifeMax;
			if (colorChange)
			{
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].type == mod.NPCType("AstrumDeusHead"))
					{
						astrumDeusTotalHP += Main.npc[i].life;
						if (deusHeadCount < 10)
						{
							deusHeadCount++;
							astrumDeusTotalHPMax += Main.npc[i].lifeMax;
						}
					}
				}
				astrumDeusHPRatio = (double)astrumDeusTotalHP / (double)astrumDeusTotalHPMax;
				if (astrumDeusHPRatio < 0.33)
				{
					if (mainDeusHPRatio > 0.33)
						colorChange = false;
				}
				else if (astrumDeusHPRatio < 0.66)
				{
					if (mainDeusHPRatio > 0.66)
						colorChange = false;
				}
			}
			npc.dontTakeDamage = colorChange;
			npc.chaseable = !colorChange;
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			float speedLimit = CalamityWorld.revenge ? 7f : 6f;
			float turnSpeedLimit = CalamityWorld.revenge ? 0.11f : 0.1f;
			if (CalamityWorld.death || CalamityWorld.bossRushActive)
			{
				speedLimit = ((npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 15f : 8f);
				turnSpeedLimit = ((npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 0.2f : 0.12f);
			}
			float speedBoost = speedLimit * (1f - (float)mainDeusHPRatio);
			float turnSpeedBoost = turnSpeedLimit * (1f - (float)mainDeusHPRatio);
			if (Main.player[npc.target].gravDir == -1f)
			{
				speedBoost = speedLimit;
				turnSpeedBoost = turnSpeedLimit;
			}
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
			if (npc.ai[3] > 0f)
			{
				npc.realLife = (int)npc.ai[3];
			}
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			npc.velocity.Length();
			if (npc.alpha != 0)
			{
				for (int num934 = 0; num934 < 2; num934++)
				{
					int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
					Main.dust[num935].noGravity = true;
					Main.dust[num935].noLight = true;
				}
			}
			npc.alpha -= 42;
			if (npc.alpha < 0)
			{
				npc.alpha = 0;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!tailSpawned && npc.ai[0] == 0f)
				{
					int Previous = npc.whoAmI;
					for (int num36 = 0; num36 < maxLength; num36++)
					{
						int lol = 0;
						if (num36 >= 0 && num36 < minLength)
						{
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AstrumDeusBodySpectral"), npc.whoAmI);
						}
						else
						{
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AstrumDeusTailSpectral"), npc.whoAmI);
						}
						if (num36 % 2 == 0)
						{
							Main.npc[lol].localAI[3] = 1f;
						}
						Main.npc[lol].realLife = npc.whoAmI;
						Main.npc[lol].ai[2] = (float)npc.whoAmI;
						Main.npc[lol].ai[1] = (float)Previous;
						Main.npc[Previous].ai[0] = (float)lol;
						NetMessage.SendData(23, -1, -1, null, lol, 0f, 0f, 0f, 0);
						Previous = lol;
					}
					tailSpawned = true;
				}
			}
			if (Main.player[npc.target].dead || Main.dayTime)
			{
				npc.TargetClosest(false);
				npc.velocity.Y = npc.velocity.Y - 3f;
				if ((double)npc.position.Y < Main.topWorld + 16f)
				{
					npc.velocity.Y = npc.velocity.Y - 3f;
				}
				if ((double)npc.position.Y < Main.topWorld + 16f)
				{
					for (int num957 = 0; num957 < 200; num957++)
					{
						if (Main.npc[num957].aiStyle == npc.aiStyle)
						{
							Main.npc[num957].active = false;
						}
					}
				}
			}
			if (npc.velocity.X < 0f)
			{
				npc.spriteDirection = -1;
			}
			else if (npc.velocity.X > 0f)
			{
				npc.spriteDirection = 1;
			}
			if (Main.player[npc.target].dead)
			{
				npc.TargetClosest(false);
			}
			float num188 = speed + speedBoost;
			float num189 = turnSpeed + turnSpeedBoost;
			Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
			float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
			int num42 = -1;
			int num43 = (int)(Main.player[npc.target].Center.X / 16f);
			int num44 = (int)(Main.player[npc.target].Center.Y / 16f);
			for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
			{
				for (int num46 = num44; num46 <= num44 + 15; num46++)
				{
					if (WorldGen.SolidTile2(num45, num46))
					{
						num42 = num46;
						break;
					}
				}
				if (num42 > 0)
				{
					break;
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
			for (int num52 = 0; num52 < 200; num52++)
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
						npc.velocity.X = npc.velocity.X + num189 * 1.1f; //changed from 1.1
					}
					else if (npc.velocity.X > num191)
					{
						npc.velocity.X = npc.velocity.X - num189 * 1.1f; //changed from 1.1
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
			npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Color lightColor = new Color(250, 150, Main.DiscoB, npc.alpha);
			Color newColor = (colorChange ? lightColor : drawColor);
			SpriteEffects spriteEffects = SpriteEffects.None;
			Microsoft.Xna.Framework.Color color24 = npc.GetAlpha(newColor);
			Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
			Texture2D texture2D3 = Main.npcTexture[npc.type];
			int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
			int y3 = num156 * (int)npc.frameCounter;
			Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, y3, texture2D3.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			int num157 = 8;
			int num158 = 2;
			int num159 = 1;
			float num160 = 0f;
			int num161 = num159;
			while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && Lighting.NotRetro)
			{
				Microsoft.Xna.Framework.Color color26 = npc.GetAlpha(color25);
				{
					goto IL_6899;
				}
			IL_6881:
				num161 += num158;
				continue;
			IL_6899:
				float num164 = (float)(num157 - num161);
				if (num158 < 0)
				{
					num164 = (float)(num159 - num161);
				}
				color26 *= num164 / ((float)NPCID.Sets.TrailCacheLength[npc.type] * 1.5f);
				Vector2 value4 = (npc.oldPos[num161]);
				float num165 = npc.rotation;
				Main.spriteBatch.Draw(texture2D3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects, 0f);
				goto IL_6881;
			}
			var something = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(texture2D3, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, something, 0);
			return false;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			if (colorChange && Main.player[npc.target].gravDir != -1f)
			{
				return false;
			}
			return true;
		}

		public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (projectile.type == mod.ProjectileType("RainbowBoom") || ProjectileID.Sets.StardustDragon[projectile.type])
			{
				damage = (int)((double)damage * 0.1);
			}
			else if (projectile.type == mod.ProjectileType("RainBolt") || projectile.type == mod.ProjectileType("AtlantisSpear2"))
			{
				damage = (int)((double)damage * 0.2);
			}
			else if (projectile.type == ProjectileID.DD2BetsyArrow || projectile.type == mod.ProjectileType("CraniumSmasherExplosive") || projectile.type == mod.ProjectileType("BigNukeExplosion"))
			{
				damage = (int)((double)damage * 0.3);
			}
			else if (projectile.type == mod.ProjectileType("SpikecragSpike"))
			{
				damage = (int)((double)damage * 0.5);
			}
			else if (projectile.type == mod.ProjectileType("GoliathExplosion"))
			{
				damage = (int)((double)damage * 0.6);
			}
			if (projectile.penetrate == -1 && !projectile.minion)
			{
				if (projectile.type == mod.ProjectileType("CosmicFire"))
					damage = (int)((double)damage * 0.3);
				else
					damage = (int)((double)damage * 0.2);
			}
			else if (projectile.penetrate > 1 && projectile.type != mod.ProjectileType("BrinySpout"))
			{
				damage /= projectile.penetrate;
			}
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
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = mod.ItemType("Stardust");
		}

		public override bool SpecialNPCLoot()
		{
			int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
				mod.NPCType("AstrumDeusHeadSpectral"),
				mod.NPCType("AstrumDeusBodySpectral"),
				mod.NPCType("AstrumDeusTailSpectral"));
			npc.position = Main.npc[closestSegmentID].position;
            return false;
		}

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItem(npc, ItemID.GreaterHealingPotion, 8, 14);
            DropHelper.DropItemChance(npc, mod.ItemType("AstrumDeusTrophy"), 10);
            DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeAstrumDeus"), !CalamityWorld.downedStarGod);
            DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeAstralInfection"), !CalamityWorld.downedStarGod);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedStarGod, 4, 2, 1);

            // Drop a large spray of all 4 lunar fragments
            int minFragments = Main.expertMode ? 20 : 12;
            int maxFragments = Main.expertMode ? 32 : 20;
            DropHelper.DropItemSpray(npc, ItemID.FragmentSolar, minFragments, maxFragments);
            DropHelper.DropItemSpray(npc, ItemID.FragmentVortex, minFragments, maxFragments);
            DropHelper.DropItemSpray(npc, ItemID.FragmentNebula, minFragments, maxFragments);
            DropHelper.DropItemSpray(npc, ItemID.FragmentStardust, minFragments, maxFragments);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                DropHelper.DropItemSpray(npc, mod.ItemType("Stardust"), 50, 80, 5);

                // Weapons
                DropHelper.DropItemChance(npc, mod.ItemType("Starfall"), 5);
                DropHelper.DropItemChance(npc, mod.ItemType("Quasar"), DropHelper.RareVariantDropRateInt);

                // Equipment
                DropHelper.DropItemChance(npc, mod.ItemType("HideofAstrumDeus"), DropHelper.RareVariantDropRateInt);

                // Vanity
                DropHelper.DropItemChance(npc, mod.ItemType("AstrumDeusMask"), 7);
            }

            // Notify players that Astral Ore can be mined if Deus has never been killed yet
            if (!CalamityWorld.downedStarGod)
            {
                string key = "Mods.CalamityMod.AstralBossText";
                Color messageColor = Color.Gold;
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(key), messageColor);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }

            // Mark Astrum Deus as dead
            CalamityWorld.downedStarGod = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("AstralInfectionDebuff"), 240, true);
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
	}
}
