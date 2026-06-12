using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Placeables.FurnitureDriftwood;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        #region Modify Fishing Attempt
        public override void ModifyFishingAttempt(ref FishingAttempt attempt)
        {
            if (enchantedPearl)
            {
                // If the player fails to roll a crate naturally and has Enchanted Pearl, reroll
                // Stacking with existing probability, it should increase chances by 15% (12.5% with Crate Potion)
                if (!attempt.crate)
                    attempt.crate = Main.rand.NextBool(6);

                // If the player does get a crate (can be from the reroll above), give it increased chances of being rarer
                if (attempt.crate)
                {
                    int uncommonRate = Math.Clamp(240 / attempt.fishingLevel, 3, 240); // Iron/Mythril (originally 300)
                    attempt.uncommon = Main.rand.NextBool(uncommonRate);

                    // These roll result bools are individually stored for the rarifying visuals
                    int rareRate = Math.Clamp(840 / attempt.fishingLevel, 4, 840); // Biome (originally 1050)
                    bool rareRoll = Main.rand.NextBool(rareRate);
                    attempt.rare = rareRoll;

                    int veryRareRate = Math.Clamp(1800 / attempt.fishingLevel, 5, 1800); // Golden/Titanium (originally 2250)
                    bool veryRareRoll = Main.rand.NextBool(veryRareRate);
                    attempt.veryrare = veryRareRoll;

                    Vector2 basePos = new Vector2(attempt.X * 16f, attempt.Y * 16f);
                    if (rareRoll || veryRareRoll)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 position = basePos + Vector2.UnitX * Main.rand.NextFloat(-16f, 16f);
                            Vector2 velocity = (Vector2.UnitY * Main.rand.NextFloat(-12f, -9f)).RotatedByRandom(MathHelper.ToRadians(18f));
                            float scale = Main.rand.NextFloat(0.5f, 1.5f);
                            CritSpark spark = new(position, velocity, Color.White, Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f), scale, 36, 0.2f, scale * 2f);
                            GeneralParticleHandler.SpawnParticle(spark);
                        }
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 position = basePos + Vector2.UnitX * Main.rand.NextFloat(-16f, 16f);
                        Vector2 velocity = (Vector2.UnitY * Main.rand.NextFloat(-3f, -0.5f)).RotatedByRandom(MathHelper.ToRadians(12f));
                        PearlParticle pearl = new(position, velocity, false, 24, Main.rand.NextFloat(0.5f, 1f), Main.hslToRgb(Main.rand.NextFloat(), 1f, (rareRoll || veryRareRoll) ? 0.75f : 1f));
                        GeneralParticleHandler.SpawnParticle(pearl);
                    }
                }
            }
        }
        #endregion

        public override bool? CanConsumeBait(Item bait)
        {
            if (bait.type == ModContent.ItemType<BloodwormItem>())
                return true;
            return null;
        }

        #region Catch Fish
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            // If vanilla catches an enemy, just immediately cancel
            if (npcSpawn > 0)
                return;

            int bait = attempt.playerFishingConditions.BaitItemType;
            int power = attempt.fishingLevel;
            int questFish = attempt.questFish;
            int poolSize = attempt.waterTilesCount;
            bool lava = attempt.inLava;
            bool honey = attempt.inHoney;

            bool sky = attempt.heightLevel == 0;
            bool surface = attempt.heightLevel == 1;
            bool underground = attempt.heightLevel == 2;
            bool cavern = attempt.heightLevel == 3;
            bool underworld = attempt.heightLevel == 4;

            // Custom rate; increased by Enchanted Pearl
            bool grabBagFish = attempt.uncommon && Main.rand.Next(100) < (enchantedPearl ? 30 : 15);

            // Set up for allowing fishing in the Sulphurous Sea
            Point point = Player.Center.ToTileCoordinates();
            bool canSulphurFish = false;
            if (Abyss.AtLeftSideOfWorld)
            {
                if (point.X < 380)
                    canSulphurFish = true;
            }
            else
            {
                if (point.X > Main.maxTilesX - 380)
                    canSulphurFish = true;
            }
            if (ZoneAbyss || ZoneSulphur)
                canSulphurFish = true;

            // Fishing in lava overrides the rest of logic
            if (lava)
            {
                // Don't do anything if you can't fish in lava
                if (!attempt.CanFishInLava)
                    return;

                if (ZoneCalamity)
                {
                    // Crates have highest priority
                    if (attempt.crate && attempt.rare)
                        itemDrop = Main.hardMode ? ModContent.ItemType<BrimstoneCrate>() : ModContent.ItemType<SlagCrate>();
                    else if (attempt.legendary)
                        itemDrop = ModContent.ItemType<DragoonDrizzlefish>();
                    else if (attempt.veryrare)
                        itemDrop = ModContent.ItemType<CharredLasher>();
                    else if (DownedBossSystem.downedProvidence && ((attempt.rare && Main.rand.NextBool(2)) || (attempt.uncommon && Main.rand.NextBool(4))))
                        itemDrop = ModContent.ItemType<Bloodfin>();
                    // Quest fish hover around this priority
                    else if (questFish == ModContent.ItemType<Brimlish>() && attempt.uncommon)
                        itemDrop = ModContent.ItemType<Brimlish>();
                    else if (questFish == ModContent.ItemType<Slurpfish>() && attempt.uncommon)
                        itemDrop = ModContent.ItemType<Slurpfish>();
                    else if (questFish == ModContent.ItemType<Havocfish>() && attempt.uncommon)
                        itemDrop = ModContent.ItemType<Havocfish>();
                    else if (attempt.rare)
                    {
                        List<int> uncommonCatches = new List<int>()
                        {
                            ModContent.ItemType<CoastalDemonfish>(),
                            ModContent.ItemType<Shadowfish>()
                        };
                        itemDrop = uncommonCatches[Main.rand.Next(uncommonCatches.Count)];
                    }
                    // Lava fish usually don't have plentiful catches but we can be more lenient
                    else
                        itemDrop = ModContent.ItemType<CragBullhead>();
                }
                return;
            }

            // Honey also overrides logic but we have nothing to catch from honey... for now
            if (honey)
                return;

            // Old Duke spawn
            if (canSulphurFish && bait == ModContent.ItemType<BloodwormItem>() && !BossRushEvent.BossRushActive)
            {
                if (!Main.projectile.Any(x => x.active && x.aiStyle == ProjAIStyleID.Bobber && x.ai[1] != 0 && x.localAI[1] == ModContent.NPCType<OldDuke>() * -1))
                    npcSpawn = ModContent.NPCType<OldDuke>();
                itemDrop = -1;
                sonar.Text = "";
                return;
            }

            if (attempt.playerFishingConditions.PoleItemType == ModContent.ItemType<WulfrumRod>())
            {
                if (Main.rand.NextBool(5))
                {
                    itemDrop = ModContent.ItemType<WulfrumMetalScrap>();
                    return;
                }
                if (Main.rand.NextBool(15))
                {
                    itemDrop = ModContent.ItemType<EnergyCore>();
                    return;
                }

                if (Main.rand.NextBool(50))
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            itemDrop = ModContent.ItemType<RoverDrive>();
                            return;
                        case 1:
                            itemDrop = ModContent.ItemType<WulfrumBattery>();
                            return;
                        case 2:
                            itemDrop = ModContent.ItemType<AbandonedWulfrumHelmet>();
                            return;
                    }
                    return;
                }
            }

            // Ignore catches if it's junk
            if (itemDrop == ItemID.OldShoe || itemDrop == ItemID.FishingSeaweed || itemDrop == ItemID.TinCan || itemDrop == ItemID.JojaCola)
                return;

            if (attempt.crate)
            {
                // This can override Golden/Titanium Crates, but so do vanilla biome crates
                // Those are still obtainable if the catch is very rare/legendary but not rare (fishing oddities)
                if (attempt.rare)
                {
                    if (ZoneAstral)
                        itemDrop = Main.hardMode ? ModContent.ItemType<AstralCrate>() : ModContent.ItemType<MonolithCrate>();
                    if (ZoneSunkenSea)
                        itemDrop = Main.hardMode ? ModContent.ItemType<PrismCrate>() : ModContent.ItemType<EutrophicCrate>();
                    if (canSulphurFish)
                        itemDrop = Main.hardMode ? ModContent.ItemType<HydrothermalCrate>() : ModContent.ItemType<SulphurousCrate>();
                }
                return;
            }

            // Ignore all top priority legendary vanilla catches
            List<int> keepCatchList = new List<int>()
            {
                ItemID.CombatBook,
                ItemID.DreadoftheRedSea,
                ItemID.FrogLeg,
                ItemID.BalloonPufferfish,
                ItemID.ZephyrFish
            };
            if (keepCatchList.Contains(itemDrop))
                return;

            // Add top priorities of our own
            if (DownedBossSystem.downedLeviathan && attempt.legendary && poolSize > 1000 && !Main.rand.NextBool(3))
            {
                itemDrop = ModContent.ItemType<Floodtide>();
                return;
            }
            //Rare abyss catches are replaced with abyss chest items post-skeletron
            if (NPC.downedBoss3 && (ZoneAbyssLayer2 || ZoneAbyssLayer3 || ZoneAbyssLayer4) && attempt.rare)
            {
                switch (Main.rand.Next(10))
                {
                    case 0:
                        itemDrop = ModContent.ItemType<Lionfish>();
                        return;
                    case 1:
                        itemDrop = ModContent.ItemType<HerringStaff>();
                        return;
                    case 2:
                        itemDrop = ModContent.ItemType<BallOFugu>();
                        return;
                    case 3:
                        itemDrop = ModContent.ItemType<BlackAnurian>();
                        return;
                    case 4:
                        itemDrop = ModContent.ItemType<Archerfish>();
                        return;
                    case 5:
                        itemDrop = ModContent.ItemType<AnechoicPlating>();
                        return;
                    case 6:
                        itemDrop = ModContent.ItemType<IronBoots>();
                        return;
                    case 7:
                        itemDrop = ModContent.ItemType<DepthCharm>();
                        return;
                    case 8:
                        itemDrop = ModContent.ItemType<StrangeOrb>();
                        return;
                    case 9:
                        itemDrop = ModContent.ItemType<TorrentialTear>();
                        return;
                }
            }

            // Quest fish
            if (sky && questFish == ModContent.ItemType<SunbeamFish>() && attempt.uncommon)
                itemDrop = ModContent.ItemType<SunbeamFish>();
            if (Player.ZoneSnow && questFish == ModContent.ItemType<FishofEleum>() && attempt.uncommon)
                itemDrop = ModContent.ItemType<FishofEleum>();

            if (grabBagFish)
            {
                if (surface && Main.bloodMoon)
                    itemDrop = ModContent.ItemType<Gorecodile>();
                else if (surface && Main.dayTime)
                    itemDrop = ModContent.ItemType<StuffedFish>();
                else if (cavern)
                    itemDrop = ModContent.ItemType<GlimmeringGemfish>();
                if (Main.hardMode && sky)
                    itemDrop = ModContent.ItemType<FishofFlight>();
            }

            // Increased chance of Enchanted Starfish if you don't have maximum mana
            if (surface && !Main.dayTime) // Surface
            {
                int chance = (Player.ConsumedManaCrystals >= Player.ManaCrystalMax) ? 20 : 5;
                if (attempt.uncommon && Main.rand.NextBool(chance))
                    itemDrop = ModContent.ItemType<EnchantedStarfish>();

                if (attempt.uncommon && Main.rand.NextBool(10))
                    itemDrop = ModContent.ItemType<Shadowfish>();
            }

            // Lower chance of Spadefish in Hardmode
            if (underground) // Underground
            {
                int chance = Main.hardMode ? 10 : 4;
                if (attempt.veryrare && Main.rand.NextBool(chance))
                {
                    itemDrop = ModContent.ItemType<Spadefish>();
                }
            }

            if (ZoneAstral)
            {
                if (attempt.legendary)
                {
                    int legendaryCatch = Utils.SelectRandom(Main.rand, new int[]
                    {
                        ModContent.ItemType<PolarisParrotfish>(),
                        ModContent.ItemType<GacruxianMollusk>(),
                        ModContent.ItemType<UrsaSergeant>()
                    });
                    itemDrop = legendaryCatch;
                }
                else if (attempt.veryrare)
                    itemDrop = ModContent.ItemType<ArcturusAstroidean>();
                else if (attempt.uncommon || attempt.rare)
                {
                    int uncommonCatch = Utils.SelectRandom(Main.rand, new int[]
                    {
                        ModContent.ItemType<ProcyonidPrawn>(),
                        ModContent.ItemType<AldebaranAlewife>()
                    });
                    itemDrop = uncommonCatch;
                }
                else
                    itemDrop = ModContent.ItemType<TwinklingPollox>();
                return;
            }
            if (ZoneSunkenSea)
            {
                // If the player is overlapping with the desert, split the catches
                if (Player.ZoneDesert && Main.rand.NextBool())
                    return;

                if (attempt.legendary)
                {
                    List<int> legendaryCatches =
                    [
                        ModContent.ItemType<RustedJingleBell>()
                    ];

                    legendaryCatches.AddWithCondition<int>(ModContent.ItemType<SerpentsBite>(), Main.hardMode);
                    itemDrop = legendaryCatches[Main.rand.Next(legendaryCatches.Count)];
                }
                else if (attempt.veryrare)
                {
                    List<int> veryRareCatches =
                    [
                        ModContent.ItemType<GreenwaveLoach>()
                    ];

                    veryRareCatches.AddWithCondition<int>(ModContent.ItemType<SparklingEmpress>(), DownedBossSystem.downedDesertScourge);
                    veryRareCatches.AddWithCondition<int>(ModContent.ItemType<SeaSpiritAmulet>(), DownedBossSystem.downedDesertScourge);
                    itemDrop = veryRareCatches[Main.rand.Next(veryRareCatches.Count)];
                }
                // Quest fish hover around this priority
                else if (questFish == ModContent.ItemType<EutrophicSandfish>() && attempt.uncommon)
                    itemDrop = ModContent.ItemType<EutrophicSandfish>();
                else if (questFish == ModContent.ItemType<SurfClam>() && attempt.uncommon)
                    itemDrop = ModContent.ItemType<SurfClam>();
                else if (questFish == ModContent.ItemType<Serpentuna>() && attempt.uncommon)
                    itemDrop = ModContent.ItemType<Serpentuna>();
                else if (attempt.uncommon || attempt.rare)
                    itemDrop = ModContent.ItemType<SunkenSailfish>();
                else if (Main.rand.NextBool()) // 50% chance the common fish is replaced with driftwood
                    itemDrop = ModContent.ItemType<Driftwood>();
                else
                    itemDrop = ModContent.ItemType<PrismaticGuppy>();
                return;
            }
            // There is no complete fishing pool here, so most of it is vanilla default
            if (canSulphurFish)
            {
                if (attempt.legendary)
                {
                    itemDrop = ModContent.ItemType<AlluringBait>();
                }
                else if (attempt.common && Main.rand.NextBool())
                    itemDrop = ModContent.ItemType<PlantyMush>();
            }
        }
        #endregion

        #region Get Fishing Level
        public override void GetFishingLevel(Item fishingRod, Item bait, ref float fishingLevel)
        {
            // Note: This is calculated after equipments (Rod, Bait, Potions, etc) and before modifications (Chum Buckets, Luck)
            if ((ZoneAstral || ZoneAbyss || ZoneSulphur) && bait.type == ModContent.ItemType<ArcturusAstroidean>())
                fishingLevel = fishingLevel * ArcturusAstroidean.FishingPowerBiomeMult;
            if (Player.ZoneSnow && fishingRod.type == ModContent.ItemType<VerstaltiteFishingRod>())
                fishingLevel = fishingLevel * VerstaltiteFishingRod.FishingPowerBiomeMult;
            if (Player.ZoneSkyHeight && fishingRod.type == ModContent.ItemType<HeronRod>())
                fishingLevel = fishingLevel * HeronRod.FishingPowerBiomeMult;

            // Rage bait gives free sonar effect
            if (bait.type == ModContent.ItemType<RageBait>())
                Player.sonarPotion = true;

            // Prevent the player from fishing if they have the Bloodworm
            if (bait.type == ModContent.ItemType<BloodwormItem>())
            {
                Point point = Player.Center.ToTileCoordinates();
                bool canSulphurFish = false;
                if (Abyss.AtLeftSideOfWorld)
                {
                    if (point.X < 380)
                        canSulphurFish = true;
                }
                else
                {
                    if (point.X > Main.maxTilesX - 380)
                        canSulphurFish = true;
                }

                if (ZoneAbyss || ZoneSulphur)
                    canSulphurFish = true;

                Item item = Player.HeldItem;
                if (!canSulphurFish || item.fishingPole <= 0 || item.holdStyle != 1)
                    fishingLevel = -1;

                // Set Fisherman's Pocket Guide to display "Warning!" with Bloodworm as bait
                // This runs only while there is a fishing bobber; logic with no fishing bobber is handled in the ModifyDisplayParameters hook in the separate class below
                Player.displayedFishingInfo = Language.GetTextValue("GameUI.FishingWarning");
            }
        }
        #endregion

        #region Modify Caught Fish
        public override void ModifyCaughtFish(Item fish)
        {
            // Increases yeild of driftwood from the Sunken Sea
            // ~7% chance that yeild is very high so that exhaustive fishing can allow for enough driftwood to make large builds
            if (fish.type == ModContent.ItemType<Driftwood>())
                fish.stack = ((Main.rand.NextBool(14) ? 20 : 1) * Main.rand.Next(8, 20 + 1));
            // Increases the yield of potion ingredient fish with Alluring Bait
            if (alluringBait)
            {
                List<int> fishList = new List<int>()
                {
                    ItemID.FlarefinKoi,
                    ItemID.Obsidifish,
                    ItemID.ArmoredCavefish,
                    ItemID.Stinkfish,
                    ItemID.SpecularFish,
                    ItemID.ChaosFish,
                    ItemID.VariegatedLardfish,
                    ItemID.DoubleCod,
                    ItemID.FrostMinnow,
                    ItemID.Ebonkoi,
                    ItemID.CrimsonTigerfish,
                    ItemID.Hemopiranha,
                    ItemID.PrincessFish,
                    ItemID.Prismite,
                    ItemID.Damselfish,
                    ModContent.ItemType<CoastalDemonfish>(),
                    ModContent.ItemType<Shadowfish>(),
                    ModContent.ItemType<AldebaranAlewife>(),
                    ModContent.ItemType<SunkenSailfish>(),
                };

                if (fishList.Contains(fish.type))
                    fish.stack += Main.rand.Next(1, 3 + 1);
            }
            if (fish.type == ModContent.ItemType<WulfrumMetalScrap>())
                fish.stack = Main.rand.Next(1, 6);
        }
        #endregion
    }

    public class BloodwormFishPowerWarning : GlobalInfoDisplay
    {
        // Set Fisherman's Pocket Guide to display "Warning!" with Bloodworm as bait
        // This runs only while there is no fishing bobber; logic with a fishing bobber is handled in the GetFishingLevel hook above
        public override void ModifyDisplayParameters(InfoDisplay currentDisplay, ref string displayValue, ref string displayName, ref Color displayColor, ref Color displayShadowColor)
        {
            if (currentDisplay == InfoDisplay.FishFinder)
            {
                foreach (Projectile p in Main.ActiveProjectiles)
                {
                    if (p.owner == Main.myPlayer && p.bobber)
                        return;
                }

                if (Main.LocalPlayer.GetFishingConditions().BaitItemType == ModContent.ItemType<BloodwormItem>())
                    displayValue = Language.GetTextValue("GameUI.FishingWarning");
            }
        }
    }
}
