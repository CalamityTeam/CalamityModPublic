using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.Astrageldon
{
    [AutoloadBossHead]
	public class Astrageldon : ModNPC
	{
		private bool stomping = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrum Aureus");
			Main.npcFrameCount[npc.type] = 6;
		}

		public override void SetDefaults()
		{
			npc.lavaImmune = true;
			npc.npcSlots = 15f;
			npc.damage = 90;
			npc.width = 400;
			npc.height = 280;
			npc.defense = 70;
			npc.lifeMax = CalamityWorld.revenge ? 122000 : 96000;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 187000;
			}
			npc.aiStyle = -1;
			aiType = -1;
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 15, 0, 0);
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.buffImmune[BuffID.Ichor] = false;
			npc.buffImmune[mod.BuffType("MarkedforDeath")] = false;
			npc.buffImmune[BuffID.CursedInferno] = false;
			npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
			npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
			npc.buffImmune[mod.BuffType("DemonFlames")] = false;
			npc.buffImmune[mod.BuffType("HolyLight")] = false;
			npc.buffImmune[mod.BuffType("Nightwither")] = false;
			npc.buffImmune[mod.BuffType("Plague")] = false;
			npc.buffImmune[mod.BuffType("Shred")] = false;
			npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
			npc.buffImmune[mod.BuffType("SilvaStun")] = false;
			npc.boss = true;
			npc.DeathSound = SoundID.NPCDeath14;
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
			if (calamityModMusic != null)
				music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Astrageldon");
			else
				music = MusicID.Boss3;
			bossBag = mod.ItemType("AstrageldonBag");
			if (NPC.downedMoonlord && CalamityWorld.revenge)
			{
				npc.lifeMax = 440000;
				npc.value = Item.buyPrice(0, 35, 0, 0);
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 1600000 : 1400000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(stomping);
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.chaseable);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			stomping = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.chaseable = reader.ReadBoolean();
		}

		public override void AI()
		{
			CalamityAI.AstrumAureusAI(npc, mod);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return (npc.alpha == 0 && npc.ai[0] > 1f);
		}

		public override void FindFrame(int frameHeight)
		{
			if (npc.ai[0] == 3f || npc.ai[0] == 4f)
			{
				if (npc.velocity.Y == 0f && npc.ai[1] >= 0f && npc.ai[0] == 3f) //idle before jump
				{
					if (stomping)
					{
						stomping = false;
					}
					npc.frameCounter += 1.0;
					if (npc.frameCounter > 12.0)
					{
						npc.frame.Y = npc.frame.Y + frameHeight;
						npc.frameCounter = 0.0;
					}
					if (npc.frame.Y >= frameHeight * 6)
					{
						npc.frame.Y = 0;
					}
				}
				else if (npc.velocity.Y <= 0f || npc.ai[1] < 0f) //prepare to jump and then jump
				{
					npc.frameCounter += 1.0;
					if (npc.frameCounter > 12.0)
					{
						npc.frame.Y = npc.frame.Y + frameHeight;
						npc.frameCounter = 0.0;
					}
					if (npc.frame.Y >= frameHeight * 5)
					{
						npc.frame.Y = frameHeight * 5;
					}
				}
				else //stomping
				{
					if (!stomping)
					{
						stomping = true;
						npc.frameCounter = 0.0;
						npc.frame.Y = 0;
					}
					npc.frameCounter += 1.0;
					if (npc.frameCounter > 12.0)
					{
						npc.frame.Y = npc.frame.Y + frameHeight;
						npc.frameCounter = 0.0;
					}
					if (npc.frame.Y >= frameHeight * 5)
					{
						npc.frame.Y = frameHeight * 5;
					}
				}
			}
			else if (npc.ai[0] >= 5f)
			{
				if (stomping)
				{
					stomping = false;
				}
				if (npc.velocity.Y == 0f) //idle before teleport
				{
					npc.frameCounter += 1.0;
					if (npc.frameCounter > 12.0)
					{
						npc.frame.Y = npc.frame.Y + frameHeight;
						npc.frameCounter = 0.0;
					}
					if (npc.frame.Y >= frameHeight * 6)
					{
						npc.frame.Y = 0;
					}
				}
				else //in-air
				{
					npc.frameCounter += 1.0;
					if (npc.frameCounter > 12.0)
					{
						npc.frame.Y = npc.frame.Y + frameHeight;
						npc.frameCounter = 0.0;
					}
					if (npc.frame.Y >= frameHeight * 5)
					{
						npc.frame.Y = frameHeight * 5;
					}
				}
			}
			else
			{
				if (stomping)
				{
					stomping = false;
				}
				npc.frameCounter += 1.0;
				if (npc.frameCounter > 12.0)
				{
					npc.frame.Y = npc.frame.Y + frameHeight;
					npc.frameCounter = 0.0;
				}
				if (npc.frame.Y >= frameHeight * 6)
				{
					npc.frame.Y = 0;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Mod mod = ModLoader.GetMod("CalamityMod");
			Texture2D NPCTexture = Main.npcTexture[npc.type];
			Texture2D GlowMaskTexture = Main.npcTexture[npc.type];
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}

			if (npc.ai[0] == 0f)
			{
				NPCTexture = Main.npcTexture[npc.type];
				GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonGlow");
			}
			else if (npc.ai[0] == 1f) //nothing special done here
			{
				NPCTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonRecharge");
			}
			else if (npc.ai[0] == 2f) //nothing special done here
			{
				NPCTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonWalk");
				GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonWalkGlow");
			}
			else if (npc.ai[0] == 3f || npc.ai[0] == 4f) //needs to have an in-air frame
			{
				if (npc.velocity.Y == 0f && npc.ai[1] >= 0f && npc.ai[0] == 3f) //idle before jump
				{
					NPCTexture = Main.npcTexture[npc.type]; //idle frames
					GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonGlow");
				}
				else if (npc.velocity.Y <= 0f || npc.ai[1] < 0f) //jump frames if flying upward or if about to jump
				{
					NPCTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonJump");
					GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonJumpGlow");
				}
				else //stomping
				{
					NPCTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonStomp");
					GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonStompGlow");
				}
			}
			else if (npc.ai[0] >= 5f) //needs to have an in-air frame
			{
				if (npc.velocity.Y == 0f) //idle before teleport
				{
					NPCTexture = Main.npcTexture[npc.type]; //idle frames
					GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonGlow");
				}
				else //in-air frames
				{
					NPCTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonJump");
					GlowMaskTexture = mod.GetTexture("NPCs/Astrageldon/AstrageldonJumpGlow");
				}
			}

			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			int frameCount = Main.npcFrameCount[npc.type];
			Microsoft.Xna.Framework.Rectangle frame = npc.frame;
			float scale = npc.scale;
			float rotation = npc.rotation;
			float offsetY = npc.gfxOffY;

			Main.spriteBatch.Draw(NPCTexture,
				new Vector2(npc.position.X - Main.screenPosition.X + (float)(npc.width / 2) - (float)Main.npcTexture[npc.type].Width * scale / 2f + vector11.X * scale,
				npc.position.Y - Main.screenPosition.Y + (float)npc.height - (float)Main.npcTexture[npc.type].Height * scale / (float)Main.npcFrameCount[npc.type] + 4f + vector11.Y * scale + 0f + offsetY),
				new Microsoft.Xna.Framework.Rectangle?(frame),
				npc.GetAlpha(drawColor),
				rotation,
				vector11,
				scale,
				spriteEffects,
				0f);

			if (npc.ai[0] != 1) //draw only if not recharging
			{
				Vector2 center = new Vector2(npc.Center.X, npc.Center.Y - 30f); //30
				Vector2 vector = center - Main.screenPosition;
				vector -= new Vector2((float)GlowMaskTexture.Width, (float)(GlowMaskTexture.Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
				vector += vector11 * 1f + new Vector2(0f, 0f + 4f + offsetY);
				Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Gold);

				Main.spriteBatch.Draw(GlowMaskTexture, vector,
					new Microsoft.Xna.Framework.Rectangle?(frame), color, rotation, vector11, 1f, spriteEffects, 0f);
			}
			return false;
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.GreaterHealingPotion;
		}

		public override void NPCLoot()
		{
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, mod.ItemType("AstrageldonTrophy"), 10);
            DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge30"), true, !CalamityWorld.downedAstrageldon);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedAstrageldon, 4, 2, 1);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
			{
                // Materials
                DropHelper.DropItemSpray(npc, mod.ItemType("Stardust"), 20, 30);
                DropHelper.DropItemSpray(npc, ItemID.FallenStar, 25, 40);

                // Weapons
                DropHelper.DropItemChance(npc, mod.ItemType("Nebulash"), 5);

                // Vanity
                DropHelper.DropItemChance(npc, mod.ItemType("AureusMask"), 7);

                // Other
                DropHelper.DropItem(npc, mod.ItemType("AstralJelly"), 9, 12);
                DropHelper.DropItemChance(npc, ItemID.HallowedKey, 5);
			}

            // Drop a large spray of all 4 lunar fragments if ML has been defeated
            if (NPC.downedMoonlord)
            {
                int minFragments = Main.expertMode ? 30 : 20;
                int maxFragments = Main.expertMode ? 48 : 36;
                DropHelper.DropItemSpray(npc, ItemID.FragmentSolar, minFragments, maxFragments);
                DropHelper.DropItemSpray(npc, ItemID.FragmentVortex, minFragments, maxFragments);
                DropHelper.DropItemSpray(npc, ItemID.FragmentNebula, minFragments, maxFragments);
                DropHelper.DropItemSpray(npc, ItemID.FragmentStardust, minFragments, maxFragments);
            }

            // Drop an Astral Meteor if applicable
            if (WorldGenerationMethods.checkAstralMeteor())
            {
                string key = "Mods.CalamityMod.AstralText";
                Color messageColor = Color.Gold;

                if (Main.netMode == 0)
                    Main.NewText(Language.GetTextValue(key), messageColor);
                else if (Main.netMode == 2)
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

                WorldGenerationMethods.dropAstralMeteor();
            }

            // If Astrum Aureus has not yet been killed, notify players of new Astral enemy drops
            if (!CalamityWorld.downedAstrageldon)
            {
                string key = "Mods.CalamityMod.AureusBossText";
                string key2 = "Mods.CalamityMod.AureusBossText2";
                Color messageColor = Color.Gold;

                if (Main.netMode == 0)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                    Main.NewText(Language.GetTextValue(key2), messageColor);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor);
                }
            }

            // Mark Astrum Aureus as dead
            CalamityWorld.downedAstrageldon = true;
            CalamityMod.UpdateServerBoolean();
        }

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.soundDelay == 0)
			{
				npc.soundDelay = 20;
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstrumAureusHit"), npc.Center);
			}

			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 150;
				npc.height = 100;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 50; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 100; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("AstralInfectionDebuff"), 240, true);
		}
	}
}
