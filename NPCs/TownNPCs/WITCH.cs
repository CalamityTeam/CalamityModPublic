using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Projectiles.Magic;
using CalamityMod.UI.CalamitasEnchants;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

using static Terraria.ModLoader.ModContent;
using SCalBoss = CalamityMod.NPCs.SupremeCalamitas.SupremeCalamitas;

namespace CalamityMod.NPCs.TownNPCs
{
	[AutoloadHead]
	public class WITCH : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Witch");

			Main.npcFrameCount[npc.type] = 27;
			NPCID.Sets.ExtraFramesCount[npc.type] = 9;
			NPCID.Sets.AttackFrameCount[npc.type] = 4;
			NPCID.Sets.DangerDetectRange[npc.type] = 700;
			NPCID.Sets.AttackType[npc.type] = 1;
			NPCID.Sets.AttackTime[npc.type] = 30;
			NPCID.Sets.AttackAverageChance[npc.type] = 5;
		}

		public override void SetDefaults()
		{
			npc.townNPC = true;
			npc.friendly = true;
			npc.lavaImmune = true;
			npc.width = 18;
			npc.height = 40;
			npc.aiStyle = 7;
			npc.damage = 10;

			// You should not be able to kill SCal under any typical circumstances.
			npc.Calamity().DR = 0.999999f;
			npc.Calamity().unbreakableDR = true;
			npc.lifeMax = 1000000;

			npc.defense = 120;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath6;
			npc.knockBackResist = 0.8f;
			animationType = NPCID.Wizard;
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money) => CalamityWorld.downedSCal;

		public override string TownNPCName() => "Calamitas";

		public override string GetChat()
		{
			// TODO: Give this town NPC unique dialog.

			if (npc.homeless) //not sure how to check if he has ever found a home before (to make this dialogue only run when he first spawns)
			{
				if (Main.rand.NextBool(2))
					return "I deeply appreciate you rescuing me from being trapped within my frozen castle... It's been many, many years...";
				else
					return "Thank you for saving me...though now I admit I am without a home since mine got destroyed.";
			}

			IList<string> dialogue = new List<string>();

			if (Main.dayTime)
			{
				dialogue.Add("I must admit...I am not quite used to this weather. It's too warm for my taste...");
				dialogue.Add("My dear! What is it you would like to talk about today?");
				dialogue.Add("Why...I don't have to worry about any time of the day! If it is hot...then I can use my ice magic to cool down!");
				dialogue.Add("I do usually prefer a spot of humidity for my ice magic. It likes to come out as steam when it's too hot and dry...");
				dialogue.Add("Magic is a skill that must be learned and practiced! You seem to have an inherent talent for it at your age. I have spent all of my life honing it...");
				dialogue.Add("Why ice magic, you ask? Well, my parents were both pyromaniacs...");
			}
			else
			{
				dialogue.Add("There be monsters lurking in the darkness. Most...unnatural monsters.");
				dialogue.Add("You could break the icy stillness in the air tonight.");
				dialogue.Add("Hmm...some would say that an unforeseen outside force is the root of the blood moon...");
				dialogue.Add("I was once the greatest Archmage of ice that ever hailed the lands. Whether or not that is still applicable, I am not sure...");
				dialogue.Add("There used to be other Archmages of other elements. I wonder where they are now...if they are also alive...");
				dialogue.Add("Oh...I wish I could tell you all about my life and the lessons I have learned, but it appears you have a great many things to do...");
			}

			dialogue.Add("I assure you, I will do my best to act as the cool grandfather figure you always wanted.");

			if (BirthdayParty.PartyIsUp)
				dialogue.Add("Sometimes...I feel like all I'm good for during these events is making ice cubes and slushies.");

			if (NPC.downedMoonlord)
			{
				dialogue.Add("Tread carefully, my friend... Now that the Moon Lord has been defeated, many powerful creatures will crawl out to challenge you...");
				dialogue.Add("I feel the balance of nature tilting farther than ever before. Is it due to you, or because of the events leading to now...?");
			}

			if (CalamityWorld.downedPolterghast)
				dialogue.Add("I felt a sudden chill down my spine. I sense something dangerous stirring in the Abyss...");

			if (CalamityWorld.downedYharon)
				dialogue.Add("...Where is Lord Yharim? He must be up to something...");

			int dryad = NPC.FindFirstNPC(NPCID.Dryad);
			if (dryad != -1)
				dialogue.Add("Yes, I am older than " + Main.npc[dryad].GivenName + ". You can stop asking now...");

			if (Main.player[Main.myPlayer].Calamity().chibii)
				dialogue.Add("What an adorable tiny companion you have! Where in the world did you find such a...creature...? Actually, I'd rather not know.");

			if (Main.player[Main.myPlayer].Calamity().cryogenSoul)
				dialogue.Add(Main.player[Main.myPlayer].name + "...just between us, I think I forgot my soul in the ice castle. If you see it, please do let me know.");

			if (Main.hardMode)
				dialogue.Add("It wouldn't be the first time something unknown and powerful dropped from the heavens...I would tread carefully if I were you...");

			return dialogue[Main.rand.Next(dialogue.Count)];
		}

		public override void SetChatButtons(ref string button, ref string button2) => button = "Enchant";

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			if (firstButton)
			{
				Main.playerInventory = true;
				Main.LocalPlayer.Calamity().newCalamitasInventory = false;
				CalamitasEnchantUI.NPCIndex = npc.whoAmI;
				CalamitasEnchantUI.CurrentlyViewing = true;

				if (!Main.LocalPlayer.Calamity().GivenBrimstoneLocus)
				{
					DropHelper.DropItem(npc, ItemType<BrimstoneLocus>());
					Main.LocalPlayer.Calamity().GivenBrimstoneLocus = true;
				}

				shop = false;
			}
		}

		// Make this Town NPC teleport to the Queen statue when triggered.
		public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

		public override void TownNPCAttackStrength(ref int damage, ref float knockback)
		{
			damage = 50;
			knockback = 10f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
		{
			cooldown = 10;
			randExtraCooldown = 50;
		}

        public override bool PreAI()
        {
			// Disappear if the SCal boss is active. She's supposed to be the boss.
			// However, this doesn't happen in Boss Rush; the SCal there is a silent puppet created by Xeroc, not SCal herself.
			if (NPC.AnyNPCs(NPCType<SCalBoss>()) && !BossRushEvent.BossRushActive)
            {
				npc.active = false;
				npc.netUpdate = true;
				return false;
            }
            return true;
        }

        //public override void TownNPCAttackMagic(ref float auraLightMultiplier)	

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
		{
			projType = ProjectileType<SeethingDischargeBrimstoneHellblast>();
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
		{
			multiplier = 2f;
		}

		// Explode into red dust on death.
		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				npc.position = npc.Center;
				npc.width = npc.height = 50;
				npc.position.X -= npc.width / 2;
				npc.position.Y -= npc.height / 2;
				for (int i = 0; i < 5; i++)
				{
					int brimstone = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
					Main.dust[brimstone].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[brimstone].scale = 0.5f;
						Main.dust[brimstone].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}

				for (int i = 0; i < 10; i++)
				{
					int fire = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
					Main.dust[fire].noGravity = true;
					Main.dust[fire].velocity *= 5f;

					fire = Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
					Main.dust[fire].velocity *= 2f;
				}
			}
		}
	}
}
