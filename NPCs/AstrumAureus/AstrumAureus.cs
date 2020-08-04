using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstrumAureus
{
	[AutoloadBossHead]
    public class AstrumAureus : ModNPC
    {
        private bool stomping = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Aureus");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.lavaImmune = true;
			npc.noGravity = true;
            npc.npcSlots = 15f;
            npc.damage = 80;
            npc.width = 400;
            npc.height = 280;
            npc.defense = 50;
			npc.DR_NERD(0.15f);
            npc.LifeMaxNERB(96000, NPC.downedMoonlord ? 440000 : 122000, 8400000); // 30 seconds in boss rush
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 15, 0, 0);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[ModContent.BuffType<MarkedforDeath>()] = false;
			npc.buffImmune[BuffID.Frostburn] = false;
			npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[BuffID.BetsysCurse] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.DryadsWardDebuff] = false;
			npc.buffImmune[BuffID.Oiled] = false;
			npc.buffImmune[BuffID.BoneJavelin] = false;
			npc.buffImmune[BuffID.Venom] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Plague>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WarCleave>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.buffImmune[ModContent.BuffType<SulphuricPoisoning>()] = false;
            npc.boss = true;
            npc.DeathSound = SoundID.NPCDeath14;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Astrageldon");
            else
                music = MusicID.Boss3;
            bossBag = ModContent.ItemType<AstrageldonBag>();
            if (NPC.downedMoonlord && CalamityWorld.revenge)
            {
                npc.value = Item.buyPrice(0, 35, 0, 0);
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
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
            return npc.alpha == 0 && npc.ai[0] > 1f;
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
            Texture2D NPCTexture = Main.npcTexture[npc.type];
            Texture2D GlowMaskTexture = Main.npcTexture[npc.type];
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (npc.ai[0] == 0f)
            {
                NPCTexture = Main.npcTexture[npc.type];
                GlowMaskTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusGlow");
            }
            else if (npc.ai[0] == 1f) //nothing special done here
            {
                NPCTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusRecharge");
            }
            else if (npc.ai[0] == 2f) //nothing special done here
            {
                NPCTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusWalk");
                GlowMaskTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusWalkGlow");
            }
            else if (npc.ai[0] == 3f || npc.ai[0] == 4f) //needs to have an in-air frame
            {
                if (npc.velocity.Y == 0f && npc.ai[1] >= 0f && npc.ai[0] == 3f) //idle before jump
                {
                    NPCTexture = Main.npcTexture[npc.type]; //idle frames
                    GlowMaskTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusGlow");
                }
                else if (npc.velocity.Y <= 0f || npc.ai[1] < 0f) //jump frames if flying upward or if about to jump
                {
                    NPCTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusJump");
                    GlowMaskTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusJumpGlow");
                }
                else //stomping
                {
                    NPCTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusStomp");
                    GlowMaskTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusStompGlow");
                }
            }
            else if (npc.ai[0] >= 5f) //needs to have an in-air frame
            {
                if (npc.velocity.Y == 0f) //idle before teleport
                {
                    NPCTexture = Main.npcTexture[npc.type]; //idle frames
                    GlowMaskTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusGlow");
                }
                else //in-air frames
                {
                    NPCTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusJump");
                    GlowMaskTexture = ModContent.GetTexture("CalamityMod/NPCs/AstrumAureus/AstrumAureusJumpGlow");
                }
            }

			int frameCount = Main.npcFrameCount[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / frameCount / 2));
            Rectangle frame = npc.frame;
            float scale = npc.scale;
            float rotation = npc.rotation;
            float offsetY = npc.gfxOffY;
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 7;
			if (npc.ai[0] == 3f || npc.ai[0] == 4f)
				num153 = 10;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = drawColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (float)(num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2((float)NPCTexture.Width, (float)(NPCTexture.Height / frameCount)) * scale / 2f;
					vector41 += vector11 * scale + new Vector2(0f, 4f + offsetY);
					spriteBatch.Draw(NPCTexture, vector41, frame, color38, rotation, vector11, scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)NPCTexture.Width, (float)(NPCTexture.Height / frameCount)) * scale / 2f;
			vector43 += vector11 * scale + new Vector2(0f, 4f + offsetY);
			spriteBatch.Draw(NPCTexture, vector43, frame, npc.GetAlpha(drawColor), rotation, vector11, scale, spriteEffects, 0f);

			if (npc.ai[0] != 1) //draw only if not recharging
            {
                Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Color.Gold);
				Color color40 = Color.Lerp(Color.White, color, 0.5f);

				if (CalamityConfig.Instance.Afterimages)
				{
					for (int num163 = 1; num163 < num153; num163++)
					{
						Color color41 = color40;
						color41 = Color.Lerp(color41, color36, amount9);
						color41 = npc.GetAlpha(color41);
						color41 *= (float)(num153 - num163) / 15f;
						Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
						vector44 -= new Vector2((float)GlowMaskTexture.Width, (float)(GlowMaskTexture.Height / frameCount)) * scale / 2f;
						vector44 += vector11 * scale + new Vector2(0f, 4f + offsetY);
						spriteBatch.Draw(GlowMaskTexture, vector44, frame, color41, rotation, vector11, scale, spriteEffects, 0f);
					}
				}

				spriteBatch.Draw(GlowMaskTexture, vector43, frame, color40, rotation, vector11, scale, spriteEffects, 0f);
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

            DropHelper.DropItemChance(npc, ModContent.ItemType<AstrageldonTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeAstrumAureus>(), true, !CalamityWorld.downedAstrageldon);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedAstrageldon, 4, 2, 1);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Wizard, ModContent.NPCType<FAP>() }, CalamityWorld.downedAstrageldon);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<Stardust>(), 20, 30);
                DropHelper.DropItemSpray(npc, ItemID.FallenStar, 25, 40);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<Nebulash>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<AuroraBlazer>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<AlulaAustralis>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<BorealisBomber>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<AuroradicalThrow>(), 4);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<AureusMask>(), 7);

                // Other
                DropHelper.DropItem(npc, ModContent.ItemType<AstralJelly>(), 9, 12);
                DropHelper.DropItemChance(npc, ItemID.HallowedKey, 5);
            }

            // Drop an Astral Meteor if applicable
            ThreadPool.QueueUserWorkItem(WorldGenerationMethods.AstralMeteorThreadWrapper);

            // If Astrum Aureus has not yet been killed, notify players of new Astral enemy drops
            if (!CalamityWorld.downedAstrageldon)
            {
                string key = "Mods.CalamityMod.AureusBossText";
                string key2 = "Mods.CalamityMod.AureusBossText2";
                Color messageColor = Color.Gold;

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                    Main.NewText(Language.GetTextValue(key2), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
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
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, hitDirection, -1f, 0, default, 1f);
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
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 100; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240, true);
        }
    }
}
