using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class SEAHOE : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sea King");

            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 5;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 700;
            NPCID.Sets.AttackType[npc.type] = 0;
            NPCID.Sets.AttackTime[npc.type] = 90;
            NPCID.Sets.AttackAverageChance[npc.type] = 30;
            NPCID.Sets.HatOffsetY[npc.type] = 16;
        }

        public override void SetDefaults()
        {
            npc.townNPC = true;
            npc.friendly = true;
            npc.width = 30;
            npc.height = 58;
            npc.aiStyle = 7;
            npc.damage = 10;
            npc.defense = 25;
            npc.lifeMax = 7500;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.65f;
            animationType = NPCID.Guide;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return CalamityWorld.downedCLAM && CalamityWorld.downedDesertScourge;
        }

        public override string TownNPCName()
        {
            return "Amidias";
        }

        public override void AI()
        {
            npc.breath += 2;
        }

        public override string GetChat()
        {
            IList<string> dialogue = new List<string>();
            Player player = Main.player[Main.myPlayer];

            if (npc.homeless)
            {
                if (Main.rand.NextBool(2))
                    return "Is this...what the world is like now? It seems so much more lifeless than when I saw it before I disappeared.";
                else
                    return "Thank you for your service, my child, but I am afraid I am without a home now.";
            }
            if (Main.dayTime)
            {
                dialogue.Add("My homeland may have dried up but the memories of my people still remain. I will not let them be in vain.");
                dialogue.Add("There is a lot that you do not know about this world, specifically regarding the past. Much of it has been lost to history due to the many wars that plagued it.");
                dialogue.Add("How can I survive on land? Ah, that is a secret. No, actually...I can breathe air like you!");
                dialogue.Add("Why do you ask if it's the males that carry the young? Don't your males carry their young?");
            }
            else
            {
                dialogue.Add("There be monsters lurking in the darkness. Most...unnatural monsters.");
                dialogue.Add("Most creatures look up at the moon and admire it. I look up and fear it.");
                dialogue.Add("Oh, me? I don't sleep, it's part of my nature.");
            }

            int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
            if (partyGirl != -1)
            {
                dialogue.Add(Main.npc[partyGirl].GivenName + " asked if my nose could be used as a vuvuzela. What is a vuvuzela?");
            }

            int lilBitch = NPC.FindFirstNPC(NPCID.Angler);
            if (lilBitch != -1)
            {
                dialogue.Add("Meet me at " + Main.npc[lilBitch].GivenName + "'s house at night. We're going to throw him to the Trashers.");
                dialogue.Add("Not sure how " + Main.npc[lilBitch].GivenName + " has not been roasted and digested by now, hanging around the sulphuric seas for so long. Peharps it got to his head.");
            }

            int cirrus = NPC.FindFirstNPC(ModContent.NPCType<FAP>());
            if (cirrus != -1)
                dialogue.Add("Rumor has it " + Main.npc[cirrus].GivenName + " drinks to forget her troubled past.");

            if (Main.bloodMoon)
            {
                dialogue.Add("I'm never keen on these nights. They're so violent.");
            }

            /*
            int yharim = NPC.FindFirstNPC(NPCID.Yharim);
            if (yharim != -1)
            {
                dialogue.Add("Oh, Yharim... did he ever realize what he was doing? He seems so...calm, as if his rage had been satiated.");
            }*/

            if (Main.player[Main.myPlayer].Calamity().sirenPet)
            {
                dialogue.Add("Ah, nice duds, my child!");
            }

            if ((Main.player[Main.myPlayer].Calamity().sirenBoobs && !Main.player[Main.myPlayer].Calamity().sirenBoobsHide) ||
                (Main.player[Main.myPlayer].Calamity().sirenBoobsAlt && !Main.player[Main.myPlayer].Calamity().sirenBoobsAltHide))
            {
                dialogue.Add("Shouldn't you be wearing a clam bra?");
            }

            if (Main.hardMode)
            {
                dialogue.Add("The balance between light and dark is tipping. Stay strong, my child.");
                dialogue.Add("Ah, you are starting to realize just how complicated this world is now. You are learning the story of what became of him.");
            }

            if (NPC.downedMoonlord)
            {
                dialogue.Add("Do you ever get the feeling that something out there is watching you very carefully? Whatever it is, it's very small and sly, I think.");
                dialogue.Add("I must admit, I am quite shaken up now. Never would I have imagined that I would see one of the dark gods again. Not in this lifetime anyhow.");
                dialogue.Add("Times like this I wish my home was still in one piece instead of evaporated away. I don't blame the witch for anything, it's just...oh, never mind.");
            }

            if (CalamityWorld.downedDoG)
            {
                dialogue.Add("I suppose that witch was mistaken. Defeating the Ceaseless Void and the Devourer has not caused our world to collapse... but I would not lower my guard if I were you.");
            }

            /*if (CalamityWorld.downedYharon)
            {
                dialogue.Add("I'm worried for Lord Yharim. He was not in a particular good mood once we found out you defeated his close friend.");
            }*/

            return dialogue[Main.rand.Next(dialogue.Count)];
        }

        public string Lore()
        {
            IList<string> dialogue = new List<string>();
            string worldEvil = WorldGen.crimson ? "Crimson" : "Corruption";

            if (CalamityWorld.downedYharon)
            {
                dialogue.Add("Pockets of ore have appeared once more in the land. This will allow you to create the most powerful weaponry and armor imagined!");
                dialogue.Add("Lord Yharim possesses god-like strength. He may not even fight you at full power which is fortunate for you. His attacks may just kill you in one hit, so be careful.");
                dialogue.Add("Draedon's style of confrontation is very...alien and hands-off. You may be more likely to fight any one of his mechs before taking him down.");
            }
            else if (CalamityWorld.downedDoG)
            {
                dialogue.Add("The Devourer of God's cosmic armor is unique in that it is capable of not only protecting his body from tearing itself apart when ripping through the fabric of space and time, but also allows him to control his powers.");
                dialogue.Add("With the cosmic steel you can fashion many of your weapons into much more powerful forms.");
                dialogue.Add("The Devourer of Gods is extremely powerful. However, he is young, foolhardy, and very lazy. Perhaps if he had been given time to develop he would have turned into quite the threat.");
                dialogue.Add("Ah, the dragon Yharon is fickle. You may find that he will refuse to use his full power unless you are strong enough to unleash the power of the Dark Sun.");
            }
            else if (CalamityWorld.downedProvidence)
            {
                dialogue.Add("The Rune of Kos holds a significant portion of Providence's brand of magic, easily distinguishable from all others. Activating it in certain places would have some...risky consequences.");
                dialogue.Add("Ironic, is it not, that Statis was defeated by the very Sentinel his people fashioned their art of stealth from. Fate so often weaves cruel tales.");
                dialogue.Add("The Dungeon seems to be more active now. You may hear the faint whisperings of angry spirits who have not left to the Void...I would recommend searching there before taking on the Sentinels.");
            }
            else if (NPC.downedMoonlord)
            {
                dialogue.Add("Your adventure focuses to the jungle it seems. The Dragonfolly and its swarming offspring should be eliminated before their numbers spiral out of control.");
                dialogue.Add("Ah...I can sense a powerful change in the weather. You may want to venture to the Sulphurous Seas once more during the rain to experience it.");
                dialogue.Add("Profaned creatures now lurk in the Hallow and in Hell. If you destroy enough and gather their essence together you shall be able to capture the attention of the Profaned Guardians.");
                dialogue.Add("The Profaned Guardians will do anything to protect their goddess. Makes sense they would die for her since they can be revived just as quickly.");
                dialogue.Add("Touching Providence's offerings is usually a death wish. Shame that Yharim didn't think to just mess with her things to get her out of hiding.");
                dialogue.Add("Providence is as much the sun goddess as much as the Moon Lord is the moon god. They are two sides of the same coin, choosing to remain neutral amongst our petty squabbles.");
                dialogue.Add("The stories have it that when Providence faced and defeated Yharim and his forces she lost a lot of energy and reverted to a more skeletal form. She's merely a fraction of the power she was before.");
            }
            else if (NPC.downedGolemBoss)
            {
                dialogue.Add("The men at the front of the dungeon are performing a ritual to keep the Moon Lord contained in his prison. In order to gain Yharim's attention, however, you may need to defeat them.");
                dialogue.Add("The Abyss has become far more active than before. You might be able to mine some of the volcanic rubble contained within.");
                dialogue.Add("The plague was just one of the many experiments authorized by Yharim to raze towns to the ground. This is probably one of the few he shelved for being too terrible.");
            }
            else if (CalamityWorld.downedCryogen)
            {
                dialogue.Add("You will find more ores have been unlocked due to the magic sealing them away being dispelled. Some of them may require more than just the ore itself to create.");
                dialogue.Add("I would recommend saving some of your old items. You never know if you can engineer them into stronger weapons in the future.");
                dialogue.Add("Once those mechanical creations have been defeated you would do well to seek out the crippled clone of the witch, Calamitas. It might provide some useful weaponry.");
                dialogue.Add("If you take an idol down to the Brimstone Crags you might be able to see just what is lurking in the shadows.");
                dialogue.Add("The Brimstone Crags...Yharim despised that place, and did everything he could to raze it to the ground. It might explain a few things about him.");
                dialogue.Add("The witch just might offer you an opportunity for a challenge if you are willing to fight during the night.");
                dialogue.Add("If you've gathered the souls used to power those automatons head once more to the jungle. You will find a powerful enemy to fight, which will unleash the full fervor of the jungle once defeated. Do not underestimate it!");
                dialogue.Add("Be careful when defeating Plantera and the Golem. You might accidentally unleash a new threat in the jungle that needs to be quelled.");
                dialogue.Add("When exploring the jungle temple be careful. You may not wish to disturb the Lihzard's idol, the Golem. It's quite the destructive force.");
            }
            else if (Main.hardMode)
            {
                dialogue.Add("Have you heard of the story of Archmage Permafrost? Rumor has it he's been locked away in an icy prison by Lord Yharim. Perhaps you would be able to free him if Cryogen was destroyed.");
            }
            else
            {
                dialogue.Add("There are rumors of ores that lay in latency. When you defeat certain bosses you will undo the ancient magic which conceals those materials.");
                dialogue.Add("Have you heard of the Brimstone Crags? It was once a grand kingdom, not too different from my own. However, it also met a similar fate. I would not advise going down there, unless you seek a painful death.");
                dialogue.Add("Ah yes, the Abyss. That trench is full of powerful creatures that could devour you in a heartbeat. I would explore the dungeon first.");
                dialogue.Add("The Sulphurous Seas are dangerous. The toxic waters will burn your skin, but if you can brave them you will be able to reach the Abyss, where there are powerful weapons and dangers aplenty.");
                dialogue.Add("The Sulphurous Seas were created long ago, when Yharim's dungeon could no longer hold as many corpses as it needed to. Many of the bodies were dumped into the ocean. This, along with severe pollution from the heydays of Draedon's experiments have turned a paradise into a wasteland.");
                dialogue.Add("Be careful what you attack in the " + worldEvil + ". You might just unveil a greater threat than what was there before.");
                dialogue.Add("Scattered across the lands are shrines dedicated to the gods. You can take whatever is in them, but a few items you may not be able to use until much later.");
                dialogue.Add("I'm assuming you've heard the legends that speak of the ninja, Statis? There are some who say that if you were able to defeat the gods which his clan once worshipped you would be able to harness some of his powers.");
                dialogue.Add("The dungeon is a dark place. None of us know of its true purpose, however, the ancient Eidolist cultists used it for worship before Lord Yharim took it over.");
                dialogue.Add("The " + worldEvil + " used to be easily manageable and controlled by nature. However, the recent wars and pollution have tipped the balance out of favor.");
            }

            return dialogue[Main.rand.Next(dialogue.Count)];

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
            if (CalamityWorld.downedDesertScourge)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<DriedSeafood>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 2, 0, 0);
                nextSlot++;
            }
            if (CalamityWorld.downedEoCAcidRain)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<CausticTear>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 3);
                nextSlot++;
            }
            if (CalamityWorld.downedAquaticScourge)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Seafood>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 20, 0, 0);
                nextSlot++;
            }
            if (NPC.downedFishron && CalamityMod.CalamityConfig.SellBossSummons)
            {
                shop.item[nextSlot].SetDefaults(ItemID.TruffleWorm);
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 40, 0, 0);
                nextSlot++;
            }
            if (CalamityWorld.downedBoomerDuke)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BloodwormItem>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(8, 0, 0, 0);
                nextSlot++;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Amidias/Amidias"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Amidias/Amidias2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Amidias/Amidias3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Amidias/Amidias4"), 1f);
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
