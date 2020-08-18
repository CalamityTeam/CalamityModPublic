using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
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

namespace CalamityMod.NPCs.Bumblebirb
{
    [AutoloadBossHead]
    public class Bumblefuck : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Dragonfolly");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override string Texture => "CalamityMod/NPCs/Bumblebirb/Birb";
        public override string BossHeadTexture => "CalamityMod/NPCs/Bumblebirb/Birb_Head_Boss";

        public override void SetDefaults()
        {
            npc.npcSlots = 32f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 160;
            npc.width = 130;
            npc.height = 100;
            npc.defense = 40;
			npc.DR_NERD(0.1f, null, null, null, true);
			CalamityGlobalNPC global = npc.Calamity();
			global.multDRReductions.Add(BuffID.CursedInferno, 0.9f);
			npc.LifeMaxNERB(227500, 252500, 3000000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.DryadsWardDebuff] = false;
			npc.buffImmune[BuffID.Oiled] = false;
			npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[BuffID.BetsysCurse] = false;
            npc.buffImmune[ModContent.BuffType<ExoFreeze>()] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WarCleave>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.boss = true;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Murderswarm");
            else
                music = MusicID.Boss4;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.value = Item.buyPrice(0, 30, 0, 0);
            npc.HitSound = SoundID.NPCHit51;
            npc.DeathSound = SoundID.NPCDeath46;
            bossBag = ModContent.ItemType<BumblebirbBag>();
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[2]);
			writer.Write(npc.localAI[3]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[2] = reader.ReadSingle();
			npc.localAI[3] = reader.ReadSingle();
		}

