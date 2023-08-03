using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Pets;
using CalamityMod.Projectiles.Rogue;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class SEAHOE : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 5;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 700;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 90;
            NPCID.Sets.AttackAverageChance[NPC.type] = 30;
            NPCID.Sets.HatOffsetY[NPC.type] = 16;
            NPC.Happiness
                .SetBiomeAffection<OceanBiome>(AffectionLevel.Like)
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Pirate, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Hate);
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 30;
            NPC.height = 58;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 25;
            NPC.lifeMax = 7500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.65f;
            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,   
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SEAHOE")
            });
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) => DownedBossSystem.downedCLAM && DownedBossSystem.downedDesertScourge;

		public override List<string> SetNPCNameList() => new List<string>() { this.GetLocalizedValue("Name.Amidias") };

        public override void AI()
        {
            NPC.breath += 2;
        }

        public override string GetChat()
        {
            WeightedRandom<string> dialogue = new WeightedRandom<string>();
            Player player = Main.player[Main.myPlayer];

            if (NPC.homeless)
                return this.GetLocalizedValue("Chat.Homeless" + Main.rand.Next(1, 2 + 1));

            if (Main.dayTime)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Day1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day2"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day3"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day4"));
            }
            else
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Night1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night2"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night3"));
            }

            int lilBitch = NPC.FindFirstNPC(NPCID.Angler);
            if (lilBitch != -1)
            {
                dialogue.Add(this.GetLocalization("Chat.Angler1").Format(Main.npc[lilBitch].GivenName));
                dialogue.Add(this.GetLocalization("Chat.Angler2").Format(Main.npc[lilBitch].GivenName));
            }

            int witch = NPC.FindFirstNPC(ModContent.NPCType<WITCH>());
            if (witch != -1)
                dialogue.Add(this.GetLocalizedValue("Chat.BrimstoneWitch"));

            int cirrus = NPC.FindFirstNPC(ModContent.NPCType<FAP>());
            if (cirrus != -1)
                dialogue.Add(this.GetLocalization("Chat.DrunkPrincess").Format(Main.npc[cirrus].GivenName));

            int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
            if (partyGirl != -1)
                dialogue.Add(this.GetLocalization("Chat.PartyGirl").Format(Main.npc[partyGirl].GivenName));

            if (Main.bloodMoon)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.BloodMoon1"));
                dialogue.Add(this.GetLocalizedValue("Chat.BloodMoon2"));
            }

            if (Main.hardMode)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Hardmode1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Hardmode2"));
            }

            if (NPC.downedMoonlord)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.MoonLordDefeated1"));
                dialogue.Add(this.GetLocalizedValue("Chat.MoonLordDefeated2"));
            }

            if (DownedBossSystem.downedDoG)
                dialogue.Add(this.GetLocalizedValue("Chat.DoGDefeated"));

            return dialogue;
        }

        public string Lore()
        {
            int selector = (int)NPC.Calamity().newAI[0];

            if (DownedBossSystem.downedYharon)
                return this.GetLocalizedValue("Help.YharonDefeated" + (1 + (selector % 3)));
            else if (DownedBossSystem.downedDoG)
                return this.GetLocalizedValue("Help.DoGDefeated" + (1 + (selector % 3)));
            else if (DownedBossSystem.downedPolterghast)
                return this.GetLocalizedValue("Help.PolterghastDefeated" + (1 + (selector % 2)));
            else if (DownedBossSystem.downedProvidence)
                return this.GetLocalizedValue("Help.ProvidenceDefeated" + (1 + (selector % 2)));
            else if (NPC.downedMoonlord)
                return this.GetLocalizedValue("Help.MoonLordDefeated" + (1 + (selector % 5)));
            else if (NPC.downedGolemBoss)
                return this.GetLocalizedValue("Help.GolemDefeated" + (1 + (selector % 3)));
            else if (Main.hardMode)
                return this.GetLocalizedValue("Help.Hardmode" + (1 + (selector % (DownedBossSystem.downedCryogen ? 6 : 7))));
            else
            {
                int chosen = 1 + (selector % 10);
                string worldEvil = Language.GetTextValue("LegacyMisc." + (WorldGen.crimson ? 102 : 101));

                // It really ended up with 6 and 9
                if (chosen == 6 || chosen == 9)
                    return this.GetLocalization("Help.PreHardmode" + chosen).Format(worldEvil);

                return this.GetLocalizedValue("Help.PreHardmode" + chosen);
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = Language.GetTextValue("LegacyInterface.51");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                Main.LocalPlayer.Calamity().newAmidiasInventory = false;
                shopName = "Shop";
            }
            else
            {
                Main.npcChatText = Lore();
                NPC.Calamity().newAI[0]++;
                Player player = Main.player[Main.myPlayer];
                player.AddBuff(ModContent.BuffType<AmidiasBlessing>(), 36000);
            }
        }
        public override void AddShops()
        {
            Condition downedOldDuke = new(CalamityUtils.GetText("Condition.PostOD"), () => DownedBossSystem.downedBoomerDuke);

            NPCShop shop = new(Type);
            shop.Add(ModContent.ItemType<Shellshooter>())
                .Add(ModContent.ItemType<SnapClam>())
                .Add(ModContent.ItemType<SandDollar>())
                .Add(ModContent.ItemType<Waywasher>())
                .Add(ModContent.ItemType<CoralCannon>())
                .Add(ModContent.ItemType<UrchinFlail>())
                .Add(ModContent.ItemType<AmidiasTrident>())
                .Add(ModContent.ItemType<EnchantedConch>())
                .Add(ModContent.ItemType<PolypLauncher>())
                .AddWithCustomValue(ItemID.TruffleWorm, Item.buyPrice(gold: 10), Condition.Hardmode)
                .AddWithCustomValue(ModContent.ItemType<BloodwormItem>(), Item.buyPrice(2), downedOldDuke)
                .AddWithCustomValue(ItemID.ShrimpPoBoy, Item.buyPrice(gold: 2, silver: 50), Condition.HappyEnough, Condition.InBeach)
                .AddWithCustomValue(ItemID.Fries, Item.buyPrice(gold: 2), Condition.HappyEnough, Condition.InBeach, Condition.DownedEyeOfCthulhu)
                .Register();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Amidias").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Amidias2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Amidias3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Amidias4").Type, 1f);
                }
            }
        }

        // Make this Town NPC teleport to the King statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => toKingStatue;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 30;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 5;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<SnapClamProj>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 16f;
            gravityCorrection = 10f;
        }
    }
}
