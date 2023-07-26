using CalamityMod.Buffs.Summon;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.CraftingStations;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.SummonItems.Invasion;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Other;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Summon.Umbrella;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

using static CalamityMod.Downed;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod
{
    internal class Downed
    {
        public static readonly Func<bool> NotDownedCalClone = () => !DownedBossSystem.downedCalamitasClone;
        public static readonly Func<bool> DownedDesertScourge = () => DownedBossSystem.downedDesertScourge;
        public static readonly Func<bool> DownedGiantClam = () => DownedBossSystem.downedCLAM;
        public static readonly Func<bool> DownedCrabulon = () => DownedBossSystem.downedCrabulon;
        public static readonly Func<bool> DownedHiveMind = () => DownedBossSystem.downedHiveMind;
        public static readonly Func<bool> DownedPerfs = () => DownedBossSystem.downedPerforator;
        public static readonly Func<bool> DownedSlimeGod = () => DownedBossSystem.downedSlimeGod;
        public static readonly Func<bool> DownedCryogen = () => DownedBossSystem.downedCryogen;
        public static readonly Func<bool> DownedBrimstoneElemental = () => DownedBossSystem.downedBrimstoneElemental;
        public static readonly Func<bool> DownedAquaticScourge = () => DownedBossSystem.downedAquaticScourge;
        public static readonly Func<bool> DownedCragmawMire = () => DownedBossSystem.downedCragmawMire;
        public static readonly Func<bool> DownedCalClone = () => DownedBossSystem.downedCalamitasClone;
        public static readonly Func<bool> DownedGSS = () => DownedBossSystem.downedGSS;
        public static readonly Func<bool> DownedLeviathan = () => DownedBossSystem.downedLeviathan;
        public static readonly Func<bool> DownedAureus = () => DownedBossSystem.downedAstrumAureus;
        public static readonly Func<bool> DownedPBG = () => DownedBossSystem.downedPlaguebringer;
        public static readonly Func<bool> DownedRavager = () => DownedBossSystem.downedRavager;
        public static readonly Func<bool> DownedDeus = () => DownedBossSystem.downedAstrumDeus;
        public static readonly Func<bool> DownedGuardians = () => DownedBossSystem.downedGuardians;
        public static readonly Func<bool> DownedBirb = () => DownedBossSystem.downedDragonfolly;
        public static readonly Func<bool> DownedProvidence = () => DownedBossSystem.downedProvidence;
        public static readonly Func<bool> DownedCeaselessVoid = () => DownedBossSystem.downedCeaselessVoid;
        public static readonly Func<bool> DownedStormWeaver = () => DownedBossSystem.downedStormWeaver;
        public static readonly Func<bool> DownedSignus = () => DownedBossSystem.downedSignus;
        public static readonly Func<bool> DownedPolterghast = () => DownedBossSystem.downedPolterghast;
        public static readonly Func<bool> DownedMauler = () => DownedBossSystem.downedMauler;
        public static readonly Func<bool> DownedNuclearTerror = () => DownedBossSystem.downedNuclearTerror;
        public static readonly Func<bool> DownedBoomerDuke = () => DownedBossSystem.downedBoomerDuke;
        public static readonly Func<bool> DownedDoG = () => DownedBossSystem.downedDoG;
        public static readonly Func<bool> DownedYharon = () => DownedBossSystem.downedYharon;
        public static readonly Func<bool> DownedExoMechs = () => DownedBossSystem.downedExoMechs;
        public static readonly Func<bool> DownedSCal = () => DownedBossSystem.downedCalamitas;
        public static readonly Func<bool> DownedPrimordialWyrm = () => DownedBossSystem.downedPrimordialWyrm;

        public static readonly Func<bool> DownedAcidRainInitial = () => DownedBossSystem.downedEoCAcidRain;
        public static readonly Func<bool> DownedAcidRainHardmode = () => DownedBossSystem.downedAquaticScourgeAcidRain;
        public static readonly Func<bool> DownedBossRush = () => DownedBossSystem.downedBossRush;
    }

    internal class WeakReferenceSupport
    {
        public const string CalamityWikiURLOld = "calamitymod.wiki.gg";
        public const string CalamityWikiURL = "https://calamitymod.wiki.gg/wiki/{}";

        /// <summary>
        /// <b>Vanilla main bosses:</b><br />
        /// 1.0 = King Slime<br />
        /// 2.0 = Eye of Cthulhu<br />
        /// 3.0 = Eater of Worlds / Brain of Cthulhu<br />
        /// 4.0 = Queen Bee<br />
        /// 5.0 = Skeletron<br />
        /// 6.0 = Deerclops<br />
        /// 7.0 = Wall of Flesh<br />
        /// 8.0 = Queen Slime<br />
        /// 9.0 = The Twins<br />
        /// 10.0 = The Destroyer<br />
        /// 11.0 = Skeletron Prime<br />
        /// 12.0 = Plantera<br />
        /// 13.0 = Golem<br />
        /// 14.0 = Duke Fishron<br />
        /// 15.0 = Empress of Light<br />
        /// 16.0 = Betsy<br />
        /// 17.0 = Lunatic Cultist<br />
        /// 18.0 = Moon Lord
        /// </summary>
        private static readonly Dictionary<string, float> ModProgression = new Dictionary<string, float>
        {
            { "DesertScourge", 1.6f },
            { "GiantClam", 1.61f },
            { "AcidRainT1", 2.67f },
            { "Crabulon", 2.7f },
            { "HiveMind", 3.98f },
            { "Perforators", 3.99f },
            { "SlimeGod", 6.7f }, // Thorium Granite Energy Storm is 6.4f, Buried Champion is 6.5f, and Star Scouter is 6.9f
            { "Cryogen", 8.5f },
            { "AquaticScourge", 9.5f },
            { "AcidRainT2", 9.51f },
            { "CragmawMire", 9.52f },
            { "BrimstoneElemental", 10.5f },
            { "CalamitasClone", 11.7f }, // Thorium Lich is 11.6f
            { "GreatSandShark", 12.09f },
            { "Leviathan", 12.8f },
            { "AstrumAureus", 12.81f },
            { "PlaguebringerGoliath", 14.5f },
            { "Ravager", 16.5f },
            { "AstrumDeus", 17.5f },
            { "ProfanedGuardians", 18.5f },
            { "Dragonfolly", 18.6f },
            { "Providence", 19f }, // Thorium Primordials (Ragnarok) is 19.5f
            { "CeaselessVoid", 19.6f },
            { "StormWeaver", 19.61f },
            { "Signus", 19.62f },
            { "Polterghast", 20f },
            { "AcidRainT3", 20.49f },
            { "Mauler", 20.491f },
            { "NuclearTerror", 20.492f },
            { "OldDuke", 20.5f },
            { "DevourerofGods", 21f },
            { "Yharon", 22f },
            { "ExoMechs", 22.99f },
            { "Calamitas", 23f },
            // { "PrimordialWyrm", 23.5f },
            { "BossRush", 25.99f },
            // { "Yharim", 24f },
            // { "Noxus", 25f },
            // { "Xeroc", 26f },
        };

        public static void Setup()
        {
            BossChecklistSupport();
            FargosSupport();
            DialogueTweakSupport();
            SummonersAssociationSupport();
        }

        public static void WikiThisSupport()
        {
            // Wikithis is a clientside mod
            if (Main.netMode == NetmodeID.Server)
                return;

            CalamityMod calamity = GetInstance<CalamityMod>();
            Mod wiki = calamity.wikithis;
            if (wiki is null)
                return;

            bool oldVersion = wiki.Version < new Version(2, 4, 7, 5);

            wiki.Call("AddModURL", calamity, oldVersion ? CalamityWikiURLOld : CalamityWikiURL);
            wiki.Call(0, calamity, oldVersion ? CalamityWikiURLOld : CalamityWikiURL);
            wiki.Call("AddWikiTexture", calamity, ModContent.Request<Texture2D>("CalamityMod/ModSupport/WikiThisIcon"));
            wiki.Call(3, calamity, ModContent.Request<Texture2D>("CalamityMod/ModSupport/WikiThisIcon"));

            // Clear up name conflicts
            void ItemRedirect(int item, string pageName)
            {
                wiki.Call(1, item, pageName);
            }
            void EnemyRedirect(int item, string pageName)
            {
                wiki.Call(2, item, pageName);
            }

            // Items
            ItemRedirect(ModContent.ItemType<BloodOrange>(), "Blood Orange (calamity)");
            ItemRedirect(ModContent.ItemType<Elderberry>(), "Elderberry (calamity)");
            ItemRedirect(ModContent.ItemType<PineapplePet>(), "Pineapple (calamity)");
            ItemRedirect(ModContent.ItemType<TrashmanTrashcan>(), "Trash Can (pet)");
            ItemRedirect(ModContent.ItemType<Butcher>(), "Butcher (weapon)");
            ItemRedirect(ModContent.ItemType<SandstormGun>(), "Sandstorm (weapon)");
            ItemRedirect(ModContent.ItemType<Thunderstorm>(), "Thunderstorm (weapon)");
            // Lore items
            ItemRedirect(ModContent.ItemType<LoreAstralInfection>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreAbyss>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreAquaticScourge>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreArchmage>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreAstrumAureus>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreAstrumDeus>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreAwakening>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreAzafure>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreBloodMoon>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreBrainofCthulhu>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreBrimstoneElemental>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreCalamitas>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreCalamitasClone>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreCeaselessVoid>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreCorruption>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreCrabulon>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreCrimson>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreCynosure>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreDesertScourge>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreDestroyer>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreDevourerofGods>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreDragonfolly>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreDukeFishron>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreEaterofWorlds>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreEmpressofLight>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreExoMechs>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreEyeofCthulhu>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreGolem>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreHiveMind>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreKingSlime>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreLeviathanAnahita>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreMechs>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreOldDuke>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LorePerforators>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LorePlaguebringerGoliath>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LorePlantera>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LorePolterghast>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LorePrelude>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreProfanedGuardians>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreProvidence>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreQueenBee>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreQueenSlime>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreRavager>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreRequiem>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreSignus>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreSkeletron>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreSkeletronPrime>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreSlimeGod>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreStormWeaver>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreSulphurSea>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreTwins>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreUnderworld>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreWallofFlesh>(), "Lore#Lore_Items");
            ItemRedirect(ModContent.ItemType<LoreYharon>(), "Lore#Lore_Items");

            // Enemies
            EnemyRedirect(ModContent.NPCType<HiveEnemy>(), "Hive (enemy)");
            EnemyRedirect(ModContent.NPCType<KingSlimeJewel>(), "Crown Jewel (enemy)");
            EnemyRedirect(ModContent.NPCType<OldDukeToothBall>(), "Tooth Ball (Old Duke)");
            EnemyRedirect(ModContent.NPCType<CalamitasEnchantDemon>(), "Enchantment");
            EnemyRedirect(ModContent.NPCType<LeviathanStart>(), "%3F%3F%3F");
        }

        // Wrapper function to detect if a subworld is in use for Subworld Library.
        internal static bool InAnySubworld()
        {
            if (CalamityMod.Instance.subworldLibrary is null)
                return false;

            foreach (Mod mod in ModLoader.Mods)
            {
                if (mod.Name.Equals(CalamityMod.Instance.subworldLibrary.Name))
                    continue;

                bool anySubworldForMod = (CalamityMod.Instance.subworldLibrary.Call("AnyActive", mod) as bool?) ?? false;
                if (anySubworldForMod)
                    return true;
            }
            return false;
        }

        // Wrapper function to add bosses to Boss Checklist.
        private static void AddBoss(Mod bossChecklist, Mod hostMod, string name, float difficulty, object npcTypes, Func<bool> downed, Dictionary<string, object> extraInfo)
            => bossChecklist.Call("LogBoss", hostMod, name, difficulty, downed, npcTypes, extraInfo);

        // Wrapper function to add minibosses to Boss Checklist.
        private static void AddMiniBoss(Mod bossChecklist, Mod hostMod, string name, float difficulty, int npcType, Func<bool> downed, Dictionary<string, object> extraInfo)
            => bossChecklist.Call("LogMiniBoss", hostMod, name, difficulty, downed, npcType, extraInfo);

        // Wrapper function to add events to Boss Checklist.
        private static void AddInvasion(Mod bossChecklist, Mod hostMod, string name, float difficulty, List<int> npcTypes, Func<bool> downed, Dictionary<string, object> extraInfo)
            => bossChecklist.Call("LogEvent", hostMod, name, difficulty, downed, npcTypes, extraInfo);

        private static void BossChecklistSupport()
        {
            CalamityMod calamity = GetInstance<CalamityMod>();
            Mod bossChecklist = calamity.bossChecklist;
            if (bossChecklist is null)
                return;

            // Adds every single Calamity boss and miniboss to Boss Checklist's Boss Log.
            AddCalamityBosses(bossChecklist, calamity);

            // Adds every single Calamity invasion to the Boss Checklist's Invasion Log.
            AddCalamityInvasions(bossChecklist, calamity);

            // Registers Calamity's special loot (ie. Lore items) and alternate spawn items to Boss Checklist's Boss Log.
            RegisterCalamityExtraInfo(bossChecklist, calamity);
        }

        private static void AddCalamityBosses(Mod bossChecklist, Mod calamity)
        {
            // Desert Scourge
            {
                string entryName = "DesertScourge";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> segments = new List<int>() { NPCType<DesertScourgeHead>(), NPCType<DesertScourgeBody>(), NPCType<DesertScourgeTail>() };
                List<int> collection = new List<int>() { ItemType<DesertScourgeRelic>(), ItemType<DesertScourgeTrophy>(), ItemType<DesertScourgeMask>(), ItemType<LoreDesertScourge>(), ItemType<ThankYouPainting>() };
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/DesertScourge/DesertScourge_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, entryName, order, segments, DownedDesertScourge, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<DesertMedallion>(),
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait
                });
            }

            // Giant Clam
            {
                string entryName = "GiantClam";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<GiantClam>();
                List<int> collection = new List<int>() { ItemType<GiantClamRelic>(), ItemType<GiantClamTrophy>() };
                AddMiniBoss(bossChecklist, calamity, entryName, order, type, DownedGiantClam, new Dictionary<string, object>()
                {
                    ["collectibles"] = collection
                });
            }

            // Crabulon
            {
                string entryName = "Crabulon";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<Crabulon>();
                List<int> collection = new List<int>() { ItemType<CrabulonRelic>(), ItemType<CrabulonTrophy>(), ItemType<CrabulonMask>(), ItemType<LoreCrabulon>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedCrabulon, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<DecapoditaSprout>(),
                    ["collectibles"] = collection
                });
            }

            // Hive Mind
            {
                string entryName = "HiveMind";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<HiveMind>();
                List<int> collection = new List<int>() { ItemType<HiveMindRelic>(), ItemType<HiveMindTrophy>(), ItemType<HiveMindMask>(), ItemType<LoreHiveMind>(), ItemType<RottingEyeball>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedHiveMind, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<Teratoma>(),
                    ["collectibles"] = collection,
                    ["overrideHeadTextures"] = "CalamityMod/NPCs/HiveMind/HiveMindP2_Head_Boss"
                });
            }

            // Perforators
            {
                string entryName = "Perforators";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<PerforatorHive>();
                List<int> collection = new List<int>() { ItemType<PerforatorsRelic>(), ItemType<PerforatorTrophy>(), ItemType<PerforatorMask>(), ItemType<LorePerforators>(), ItemType<BloodyVein>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedPerfs, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<BloodyWormFood>(),
                    ["collectibles"] = collection
                });
            }

            // Slime God
            {
                string entryName = "SlimeGod";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> bosses = new List<int>() { NPCType<SlimeGodCore>(), NPCType<EbonianPaladin>(), NPCType<CrimulanPaladin>() };
                List<int> collection = new List<int>() { ItemType<SlimeGodRelic>(), ItemType<SlimeGodTrophy>(), ItemType<SlimeGodMask>(), ItemType<SlimeGodMask2>(), ItemType<LoreSlimeGod>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, bosses, DownedSlimeGod, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<OverloadedSludge>(),
                    ["collectibles"] = collection
                });
            }

            // Cryogen
            {
                string entryName = "Cryogen";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<Cryogen>();
                List<int> collection = new List<int>() { ItemType<CryogenRelic>(), ItemType<CryogenTrophy>(), ItemType<CryogenMask>(), ItemType<LoreArchmage>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedCryogen, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<CryoKey>(),
                    ["collectibles"] = collection,
                    ["overrideHeadTextures"] = "CalamityMod/NPCs/Cryogen/Cryogen_Phase1_Head_Boss"
                });
            }

            // Aquatic Scourge
            {
                string entryName = "AquaticScourge";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> segments = new List<int>() { NPCType<AquaticScourgeHead>(), NPCType<AquaticScourgeBody>(), NPCType<AquaticScourgeBodyAlt>(), NPCType<AquaticScourgeTail>() };
                List<int> collection = new List<int>() { ItemType<AquaticScourgeRelic>(), ItemType<AquaticScourgeTrophy>(), ItemType<AquaticScourgeMask>(), ItemType<LoreAquaticScourge>(), ItemType<LoreSulphurSea>(), ItemType<ThankYouPainting>() };
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AquaticScourge/AquaticScourge_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, entryName, order, segments, DownedAquaticScourge, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<Seafood>(),
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait
                });
            }

            // Cragmaw Mire
            {
                string entryName = "CragmawMire";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<CragmawMire>();
                List<int> collection = new List<int>() { ItemType<CragmawMireRelic>(), ItemType<CragmawMireTrophy>() };
                AddMiniBoss(bossChecklist, calamity, entryName, order, type, DownedCragmawMire, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<CausticTear>(),
                    ["collectibles"] = collection,
                    ["availability"] = DownedAcidRainInitial
                });
            }

            // Brimstone Elemental
            {
                string entryName = "BrimstoneElemental";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<BrimstoneElemental>();
                List<int> collection = new List<int>() { ItemType<BrimstoneElementalRelic>(), ItemType<BrimstoneElementalTrophy>(), ItemType<BrimstoneWaifuMask>(), ItemType<LoreAzafure>(), ItemType<LoreBrimstoneElemental>(), ItemType<CharredRelic>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedBrimstoneElemental, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<CharredIdol>(),
                    ["collectibles"] = collection
                });
            }

            // Calamitas Clone
            {
                string entryName = "CalamitasClone";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<CalamitasClone>();
                List<int> collection = new List<int>() { ItemType<CalamitasCloneRelic>(), ItemType<CalamitasCloneTrophy>(), ItemType<CataclysmTrophy>(), ItemType<CatastropheTrophy>(), ItemType<CalamitasCloneMask>(), ItemType<HoodOfCalamity>(), ItemType<RobesOfCalamity>(), ItemType<LoreCalamitasClone>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedCalClone, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<EyeofDesolation>(),
                    ["collectibles"] = collection
                });
            }

            // Great Sand Shark
            {
                string entryName = "GreatSandShark";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<GreatSandShark>();
                List<int> collection = new List<int>() { ItemType<GreatSandSharkRelic>(), ItemType<GreatSandSharkTrophy>(), ItemID.MusicBoxSandstorm };
                AddMiniBoss(bossChecklist, calamity, entryName, order, type, DownedGSS, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<SandstormsCore>(),
                    ["collectibles"] = collection
                });
            }

            // Anahita and Leviathan
            {
                string entryName = "Leviathan";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> bosses = new List<int>() { NPCType<Leviathan>(), NPCType<Anahita>() };
                List<int> collection = new List<int>() { ItemType<LeviathanAnahitaRelic>(), ItemType<LeviathanTrophy>(), ItemType<AnahitaTrophy>(), ItemType<LeviathanMask>(), ItemType<AnahitaMask>(), ItemType<LoreAbyss>(), ItemType<LoreLeviathanAnahita>(), ItemType<ThankYouPainting>() };
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Leviathan/AnahitaLevi_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, entryName, order, bosses, DownedLeviathan, new Dictionary<string, object>()
                {
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait
                });
            }

            // Astrum Aureus
            {
                string entryName = "AstrumAureus";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<AstrumAureus>();
                List<int> collection = new List<int>() { ItemType<AstrumAureusRelic>(), ItemType<AstrumAureusTrophy>(), ItemType<AstrumAureusMask>(), ItemType<LoreAstrumAureus>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedAureus, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<AstralChunk>(),
                    ["collectibles"] = collection
                });
            }

            // Plaguebringer Goliath
            {
                string entryName = "PlaguebringerGoliath";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<PlaguebringerGoliath>();
                List<int> collection = new List<int>() { ItemType<PlaguebringerGoliathRelic>(), ItemType<PlaguebringerGoliathTrophy>(), ItemType<PlaguebringerGoliathMask>(), ItemType<LorePlaguebringerGoliath>(), ItemType<PlagueCaller>(), ItemType<ThankYouPainting>() };
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliath_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedPBG, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<Abombination>(),
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait
                });
            }

            // Ravager
            {
                string entryName = "Ravager";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> segments = new List<int>() { NPCType<RavagerBody>(), NPCType<RavagerClawLeft>(), NPCType<RavagerClawRight>(), NPCType<RavagerHead>(), NPCType<RavagerLegLeft>(), NPCType<RavagerLegRight>() };
                List<int> collection = new List<int>() { ItemType<RavagerRelic>(), ItemType<RavagerTrophy>(), ItemType<RavagerMask>(), ItemType<LoreRavager>(), ItemType<ThankYouPainting>() };
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/Ravager_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, entryName, order, segments, DownedRavager, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<DeathWhistle>(),
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait
                });
            }

            // Astrum Deus
            {
                string entryName = "AstrumDeus";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> segments = new List<int>() { NPCType<AstrumDeusHead>(), NPCType<AstrumDeusBody>(), NPCType<AstrumDeusTail>() };
                List<int> summons = new List<int>() { ItemType<TitanHeart>(), ItemType<Starcore>() };
                List<int> collection = new List<int>() { ItemType<AstrumDeusRelic>(), ItemType<AstrumDeusTrophy>(), ItemType<AstrumDeusMask>(), ItemType<LoreAstrumDeus>(), ItemType<LoreAstralInfection>(), ItemType<ChromaticOrb>(), ItemType<ThankYouPainting>() };
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeus_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };

                AddBoss(bossChecklist, calamity, entryName, order, segments, DownedDeus, new Dictionary<string, object>()
                {
                    ["spawnItems"] = summons,
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait
                });
            }

            // Profaned Guardians
            {
                string entryName = "ProfanedGuardians";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<ProfanedGuardianCommander>();
                List<int> collection = new List<int>() { ItemType<ProfanedGuardiansRelic>(), ItemType<ProfanedGuardianTrophy>(), ItemType<ProfanedGuardianMask>(), ItemType<LoreProfanedGuardians>(), ItemType<ThankYouPainting>() };
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardians_BossChecklist").Value;
                    float scale = 0.7f;
                    Vector2 centered = new Vector2(rect.Center.X - texture.Width * scale / 2, rect.Center.Y - texture.Height * scale / 2);
                    sb.Draw(texture, centered, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedGuardians, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<ProfanedShard>(),
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait,
                    ["overrideHeadTextures"] = "CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianCommander_Head_Boss"
                });
            }

            // Dragonfolly
            {
                string entryName = "Dragonfolly";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<Bumblefuck>();
                List<int> collection = new List<int>() { ItemType<DragonfollyRelic>(), ItemType<DragonfollyTrophy>(), ItemType<BumblefuckMask>(), ItemType<LoreDragonfolly>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedBirb, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<ExoticPheromones>(),
                    ["collectibles"] = collection
                });
            }

            // Providence
            {
                string entryName = "Providence";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<Providence>();
                List<int> collection = new List<int>() { ItemType<ProvidenceRelic>(), ItemType<ProvidenceTrophy>(), ItemType<ProvidenceMask>(), ItemType<LoreProvidence>(), ItemType<ThankYouPainting>() };
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Providence/Providence_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedProvidence, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<ProfanedCore>(),
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait
                });
            }

            // Ceaseless Void
            {
                string entryName = "CeaselessVoid";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> bosses = new List<int>() { NPCType<CeaselessVoid>(), NPCType<DarkEnergy>() };
                List<int> collection = new List<int>() { ItemType<CeaselessVoidRelic>(), ItemType<CeaselessVoidTrophy>(), ItemType<CeaselessVoidMask>(), ItemType<AncientGodSlayerHelm>(), ItemType<AncientGodSlayerChestplate>(), ItemType<AncientGodSlayerLeggings>(), ItemType<LoreCeaselessVoid>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, bosses, DownedCeaselessVoid, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<RuneofKos>(),
                    ["collectibles"] = collection
                });
            }

            // Storm Weaver
            {
                string entryName = "StormWeaver";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> segments = new List<int>() { NPCType<StormWeaverHead>(), NPCType<StormWeaverBody>(), NPCType<StormWeaverTail>() };
                List<int> collection = new List<int>() { ItemType<WeaverTrophy>(), ItemType<StormWeaverMask>(), ItemType<AncientGodSlayerHelm>(), ItemType<AncientGodSlayerChestplate>(), ItemType<AncientGodSlayerLeggings>(), ItemType<LoreStormWeaver>(), ItemType<LittleLight>(), ItemType<ThankYouPainting>() };
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/StormWeaver/StormWeaver_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, entryName, order, segments, DownedStormWeaver, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<RuneofKos>(),
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait,
                    ["overrideHeadTextures"] = "CalamityMod/NPCs/StormWeaver/StormWeaverHead_Head_Boss"
                });
            }

            // Signus
            {
                string entryName = "Signus";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<Signus>();
                List<int> collection = new List<int>() { ItemType<SignusRelic>(), ItemType<SignusTrophy>(), ItemType<SignusMask>(), ItemType<AncientGodSlayerHelm>(), ItemType<AncientGodSlayerChestplate>(), ItemType<AncientGodSlayerLeggings>(), ItemType<LoreSignus>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedSignus, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<RuneofKos>(),
                    ["collectibles"] = collection
                });
            }

            // Polterghast
            {
                string entryName = "Polterghast";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> bosses = new List<int>() { NPCType<Polterghast>(), NPCType<PolterPhantom>() };
                List<int> collection = new List<int>() { ItemType<PolterghastRelic>(), ItemType<PolterghastTrophy>(), ItemType<PolterghastMask>(), ItemType<LorePolterghast>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, bosses, DownedPolterghast, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<NecroplasmicBeacon>(),
                    ["collectibles"] = collection
                });
            }

            // Mauler
            {
                string entryName = "Mauler";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<Mauler>();
                List<int> collection = new List<int>() { ItemType<MaulerRelic>(), ItemType<MaulerTrophy>() };
                AddMiniBoss(bossChecklist, calamity, entryName, order, type, DownedMauler, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<CausticTear>(),
                    ["collectibles"] = collection,
                    ["availability"] = DownedAcidRainHardmode
                });
            }

            // Nuclear Terror
            {
                string entryName = "NuclearTerror";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<NuclearTerror>();
                List<int> collection = new List<int>() { ItemType<NuclearTerrorRelic>(), ItemType<NuclearTerrorTrophy>() };
                AddMiniBoss(bossChecklist, calamity, entryName, order, type, DownedNuclearTerror, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<CausticTear>(),
                    ["collectibles"] = collection,
                    ["availability"] = DownedAcidRainHardmode
                });
            }

            // Old Duke
            {
                string entryName = "OldDuke";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<OldDuke>();
                List<int> collection = new List<int>() { ItemType<OldDukeRelic>(), ItemType<OldDukeTrophy>(), ItemType<OldDukeMask>(), ItemType<LoreOldDuke>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedBoomerDuke, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<BloodwormItem>(),
                    ["collectibles"] = collection
                });
            }

            // Devourer of Gods
            {
                string entryName = "DevourerofGods";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<DevourerofGodsHead>();
                List<int> collection = new List<int>() { ItemType<DevourerOfGodsRelic>(), ItemType<DevourerofGodsTrophy>(), ItemType<DevourerofGodsMask>(), ItemType<LoreDevourerofGods>(), ItemType<ThankYouPainting>() };
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGods_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedDoG, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<CosmicWorm>(),
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait,
                    ["overrideHeadTextures"] = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead_Head_Boss"
                });
            }

            // Yharon
            {
                string entryName = "Yharon";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<Yharon>();
                List<int> collection = new List<int>() { ItemType<YharonRelic>(), ItemType<YharonTrophy>(), ItemType<YharonMask>(), ItemType<LoreYharon>(), ItemType<ForgottenDragonEgg>(), ItemType<McNuggets>(), ItemType<FoxDrive>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedYharon, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<YharonEgg>(),
                    ["collectibles"] = collection
                });
            }

            // Exo Mechs
            // Collection requires edits
            // Instructions require edits
            // Despawn requires edits
            {
                string entryName = "ExoMechs";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> bosses = new List<int>() { NPCType<Apollo>(), NPCType<AresBody>(), NPCType<Artemis>(), NPCType<ThanatosHead>() };
                List<int> collection = new List<int>() { ItemType<DraedonRelic>(), ItemType<AresTrophy>(), ItemType<ThanatosTrophy>(), ItemType<ArtemisTrophy>(), ItemType<ApolloTrophy>(), ItemType<DraedonMask>(), ItemType<AresMask>(), ItemType<ThanatosMask>(), ItemType<ArtemisMask>(), ItemType<ApolloMask>(), ItemType<LoreExoMechs>(), ItemType<LoreCynosure>(), ItemType<ThankYouPainting>() };
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = Request<Texture2D>("CalamityMod/NPCs/ExoMechs/ExoMechs_BossChecklist").Value;
                    float scale = 0.7f;
                    Vector2 centered = new Vector2(rect.Center.X - texture.Width * scale / 2, rect.Center.Y - texture.Height * scale / 2);
                    sb.Draw(texture, centered, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                };

                AddBoss(bossChecklist, calamity, entryName, order, bosses, DownedExoMechs, new Dictionary<string, object>()
                {
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait
                });
            }

            // Calamitas
            {
                string entryName = "Calamitas";
                ModProgression.TryGetValue(entryName, out float order);
                int type = NPCType<SupremeCalamitas>();
                List<int> summons = new List<int>() { ItemType<AshesofCalamity>(), ItemType<CeremonialUrn>() };
                List<int> collection = new List<int>() { ItemType<CalamitasRelic>(), ItemType<SupremeCalamitasTrophy>(), ItemType<SupremeCataclysmTrophy>(), ItemType<SupremeCatastropheTrophy>(), ItemType<AshenHorns>(), ItemType<SCalMask>(), ItemType<SCalRobes>(), ItemType<SCalBoots>(), ItemType<LoreCalamitas>(), ItemType<LoreCynosure>(), ItemType<BrimstoneJewel>(), ItemType<Levi>(), ItemType<ThankYouPainting>() };
                AddBoss(bossChecklist, calamity, entryName, order, type, DownedSCal, new Dictionary<string, object>()
                {
                    ["spawnItems"] = summons,
                    ["collectibles"] = collection,
                    ["overrideHeadTextures"] = "CalamityMod/NPCs/SupremeCalamitas/HoodedHeadIcon"
                });
            }
        }

        private static void AddCalamityInvasions(Mod bossChecklist, Mod calamity)
        {
            // Initial Acid Rain
            {
                string entryName = "AcidRainT1";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> enemies = AcidRainEvent.PossibleEnemiesPreHM.Select(enemy => enemy.Key).ToList();
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Events/AcidRainT1_BossChecklist").Value;
                    float scale = 1f;
                    Vector2 centered = new Vector2(rect.Center.X - texture.Width * scale / 2, rect.Center.Y - texture.Height * scale / 2);
                    sb.Draw(texture, centered, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                };
                AddInvasion(bossChecklist, calamity, entryName, order, enemies, DownedAcidRainInitial, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<CausticTear>(),
                    ["collectibles"] = ItemType<RadiatingCrystal>(),
                    ["customPortrait"] = portrait,
                    ["overrideHeadTextures"] = "CalamityMod/UI/MiscTextures/AcidRainIcon"
                });
            }
            // Post-Aquatic Scourge Acid Rain
            {
                string entryName = "AcidRainT2";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> enemies = AcidRainEvent.PossibleEnemiesAS.Select(enemy => enemy.Key).ToList();
                enemies.Add(ModContent.NPCType<IrradiatedSlime>());
                enemies.AddRange(AcidRainEvent.PossibleMinibossesAS.Select(miniboss => miniboss.Key));
                List<int> collection = new List<int>() { ItemType<CragmawMireRelic>(), ItemType<CragmawMireTrophy>(), ItemType<RadiatingCrystal>()};
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Events/AcidRainT2_BossChecklist").Value;
                    float scale = 0.9f;
                    Vector2 centered = new Vector2(rect.Center.X - texture.Width * scale / 2, rect.Center.Y - texture.Height * scale / 2);
                    sb.Draw(texture, centered, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                };
                AddInvasion(bossChecklist, calamity, entryName, order, enemies, DownedAcidRainHardmode, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<CausticTear>(),
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait,
                    ["overrideHeadTextures"] = "CalamityMod/UI/MiscTextures/AcidRainIcon",
                    ["availability"] = DownedAcidRainInitial
                });
            }
            // Post-Polterghast Acid Rain
            {
                string entryName = "AcidRainT3";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> enemies = AcidRainEvent.PossibleEnemiesPolter.Select(enemy => enemy.Key).ToList();
                enemies.AddRange(AcidRainEvent.PossibleMinibossesPolter.Select(miniboss => miniboss.Key));
                List<int> collection = new List<int>() { ItemType<CragmawMireRelic>(), ItemType<CragmawMireTrophy>(), ItemType<MaulerRelic>(), ItemType<MaulerTrophy>(), ItemType<NuclearTerrorRelic>(), ItemType<NuclearTerrorTrophy>(), ItemType<RadiatingCrystal>()};
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Events/AcidRainT3_BossChecklist").Value;
                    float scale = 0.9f;
                    Vector2 centered = new Vector2(rect.Center.X - texture.Width * scale / 2, rect.Center.Y - texture.Height * scale / 2);
                    sb.Draw(texture, centered, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                };
                AddInvasion(bossChecklist, calamity, entryName, order, enemies, DownedBoomerDuke, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<CausticTear>(),
                    ["collectibles"] = collection,
                    ["customPortrait"] = portrait,
                    ["overrideHeadTextures"] = "CalamityMod/UI/MiscTextures/AcidRainIcon",
                    ["availability"] = DownedAcidRainHardmode
                });
            }
            // Boss Rush
            {
                string entryName = "BossRush";
                ModProgression.TryGetValue(entryName, out float order);
                List<int> enemies = new List<int>() { NPCID.None }; // This is for loot purposes, which no bosses give during the event
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Skies/XerocEye").Value;
                    float scale = 0.5f;
                    Vector2 centered = new Vector2(rect.Center.X - texture.Width * scale / 2, rect.Center.Y - texture.Height * scale / 2);
                    sb.Draw(texture, centered, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                };
                AddInvasion(bossChecklist, calamity, entryName, order, enemies, DownedBossRush, new Dictionary<string, object>()
                {
                    ["spawnItems"] = ItemType<Terminus>(),
                    ["collectibles"] = ItemType<Rock>(),
                    ["customPortrait"] = portrait,
                    ["overrideHeadTextures"] = "CalamityMod/UI/MiscTextures/BossRushIcon"
                });
            }
        }

        private static void RegisterCalamityExtraInfo(Mod bossChecklist, Mod calamity)
        {
            bossChecklist.Call("SubmitEntrySpawnItems", calamity, new Dictionary<string, object>()
            {
                { "Terraria Plantera", ItemType<Portabulb>() },
                { "Terraria Golem", ItemType<OldPowerCell>() },
                { "Terraria CultistBoss", ItemType<EidolonTablet>() }
            });

            bossChecklist.Call("SubmitEntryCollectibles", calamity, new Dictionary<string, object>()
            {
                { "Terraria KingSlime", new List<int>() { ItemType<LoreKingSlime>(), ItemType<ThankYouPainting>() } },
                { "Terraria EyeofCthulhu", new List<int>() { ItemType<LoreEyeofCthulhu>(), ItemType<ThankYouPainting>() } },
                { "Terraria EaterofWorldsHead", new List<int>() { ItemType<LoreEaterofWorlds>(), ItemType<LoreCorruption>(), ItemType<ThankYouPainting>() } },
                { "Terraria BrainofCthulhu", new List<int>() { ItemType<LoreBrainofCthulhu>(), ItemType<LoreCrimson>(), ItemType<ThankYouPainting>() } },
                { "Terraria QueenBee", new List<int>() { ItemType<LoreQueenBee>(), ItemType<ThankYouPainting>() } },
                { "Terraria SkeletronHead", new List<int>() { ItemType<LoreSkeletron>(), ItemType<ThankYouPainting>() } },
                { "Terraria WallofFlesh", new List<int>() { ItemType<LoreWallofFlesh>(), ItemType<LoreUnderworld>(), ItemType<HermitsBoxofOneHundredMedicines>(), ItemType<ThankYouPainting>() } },
                { "Terraria QueenSlimeBoss", new List<int>() { ItemType<LoreQueenSlime>(), ItemType<ThankYouPainting>() } },
                { "Terraria TheTwins", new List<int>() { ItemType<LoreTwins>(), ItemType<LoreMechs>(), ItemType<ThankYouPainting>() } },
                { "Terraria TheDestroyer", new List<int>() { ItemType<LoreDestroyer>(), ItemType<LoreMechs>(), ItemType<ThankYouPainting>() } },
                { "Terraria SkeletronPrime", new List<int>() { ItemType<LoreSkeletronPrime>(), ItemType<LoreMechs>(), ItemType<ThankYouPainting>() } },
                { "Terraria Plantera", new List<int>() { ItemType<LorePlantera>(), ItemType<ThankYouPainting>() } },
                { "Terraria Golem", new List<int>() { ItemType<LoreGolem>(), ItemType<ThankYouPainting>() } },
                { "Terraria HallowBoss", new List<int>() { ItemType<LoreEmpressofLight>(), ItemType<ThankYouPainting>() } },
                { "Terraria DukeFishron", new List<int>() { ItemType<LoreDukeFishron>(), ItemType<ThankYouPainting>() } },
                { "Terraria CultistBoss", new List<int>() { ItemType<LorePrelude>(), ItemType<ThankYouPainting>() } },
                { "Terraria MoonLord", new List<int>() { ItemType<LoreRequiem>(), ItemType<ThankYouPainting>() } }
            });
        }

        private static void FargosSupport()
        {
            Mod fargos = GetInstance<CalamityMod>().fargos;
            if (fargos is null)
                return;

            void AddToMutantShop(string bossName, string summonItemName, Func<bool> downed, int price)
            {
                ModProgression.TryGetValue(bossName, out float order);
                fargos.Call("AddSummon", order, "CalamityMod", summonItemName, downed, price);
            }

            fargos.Call("AbominationnClearEvents", "CalamityMod", AcidRainEvent.AcidRainEventIsOngoing, true);

            AddToMutantShop("OldDuke", "BloodwormItem", DownedBoomerDuke, Item.buyPrice(platinum: 2));
        }

        private static void DialogueTweakSupport()
        {
            Mod dialogueMod = GetInstance<CalamityMod>().dialogueTweak;
            if (dialogueMod != null)
            {
                dialogueMod.Call("ReplaceShopButtonIcon", NPCType<WITCH>(), "Head");
            }
        }

        private static void SummonersAssociationSupport()
        {
            Mod sAssociation = GetInstance<CalamityMod>().summonersAssociation;
            if (sAssociation is null)
                return;

            void RegisterSummon(int summonItem, int summonBuff, int summonProjectile)
            {
                sAssociation.Call("AddMinionInfo", summonItem, summonBuff, summonProjectile);
            }
            RegisterSummon(ItemType<WulfrumController>(), BuffType<WulfrumDroidBuff>(), ProjectileType<WulfrumDroid>());
            RegisterSummon(ItemType<SunSpiritStaff>(), BuffType<SolarSpirit>(), ProjectileType<SolarPixie>());
            RegisterSummon(ItemType<FrostBlossomStaff>(), BuffType<FrostBlossomBuff>(), ProjectileType<FrostBlossom>());
            RegisterSummon(ItemType<BelladonnaSpiritStaff>(), BuffType<BelladonnaSpiritBuff>(), ProjectileType<BelladonnaSpirit>());
            RegisterSummon(ItemType<StormjawStaff>(), BuffType<BabyStormlionBuff>(), ProjectileType<StormjawBaby>());
            RegisterSummon(ItemType<SeaboundStaff>(), BuffType<BrittleStar>(), ProjectileType<BrittleStarMinion>());
            RegisterSummon(ItemType<EnchantedConch>(), BuffType<HermitCrab>(), ProjectileType<HermitCrabMinion>());
            RegisterSummon(ItemType<DeathstareRod>(), BuffType<MiniatureEyeofCthulhu>(), ProjectileType<DeathstareEyeball>());
            RegisterSummon(ItemType<PuffShroom>(), BuffType<PuffWarriorBuff>(), ProjectileType<PuffWarrior>());
            RegisterSummon(ItemType<VileFeeder>(), BuffType<VileFeederBuff>(), ProjectileType<VileFeederSummon>());
            RegisterSummon(ItemType<ScabRipper>(), BuffType<BabyBloodCrawlerBuff>(), ProjectileType<BabyBloodCrawler>());
            RegisterSummon(ItemType<CinderBlossomStaff>(), BuffType<CinderBlossomBuff>(), ProjectileType<CinderBlossom>());
            RegisterSummon(ItemType<BloodClotStaff>(), BuffType<BloodClot>(), ProjectileType<BloodClotMinion>());
            RegisterSummon(ItemType<DankStaff>(), BuffType<DankCreeperBuff>(), ProjectileType<DankCreeperMinion>());
            RegisterSummon(ItemType<StarSwallowerContainmentUnit>(), BuffType<StarSwallowerBuff>(), ProjectileType<StarSwallowerSummon>());
            RegisterSummon(ItemType<HerringStaff>(), BuffType<Herring>(), ProjectileType<HerringMinion>());
            RegisterSummon(ItemType<EyeOfNight>(), BuffType<EyeOfNightBuff>(), ProjectileType<EyeOfNightSummon>());
            RegisterSummon(ItemType<FleshOfInfidelity>(), BuffType<FleshBallBuff>(), ProjectileType<FleshBallMinion>());
            RegisterSummon(ItemType<CorroslimeStaff>(), BuffType<Corroslime>(), ProjectileType<CorroslimeMinion>());
            RegisterSummon(ItemType<CrimslimeStaff>(), BuffType<Crimslime>(), ProjectileType<CrimslimeMinion>());
            RegisterSummon(ItemType<BlackHawkRemote>(), BuffType<BlackHawkBuff>(), ProjectileType<BlackHawkSummon>());
            RegisterSummon(ItemType<CausticStaff>(), BuffType<CausticStaffBuff>(), ProjectileType<CausticStaffSummon>());
            RegisterSummon(ItemType<AncientIceChunk>(), BuffType<IceClasperBuff>(), ProjectileType<IceClasperMinion>());
            RegisterSummon(ItemType<ShellfishStaff>(), BuffType<ShellfishBuff>(), ProjectileType<Shellfish>());
            RegisterSummon(ItemType<HauntedScroll>(), BuffType<HauntedDishesBuff>(), ProjectileType<HauntedDishes>());
            RegisterSummon(ItemType<ForgottenApexWand>(), BuffType<AncientMineralSharkBuff>(), ProjectileType<ApexShark>());
            RegisterSummon(ItemType<DaedalusGolemStaff>(), BuffType<DaedalusGolemBuff>(), ProjectileType<DaedalusGolem>());
            RegisterSummon(ItemType<GlacialEmbrace>(), BuffType<GlacialEmbraceBuff>(), ProjectileType<GlacialEmbracePointyThing>());
            RegisterSummon(ItemType<MountedScanner>(), BuffType<MountedScannerBuff>(), ProjectileType<MountedScannerSummon>());
            RegisterSummon(ItemType<DeepseaStaff>(), BuffType<AquaticStar>(), ProjectileType<AquaticStarMinion>());
            RegisterSummon(ItemType<SunGodStaff>(), BuffType<SolarGodSpiritBuff>(), ProjectileType<SolarGod>());
            RegisterSummon(ItemType<TundraFlameBlossomsStaff>(), BuffType<TundraFlameBlossomsBuff>(), ProjectileType<TundraFlameBlossom>());
            RegisterSummon(ItemType<DormantBrimseeker>(), BuffType<BrimseekerBuff>(), ProjectileType<DormantBrimseekerBab>());
            RegisterSummon(ItemType<IgneousExaltation>(), BuffType<IgneousExaltationBuff>(), ProjectileType<IgneousBlade>());
            RegisterSummon(ItemType<PlantationStaff>(), BuffType<PlantationBuff>(), ProjectileType<PlantSummon>());
            RegisterSummon(ItemType<ViralSprout>(), BuffType<SageSpiritBuff>(), ProjectileType<SageSpirit>());
            RegisterSummon(ItemType<SandSharknadoStaff>(), BuffType<Sandnado>(), ProjectileType<SandnadoMinion>());
            RegisterSummon(ItemType<GastricBelcherStaff>(), BuffType<GastricAberrationBuff>(), ProjectileType<GastricBelcher>());
            RegisterSummon(ItemType<FuelCellBundle>(), BuffType<MiniPlaguebringerBuff>(), ProjectileType<PlaguebringerMK2>());
            RegisterSummon(ItemType<WitherBlossomsStaff>(), BuffType<WitherBlossomsBuff>(), ProjectileType<WitherBlossom>());
            RegisterSummon(ItemType<GodspawnHelixStaff>(), BuffType<AstralProbeBuff>(), ProjectileType<AstralProbeSummon>());
            RegisterSummon(ItemType<TacticalPlagueEngine>(), BuffType<TacticalPlagueEngineBuff>(), ProjectileType<TacticalPlagueJet>());
            RegisterSummon(ItemType<ElementalAxe>(), BuffType<ElementalAxeBuff>(), ProjectileType<ElementalAxeMinion>());
            RegisterSummon(ItemType<FlowersOfMortality>(), BuffType<FlowersOfMortalityBuff>(), ProjectileType<FlowersOfMortalityPetal>());
            RegisterSummon(ItemType<SnakeEyes>(), BuffType<SnakeEyesBuff>(), ProjectileType<SnakeEyesSummon>());
            RegisterSummon(ItemType<DazzlingStabberStaff>(), BuffType<DazzlingStabberBuff>(), ProjectileType<DazzlingStabber>());
            RegisterSummon(ItemType<ViridVanguard>(), BuffType<ViridVanguardBuff>(), ProjectileType<ViridVanguardBlade>());
            RegisterSummon(ItemType<DragonbloodDisgorger>(), BuffType<SkeletalDragonsBuff>(), ProjectileType<SkeletalDragonMother>());
            RegisterSummon(ItemType<Cosmilamp>(), BuffType<CosmilampBuff>(), ProjectileType<CosmilampMinion>());
            RegisterSummon(ItemType<VoidConcentrationStaff>(), BuffType<VoidConcentrationBuff>(), ProjectileType<VoidConcentrationAura>());
            RegisterSummon(ItemType<EtherealSubjugator>(), BuffType<Phantom>(), ProjectileType<PhantomGuy>());
            RegisterSummon(ItemType<CalamarisLament>(), BuffType<Calamari>(), ProjectileType<CalamariMinion>());
            RegisterSummon(ItemType<GammaHeart>(), BuffType<GammaHydraBuff>(), ProjectileType<GammaHead>());
            RegisterSummon(ItemType<WarloksMoonFist>(), BuffType<MoonFistBuff>(), ProjectileType<MoonFist>());
            RegisterSummon(ItemType<StaffoftheMechworm>(), BuffType<Mechworm>(), ProjectileType<MechwormBody>());
            RegisterSummon(ItemType<CorvidHarbringerStaff>(), BuffType<CorvidHarbringerBuff>(), ProjectileType<PowerfulRaven>());
            RegisterSummon(ItemType<EndoHydraStaff>(), BuffType<EndoHydraBuff>(), ProjectileType<EndoHydraHead>());
            RegisterSummon(ItemType<CosmicViperEngine>(), BuffType<CosmicViperEngineBuff>(), ProjectileType<CosmicViperSummon>());
            RegisterSummon(ItemType<YharonsKindleStaff>(), BuffType<FieryDraconidBuff>(), ProjectileType<FieryDraconid>());
            RegisterSummon(ItemType<MidnightSunBeacon>(), BuffType<MidnightSunBuff>(), ProjectileType<MidnightSunUFO>());
            RegisterSummon(ItemType<PoleWarper>(), BuffType<PoleWarperBuff>(), ProjectileType<PoleWarperSummon>());
            RegisterSummon(ItemType<Vigilance>(), BuffType<SoulSeekerBuff>(), ProjectileType<SeekerSummonProj>());
            RegisterSummon(ItemType<Metastasis>(), BuffType<SepulcherMinionBuff>(), ProjectileType<SepulcherMinion>());
            RegisterSummon(ItemType<CosmicImmaterializer>(), BuffType<CosmicEnergy>(), ProjectileType<CosmicEnergySpiral>());
            RegisterSummon(ItemType<TemporalUmbrella>(), BuffType<MagicHatBuff>(), ProjectileType<MagicHat>());
            RegisterSummon(ItemType<Endogenesis>(), BuffType<EndoCooperBuff>(), ProjectileType<EndoCooperBody>());
            
            sAssociation.Call("AddMinionInfo", ItemType<EntropysVigil>(), BuffType<EntropysVigilBuff>(), new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>()
                {
                    ["ProjID"] = ProjectileType<Calamitamini>(),
                    ["Slot"] = 1-(1f/3f)
                },
                new Dictionary<string, object>()
                {
                    ["ProjID"] = ProjectileType<Cataclymini>(),
                    ["Slot"] = 2f/3f
                },
                new Dictionary<string, object>()
                {
                    ["ProjID"] = ProjectileType<Catastromini>(),
                    ["Slot"] = 2f/3f
                }
            });
            //Entropy's Vigil is a bruh moment
            sAssociation.Call("AddMinionInfo", ItemType<ResurrectionButterfly>(), BuffType<ResurrectionButterflyBuff>(), new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>()
                {
                    ["ProjID"] = ProjectileType<PurpleButterfly>()
                },
                new Dictionary<string, object>()
                {
                    ["ProjID"] = ProjectileType<PinkButterfly>()
                }
            });
            sAssociation.Call("AddMinionInfo", ItemType<KingofConstellationsTenryu>(), BuffType<KingofConstellationsBuff>(), new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>()
                {
                    ["ProjID"] = ProjectileType<BlackDragonHead>()
                },
                new Dictionary<string, object>()
                {
                    ["ProjID"] = ProjectileType<WhiteDragonHead>()
                }
            });
        }
    }
}
