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
using Terraria.Audio;
namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class THIEF : ModNPC
    {
        string npcName;

        public static List<string> PossibleNames = new List<string>()
        {
            // Patron names
            "Xplizzy", // <@!98826096237109248> Whitegiraffe#6342
			"Freakish", // <@!750363283520749598> Freakish#0001

            // Original names
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

            Main.npcFrameCount[NPC.type] = 23;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 500;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 60;
            NPCID.Sets.AttackAverageChance[NPC.type] = 10;
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.lavaImmune = false;
            NPC.width = 18;
            NPC.height = 44;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250; //Im not special :(
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.PartyGirl;
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

		public override List<string> SetNPCNameList() => PossibleNames;

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
                PossibleDialogs.Add("Hm, the stars are too bright tonight. Makes sneaking around a little more difficult.");
                PossibleDialogs.Add("You think those stars that fall occasionally would make good throwing weapons?");
            }

            if (BirthdayParty.PartyIsUp)
            {
                PossibleDialogs.Add("Where is my party hat? Well, I stole it of course.");
            }
            if (NPC.GivenName == "Laura")
            {
                PossibleDialogs.Add("The nice thing about maps is I can track anything that has fallen.");
            }
            if (NPC.GivenName == "Penelope")
            {
                PossibleDialogs.Add("Imagine how fast you could throw if you just had more hands.");
            }
            if (NPC.GivenName == "Valerie")
            {
                PossibleDialogs.Add("I also take food for currency.");
            }
            if (NPC.GivenName == "Rowan")
            {
                PossibleDialogs.Add("Usually I only think of animals as food or target practice, but dragons are an exception.");
            }

            PossibleDialogs.Add("Anything is a weapon if you throw it hard enough.");
            PossibleDialogs.Add("That's your chucking arm? You need to work out more.");
            PossibleDialogs.Add("Listen here. It's all in the wrist, the wrist! Oh, forget it.");
            PossibleDialogs.Add("Eh you know how it goes; steal from the rich, give to the poor, but I do take a cut of the profit.");
            PossibleDialogs.Add("Snakes! Why does it always have to be snakes!");
            PossibleDialogs.Add("It's super nice you know, to just have everything you want. Some people never got that luxury.");
            PossibleDialogs.Add("It's not stealing! I'm just borrowing it until I die!");

            if (Main.LocalPlayer.InventoryHas(ItemID.BoneGlove))
            {
                PossibleDialogs.Add("Wouldn't be the first time I used remains as weapons.");
            }
            if (Main.hardMode)
            {
                PossibleDialogs.Add("All sorts of new weapons to be found and looted. Get to that, and I'll share some of my collection too!");
                PossibleDialogs.Add("There's so much scrap around this land with valuable parts to them. Makes you wonder who could afford to leave em all around.");
                PossibleDialogs.Add("Crypts, tombs, dungeons, those're all just treasure troves to me. The dead are dead, they've got nothing to do with it.");
            }
            if (NPC.downedMoonlord)
            {
                PossibleDialogs.Add("If you find anything cool, make sure to drop by and show it to me, I promise I’ll keep my hands off it.");
                PossibleDialogs.Add("So many new things to steal, I can’t think of where to start!");
                PossibleDialogs.Add("If I end up angering some deities or whatever, would you mind taking the blame for me?");
            }
            if (Main.LocalPlayer.InventoryHas(ModContent.ItemType<Valediction>()))
            {
                PossibleDialogs.Add("Oh man, did you rip that off a shark!? Now that's a weapon!");
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

            int witch = NPC.FindFirstNPC(ModContent.NPCType<WITCH>());
            if(witch != -1)
            {
                PossibleDialogs.Add("Hey, hey, has Calamitas seriously moved in here with us? Why???");
            }

            int cirrusIndex = NPC.FindFirstNPC(ModContent.NPCType<FAP>());
            if (cirrusIndex != -1)
            {
                NPC cirrus = Main.npc[cirrusIndex]; //please help me I'm stuck in a children's video game - Fabsol
                PossibleDialogs.Add($"I learned never to steal {cirrus.GivenName}'s drinks. She doesn't appreciate me right now, so I'll go back to hiding.");
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
                int[] coinCounts = Utils.CoinsSplit(CalamityWorld.MoneyStolenByBandit);
                if (coinCounts[0] > 0)
                    Item.NewItem(NPC.GetItemSource_Loot(), NPC.Hitbox, ItemID.PlatinumCoin, coinCounts[0]);
                if (coinCounts[1] > 0)
                    Item.NewItem(NPC.GetItemSource_Loot(), NPC.Hitbox, ItemID.GoldCoin, coinCounts[1]);
                if (coinCounts[2] > 0)
                    Item.NewItem(NPC.GetItemSource_Loot(), NPC.Hitbox, ItemID.SilverCoin, coinCounts[2]);
                if (coinCounts[3] > 0)
                    Item.NewItem(NPC.GetItemSource_Loot(), NPC.Hitbox, ItemID.CopperCoin, coinCounts[3]);

                CalamityWorld.MoneyStolenByBandit = 0;
                NPC goblinFucker = Main.npc[goblinIndex];
                SoundEngine.PlaySound(SoundID.Coins, -1, -1, 1, 1f, 0f); // Money dink sound
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var something = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/NPCs/TownNPCs/THIEF" + (BirthdayParty.PartyIsUp ? "Alt" : "")).Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY) - new Vector2(0f, 6f), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, something, 0);
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
            // All prices are manually set. This means the Discount Card does not work.
            // The Bandit doesn't believe in discounts.
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
            shop.item[nextSlot].shopCustomPrice = shop.item[nextSlot].value;
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ItemID.InvisibilityPotion);
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 1, 0, 0);
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ItemID.NightOwlPotion);
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 1, 0, 0);
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ItemID.TrapsightPotion);
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(0, 1, 0, 0);
            nextSlot++;
            if (DownedBossSystem.downedSlimeGod)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<GelDart>());
                shop.item[nextSlot].shopCustomPrice = shop.item[nextSlot].value;
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
                shop.item[nextSlot].shopCustomPrice = shop.item[nextSlot].value;
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<LatcherMine>());
                shop.item[nextSlot].shopCustomPrice = shop.item[nextSlot].value;
                nextSlot++;
            }
            if (DownedBossSystem.downedCalamitas)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<DeepWounder>());
                shop.item[nextSlot].shopCustomPrice = shop.item[nextSlot].value;
                nextSlot++;
            }
            if (NPC.downedPlantBoss)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<MonkeyDarts>());
                shop.item[nextSlot].shopCustomPrice = shop.item[nextSlot].value;
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<GloveOfPrecision>());
                shop.item[nextSlot].shopCustomPrice = shop.item[nextSlot].value;
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<GloveOfRecklessness>());
                shop.item[nextSlot].shopCustomPrice = shop.item[nextSlot].value;
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
            if (DownedBossSystem.downedProvidence)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<SylvanSlasher>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(5, 0, 0, 0);
                nextSlot++;
            }
            if (DownedBossSystem.downedDoG)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<VeneratedLocket>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(25, 0, 0, 0);
                nextSlot++;
            }
            if (DownedBossSystem.downedYharon)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<DragonScales>());
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(40, 0, 0, 0);
                nextSlot++;
            }
            //:BearWatchingYou:
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<BearEye>());
            shop.item[nextSlot].shopCustomPrice = shop.item[nextSlot].value;
            nextSlot++;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit2").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit3").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit4").Type, 1f);
                }
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
