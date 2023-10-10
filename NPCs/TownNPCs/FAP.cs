using CalamityMod.Events;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class FAP : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 400;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 60;
            NPCID.Sets.AttackAverageChance[NPC.type] = 15;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
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
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,                
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.FAP")
            });
        }

        public override void AI()
        {
            if (!CalamityWorld.spawnedCirrus)
                CalamityWorld.spawnedCirrus = true;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas.SupremeCalamitas>()) && Main.zenithWorld)
                return false;

            if (CalamityWorld.spawnedCirrus)
                return true;

            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
                bool hasVodka = player.InventoryHas(ModContent.ItemType<FabsolsVodka>()) || player.PortableStorageHas(ModContent.ItemType<FabsolsVodka>());
                if (player.active && hasVodka)
                    return Main.hardMode;
            }
            return false;
        }

		public override List<string> SetNPCNameList() => new List<string>() { this.GetLocalizedValue("Name.Cirrus") };

        public override string GetChat()
        {
            Player player = Main.player[Main.myPlayer];
            if (Main.zenithWorld)
            {
                player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.CirrusSlap" + Main.rand.Next(1, 2 + 1)).Format(player.name)), player.statLife / 2, -player.direction, false, false, -1, false);
                SoundEngine.PlaySound(CnidarianJellyfishOnTheString.SlapSound, player.Center);
            }

            if (CalamityUtils.AnyBossNPCS())
                return this.GetLocalizedValue("Chat.BossAlive");

            if (NPC.homeless)
                return this.GetLocalizedValue("Chat.Homeless" + Main.rand.Next(1, 2 + 1));

            int wife = NPC.FindFirstNPC(NPCID.Stylist);
            bool wifeIsAround = wife != -1;
            bool beLessDrunk = wifeIsAround && NPC.downedMoonlord;

            if (Main.bloodMoon)
            {
                if (Main.rand.NextBool(4))
                {
                    player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.CirrusSlap" + Main.rand.Next(1, 2 + 1)).Format(player.name)), player.statLife / 2, -player.direction, false, false, -1, false); ;
                    SoundEngine.PlaySound(CnidarianJellyfishOnTheString.SlapSound, player.Center);
                    return this.GetLocalizedValue("Chat.BloodMoonSlap");
                }
                return this.GetLocalizedValue("Chat.BloodMoon" + Main.rand.Next(1, 3 + 1));
            }

            WeightedRandom<string> dialogue = new WeightedRandom<string>();

            dialogue.Add(this.GetLocalizedValue("Chat.Normal1"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal2"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal3"));
            if (ChildSafety.Disabled)
                dialogue.Add(this.GetLocalizedValue("Chat.Normal4"));

            int tavernKeep = NPC.FindFirstNPC(NPCID.DD2Bartender);
            if (tavernKeep != -1)
            {
                dialogue.Add(this.GetLocalization("Chat.Tavernkeep1").Format(Main.npc[tavernKeep].GivenName));
                dialogue.Add(this.GetLocalization("Chat.Tavernkeep2").Format(Main.npc[tavernKeep].GivenName));

                if (ChildSafety.Disabled)
                    dialogue.Add(this.GetLocalizedValue("Chat.Tavernkeep3"));
            }

            int permadong = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (permadong != -1)
                dialogue.Add(this.GetLocalization("Chat.Archmage").Format(Main.npc[permadong].GivenName));

            int witch = NPC.FindFirstNPC(ModContent.NPCType<WITCH>());
            if (witch != -1)
                dialogue.Add(this.GetLocalization("Chat.BrimstoneWitch").Format(Main.npc[witch].GivenName));

            if (wifeIsAround)
            {
                dialogue.Add(this.GetLocalization("Chat.Stylist1").Format(Main.npc[wife].GivenName));
                if (ChildSafety.Disabled)
                {
                    dialogue.Add(this.GetLocalization("Chat.Stylist2").Format(Main.npc[wife].GivenName));
                    dialogue.Add(this.GetLocalization("Chat.Stylist3").Format(Main.npc[wife].GivenName));
                }
            }

            if (Main.dayTime)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Day1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day2"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day3"));
                dialogue.Add(this.GetLocalizedValue("Chat.Day4"));

                if (beLessDrunk)
                {
                    dialogue.Add(this.GetLocalization("Chat.DayStylist1").Format(Main.npc[wife].GivenName));
                    dialogue.Add(this.GetLocalization("Chat.DayStylist2").Format(Main.npc[wife].GivenName));
                }
                else
                {
                    dialogue.Add(this.GetLocalizedValue("Chat.DayDrunk1"));
                    dialogue.Add(this.GetLocalizedValue("Chat.DayDrunk2"));
                }
            }
            else
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Night1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night2"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night3"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night4"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night5"));

                if (wifeIsAround)
                    dialogue.Add(this.GetLocalization("Chat.NightStylist").Format(Main.npc[wife].GivenName));
            }

            if (BirthdayParty.PartyIsUp)
                dialogue.Add(this.GetLocalizedValue("Chat.Party"));

            if (AcidRainEvent.AcidRainEventIsOngoing)
                dialogue.Add(this.GetLocalizedValue("Chat.AcidRain"));

            if (Main.invasionType == InvasionID.MartianMadness)
                dialogue.Add(this.GetLocalizedValue("Chat.Martians"));

            if (DownedBossSystem.downedCryogen && ChildSafety.Disabled)
                dialogue.Add(this.GetLocalizedValue("Chat.CryogenDefeated"));

            if (DownedBossSystem.downedLeviathan)
                dialogue.Add(this.GetLocalizedValue("Chat.LeviathanDefeated"));

            if (NPC.downedMoonlord)
                dialogue.Add(this.GetLocalizedValue("Chat.MoonLordDefeated"));

            if (DownedBossSystem.downedPolterghast)
                dialogue.Add(this.GetLocalizedValue("Chat.PolterghastDefeated"));

            if (DownedBossSystem.downedDoG)
                dialogue.Add(this.GetLocalizedValue("Chat.DoGDefeated"));

            if (player.Calamity().chibii)
                dialogue.Add(this.GetLocalizedValue("Chat.HasChibii"));

            if (player.Calamity().aquaticHeart && !player.Calamity().aquaticHeartHide && ChildSafety.Disabled)
                dialogue.Add(this.GetLocalizedValue("Chat.HasAnahitaTrans"));

            if (player.Calamity().fabsolVodka)
                dialogue.Add(this.GetLocalizedValue("Chat.HasVodka"));

            if (player.HasItem(ModContent.ItemType<Fabsol>()))
            {
                dialogue.Add(this.GetLocalizedValue("Chat.HasAlicorn1"));
                dialogue.Add(this.GetLocalizedValue("Chat.HasAlicorn2"));
                if (ChildSafety.Disabled)
                    dialogue.Add(this.GetLocalizedValue("Chat.HasAlicorn3"));
            }

            return dialogue;
        }

        public string Death()
        {
            int deaths = Main.player[Main.myPlayer].numberOfDeathsPVE;

            string text = this.GetLocalization("DeathCount").Format(deaths);

            if (deaths > 10000)
                text += " " + this.GetLocalizedValue("Death10000");
            else if (deaths > 5000)
                text += " " + this.GetLocalizedValue("Death5000");
            else if (deaths > 2500)
                text += " " + this.GetLocalizedValue("Death2500");
            else if (deaths > 1000)
                text += " " + this.GetLocalizedValue("Death1000");
            else if (deaths > 500)
                text += " " + this.GetLocalizedValue("Death500");
            else if (deaths > 250)
                text += " " + this.GetLocalizedValue("Death250");
            else if (deaths > 100)
                text += " " + this.GetLocalizedValue("Death100");

            IList<string> donorList = new List<string>(CalamityLists.donatorList);
            int maxDonorsListed = 25;
            string[] donors = new string[maxDonorsListed];
            for (int i = 0; i < maxDonorsListed; i++)
            {
                donors[i] = donorList[Main.rand.Next(donorList.Count)];
                donorList.Remove(donors[i]);
            }

            text += ("\n\n" + this.GetLocalization("DonorShoutout").Format(donors));

            return text;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = this.GetLocalizedValue("DeathCountButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Shop";
            }
            else
            {
                Main.npcChatText = Death();
            }
        }

        public override void AddShops()
        {
            Condition potionSells = new(CalamityUtils.GetText("Condition.PotionConfig"), () => CalamityConfig.Instance.PotionSelling);
            Condition downedAureus = new(CalamityUtils.GetText("Condition.PostAureus"), () => DownedBossSystem.downedAstrumAureus);

            NPCShop shop = new(Type);
            shop.AddWithCustomValue(ItemID.LovePotion, Item.buyPrice(silver: 25), potionSells, Condition.HappyEnough)
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
