using CalamityMod.Events;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class FAP : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Drunk Princess");

            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 400;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 60;
            NPCID.Sets.AttackAverageChance[NPC.type] = 15;
            NPC.Happiness
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Love)
                .SetBiomeAffection<OceanBiome>(AffectionLevel.Like)
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Stylist, AffectionLevel.Love)
                .SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Truffle, AffectionLevel.Like)
                .SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Like)
                .SetNPCAffection(NPCID.DD2Bartender, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Hate)
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
            NPC.lavaImmune = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,                

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("No one knows where she came from, but no one minds her either. She's a good person to share a drink with, given you don't make her mad.")
            });
        }

        public override void AI()
        {
            if (!CalamityWorld.spawnedCirrus)
                CalamityWorld.spawnedCirrus = true;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        { 
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
                bool hasVodka = player.InventoryHas(ModContent.ItemType<FabsolsVodka>()) || player.PortableStorageHas(ModContent.ItemType<FabsolsVodka>());
                if (player.active && hasVodka)
                    return Main.hardMode || CalamityWorld.spawnedCirrus;
            }
            return CalamityWorld.spawnedCirrus;
        }

		public override List<string> SetNPCNameList() => new List<string>() { "Cirrus" };

        public override string GetChat()
        {
            if (CalamityWorld.getFixedBoi)
            {
                Main.player[Main.myPlayer].Hurt(PlayerDeathReason.ByCustomReason(Main.player[Main.myPlayer].name + " was slapped too hard."), Main.player[Main.myPlayer].statLife / 2, -Main.player[Main.myPlayer].direction, false, false, -1, false);
                SoundEngine.PlaySound(CnidarianJellyfishOnTheString.SlapSound, Main.player[Main.myPlayer].Center);
            }

            if (CalamityUtils.AnyBossNPCS())
                return "Why are you talking to me right now? Shouldn't you be bumbling around and dying for my amusement?";

            if (NPC.homeless)
            {
                if (Main.rand.NextBool(2))
                    return "I could smell my vodka from MILES away!";
                else
                    return "Have any spare rooms available? Preferably candle-lit with a hefty supply of booze?";
            }

            int wife = NPC.FindFirstNPC(NPCID.Stylist);
            bool wifeIsAround = wife != -1;
            bool beLessDrunk = wifeIsAround && NPC.downedMoonlord;

            if (Main.bloodMoon)
            {
                int random = Main.rand.Next(4);
                if (random == 0)
                {
                    return "I'm gonna make some Bloody Marys to relax, celery included. Want one?";
                }
                else if (random == 1)
                {
                    return "If you're too lazy to craft potions normally, try Blood Orbs. Blood is fuel, dumbass.";
                }
                else if (random == 2)
                {
                    return "I'm trying to not be bitchy tonight, but it's hard when everyone else won't shut up.";
                }
                else
                {
                    Main.player[Main.myPlayer].Hurt(PlayerDeathReason.ByCustomReason(Main.player[Main.myPlayer].name + " was slapped too hard."), Main.player[Main.myPlayer].statLife / 2, -Main.player[Main.myPlayer].direction, false, false, -1, false); ;
                    SoundEngine.PlaySound(CnidarianJellyfishOnTheString.SlapSound, Main.player[Main.myPlayer].Center);
                    return "Sorry, I have no moral compass at the moment.";
                }
            }

            IList<string> dialogue = new List<string>();

            if (wifeIsAround)
            {
                dialogue.Add("You can't stop me from trying to move in with " + Main.npc[wife].GivenName + ".");
                dialogue.Add("I love it when " + Main.npc[wife].GivenName + "'s hands get sticky from all that... wax.");
                dialogue.Add("Ever since " + Main.npc[wife].GivenName + " moved in I haven't been drinking as much... a strange but not unwelcome feeling.");
            }

            if (Main.dayTime)
            {
                if (beLessDrunk)
                    dialogue.Add(Main.npc[wife].GivenName + " helped me learn to accept my past. It's been rough, but I think I'm on the right track now.");
                else
                    dialogue.Add("I drink to forget certain... things. What things, you might ask? Well, the point is to forget them, isn't it?");

                dialogue.Add("I'm literally balls drunk off my sass right now, what do you want?");
                dialogue.Add("I'm either laughing because I'm drunk or because I've lost my mind, probably both.");
                dialogue.Add("When I'm drunk I'm way happier... at least until the talking worms start to appear.");
                dialogue.Add("I should reprogram the whole mod, while drunk, then send it back to the testers.");

                if (beLessDrunk)
                    dialogue.Add("Might go out for a jog later with " + Main.npc[wife].GivenName + ". Nice day for it.");
                else
                    dialogue.Add("What a great day! Might just drink so much that I get poisoned again.");
            }
            else
            {
                dialogue.Add("A perfect night to light some candles, drink some wine and relax.");
                dialogue.Add("Here's a challenge... take a shot for every time you've had to look at the wiki. Oh wait, you'd die.");
                dialogue.Add("Yes, everyone knows the mechworm is buggy. Well, not anymore, but still.");
                dialogue.Add("You lost or something? I don't mind company, but I'd rather be left alone at night.");
                dialogue.Add("Are you sure you're 21? ...Alright, fine, but don't tell anyone I sold you these.");

                if (wifeIsAround)
                    dialogue.Add("I should watch some movies with " + Main.npc[wife].GivenName + " tonight. You could come too, but only if you bring snacks for us.");
            }

            dialogue.Add("I HATE WALMART! ...Anyway, what do you want this time?");
            dialogue.Add("Drink something that turns you into a magical flying unicorn so you can be just like me.");
            dialogue.Add("Did anyone ever tell you that large assets cause back pain? Well, they were right.");
            dialogue.Add("Deals so good I'll [$$!$] myself! ...Sorry, just had a minor stroke!");

            if (BirthdayParty.PartyIsUp)
                dialogue.Add("You'll always find me at parties where booze is involved... well, you'll always find BOOZE where I'M involved!");

            if (Main.invasionType == InvasionID.MartianMadness)
                dialogue.Add("You should probably deal with those ayy lmaos before anything else, but whatever.");

            if (DownedBossSystem.downedCryogen)
                dialogue.Add("God I can't wait to smash some ice again! ...For drinks, of course.");

            if (DownedBossSystem.downedLeviathan)
                dialogue.Add("How could you murder such a beautiful creature!? ...The blue sexy one, not the obese cucumber.");

            if (NPC.downedMoonlord)
                dialogue.Add("Ever wondered why the Moon Lord needed so many tentacles? Uh... on second thought, I won't answer that.");

            if (AcidRainEvent.AcidRainEventIsOngoing)
                dialogue.Add("I'm melting! Put a stop to this inclement weather this instant before it ruins my hair!");

            if (DownedBossSystem.downedPolterghast)
                dialogue.Add("I saw a ghost down by the old train tracks back at my homeland once, flailing wildly at the lily pads... frightening times those were.");

            if (DownedBossSystem.downedDoG)
                dialogue.Add("I hear it's amazing when the famous Devourer of Gods out in flap-jaw space, with the tuning fork, does a raw blink on Hara-kiri rock. I need scissors! 61!");

            int tavernKeep = NPC.FindFirstNPC(NPCID.DD2Bartender);
            if (tavernKeep != -1)
            {
                dialogue.Add("I've had to tell baldie where my eyes are so many times that I've lost count.");
                dialogue.Add("Tell " + Main.npc[tavernKeep].GivenName + " to stop calling me. He's not wanted.");
                dialogue.Add("My booze will always be better than " + Main.npc[tavernKeep].GivenName + "'s, and nobody can convince me otherwise.");
            }

            int permadong = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (permadong != -1)
                dialogue.Add("I never realized how well-endowed " + Main.npc[permadong].GivenName + " was. It had to be the largest icicle I'd ever seen.");

            int witch = NPC.FindFirstNPC(ModContent.NPCType<WITCH>());
            if (witch != -1)
                dialogue.Add("The abuse " + Main.npc[witch].GivenName + " went through is something I can hardly comprehend. I'd offer her a drink, but I don't think she'd enjoy it.");

            if (Main.player[Main.myPlayer].Calamity().chibii)
                dialogue.Add("The hell is that? Looks like something I'd carry around if I was 5 years old.");

            if (Main.player[Main.myPlayer].Calamity().aquaticHeart && !Main.player[Main.myPlayer].Calamity().aquaticHeartHide)
                dialogue.Add("Nice scales... is it hot in here or is it just me?");

            if (Main.player[Main.myPlayer].Calamity().fabsolVodka)
                dialogue.Add("Do you like my vodka? I created it by mixing fairy dust, crystallized cave sweat and other magical crap.");

            if (Main.player[Main.myPlayer].HasItem(ModContent.ItemType<Fabsol>()))
            {
                dialogue.Add("So... you found my special bottle. Hope you enjoy it, I know I will.");
                dialogue.Add("Be sure to dismount me once in a while, I get tired. And besides, I can't rip you off-I mean offer you excellent deals you won't find anywhere else if you're riding me 24/7.");
                dialogue.Add("Before you ask, no, I do NOT have a heart on my butt while in human form. Don't question my transformation preferences!");
            }

            return dialogue[Main.rand.Next(dialogue.Count)];
        }

        public string Death()
        {
            int deaths = Main.player[Main.myPlayer].Calamity().deathCount;

            string text = "You have failed " + Main.player[Main.myPlayer].Calamity().deathCount +
                (Main.player[Main.myPlayer].Calamity().deathCount == 1 ? " time." : " times.");

            if (deaths > 10000)
                text += " Congratulations! You are now, officially, the biggest loser in Terraria's history! Who was number two? Hell if I know.";
            else if (deaths > 5000)
                text += " I'm not sure what to say this time. That you're bad and should feel bad? That much was known already.";
            else if (deaths > 2500)
                text += " Bless your heart. I could dodge better than you even if I were drunk high.";
            else if (deaths > 1000)
                text += " It is said the average Terrarian has a lifespan of 2 minutes or less. ...Well, not really, but I feel like you'd be part of that statistic.";
            else if (deaths > 500)
                text += " Your inability to avoid dying to even the most basic of attacks is astonishing to me.";
            else if (deaths > 250)
                text += " I admire your tenacity. Keep it up, your enemies are racking up quite the kill count!";
            else if (deaths > 100)
                text += " Consider lowering the difficulty. If you found that statement irritating, good.";

            IList<string> donorList = new List<string>(CalamityLists.donatorList);
            int maxDonorsListed = 25;
            string[] donors = new string[maxDonorsListed];
            for (int i = 0; i < maxDonorsListed; i++)
            {
                donors[i] = donorList[Main.rand.Next(donorList.Count)];
                donorList.Remove(donors[i]);
            }

            text += ("\n\nHey " + donors[0] + ", " + donors[1] + ", " + donors[2] + ", " + donors[3] + ", " + donors[4] + ", " + donors[5] + ", " + donors[6] +
                ", " + donors[7] + ", " + donors[8] + ", " + donors[9] + ", " + donors[10] + ", " + donors[11] + ", " + donors[12] + ", " + donors[13] +
                ", " + donors[14] + ", " + donors[15] + ", " + donors[16] + ", " + donors[17] + ", " + donors[18] + ", " + donors[19] + ", " + donors[20] +
                ", " + donors[21] + ", " + donors[22] + ", " + donors[23] + " and " + donors[24] + "! You're all pretty good!");

            return text;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = "Death Count + Donors";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                Main.LocalPlayer.Calamity().newCirrusInventory = false;
                shopName = "Shop";
            }
            else
            {
                Main.npcChatText = Death();
            }
        }

        public override void AddShops()
        {
            Condition potionSells = new("While the Town NPC Potion Selling configuration option is enabled", () => CalamityConfig.Instance.PotionSelling);
            Condition downedAureus = new("If Astrum Aureus has been defeated", () => DownedBossSystem.downedAstrumAureus);

            NPCShop shop = new(Type);
            shop.AddWithCustomValue(ItemID.HeartreachPotion, Item.buyPrice(gold: 4), potionSells, Condition.HappyEnough)
                .AddWithCustomValue(ItemID.LifeforcePotion, Item.buyPrice(gold: (NPC.downedMoonlord ? 16 : Main.hardMode ? 8 : 4)), potionSells, Condition.HappyEnough)
                .AddWithCustomValue(ItemID.LovePotion, Item.buyPrice(silver: 25), potionSells, Condition.HappyEnough)
                .AddWithCustomValue(ModContent.ItemType<GrapeBeer>(), Item.buyPrice(silver: 30))
                .AddWithCustomValue(ModContent.ItemType<RedWine>(), Item.buyPrice(gold: 1))
                .AddWithCustomValue(ModContent.ItemType<Whiskey>(), Item.buyPrice(gold: 2))
                .AddWithCustomValue(ModContent.ItemType<Rum>(), Item.buyPrice(gold: 2))
                .AddWithCustomValue(ModContent.ItemType<Tequila>(), Item.buyPrice(gold: 2))
                .AddWithCustomValue(ModContent.ItemType<Fireball>(), Item.buyPrice(gold: 3))
                .AddWithCustomValue(ModContent.ItemType<FabsolsVodka>(), Item.buyPrice(gold: 4))
                .AddWithCustomValue(ModContent.ItemType<Vodka>(), Item.buyPrice(gold: 2), Condition.DownedMechBossAll)
                .AddWithCustomValue(ModContent.ItemType<Screwdriver>(), Item.buyPrice(gold: 6), Condition.DownedMechBossAll)
                .AddWithCustomValue(ModContent.ItemType<WhiteWine>(), Item.buyPrice(gold: 6), Condition.DownedMechBossAll)
                .AddWithCustomValue(ModContent.ItemType<EvergreenGin>(), Item.buyPrice(gold: 8), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<CaribbeanRum>(), Item.buyPrice(gold: 8), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<Margarita>(), Item.buyPrice(gold: 8), Condition.DownedPlantera)
                .AddWithCustomValue(ItemID.EmpressButterfly, Item.buyPrice(gold: 10), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<Everclear>(), Item.buyPrice(gold: 3), downedAureus)
                .AddWithCustomValue(ModContent.ItemType<BloodyMary>(), Item.buyPrice(gold: 4), downedAureus, Condition.BloodMoon)
                .AddWithCustomValue(ModContent.ItemType<StarBeamRye>(), Item.buyPrice(gold: 6), downedAureus, Condition.TimeNight)
                .AddWithCustomValue(ModContent.ItemType<Moonshine>(), Item.buyPrice(gold: 2), Condition.DownedGolem)
                .AddWithCustomValue(ModContent.ItemType<MoscowMule>(), Item.buyPrice(gold: 8), Condition.DownedGolem)
                .AddWithCustomValue(ModContent.ItemType<CinnamonRoll>(), Item.buyPrice(gold: 8), Condition.DownedGolem)
                .AddWithCustomValue(ModContent.ItemType<TequilaSunrise>(), Item.buyPrice(gold: 10), Condition.DownedGolem)
                .AddWithCustomValue(ItemID.BloodyMoscato, Item.buyPrice(gold: 1), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ItemID.BananaDaiquiri, Item.buyPrice(silver: 75), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ItemID.PeachSangria, Item.buyPrice(silver: 50), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ItemID.PinaColada, Item.buyPrice(gold: 1), Condition.DownedMoonLord, Condition.NpcIsPresent(NPCID.Stylist))
                .AddWithCustomValue(ModContent.ItemType<WeightlessCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<VigorousCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<ResilientCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<SpitefulCandle>(), Item.buyPrice(gold: 50))
                .AddWithCustomValue(ModContent.ItemType<OddMushroom>(), Item.buyPrice(1))
                .AddWithCustomValue(ItemID.UnicornHorn, Item.buyPrice(0, 2, 50), Condition.HappyEnough, Condition.InHallow)
                .AddWithCustomValue(ItemID.Milkshake, Item.buyPrice(gold: 5), Condition.HappyEnough, Condition.InHallow, Condition.NpcIsPresent(NPCID.Stylist))
                .Register();
        }

        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].type == ItemID.HeartreachPotion && NPC.downedMoonlord)
                {
                    items[i].shopCustomPrice = Item.buyPrice(gold: 8);
                }
            }
        }

        // Make this Town NPC teleport to the Queen statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 15;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 180;
            randExtraCooldown = 60;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<FabRay>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 11.5f;
        }
    }
}
