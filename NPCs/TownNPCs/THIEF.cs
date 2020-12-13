using CalamityMod.Items.Accessories;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class THIEF : ModNPC
    {
        string npcName;

        public static List<string> PossibleNames = new List<string>()
        {
			//Patron names
			"Xplizzy", // <@!98826096237109248> Whitegiraffe #6342

			//Original names
            "Laura", "Mie", "Bonnie",
            "Sarah", "Diane", "Kate",
            "Penelope", "Marisa", "Maribel",
            "Valerie", "Jessica", "Rowan",
            "Jessie", "Jade", "Hearn",
            "Amber", "Anne", "Indiana"
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bandit");

            Main.npcFrameCount[npc.type] = 23;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 500;
            NPCID.Sets.AttackType[npc.type] = 0;
            NPCID.Sets.AttackTime[npc.type] = 60;
            NPCID.Sets.AttackAverageChance[npc.type] = 10;
        }

        public override void SetDefaults()
        {
            npc.townNPC = true;
            npc.friendly = true;
            npc.lavaImmune = false;
            npc.width = 18;
            npc.height = 44;
            npc.aiStyle = 7;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 250; //Im not special :(
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.5f;
            animationType = NPCID.PartyGirl;
        }

		public override void AI()
		{
			if (!CalamityWorld.spawnedBandit)
			{
				CalamityWorld.spawnedBandit = true;
			}
		}

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
				bool rich = player.InventoryHas(ItemID.PlatinumCoin) || player.PortableStorageHas(ItemID.PlatinumCoin);
                if (player.active && rich)
                {
                    return NPC.downedBoss3 || CalamityWorld.spawnedBandit;
                }
            }
            return CalamityWorld.spawnedBandit;
        }

        public override string TownNPCName()
        {
            npcName = PossibleNames[Main.rand.Next(PossibleNames.Count)];
            return npcName;
        }

        public override string GetChat()
        {
            List<string> PossibleDialogs = new List<string>();
            if (!Main.dayTime && Main.bloodMoon)
            {
                PossibleDialogs.Add("Oy, watch where you're going or I might just use you for dart practice.");
                PossibleDialogs.Add("Bet you'd look good as a pincushion, amiright?");
                PossibleDialogs.Add("Zombies don't dodge very well. Maybe you'll do a bit better.");
                PossibleDialogs.Add("Hey, careful over there. I've rigged the place. One wrong step and you're going to get a knife in your forehead.");
            }
            else if (!Main.dayTime && !Main.bloodMoon)
            {
                PossibleDialogs.Add("You know what's really cool? Watching the glint of throwing stars as they reflect the moon.");
                PossibleDialogs.Add("You think those stars that fall occasionally would make good throwing weapons?");
                PossibleDialogs.Add("Statis' clan's got nothing on me. Mostly cause they're all dead.");
            }

            if (BirthdayParty.PartyIsUp)
            {
                PossibleDialogs.Add("Where is my party hat? Well, I stole it of course.");
            }
            if (npc.GivenName == "Laura")
            {
                PossibleDialogs.Add("The nice thing about maps is I can track anything that has fallen.");
            }
            if (npc.GivenName == "Penelope")
            {
                PossibleDialogs.Add("Imagine how fast you could throw if you just had more hands.");
            }
            if (npc.GivenName == "Valerie")
            {
                PossibleDialogs.Add("I also take food for currency.");
            }
            if (npc.GivenName == "Rowan")
            {
                PossibleDialogs.Add("Usually I only think of animals as food or target practice, but dragons are an exception.");
            }

            PossibleDialogs.Add("Anything is a weapon if you throw it hard enough.");
            PossibleDialogs.Add("That's your chucking arm? You need to work out more.");
            PossibleDialogs.Add("Listen here. It's all in the wrist, the wrist! Oh, forget it.");
            PossibleDialogs.Add("I don't think Mom and Dad are proud of the job I have right now.");
            PossibleDialogs.Add("Eh you know how it goes; steal from the rich, give to the poor. Of course, for a price.");
            PossibleDialogs.Add("Want to hear about this one time I was stuck in a room with a rabid dog and a dead guy?");
            PossibleDialogs.Add("Argh snakes. For some reason it's always snakes.");
            PossibleDialogs.Add("Maybe I'm bitter. It's been a long time, so whatever. Just do a good job out there.");
            PossibleDialogs.Add("It's not stealing! I'm just borrowing it until I die!");

            if (Main.LocalPlayer.InventoryHas(ItemID.BoneGlove))
            {
                PossibleDialogs.Add("Wouldn't be the first time I used my friends' remains as weapons.");
            }
            if (Main.hardMode)
            {
                PossibleDialogs.Add("With all of this new stuff cropping up, looks like we got some easy loot and new items to craft up, eh? Well, YOU craft them, I'll steal em.");
                PossibleDialogs.Add("Draedon thinks he can build awesome machines, but he doesn't know how much crap I've stolen from him and sold by dismantling his drones.");
                PossibleDialogs.Add("Gramma always said never to invade ancient temples or you'll be cursed and die. Let's say both of us attest that is untrue. We're still alive. Somewhat.");
            }
            if (NPC.downedMoonlord)
            {
                PossibleDialogs.Add("I heard that there's some really neat and awesome rogue items you can get. Show em to me if you ever get the time.");
                PossibleDialogs.Add("Providence HATES it when you take her stuff. I learned that the hard way.");
                PossibleDialogs.Add("You think I can get away with looting from ghosts? It ain't like they can pick things up.");
            }
            if (Main.LocalPlayer.InventoryHas(ModContent.ItemType<Valediction>()) ||
                Main.LocalPlayer.InventoryHas(ModContent.ItemType<TheReaper>()))
            {
                PossibleDialogs.Add("Oh man, did you rip that off a shark!? Now that's a weapon!");
            }
            if (CalamityWorld.downedDoG)
            {
                PossibleDialogs.Add("I tried looting Storm Weaver's armor once. Before I could get a chunk of the stuff... well let's just say the bigger, fatter cosmic worm arrived and it didn't end well.");
            }
            if (Main.LocalPlayer.ZoneJungle)
            {
                PossibleDialogs.Add("I'd rather not be here. This place has bad vibes, y'know? It brings back some unpleasant memories.");
            }

            int merchantIndex = NPC.FindFirstNPC(NPCID.Merchant);
            if (merchantIndex != -1)
            {
                NPC nerd = Main.npc[merchantIndex];
                PossibleDialogs.Add($"Don't tell {nerd.GivenName}, but I took some of his stuff and replaced it with Angel Statues.");
            }

            int cirrusIndex = NPC.FindFirstNPC(ModContent.NPCType<FAP>());
            if (cirrusIndex != -1)
            {
                NPC cirrus = Main.npc[cirrusIndex]; //please help me I'm stuck in a children's video game - Fabsol
                PossibleDialogs.Add($"I learned never to steal {cirrus.GivenName}'s drinks. She doesn't appreciate me right now so I'll go back to hiding.");
            }

            int armsDealerIndex = NPC.FindFirstNPC(NPCID.ArmsDealer);
            int nurseIndex = NPC.FindFirstNPC(NPCID.Nurse);
            if (armsDealerIndex != -1 && nurseIndex != -1)
            {
                NPC cheeseMachine = Main.npc[nurseIndex];
                NPC minisharkMan = Main.npc[armsDealerIndex];
                PossibleDialogs.Add($"Don't tell {cheeseMachine.GivenName} that I was responsible for {minisharkMan.GivenName}'s injuries.");
            }

            return PossibleDialogs[Main.rand.Next(PossibleDialogs.Count)];
        }

        public string Refund()
        {
            int goblinIndex = NPC.FindFirstNPC(NPCID.GoblinTinkerer);
            if (goblinIndex != -1 && CalamityWorld.Reforges >= 1)
            {
                CalamityWorld.Reforges = 0;
                Main.LocalPlayer.SellItem(CalamityWorld.MoneyStolenByBandit, 1);
                CalamityWorld.MoneyStolenByBandit = 0;
                NPC goblinFucker = Main.npc[goblinIndex];
                Main.PlaySound(SoundID.Coins, -1, -1, 1, 1f, 0f); // Money dink sound
                switch (Main.rand.Next(2))
                {
                    case 0:
                        return $"Want in on a little secret? Since {goblinFucker.GivenName} always gets so much cash from you, I've been stealing some of it as we go. I need you to keep quiet about it, so here.";
                    case 1:
                        return "Hey, if government officials can get tax, why can't I? The heck do you mean that these two things are nothing alike?";
                }
                CalamityNetcode.SyncWorld();
            }
            return "Sorry, I got nothing. Perhaps you could reforge something and come back later...";
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var something = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/TownNPCs/THIEF" + (BirthdayParty.PartyIsUp ? "Alt" : "")), npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY) - new Vector2(0f, 6f), npc.frame, drawColor, npc.rotation, npc.frame.Size() / 2, npc.scale, something, 0);
            return false;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = "Refund";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
				Main.LocalPlayer.Calamity().newBanditInventory = false;
				shop = true;
            }
            else
            {
                shop = false;
                Main.npcChatText = Refund();
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Cinquedea>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 9, 0, 0);
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Glaive>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 3, 0, 0);
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Kylie>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 9, 0, 0);
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<OldDie>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 40, 0, 0);
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ItemID.TigerClimbingGear);
            nextSlot++;
            if (CalamityWorld.downedSlimeGod)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<GelDart>());
                nextSlot++;
            }
            if (Main.hardMode)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<SlickCane>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 25, 0, 0);
                nextSlot++;
            }
            if (NPC.downedPirates)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<ThiefsDime>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(1, 0, 0, 0);
                nextSlot++;
            }
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<MomentumCapacitor>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 60, 0, 0);
                nextSlot++;
            }
            if (NPC.downedMechBossAny)
			{
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BouncingBetty>());
				nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<LatcherMine>());
				nextSlot++;
            }
			if (CalamityWorld.downedCalamitas)
			{
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<DeepWounder>());
				nextSlot++;
			}
            if (NPC.downedPlantBoss)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<MonkeyDarts>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<GloveOfPrecision>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<GloveOfRecklessness>());
                nextSlot++;
            }
            if (NPC.downedGolemBoss)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<EtherealExtorter>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(1, 0, 0, 0);
				nextSlot++;
			}
            if (NPC.downedMoonlord)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<CelestialReaper>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(2, 0, 0, 0);
                nextSlot++;
            }
            if (CalamityWorld.downedProvidence)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<SylvanSlasher>());
				shop.item[nextSlot].shopCustomPrice = Item.buyPrice(5, 0, 0, 0);
				nextSlot++;
			}
            if (CalamityWorld.downedDoG)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<VeneratedLocket>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(25, 0, 0, 0);
                nextSlot++;
            }
            if (CalamityWorld.downedYharon)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<DragonScales>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(40, 0, 0, 0);
                nextSlot++;
            }
            //:BearWatchingYou:
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<BearEye>());
			nextSlot++;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Bandit/Bandit"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Bandit/Bandit2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Bandit/Bandit3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Bandit/Bandit4"), 1f);
            }
        }

        // Make this Town NPC teleport to the Queen statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 50;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 180;
            randExtraCooldown = 60;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<CinquedeaProj>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 2f;
        }
    }
}
