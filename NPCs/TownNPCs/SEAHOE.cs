using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Pets;
using CalamityMod.Projectiles.Rogue;
using IL.Terraria.DataStructures;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class SEAHOE : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sea King");

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
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Hate)
            ;
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
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,   
                
                //Change the info part in his entry when we do the critter barter

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("The Sea King of an ancient civilization long lost to the sands of time— and the desert. He is able to give useful advice on the world around you, having lived for so long.")
            });
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return DownedBossSystem.downedCLAM && DownedBossSystem.downedDesertScourge;
        }

		public override List<string> SetNPCNameList() => new List<string>() { "Amidias" };

        public override void AI()
        {
            NPC.breath += 2;
        }

        public override string GetChat()
        {
            IList<string> dialogue = new List<string>();
            Player player = Main.player[Main.myPlayer];

            if (NPC.homeless)
            {
                return "How much more has the world fallen to ruin? Even the Tyrant’s empire...";
            }
            if (Main.dayTime)
            {
                dialogue.Add("My home may have been destroyed and my people lost... But I will assist you to honor their memory.");
                dialogue.Add("How odd it is, that your people leave the care of those yet to be born to the females. Our males carry the eggs until they hatch.");
                dialogue.Add("How can I survive on land? Ah, that is a secret. No, actually... I can breathe air like you!");
                dialogue.Add("Ah, if only you could have seen the beauty of a kingdom submerged in water. The way the light refracted and shone over our coral homes...");
            }
            else
            {
                dialogue.Add("There lurk horrifying creatures beyond the light of our homes. You should take care.");
                dialogue.Add("My eyes are not well suited to bright lights after so many years of darkness. The peace of the night is welcome.");
                dialogue.Add("Oh, me? I do not sleep, it is part of my nature.");
            }

            int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
            if (partyGirl != -1)
            {
                dialogue.Add(Main.npc[partyGirl].GivenName + " asked if my nose could be used as a vuvuzela. What is a vuvuzela?");
            }

            int lilBitch = NPC.FindFirstNPC(NPCID.Angler);
            if (lilBitch != -1)
            {
                dialogue.Add("Meet me at " + Main.npc[lilBitch].GivenName + "'s house at night. We will throw him to the Trashers.");
                dialogue.Add("Not sure how " + Main.npc[lilBitch].GivenName + " has not been roasted and digested by now, hanging around the sulphuric seas for so long. Perhaps it got to his head.");
            }


            int cirrus = NPC.FindFirstNPC(ModContent.NPCType<FAP>());
            if (cirrus != -1)
                        {
                dialogue.Add("Rumor has it " + Main.npc[cirrus].GivenName + " drinks to forget her troubled past.");
                        }

                        int witch = NPC.FindFirstNPC(ModContent.NPCType<WITCH>());
                        if (witch != -1)
                        {
                                dialogue.Add("I must admit, the Witch's presence is unsettling to me. But so many years have passed, and she too has suffered much.");
                        }

            if (Main.bloodMoon)
            {
                dialogue.Add("Since ancient times people have said that deities cause celestial events. Which one then, is the cause for these?");
                dialogue.Add("I've never been keen on these nights. Such violence.");
            }

            if (Main.hardMode)
            {
                dialogue.Add("Your presence is now known to a great many things. It is unlikely that they will be as friendly towards you as I have been.");
                dialogue.Add("Hm... The veil has fallen, and the world begins to show its true colors. I hope you will trek a righteous path, though even I am not sure what that may be.");
            }

            if (NPC.downedMoonlord)
            {
                dialogue.Add("These days, the night sky feels... just a little less oppressive now.");
                dialogue.Add("Some of these beings I had thought previously to be only legends. To see them in all their glory... what a macabre privilege.");
            }

            if (DownedBossSystem.downedDoG)
            {
                dialogue.Add("To see that Tyrant’s serpent free of its shackles. It gave me chills.");
            }

            return dialogue[Main.rand.Next(dialogue.Count)];
        }

        public string Lore()
        {
            IList<string> dialogue = new List<string>();
            string worldEvil = WorldGen.crimson ? "Crimson" : "Corruption";
            string displayThisText = "If this shows up, something went wrong.";

            if (DownedBossSystem.downedYharon)
            {
                switch (NPC.Calamity().newAI[0] % 3f)
                {
                    case 0f:
                        displayThisText = "Auric is near impossible to work with without being attuned to a Dragon or using the claimed soul of a Dragon to perform a \"mock\" attunement. However, if one can work with it truly powerful things can be created; enough so that using it in anything was deemed heretical by the Gods.";
                        break;
                    case 1f:
                        displayThisText = "The Golden Heretic, Yharim, possesses strength that may just surpass all of the Gods. When you face him, be well prepared for the battle of your life.";
                        break;
                    case 2f:
                        displayThisText = "Draedon isn't a frontline fighter; he is a scientist and inventor. He is unlikely to face you directly, and will likely have his creations confront you instead. Be prepared for anything that blasphemous machine could throw at you.";
                        break;
                }
            }
            else if (DownedBossSystem.downedDoG)
            {
                switch (NPC.Calamity().newAI[0] % 3f)
                {
                    case 0f:
                        displayThisText = "The Devourer's armor was specially made by Draedon. Extremely flexible yet durable and massive, it may be one of his most impressive creations. I shudder to think of what unholy things he may be able to create now, if his research has not stopped.";
                        break;
                    case 1f:
                        displayThisText = "With the cosmic steel you can fashion many of your weapons into much more powerful forms.";
                        break;
                    case 2f:
                        displayThisText = "The Devourer of Gods is truly an enigma. Some say it's not even from this world at all. What is known is that he is a being that can devour the essence of Gods entirely, leaving absolutely nothing left. It's no wonder Yharim recruited that monster.";
                        break;
                }
            }
            else if (DownedBossSystem.downedPolterghast)
            {
                switch (NPC.Calamity().newAI[0] % 2f)
                {
                    case 0f:
                        displayThisText = "The Abyss holds many secrets revealed with time. Checking it out again may not be a bad idea.";
                        break;
                    case 1f:
                        displayThisText = "Ah... I can sense a powerful change in the weather. You may want to venture to the Sulphurous Seas once more during the rain to experience it.";
                        break;
                }
            }
            else if (DownedBossSystem.downedProvidence)
            {
                switch (NPC.Calamity().newAI[0] % 3f)
                {
                    case 0f:
                        displayThisText = "The Rune of Kos holds a significant portion of Providence's brand of magic, easily distinguishable from all others. Activating it in certain places would have some... risky consequences.";
                        break;
                    case 1f:
                        displayThisText = "I do wonder what has happened to Braelor and Statis. I have heard or seen very little of either. In my days, both were great heros to those who worshiped the Gods. Have they been killed, captured, or are they simply in hiding?";
                        break;
                }
            }
            else if (NPC.downedMoonlord)
            {
                switch (NPC.Calamity().newAI[0] % 6f)
                {
                    case 0f:
                        displayThisText = "Your adventure focuses to the jungle it seems. The Dragonfolly and its swarming offspring should be eliminated before their numbers spiral out of control.";
                        break;
                    case 1f:
                        displayThisText = "Profaned creatures now lurk in the Hallow and in Hell. If you destroy enough and gather their essence together you shall be able to capture the attention of the Profaned Guardians.";
                        break;
                    case 2f:
                        displayThisText = "The Profaned Guardians serve Providence, one of the few Gods even I despise. The Guardians attempt to eliminate threats to their Goddess preemptively, and it is likely that their next target is you.";
                        break;
                    case 3f:
                        displayThisText = "Providence can likely be drawn out for combat by using the relic her Guardians left behind. I wonder... Why has Yharim not challenged her? Is he simply that negligent now?";
                        break;
                    case 4f:
                        displayThisText = "Providence was a great threat to Yharim and his army. She would appear out of seemingly thin air, wreck havoc, and vanish before much damage could be done to her. Perhaps she would be overconfident in facing but a single warrior.";
                        break;
                }
            }
            else if (NPC.downedGolemBoss)
            {
                switch (NPC.Calamity().newAI[0] % 3f)
                {
                    case 0f:
                        displayThisText = "The men at the front of the dungeon are performing a ritual to keep the Moon Lord contained in his prison. In order to gain Yharim's attention, however, you may need to defeat them.";
                        break;
                    case 1f:
                        displayThisText = "The Abyss has become far more active than before. You might be able to mine some of the volcanic rubble contained within.";
                        break;
                    case 2f:
                        displayThisText = "The nature of this plague in the jungle bothers me; it certainly is not the average infection. From afar they seem normal, yet up close it is obvious that the infected creatures are equipped with lights and metallic plating. Whatever it may be, it certainly isn't natural.";
                        break;
                }
            }
            else if (Main.hardMode)
            {
                switch (NPC.Calamity().newAI[0] % 8f)
                {
                    case 0f:
                        displayThisText = !DownedBossSystem.downedCryogen ? "Have you ever heard of the Archmage, Permafrost? Once an advisor to Yharim, he one day vanished and Calamitas abandoned Yharim's cause soon after. From what I know, Permafrost was akin to a father to her. Perhaps if she had something to do with his disappearance, he may yet live...?" : "";
                        break;
                    case 1f:
                        displayThisText = "I would recommend saving some of your old items. You never know if you can engineer them into stronger weapons in the future.";
                        break;
                    case 2f:
                        displayThisText = "If you take an idol down to the Brimstone Crags you might be able to see just what is lurking in the shadows.";
                        break;
                    case 3f:
                        displayThisText = "Azafure, the city founded in the Brimstone Crags, was one of the first places to support Yharim in his war. Now, it is nothing more than ruins... Just what happened there?";
                        break;
                    case 4f:
                        displayThisText = "If you've gathered the souls used to power those automatons head once more to the jungle. You will find a powerful enemy to fight, which will unleash the full fervor of the jungle once defeated. Do not underestimate it!";
                        break;
                    case 5f:
                        displayThisText = "Be careful when defeating Plantera and the Golem. You might accidentally unleash a new threat in the jungle that needs to be quelled.";
                        break;
                    case 6f:
                        displayThisText = "When exploring the jungle temple be careful. You may not wish to disturb the Lihzahrd's idol, the Golem. It's quite the destructive force.";
                        break;
                }
            }
            else
            {
                switch (NPC.Calamity().newAI[0] % 10f)
                {
                    case 0f:
                        displayThisText = "There are rumors of ores that lay in latency. When you defeat certain bosses you will undo the ancient magic which conceals those materials.";
                        break;
                    case 1f:
                        displayThisText = "Have you heard of the city in the Brimstone Crags? It lies in the Underworld, and was constructed beneath the Abyss. It was once the oldest and largest city in the world, though it seems nothing but danger lies there now. It would be wise to avoid it.";
                        break;
                    case 2f:
                        displayThisText = "Ah yes, the Abyss. That trench is full of powerful creatures that could devour you in a heartbeat. I would explore the dungeon first.";
                        break;
                    case 3f:
                        displayThisText = "The Sulphurous Seas are dangerous. The toxic waters will burn your skin, but if you can brave them you will be able to reach the Abyss, where there are powerful weapons and dangers aplenty.";
                        break;
                    case 4f:
                        displayThisText = "The Sulphurous Sea was created more than a millenia ago, but it has gotten truly unbearable in the past decades. It's the fault of that accursed robot, Draedon, dumping waste and abandoned projects in there constantly. It's viler than it's ever been, thanks to him.";
                        break;
                    case 5f:
                        displayThisText = "Be careful what you attack in the " + worldEvil + ". You might just unveil a greater threat than what was there before.";
                        break;
                    case 6f:
                        displayThisText = "Scattered across the lands are shrines dedicated to the gods. They contain powerful gear that may help you on your adventures.";
                        break;
                    case 7f:
                        displayThisText = "I assume you have heard of the legendary ninja, Statis? He and his clan worshiped the God of slime, who may be a survivor of the war due to its trickery and knowing when to hide or flee.";
                        break;
                    case 8f:
                        displayThisText = "The " + worldEvil + " is a relatively recent development in the world, and its formation marked the start of Yharim's treacherous war. Although the source of its growth would be sealed away, nothing was ever done about the original manifestations.";
                        break;
                    case 9f:
                        displayThisText = "Throughout the world lie various structures left behind by Draedon. Archaic defenses may remain, but the goods inside may be worth your while.";
                        break;
                }
            }

            return displayThisText;

        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = "Help";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                Main.LocalPlayer.Calamity().newAmidiasInventory = false;
                shop = true;
            }
            else
            {
                shop = false;
                Main.npcChatText = Lore();
                NPC.Calamity().newAI[0]++;
                Player player = Main.player[Main.myPlayer];
                player.AddBuff(ModContent.BuffType<AmidiasBlessing>(), 36000);
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Shellshooter>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<SnapClam>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<SandDollar>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Waywasher>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<CoralCannon>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<UrchinFlail>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<AmidiasTrident>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<MagicalConch>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<PolypLauncher>());
            nextSlot++;
            if (CalamityConfig.Instance.PotionSelling)
            {
                shop.item[nextSlot].SetDefaults(ItemID.GillsPotion);
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 1, 0, 0);
                if (Main.LocalPlayer.discount)
                shop.item[nextSlot].shopCustomPrice = (int)(shop.item[nextSlot].shopCustomPrice * 0.8);
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ItemID.WaterWalkingPotion);
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 1, 0, 0);
                if (Main.LocalPlayer.discount)
                shop.item[nextSlot].shopCustomPrice = (int)(shop.item[nextSlot].shopCustomPrice * 0.8);
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ItemID.FlipperPotion);
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 1, 0, 0);
                if (Main.LocalPlayer.discount)
                shop.item[nextSlot].shopCustomPrice = (int)(shop.item[nextSlot].shopCustomPrice * 0.8);
                nextSlot++;
            }
            if (Main.hardMode)
            {
            shop.item[nextSlot].SetDefaults(ItemID.TruffleWorm);
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 10, 0, 0);
            if (Main.LocalPlayer.discount)
                shop.item[nextSlot].shopCustomPrice = (int)(shop.item[nextSlot].shopCustomPrice * 0.8);
            nextSlot++;
            }
            if (DownedBossSystem.downedBoomerDuke)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BloodwormItem>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(2, 0, 0, 0);
                if (Main.LocalPlayer.discount)
                  shop.item[nextSlot].shopCustomPrice = (int)(shop.item[nextSlot].shopCustomPrice * 0.8);
                nextSlot++;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
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
