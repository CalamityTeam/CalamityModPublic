using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.SummonItems;
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
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 30;
            NPC.height = 58;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 25;
            NPC.lifeMax = 7500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.65f;
            AnimationType = NPCID.Guide;
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
                if (Main.rand.NextBool(2))
                    return "How much more has the world fallen to ruin? Even the Tyrant’s empire...";
                else
                    return "Thank you for your service, my child, but I am afraid I am without a home now.";
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
                        displayThisText = "Pockets of ore have appeared once more in the land. This will allow you to create the most powerful weaponry and armor imagined!";
                        break;
                    case 1f:
                        displayThisText = "Lord Yharim possesses god-like strength. He may not even fight you at full power which is fortunate for you. His attacks may just kill you in one hit, so be careful.";
                        break;
                    case 2f:
                        displayThisText = "Draedon's style of confrontation is very... alien and hands-off. You may be more likely to fight any one of his mechs before taking him down.";
                        break;
                }
            }
            else if (DownedBossSystem.downedDoG)
            {
                switch (NPC.Calamity().newAI[0] % 3f)
                {
                    case 0f:
                        displayThisText = "The Devourer of God's cosmic armor is unique in that it is capable of not only protecting his body from tearing itself apart when ripping through the fabric of space and time, but also allows him to control his powers.";
                        break;
                    case 1f:
                        displayThisText = "With the cosmic steel you can fashion many of your weapons into much more powerful forms.";
                        break;
                    case 2f:
                        displayThisText = "The Devourer of Gods is extremely powerful. However, he is young, foolhardy, and very lazy. Perhaps if he had been given time to develop he would have turned into quite the threat.";
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
                        displayThisText = "Ironic, is it not, that Statis was defeated by the very Sentinel his people fashioned their art of stealth from. Fate so often weaves cruel tales.";
                        break;
                    case 2f:
                        displayThisText = "The Dungeon seems to be more active now. You may hear the faint whisperings of angry spirits who have not left to the Void... I would recommend searching there before taking on the Sentinels.";
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
                        displayThisText = "The Profaned Guardians will do anything to protect their goddess. Makes sense they would die for her since they can be revived just as quickly.";
                        break;
                    case 3f:
                        displayThisText = "Touching Providence's offerings is usually a death wish. Shame that Yharim didn't think to just mess with her things to get her out of hiding.";
                        break;
                    case 4f:
                        displayThisText = "Providence is as much the sun goddess as much as the Moon Lord is the moon god. They are two sides of the same coin, choosing to remain neutral amongst our petty squabbles.";
                        break;
                    case 5f:
                        displayThisText = "The stories have it that when Providence faced and defeated Yharim and his forces she lost a lot of energy and reverted to a more skeletal form. She's merely a fraction of the power she was before.";
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
                        displayThisText = "The plague was just one of the many experiments authorized by Yharim to raze towns to the ground. This is probably one of the few he shelved for being too terrible.";
                        break;
                }
            }
            else if (Main.hardMode)
            {
                switch (NPC.Calamity().newAI[0] % 8f)
                {
                    case 0f:
                        displayThisText = !DownedBossSystem.downedCryogen ? "Have you heard of the story of Archmage Permafrost? Rumor has it he's been locked away in an icy prison by Lord Yharim. Perhaps you would be able to free him if Cryogen was destroyed." : "You will find more ores have been unlocked due to the magic sealing them away being dispelled. Some of them may require more than just the ore itself to create.";
                        break;
                    case 1f:
                        displayThisText = "I would recommend saving some of your old items. You never know if you can engineer them into stronger weapons in the future.";
                        break;
                    case 2f:
                        displayThisText = "Once those mechanical creations have been defeated you would do well to seek out the crippled clone of the witch, Calamitas. It might provide some useful weaponry.";
                        break;
                    case 3f:
                        displayThisText = "If you take an idol down to the Brimstone Crags you might be able to see just what is lurking in the shadows.";
                        break;
                    case 4f:
                        displayThisText = "The Brimstone Crags... Yharim despised that place, and did everything he could to raze it to the ground. It might explain a few things about him.";
                        break;
                    case 5f:
                        displayThisText = "If you've gathered the souls used to power those automatons head once more to the jungle. You will find a powerful enemy to fight, which will unleash the full fervor of the jungle once defeated. Do not underestimate it!";
                        break;
                    case 6f:
                        displayThisText = "Be careful when defeating Plantera and the Golem. You might accidentally unleash a new threat in the jungle that needs to be quelled.";
                        break;
                    case 7f:
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
                        displayThisText = "Have you heard of the Brimstone Crags? It was once a grand kingdom, not too different from my own. However, it also met a similar fate. I would not advise going down there, unless you seek a painful death.";
                        break;
                    case 2f:
                        displayThisText = "Ah yes, the Abyss. That trench is full of powerful creatures that could devour you in a heartbeat. I would explore the dungeon first.";
                        break;
                    case 3f:
                        displayThisText = "The Sulphurous Seas are dangerous. The toxic waters will burn your skin, but if you can brave them you will be able to reach the Abyss, where there are powerful weapons and dangers aplenty.";
                        break;
                    case 4f:
                        displayThisText = "The Sulphurous Seas were created long ago, when Yharim's dungeon could no longer hold as many corpses as it needed to. Many of the bodies were dumped into the ocean. This, along with severe pollution from the heydays of Draedon's experiments have turned a paradise into a wasteland.";
                        break;
                    case 5f:
                        displayThisText = "Be careful what you attack in the " + worldEvil + ". You might just unveil a greater threat than what was there before.";
                        break;
                    case 6f:
                        displayThisText = "Scattered across the lands are shrines dedicated to the gods. They contain powerful gear that may help you on your adventures.";
                        break;
                    case 7f:
                        displayThisText = "I'm assuming you've heard the legends that speak of the ninja, Statis? There are some who say that if you were able to defeat the gods which his clan once worshipped you would be able to harness some of his powers.";
                        break;
                    case 8f:
                        displayThisText = "The " + worldEvil + " used to be easily manageable and controlled by nature. However, the recent wars and pollution have tipped the balance out of favor.";
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
            if (NPC.downedFishron)
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
