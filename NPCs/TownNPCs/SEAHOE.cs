using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.TownNPCs
{
	[AutoloadHead]
	public class SEAHOE : ModNPC
	{
		public bool tips = false;

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
		}

		public override void SetDefaults()
		{
			npc.townNPC = true;
			npc.friendly = true;
			npc.width = 30;
			npc.height = 58;
			npc.aiStyle = 7;
			npc.damage = 10;
			npc.defense = 15;
			npc.lifeMax = 250;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.knockBackResist = 0.5f;
			animationType = NPCID.Guide;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money)
		{
			return CalamityWorld.downedCLAM;
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
				if (Main.rand.Next(2) == 0)
					return "Is this... what the world is like now? It seems so much more lifeless than when I saw it when I disappeared.";
				else
					return "Thank you for your service, my child, but I am afraid I am without a home now.";
			}
			if (Main.dayTime)
			{
				dialogue.Add("My homeland may have been dried up, but the memory of my people still exist. I will not let that be in vain.");
				dialogue.Add("There is a lot that you do not know about this world, especially regarding the past. Much of it has been lost to history during the many wars that plagued it.");
				dialogue.Add("How can I survive on land? Ah, that is a secret. No, actually... I can breathe air like you!");
				dialogue.Add("Why do you ask if it's the males who carry their young? Don't your males carry their young?");
			}
			else
			{
				dialogue.Add("There be monsters lurking in the darkness. Most...unnatural monsters.");
				dialogue.Add("Most creatures look up at the moon and admire it. I look up at it and fear it.");
				dialogue.Add("Oh, me? I don't sleep. It's part of my nature.");
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
				dialogue.Add("Not sure how " + Main.npc[lilBitch].GivenName + " has not been roasted and digested by now, hanging around the sulphuric seas for so long. Peharps it has gotten to his head.");
			}

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

			if (Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>().sirenPet)
			{
				dialogue.Add("Ah, nice duds, my child!");
			}

			if ((Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>().sirenBoobs && !Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>().sirenBoobsHide) ||
				(Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>().sirenBoobsAlt && !Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>().sirenBoobsAltHide))
			{
				dialogue.Add("Shouldn't you be wearing a clam bra?");
			}

			if (Main.hardMode)
			{
				dialogue.Add("The balance between light and darkness is tipping. Stay strong, my child.");
				dialogue.Add("Ah, you are starting to realize just how complicated this world is now. You are learning the story of what became of him.");
			}

			if (NPC.downedMoonlord)
			{
				dialogue.Add("I must admit, I am quite shaken up by now. I thought I would never see the elder god again in my life when he was sealed in the moon for the first time...");
				dialogue.Add("Times like this I wish my home was still in one piece instead of evaporated away. I don't blame the witch for anything, it's just... Oh, never mind.");
			}

			if (CalamityWorld.downedDoG)
			{
				dialogue.Add("Do you ever get the feeling that something out there is watching you very carefully? Whatever it is, it's very small and very sly, I think.");
				dialogue.Add("Calamitas was wrong, I guess. Defeating Ceaseless Void and the Devourer of Gods did not wipe out the entire universe... but it didn't do absolutely nothing.");
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
				dialogue.Add("Defeating the Jungle dragon has allowed you a choice. You can face off against Supreme Calamitas, Draedon, or Lord Yharim himself. Choose wisely.");
				dialogue.Add("Pockets of ore have appeared once more in the land. These will allow you to create the most powerful weaponry and armor imagined!");
				dialogue.Add("Supreme Calamitas is no mere witch. Myth has it that Calamitas rejected her old name in the face of her tragedy and to say her old name is considered a death sentence.");
				dialogue.Add("Lord Yharim is very strong. He may not even fight you at his fullest power, which is fortunate for you. His attacks may just kill you in one hit, so be careful.");
				dialogue.Add("Draedon's style of confrontation is very... alien and hands off. You may be more likely to fight any one of his mechs before taking him down.");
			}
			else if (CalamityWorld.downedDoG)
			{
				dialogue.Add("The Devourer of Gods cosmic armor is unique in that it is capable of not only protecting his body from tearing itself apart when ripping pockets of space and time, but also controlling his own powers.");
				dialogue.Add("With the cosmic bars you have, you can create and combine your weapons into extremely powerful and useful items.");
				dialogue.Add("The Devourer of Gods is very, very powerful. However, he is young, foolhardy, and very lazy. Perhaps if he's allowed to grow up some more and train his powers, he would be a force to be reckoned with.");
				dialogue.Add("Your adventure focuses to the jungle, it seems. Clearing out the infestation of the Bumblebirbs that have cropped up is an excellent idea.");
				dialogue.Add("Ah, the dragon Yharon is fickle. You may find that he may refuse to use his full power unless you are strong enough to unleash the power of the Dark Sun.");
			}
			else if (CalamityWorld.downedProvidence)
			{
				dialogue.Add("The Rune of Kos is an ancient Rune used to awaken people from a crystalline prison. When you activate the Rune's gemtech code, you run the risk of attracting the attention of the Sentinels...");
				dialogue.Add("There used to be five Sentinels of the Devourer that patrolled the lands looking for the profaned goddess. You only have to worry about three of them now, in certain locations.");
				dialogue.Add("Ironic, is it not, that Statis was defeated by the very Sentinel his people were inspired from. Then again, Braelor's death was just as ironic against the claws of Draedon's machines.");
				dialogue.Add("The Dungeon seems to be active now. You may hear the faint whisperings of angry spirits who have not left to the Void.. I would recommend looking there before taking on the Sentinels.");
			}
			else if (NPC.downedMoonlord)
			{
				dialogue.Add("The rain by now has turned to acid rain. Your skin will be burning if you stay in it too long.");
				dialogue.Add("Profaned creatures now lurk in the Hallow and in Hell. You defeat enough of them, you'll be able to catch the attention of the Profaned Guardians, and more.");
				dialogue.Add("The Profaned Guardians will do anything to protect their guardian. Makes sense they die for her, since they can be revived just as quickly.");
				dialogue.Add("Touching Providence's offerings is usually a death wish. Shame that Yharim didn't think to just mess with her things to get her out of hiding.");
				dialogue.Add("Providence is as much the sun goddess as much as the Moon Lord is the moon god. They are two sides of each coin, choosing to remain neutral amongst our petty squabbles.");
				dialogue.Add("The stories have it that when Providence faced and defeated Yharim and his forces, she lost a lot of energy and reverted to a more skeletal form. She's merely a fraction of the power as she was before");
				dialogue.Add("If you pay attention to the Profaned Guardians and Providence, you'll notice they have gems in their bodies. That 'gemtech' is actually their source of power and self awareness.");
			}
			else if (NPC.downedGolemBoss)
			{
				dialogue.Add("The men at the front of the dungeon are performing a dangerous ritual to release the Moon Lord from his prison. You must defeat them!");
				dialogue.Add("Here's a little bit of gossip for you; Supposedly that Lunatic Cultist was formerly the apprentice of Yharim's own brother.");
				dialogue.Add("The Abyss is become far more active than it has been before. You might be able to mine out some of the abyss by now.");
				dialogue.Add("The plague was just one of the many experiments authorized by Yharim to raze towns to the ground. This is probably one of the few he shelved for being too terrible.");
			}
			else if (CalamityWorld.downedCryogen)
			{
				dialogue.Add("You will find more ores will be unlocked as the magic sealing them away is broken. Some of them may require more than just the ore itself to create.");
				dialogue.Add("I would recommend saving some of your old items. You never know if you can engineer them into stronger weapons in the future.");
				dialogue.Add("Draedon's Mechs have been rampaging the landscape for ages before they were sealed. Defeating at least one of them opens up the possibility to fight other creatures that lurk in the night.");
				dialogue.Add("If you take an idol down to the Brimstone Crags, you might be able to see just what is lurking in the shadows.");
				dialogue.Add("The Brimstone Crags... Yharim never liked that place, and did everything he could to raze it to the ground. It might explain a few things about him.");
				dialogue.Add("The witch just might offer you an opportunity for a challenge if you are willing to during the night.");
				dialogue.Add("Defeating the mechanical creatures that lurk in the night will unveil a path in the jungle. There you will be able to release more of the ores sealed by magic.");
				dialogue.Add("Be careful when defeating Plantera and Golem. You might accidentally unleash a new threat in the jungle that needs to be quelled.");
				dialogue.Add("The Golem is the protector of the Lihzhard Temple, ensuring that the Sun God's treasures are protected. It's not the only Golem, either.");
			}
			else if (Main.hardMode)
			{
				dialogue.Add("Have you heard of the story of Archmage Permafrost? Rumor has it he's been locked away in his ice castle by Lord Yharim. I wonder if you can free him after castle Cryogen is put on ice...?");
			}
			else
			{
				dialogue.Add("There are rumor of ores that are in latency. When you defeat certain bosses, you are able to release the magic locked up by them to mine and use.");
				dialogue.Add("Been to the Brimstone Crags? I wouldn't recommend going there unless you are strong enough to. There's valuable loot if you have a shadow key, though.");
				dialogue.Add("Ah yes, the Abyss. It is full of strong enemies that could kill you in a heartbeat. I would explore the dungeon first before you go down there.");
				dialogue.Add("The Sulphurous Seas are dangerous, but if you can brave them, you will find the most interesting treasures and secrets down in the Abyss.");
				dialogue.Add("The Sulphurous Seas are a result of a combination of natural volcanic soils leeching into the waters, along with severe pollution in the heydays of Draedon's experiments.");
				dialogue.Add("Be careful what you attack in the " + worldEvil + ". You might just unveil a greater threat than what was there before.");
				dialogue.Add("Scattered across the lands are shrines dedicated to the gods. You can take whatever is in them, but a few items you may not be able to use until much, much later.");
				dialogue.Add("Have you heard the story of Statis? Rumor has it that their Gods are still alive, and it might take releasing the dungeon of its shackles to meet them.");
				dialogue.Add("The dungeon is a dark place. None of us know what it is for, however the ancient Eidolist Cultists used it for worship before Lord Yharim took it over.");
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
				shop = true;
				tips = false;
			}
			else
			{
				shop = false;
				tips = true;
				Main.npcChatText = Lore();
				Player player = Main.player[Main.myPlayer];
				player.AddBuff(mod.BuffType("AmidiasBlessing"), 36000);
			}
		}

		public override void SetupShop(Chest shop, ref int nextSlot)
		{
			shop.item[nextSlot].SetDefaults(mod.ItemType("Shellshooter"));
			nextSlot++;
			shop.item[nextSlot].SetDefaults(mod.ItemType("SnapClam"));
			nextSlot++;
			shop.item[nextSlot].SetDefaults(mod.ItemType("SandDollar"));
			nextSlot++;
			shop.item[nextSlot].SetDefaults(mod.ItemType("Waywasher"));
			nextSlot++;
			shop.item[nextSlot].SetDefaults(mod.ItemType("CoralCannon"));
			nextSlot++;
			shop.item[nextSlot].SetDefaults(mod.ItemType("UrchinFlail"));
			nextSlot++;
			shop.item[nextSlot].SetDefaults(mod.ItemType("AmidiasTrident"));
			nextSlot++;
			shop.item[nextSlot].SetDefaults(mod.ItemType("MagicalConch"));
			nextSlot++;
			if (NPC.downedFishron)
			{
				shop.item[nextSlot].SetDefaults(ItemID.TruffleWorm);
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 40, 0, 0);
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
			projType = mod.ProjectileType("SnapClamProj");
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 16f;
			gravityCorrection = 10f;
		}
	}
}