		public override void AI()
        {
			CalamityAI.BumblebirbAI(npc, mod);
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Phases
			bool phase2 = lifeRatio < (revenge ? 0.75f : 0.5f) || death;
			bool phase3 = lifeRatio < (death ? 0.4f : revenge ? 0.25f : 0.1f);
			bool birbSpawn = npc.ai[0] == 4f && npc.ai[1] > 0f;

			float newPhaseTimer = 180f;
			bool phaseSwitchPhase = (phase2 && calamityGlobalNPC.newAI[0] < newPhaseTimer && calamityGlobalNPC.newAI[2] != 1f) ||
				(phase3 && calamityGlobalNPC.newAI[1] < newPhaseTimer && calamityGlobalNPC.newAI[3] != 1f);

			if (phaseSwitchPhase || birbSpawn)
			{
				float frameGateValue = birbSpawn ? npc.ai[1] : phase3 ? calamityGlobalNPC.newAI[1] : calamityGlobalNPC.newAI[0];
				int num116 = 180;
				if (frameGateValue < (num116 - 60) || frameGateValue > (num116 - 20))
				{
					npc.frameCounter += 1D;
					if (npc.frameCounter > 5D)
					{
						npc.frameCounter = 0D;
						npc.frame.Y += frameHeight;
					}
					if (npc.frame.Y >= frameHeight * 5)
					{
						npc.frame.Y = 0;
					}
				}
				else
				{
					npc.frame.Y = frameHeight * 4;
					if (frameGateValue > (num116 - 50) && frameGateValue < (num116 - 25))
					{
						npc.frame.Y = frameHeight * 5;
					}
				}
			}
			else if (npc.ai[0] == 5f)
			{
				int num115 = 120;
				if (npc.ai[1] < (num115 - 50) || npc.ai[1] > (num115 - 10))
				{
					npc.frameCounter += 1D;
					if (npc.frameCounter > 5D)
					{
						npc.frameCounter = 0D;
						npc.frame.Y += frameHeight;
					}
					if (npc.frame.Y >= frameHeight * 5)
					{
						npc.frame.Y = 0;
					}
				}
				else
				{
					npc.frame.Y = frameHeight * 4;
					if (npc.ai[1] > (num115 - 40) && npc.ai[1] < (num115 - 15))
					{
						npc.frame.Y = frameHeight * 5;
					}
				}
			}
            else
            {
				npc.frameCounter += (npc.ai[0] == 3.2f ? 1.5 : 1D);
				if (npc.frameCounter > 4D) //iban said the time between frames was 5 so using that as a base
                {
					npc.frameCounter = 0D;
					npc.frame.Y += frameHeight;
                }
				if (npc.frame.Y >= frameHeight * 5)
				{
					npc.frame.Y = 0;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;

			// Percent life remaining
			float lifeRatio = (float)npc.life / (float)npc.lifeMax;

			// Phases
			bool phase2 = lifeRatio < (revenge ? 0.75f : 0.5f) || death;
			bool phase3 = lifeRatio < (death ? 0.4f : revenge ? 0.25f : 0.1f);

			float newPhaseTimer = 180f;
			bool phaseSwitchPhase = (phase2 && calamityGlobalNPC.newAI[0] < newPhaseTimer && calamityGlobalNPC.newAI[2] != 1f) ||
				(phase3 && calamityGlobalNPC.newAI[1] < newPhaseTimer && calamityGlobalNPC.newAI[3] != 1f);

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			Color color = lightColor;
			Color color36 = Color.White;

			float amount9 = 0f;
			int num150 = 120;
			int num151 = 60;

			if (phase3 && calamityGlobalNPC.newAI[3] == 1f)
			{
				color = CalamityGlobalNPC.buffColor(color, 0.9f, 0.6f, 0.2f, 1f);
			}
			else if (phase2 && calamityGlobalNPC.newAI[2] == 1f)
			{
				color = CalamityGlobalNPC.buffColor(color, 0.7f, 0.7f, 0.3f, 1f);
			}
			else if (phase2 && calamityGlobalNPC.newAI[0] > (float)num150)
			{
				float num152 = calamityGlobalNPC.newAI[0] - (float)num150;
				num152 /= (float)num151;
				color = CalamityGlobalNPC.buffColor(color, 1f - 0.3f * num152, 1f - 0.3f * num152, 1f - 0.7f * num152, 1f);
			}

			int num153 = 10;
			int num154 = 2;
			if (npc.ai[0] == 0f || npc.ai[0] == 3.1f || npc.ai[0] == 4f || npc.ai[0] == 4.2f)
			{
				num153 = 4;
			}
			if (npc.ai[0] == 1f || npc.ai[0] == 3f || npc.ai[0] == 4.1f)
			{
				num153 = 7;
			}
			if (npc.ai[0] == 2f || npc.ai[0] == 3.2f || (phase2 && calamityGlobalNPC.newAI[2] == 1f))
			{
				color36 = Color.Yellow;
				amount9 = 0.5f;
			}
			else
			{
				color = lightColor;
			}

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += num154)
				{
					Color color38 = color;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (float)(num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			int num156 = 0;
			float num157 = 0f;
			float scaleFactor9 = 0f;

			if (npc.ai[0] == 0f || npc.ai[0] == 3.1f || npc.ai[0] == 4f || npc.ai[0] == 4.2f)
			{
				num156 = 4;
			}

			if (npc.ai[0] == 5f)
			{
				int num158 = 60;
				int num159 = 30;
				if (npc.ai[1] > (float)num158)
				{
					num156 = 6;
					num157 = 1f - (float)Math.Cos((double)((npc.ai[1] - (float)num158) / (float)num159 * MathHelper.TwoPi));
					num157 /= 3f;
					scaleFactor9 = 40f;
				}
			}

			if (phaseSwitchPhase)
			{
				if (phase3 && calamityGlobalNPC.newAI[1] > (float)num150)
				{
					num156 = 6;
					num157 = 1f - (float)Math.Cos((double)((calamityGlobalNPC.newAI[1] - (float)num150) / (float)num151 * MathHelper.TwoPi));
					num157 /= 3f;
					scaleFactor9 = 60f;
				}
				else if (phase2 && calamityGlobalNPC.newAI[0] > (float)num150)
				{
					num156 = 6;
					num157 = 1f - (float)Math.Cos((double)((calamityGlobalNPC.newAI[0] - (float)num150) / (float)num151 * MathHelper.TwoPi));
					num157 /= 3f;
					scaleFactor9 = 60f;
				}
			}

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num160 = 0; num160 < num156; num160++)
				{
					Color color39 = lightColor;
					color39 = Color.Lerp(color39, color36, amount9);
					color39 = npc.GetAlpha(color39);
					color39 *= 1f - num157;
					Vector2 vector42 = npc.Center + ((float)num160 / (float)num156 * MathHelper.TwoPi + npc.rotation).ToRotationVector2() * scaleFactor9 * num157 - Main.screenPosition;
					vector42 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector42 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector42, npc.frame, color39, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Color color2 = lightColor;
			color2 = Color.Lerp(color2, color36, amount9);
			color2 = npc.GetAlpha(color2);
			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, (phase3 && calamityGlobalNPC.newAI[3] == 1f ? color2 : npc.GetAlpha(lightColor)), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			if (phase2)
			{
				texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Bumblebirb/BirbGlow");
				Color color40 = Color.Lerp(Color.White, Color.Red, 0.5f);
				color36 = Color.Red;

				amount9 = 1f;
				num157 = 0.5f;
				scaleFactor9 = 10f;
				num154 = 1;

				if (phaseSwitchPhase)
				{
					float num161 = (phase3 ? calamityGlobalNPC.newAI[1] : calamityGlobalNPC.newAI[0]) - (float)num150;
					num161 /= (float)num151;
					color36 *= num161;
					color40 *= num161;
				}

				if (CalamityConfig.Instance.Afterimages)
				{
					for (int num163 = 1; num163 < num153; num163 += num154)
					{
						Color color41 = color40;
						color41 = Color.Lerp(color41, color36, amount9);
						color41 *= (float)(num153 - num163) / 15f;
						Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
						vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
						vector44 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
						spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
					}

					for (int num164 = 1; num164 < num156; num164++)
					{
						Color color42 = color40;
						color42 = Color.Lerp(color42, color36, amount9);
						color42 = npc.GetAlpha(color42);
						color42 *= 1f - num157;
						Vector2 vector45 = npc.Center + ((float)num164 / (float)num156 * MathHelper.TwoPi + npc.rotation).ToRotationVector2() * scaleFactor9 * num157 - Main.screenPosition;
						vector45 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
						vector45 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
						spriteBatch.Draw(texture2D15, vector45, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
					}
				}

				spriteBatch.Draw(texture2D15, vector43, npc.frame, color40, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
			}

			return false;
		}

		private static Color buffColor(Color newColor, float R, float G, float B, float A)
		{
			newColor.R = (byte)((float)newColor.R * R);
			newColor.G = (byte)((float)newColor.G * G);
			newColor.B = (byte)((float)newColor.B * B);
			newColor.A = (byte)((float)newColor.A * A);
			return newColor;
		}

		public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A Dragonfolly";
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<BumblebirbTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeBumblebirb>(), true, !CalamityWorld.downedBumble);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedBumble, 5, 2, 1);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.WitchDoctor }, CalamityWorld.downedBumble);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<EffulgentFeather>(), 11, 17);

				// Weapons
				float w = DropHelper.DirectWeaponDropRateFloat;
				DropHelper.DropEntireWeightedSet(npc,
					DropHelper.WeightStack<GildedProboscis>(w),
					DropHelper.WeightStack<GoldenEagle>(w),
					DropHelper.WeightStack<RougeSlash>(w)
				);

				DropHelper.DropItemChance(npc, ModContent.ItemType<Swordsplosion>(), DropHelper.RareVariantDropRateInt);

                // Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<BirdSeed>(), 4);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<BumblefuckMask>(), 7);
            }

            // Mark The Dragonfolly as dead
            CalamityWorld.downedBumble = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
                }
                for (int i = 0; i < 6; i++) // 1 head, 1 wing, 4 legs = 6. one wing due to them being chonky boyes now
                {
                    string gore = "Gores/Bumble";
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    gore += i == 0 ? "Head" : i > 1 ? "Leg" : "Wing";
                    Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot(gore), 1f);
                }
            }
        }
    }
}
