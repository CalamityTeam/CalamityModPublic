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
using CalamityMod.NPCs.AdultEidolonWyrm;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
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
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Projectiles.Summon;
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
        public static readonly Func<bool> NotDownedCalamitas = () => !DownedBossSystem.downedCalamitas;
        public static readonly Func<bool> DownedDesertScourge = () => DownedBossSystem.downedDesertScourge;
        public static readonly Func<bool> DownedGiantClam = () => DownedBossSystem.downedCLAM;
        public static readonly Func<bool> DownedCrabulon = () => DownedBossSystem.downedCrabulon;
        public static readonly Func<bool> DownedHiveMind = () => DownedBossSystem.downedHiveMind;
        public static readonly Func<bool> DownedPerfs = () => DownedBossSystem.downedPerforator;
        public static readonly Func<bool> DownedSlimeGod = () => DownedBossSystem.downedSlimeGod;
        public static readonly Func<bool> DownedCryogen = () => DownedBossSystem.downedCryogen;
        public static readonly Func<bool> DownedBrimstoneElemental = () => DownedBossSystem.downedBrimstoneElemental;
        public static readonly Func<bool> DownedAquaticScourge = () => DownedBossSystem.downedAquaticScourge;
        public static readonly Func<bool> DownedCalamitas = () => DownedBossSystem.downedCalamitas;
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
        public static readonly Func<bool> DownedBoomerDuke = () => DownedBossSystem.downedBoomerDuke;
        public static readonly Func<bool> DownedDoG = () => DownedBossSystem.downedDoG;
        public static readonly Func<bool> DownedYharon = () => DownedBossSystem.downedYharon;
        public static readonly Func<bool> DownedExoMechs = () => DownedBossSystem.downedExoMechs;
        public static readonly Func<bool> DownedSCal = () => DownedBossSystem.downedSCal;
        public static readonly Func<bool> DownedAdultEidolonWyrm = () => DownedBossSystem.downedAdultEidolonWyrm;

        public static readonly Func<bool> DownedAcidRainInitial = () => DownedBossSystem.downedEoCAcidRain;
        public static readonly Func<bool> DownedAcidRainHardmode = () => DownedBossSystem.downedAquaticScourgeAcidRain;
    }

    internal class WeakReferenceSupport
    {
        private static readonly Dictionary<string, float> BossDifficulty = new Dictionary<string, float>
        {
            { "DesertScourge", 1.6f },
            { "GiantClam", 1.61f },
            { "Crabulon", 2.7f },
            { "HiveMind", 3.98f },
            { "Perforators", 3.99f },
            { "SlimeGod", 6.5f },
            { "Cryogen", 8.5f },
            { "AquaticScourge", 9.5f },
            { "BrimstoneElemental", 10.5f },
            { "Calamitas", 11.7f },
            { "GreatSandShark", 12.09f },
            { "Leviathan", 12.8f },
            { "AstrumAureus", 12.81f },
            { "PlaguebringerGoliath", 14.5f },
            { "Ravager", 16.5f },
            { "AstrumDeus", 17.5f },
            { "ProfanedGuardians", 18.5f },
            { "Dragonfolly", 18.6f },
            { "Providence", 19.01f }, // Thorium's Ragnarok will most likely be 19f
            { "CeaselessVoid", 19.5f },
            { "StormWeaver", 19.51f },
            { "Signus", 19.52f },
            { "Polterghast", 20f },
            { "OldDuke", 20.5f },
            { "DevourerOfGods", 21f },
            { "Yharon", 22f },
            { "ExoMechs", 22.5f },
            { "SupremeCalamitas", 23f },
            { "AdultEidolonWyrm", 23.5f },
            // { "Yharim", 24f },
            // { "Noxus", 25f },
            // { "Xeroc", 26f },
        };

        private static readonly Dictionary<string, float> InvasionDifficulty = new Dictionary<string, float>
        {
            { "Acid Rain Initial", 2.67f },
            { "Acid Rain Aquatic Scourge", 9.51f },
            { "Acid Rain Polterghast", 19.49f }
        };

        public static void Setup()
        {
            BossChecklistSupport();
            FargosSupport();
            CensusSupport();
            SummonersAssociationSupport();
        }

        // Wrapper function to add bosses to Boss Checklist.
        private static void AddBoss(Mod bossChecklist, Mod hostMod, string name, float difficulty, int npcType, Func<bool> downed, object summon,
            List<int> collection, string instructions, string despawn, Func<bool> available, Action<SpriteBatch, Rectangle, Color> portrait = null, string bossHeadTex = null)
        {
            bossChecklist.Call("AddBoss", hostMod, name, npcType, difficulty, downed, available, collection, summon ?? null, instructions, despawn, portrait, bossHeadTex);
        }

        // Wrapper function to add bosses with multiple segments or phases to Boss Checklist.
        private static void AddBoss(Mod bossChecklist, Mod hostMod, string name, float difficulty, List<int> npcTypes, Func<bool> downed, object summon,
            List<int> collection, string instructions, string despawn, Func<bool> available, Action<SpriteBatch, Rectangle, Color> portrait = null, string bossHeadTex = null)
        {
            bossChecklist.Call("AddBoss", hostMod, name, npcTypes, difficulty, downed, available, collection, summon ?? null, instructions, despawn, portrait, bossHeadTex);
        }

        // Wrapper function to add minibosses to Boss Checklist.
        private static void AddMiniBoss(Mod bossChecklist, Mod hostMod, string name, float difficulty, int npcType, Func<bool> downed, object summon,
            List<int> collection, string instructions, string despawn, Func<bool> available, Action<SpriteBatch, Rectangle, Color> portrait = null, string bossHeadTex = null)
        {
            bossChecklist.Call("AddMiniBoss", hostMod, name, npcType, difficulty, downed, available, collection, summon ?? null, instructions, despawn, portrait, bossHeadTex);
        }

        // Wrapper function to add events to Boss Checklist.
        private static void AddInvasion(Mod bossChecklist, Mod hostMod, string name, float difficulty, List<int> npcTypes, Func<bool> downed, object summon,
            List<int> collection, string instructions, Func<bool> available, Action<SpriteBatch, Rectangle, Color> portrait = null, string bossHeadTex = null)
        {
            bossChecklist.Call("AddEvent", hostMod, name, npcTypes, difficulty, downed, available, collection, summon ?? null, instructions, portrait, bossHeadTex);
        }

        // Wrapper function to add loot and collection items to vanilla bosses for Boss Checklist.
        private static void AddLoot(Mod bossChecklist, string bossName, List<int> loot = null, List<int> collection = null)
            => AddLoot(bossChecklist, "Terraria", bossName, loot, collection);

        // Wrapper function to add loot and collection items to other mods' bosses for Boss Checklist.
        private static void AddLoot(Mod bossChecklist, string mod, string bossName, List<int> loot = null, List<int> collection = null)
        {
            if (loot != null)
                bossChecklist.Call("AddToBossLoot", mod, bossName, loot);
            if (collection != null)
                bossChecklist.Call("AddToBossCollection", mod, bossName, collection);
        }

        // Wrapper function to add summon items to vanilla bosses for Boss Checklist.
        private static void AddSummons(Mod bossChecklist, string bossName, List<int> summons) => AddSummons(bossChecklist, "Terraria", bossName, summons);

        // Wrapper function to add summon items to other mods' bosses for Boss Checklist.
        private static void AddSummons(Mod bossChecklist, string mod, string bossName, List<int> summons) => bossChecklist.Call("AddToBossSpawnItems", mod, bossName, summons);

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

        /// <summary>
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
        /// 14.0 = Betsy<br />
        /// 15.0 = Empress of Light<br />
        /// 16.0 = Duke Fishron<br />
        /// 17.0 = Lunatic Cultist<br />
        /// 18.0 = Moon Lord
        /// </summary>
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

            // Loot which Calamity adds to vanilla bosses and events is also added to Boss Checklist's Boss Log.
            AddCalamityBossLoot(bossChecklist);
            AddCalamityEventLoot(bossChecklist);
        }

        private static void AddCalamityBosses(Mod bossChecklist, Mod calamity)
        {
            // Desert Scourge
            {
                BossDifficulty.TryGetValue("DesertScourge", out float order);
                List<int> segments = new List<int>() { NPCType<DesertScourgeHead>(), NPCType<DesertScourgeBody>(), NPCType<DesertScourgeTail>() };
                int summon = ItemType<DesertMedallion>();
                List<int> collection = new List<int>() { ItemType<DesertScourgeTrophy>(), ItemType<DesertScourgeMask>(), ItemType<KnowledgeDesertScourge>() };
                string instructions = $"Use a [i:{summon}] in the Desert Biome";
                string despawn = CalamityUtils.ColorMessage("The scourge of the desert delved back into the sand.", new Color(0xEE, 0xE8, 0xAA));
                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/DesertScourge/DesertScourge_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                string bossHeadTex = "CalamityMod/NPCs/DesertScourge/DesertScourgeHead_Head_Boss";
                AddBoss(bossChecklist, calamity, "Desert Scourge", order, segments, DownedDesertScourge, summon, collection, instructions, despawn, () => true, portrait, bossHeadTex);
            }

            // Giant Clam
            {
                BossDifficulty.TryGetValue("GiantClam", out float order);
                int type = NPCType<GiantClam>();
                string instructions = "Can spawn naturally in the Sunken Sea";
                string despawn = CalamityUtils.ColorMessage("The Giant Clam returns into hiding in its grotto.", new Color(0x7F, 0xFF, 0xD4));
                AddMiniBoss(bossChecklist, calamity, "Giant Clam", order, type, DownedGiantClam, null, null, instructions, despawn, () => true);
            }

            // Crabulon
            {
                BossDifficulty.TryGetValue("Crabulon", out float order);
                int type = NPCType<Crabulon>();
                int summon = ItemType<DecapoditaSprout>();
                List<int> collection = new List<int>() { ItemType<CrabulonTrophy>(), ItemType<CrabulonMask>(), ItemType<KnowledgeCrabulon>() };
                string instructions = $"Use a [i:{summon}] in the Mushroom Biome";
                string despawn = CalamityUtils.ColorMessage("The mycleium crab has lost interest.", new Color(0x64, 0x95, 0xED));
                AddBoss(bossChecklist, calamity, "Crabulon", order, type, DownedCrabulon, summon, collection, instructions, despawn, () => true);
            }

            // Hive Mind
            {
                BossDifficulty.TryGetValue("HiveMind", out float order);
                int type = NPCType<HiveMind>();
                int summon = ItemType<Teratoma>();
                List<int> collection = new List<int>() { ItemType<HiveMindTrophy>(), ItemType<HiveMindMask>(), ItemType<KnowledgeHiveMind>(), ItemType<RottingEyeball>() };
                string instructions = $"Kill a Cyst in the Corruption or use a [i:{summon}] in the Corruption";
                string despawn = CalamityUtils.ColorMessage("The corrupted colony began searching for a new breeding ground.", new Color(0x94, 0x00, 0xD3));
                string bossHeadTex = "CalamityMod/NPCs/HiveMind/HiveMindP2_Head_Boss";
                AddBoss(bossChecklist, calamity, "The Hive Mind", order, type, DownedHiveMind, summon, collection, instructions, despawn, () => true, null, bossHeadTex);
            }

            // Perforators
            {
                BossDifficulty.TryGetValue("Perforators", out float order);
                int type = NPCType<PerforatorHive>();
                int summon = ItemType<BloodyWormFood>();
                List<int> collection = new List<int>() { ItemType<PerforatorTrophy>(), ItemType<PerforatorMask>(), ItemType<KnowledgePerforators>(), ItemType<BloodyVein>() };
                string instructions = $"Kill a Cyst in the Crimson or use a [i:{summon}] in the Crimson";
                string despawn = CalamityUtils.ColorMessage("The parasitic hive began searching for a new host.", new Color(0xDC, 0x14, 0x3C));
                AddBoss(bossChecklist, calamity, "The Perforators", order, type, DownedPerfs, summon, collection, instructions, despawn, () => true);
            }

            // Slime God
            {
                BossDifficulty.TryGetValue("SlimeGod", out float order);
                List<int> bosses = new List<int>() { NPCType<SlimeGodCore>(), NPCType<EbonianSlimeGod>(), NPCType<CrimulanSlimeGod>() };
                int summon = ItemType<OverloadedSludge>();
                List<int> collection = new List<int>() { ItemType<SlimeGodTrophy>(), ItemType<SlimeGodMask>(), ItemType<SlimeGodMask2>(), ItemType<KnowledgeSlimeGod>() };
                string instructions = $"Use an [i:{summon}]";
                string despawn = CalamityUtils.ColorMessage("The gelatinous monstrosity achieved vengeance for its brethren.", new Color(0xBA, 0x55, 0x33));
                AddBoss(bossChecklist, calamity, "Slime God", order, bosses, DownedSlimeGod, summon, collection, instructions, despawn, () => true);
            }

            // Cryogen
            {
                BossDifficulty.TryGetValue("Cryogen", out float order);
                int type = NPCType<Cryogen>();
                int summon = ItemType<CryoKey>();
                List<int> collection = new List<int>() { ItemType<CryogenTrophy>(), ItemType<CryogenMask>(), ItemType<KnowledgeCryogen>() };
                string instructions = $"Use a [i:{summon}] in the Snow Biome";
                string despawn = CalamityUtils.ColorMessage("Cryogen drifts away, carried on a freezing wind.", new Color(0x00, 0xFF, 0xFF));
                string bossLogTex = "CalamityMod/NPCs/Cryogen/Cryogen_Phase1_Head_Boss";
                AddBoss(bossChecklist, calamity, "Cryogen", order, type, DownedCryogen, summon, collection, instructions, despawn, () => true, null, bossLogTex);
            }

            // Aquatic Scourge
            {
                BossDifficulty.TryGetValue("AquaticScourge", out float order);
                List<int> segments = new List<int>() { NPCType<AquaticScourgeHead>(), NPCType<AquaticScourgeBody>(), NPCType<AquaticScourgeBodyAlt>(), NPCType<AquaticScourgeTail>() };
                int summon = ItemType<Seafood>();
                List<int> collection = new List<int>() { ItemType<AquaticScourgeTrophy>(), ItemType<AquaticScourgeMask>(), ItemType<KnowledgeAquaticScourge>(), ItemType<KnowledgeSulphurSea>() };
                string instructions = $"Use a [i:{summon}] in the Sulphuric Sea or wait for it to spawn in the Sulphuric Sea";
                string despawn = CalamityUtils.ColorMessage("The Aquatic Scourge swam back into the open ocean.", new Color(0xF0, 0xE6, 0x8C));
                string bossLogTex = "CalamityMod/NPCs/AquaticScourge/AquaticScourgeHead_Head_Boss";
                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AquaticScourge/AquaticScourge_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, "Aquatic Scourge", order, segments, DownedAquaticScourge, summon, collection, instructions, despawn, () => true, portrait, bossLogTex);
            }

            // Brimstone Elemental
            {
                BossDifficulty.TryGetValue("BrimstoneElemental", out float order);
                int type = NPCType<BrimstoneElemental>();
                int summon = ItemType<CharredIdol>();
                List<int> collection = new List<int>() { ItemType<BrimstoneElementalTrophy>(), ItemType<BrimstoneWaifuMask>(), ItemType<KnowledgeBrimstoneCrag>(), ItemType<KnowledgeBrimstoneElemental>(), ItemType<CharredRelic>() };
                string instructions = $"Use a [i:{summon}] in the Brimstone Crag";
                string despawn = CalamityUtils.ColorMessage("Brimstone Elemental withdraws to the ruins of her shrine.", new Color(0xDC, 0x14, 0x3C));
                AddBoss(bossChecklist, calamity, "Brimstone Elemental", order, type, DownedBrimstoneElemental, summon, collection, instructions, despawn, () => true);
            }

            // Calamitas
            {
                BossDifficulty.TryGetValue("Calamitas", out float order);
                int type = NPCType<CalamitasClone>();
                int summon = ItemType<EyeofDesolation>();
                List<int> collection = new List<int>() { ItemType<CalamitasTrophy>(), ItemType<CataclysmTrophy>(), ItemType<CatastropheTrophy>(), ItemType<CalamitasMask>(), ItemType<HoodOfCalamity>(), ItemType<RobesOfCalamity>(), ItemType<KnowledgeCalamitasClone>() };
                string instructions = $"Use an [i:{summon}] at Night";
                string despawn = CalamityUtils.ColorMessage("If you wanted a fight, you should've came more prepared.", new Color(0xFF, 0xA5, 0x00));
                //Func<bool> available = () => !DownedBossSystem.downedCalamitas;
                AddBoss(bossChecklist, calamity, "The Calamitas Clone", order, type, DownedCalamitas, summon, collection, instructions, despawn, () => true);
                //AddBoss(bossChecklist, calamity, "Calamitas", order, type, DownedCalamitas, summon, collection, instructions, despawn, available);
            }

            // Great Sand Shark
            {
                BossDifficulty.TryGetValue("GreatSandShark", out float order);
                int type = NPCType<GreatSandShark>();
                int summon = ItemType<SandstormsCore>();
                List<int> collection = new List<int>() { ItemID.MusicBoxSandstorm };
                string instructions = $"Kill 10 sand sharks after defeating Plantera or use a [i:{summon}] in the Desert Biome";
                string despawn = CalamityUtils.ColorMessage("The apex predator of the sands disappears into the dunes...", new Color(0xDA, 0xA5, 0x20));
                AddMiniBoss(bossChecklist, calamity, "Great Sand Shark", order, type, DownedGSS, summon, collection, instructions, despawn, () => true);
            }

            // Siren and Leviathan
            {
                BossDifficulty.TryGetValue("Leviathan", out float order);
                List<int> bosses = new List<int>() { NPCType<Leviathan>(), NPCType<Anahita>() };
                List<int> collection = new List<int>() { ItemType<LeviathanTrophy>(), ItemType<AnahitaTrophy>(), ItemType<LeviathanMask>(), ItemType<AnahitaMask>(), ItemType<KnowledgeOcean>(), ItemType<KnowledgeLeviathanAnahita>() };
                string instructions = "By killing an unknown entity in the Ocean Biome";
                string despawn = CalamityUtils.ColorMessage("The aquatic entities sink back beneath the ocean depths.", new Color(0x7F, 0xFF, 0xD4));

                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Leviathan/AnahitaLevi_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, "Leviathan", order, bosses, DownedLeviathan, null, collection, instructions, despawn, () => true, portrait);
            }

            // Astrum Aureus
            {
                BossDifficulty.TryGetValue("AstrumAureus", out float order);
                int type = NPCType<AstrumAureus>();
                int summon = ItemType<AstralChunk>();
                List<int> collection = new List<int>() { ItemType<AstrumAureusTrophy>(), ItemType<AstrumAureusMask>(), ItemType<KnowledgeAstrumAureus>() };
                string instructions = $"Use an [i:{summon}] at Night in the Astral Biome";
                string despawn = CalamityUtils.ColorMessage("Astrum Aureus’ program has been executed. Initiate recall.", new Color(0xFF, 0xD7, 0x00));
                string bossLogTex = "CalamityMod/NPCs/AstrumAureus/AstrumAureus_Head_Boss";
                AddBoss(bossChecklist, calamity, "Astrum Aureus", order, type, DownedAureus, summon, collection, instructions, despawn, () => true, null, bossLogTex);
            }

            // Plaguebringer Goliath
            {
                BossDifficulty.TryGetValue("PlaguebringerGoliath", out float order);
                int type = NPCType<PlaguebringerGoliath>();
                int summon = ItemType<Abombination>();
                List<int> collection = new List<int>() { ItemType<PlaguebringerGoliathTrophy>(), ItemType<PlaguebringerGoliathMask>(), ItemType<KnowledgePlaguebringerGoliath>(), ItemType<PlagueCaller>() };
                string instructions = $"Use an [i:{summon}] in the Jungle Biome";
                string despawn = CalamityUtils.ColorMessage("HOSTILE SPECIMENS TERMINATED. INITIATE RECALL TO HOME BASE.", new Color(0x00, 0xFF, 0x00));
                string bossLogTex = "CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliath_Head_Boss";

                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/PlaguebringerGoliath/PlaguebringerGoliath_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, "Plaguebringer Goliath", order, type, DownedPBG, summon, collection, instructions, despawn, () => true, portrait, bossLogTex);
            }

            // Ravager
            {
                BossDifficulty.TryGetValue("Ravager", out float order);
                List<int> segments = new List<int>() { NPCType<RavagerBody>(), NPCType<RavagerClawLeft>(), NPCType<RavagerClawRight>(), NPCType<RavagerHead>(), NPCType<RavagerLegLeft>(), NPCType<RavagerLegRight>() };
                int summon = ItemType<DeathWhistle>();
                List<int> collection = new List<int>() { ItemType<RavagerTrophy>(), ItemType<RavagerMask>(), ItemType<KnowledgeRavager>() };
                string instructions = $"Use a [i:{summon}]";
                string despawn = CalamityUtils.ColorMessage("The automaton of misshapen victims went looking for the true perpetrator.", new Color(0xB2, 0x22, 0x22));
                string bossLogTex = "CalamityMod/NPCs/Ravager/RavagerBody_Head_Boss";

                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Ravager/Ravager_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, "Ravager", order, segments, DownedRavager, summon, collection, instructions, despawn, () => true, portrait, bossLogTex);
            }

            // Astrum Deus
            {
                BossDifficulty.TryGetValue("AstrumDeus", out float order);
                List<int> segments = new List<int>() { NPCType<AstrumDeusHead>(), NPCType<AstrumDeusBody>(), NPCType<AstrumDeusTail>() };
                int summon1 = ItemType<TitanHeart>();
                int summon2 = ItemType<Starcore>();
                int altar = ItemType<AstralBeaconItem>();
                List<int> summons = new List<int>() { summon1, summon2 };
                List<int> collection = new List<int>() { ItemType<AstrumDeusTrophy>(), ItemType<AstrumDeusMask>(), ItemType<KnowledgeAstrumDeus>(), ItemType<KnowledgeAstralInfection>(), ItemType<ChromaticOrb>() };
                string instructions = $"Use a [i:{summon1}] or [i:{summon2}] as offering at an [i:{altar}]";
                string despawn = CalamityUtils.ColorMessage("The infected deity retreats to the heavens.", new Color(0xFF, 0xD7, 0x00));
                string bossLogTex = "CalamityMod/NPCs/AstrumDeus/AstrumDeusHead_Head_Boss";

                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeus_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };

                AddBoss(bossChecklist, calamity, "Astrum Deus", order, segments, DownedDeus, summons, collection, instructions, despawn, () => true, portrait, bossLogTex);
            }

            // Profaned Guardians
            {
                BossDifficulty.TryGetValue("ProfanedGuardians", out float order);
                int type = NPCType<ProfanedGuardianCommander>();
                int summon = ItemType<ProfanedShard>();
                List<int> collection = new List<int>() { ItemType<ProfanedGuardianTrophy>(), ItemType<ProfanedGuardianMask>(), ItemType<KnowledgeProfanedGuardians>() };
                string instructions = $"Use a [i:{summon}] in the Hallow or Underworld Biomes";
                string despawn = CalamityUtils.ColorMessage("The guardians must protect their goddess at all costs.", new Color(0xFF, 0xA5, 0x00));
                string bossLogTex = "CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianCommander_Head_Boss";

                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardians_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, "Profaned Guardians", order, type, DownedGuardians, summon, collection, instructions, despawn, () => true, portrait, bossLogTex);
            }

            // Dragonfolly
            {
                BossDifficulty.TryGetValue("Dragonfolly", out float order);
                int type = NPCType<Bumblefuck>();
                int summon = ItemType<ExoticPheromones>();
                List<int> collection = new List<int>() { ItemType<DragonfollyTrophy>(), ItemType<BumblefuckMask>(), ItemType<KnowledgeDragonfolly>() };
                string instructions = $"Use [i:{summon}] in the Jungle Biome";
                string despawn = CalamityUtils.ColorMessage("The failed experiment returns to its reproductive routine.", new Color(0xFF, 0xD7, 0x00));
                AddBoss(bossChecklist, calamity, "Dragonfolly", order, type, DownedBirb, summon, collection, instructions, despawn, () => true);
            }

            // Providence
            {
                BossDifficulty.TryGetValue("Providence", out float order);
                int type = NPCType<Providence>();
                int summon = ItemType<ProfanedCore>();
                List<int> collection = new List<int>() { ItemType<ProvidenceTrophy>(), ItemType<ProvidenceMask>(), ItemType<KnowledgeProvidence>() };
                string instructions = $"Use [i:{summon}] in the Hallow or Underworld Biomes";
                string despawn = CalamityUtils.ColorMessage("The Profaned Goddess vanishes in a burning blaze.", new Color(0xFF, 0xA5, 0x00));
                string bossLogTex = "CalamityMod/NPCs/Providence/Providence_Head_Boss";

                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Providence/Providence_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, "Providence", order, type, DownedProvidence, summon, collection, instructions, despawn, () => true, portrait, bossLogTex);
            }

            // Ceaseless Void
            {
                BossDifficulty.TryGetValue("CeaselessVoid", out float order);
                List<int> bosses = new List<int>() { NPCType<CeaselessVoid>(), NPCType<DarkEnergy>() };
                int summon = ItemType<RuneofKos>();
                List<int> collection = new List<int>() { ItemType<CeaselessVoidTrophy>(), ItemType<CeaselessVoidMask>(), ItemType<AncientGodSlayerHelm>(), ItemType<AncientGodSlayerChestplate>(), ItemType<AncientGodSlayerLeggings>(), ItemType<KnowledgeSentinels>() };
                string instructions = $"Use a [i:{summon}] in the Dungeon";
                string despawn = CalamityUtils.ColorMessage("The rift in time and space has moved away from your reach.", new Color(0x4B, 0x00, 0x82));
                AddBoss(bossChecklist, calamity, "Ceaseless Void", order, bosses, DownedCeaselessVoid, summon, collection, instructions, despawn, () => true);
            }

            // Storm Weaver
            {
                BossDifficulty.TryGetValue("StormWeaver", out float order);
                List<int> segments = new List<int>() { NPCType<StormWeaverHead>(), NPCType<StormWeaverBody>(), NPCType<StormWeaverTail>() };
                int summon = ItemType<RuneofKos>();
                List<int> collection = new List<int>() { ItemType<WeaverTrophy>(), ItemType<StormWeaverMask>(), ItemType<AncientGodSlayerHelm>(), ItemType<AncientGodSlayerChestplate>(), ItemType<AncientGodSlayerLeggings>(), ItemType<KnowledgeSentinels>(), ItemType<LittleLight>() };
                string instructions = $"Use a [i:{summon}] in Space";
                string despawn = CalamityUtils.ColorMessage("Storm Weaver hid itself once again within the stormfront.", new Color(0xEE, 0x82, 0xEE));
                string bossLogTex = "CalamityMod/NPCs/StormWeaver/StormWeaverHead_Head_Boss";
                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/StormWeaver/StormWeaver_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, "Storm Weaver", order, segments, DownedStormWeaver, summon, collection, instructions, despawn, () => true, portrait, bossLogTex);
            }

            // Signus
            {
                BossDifficulty.TryGetValue("Signus", out float order);
                int type = NPCType<Signus>();
                int summon = ItemType<RuneofKos>();
                List<int> collection = new List<int>() { ItemType<SignusTrophy>(), ItemType<SignusMask>(), ItemType<AncientGodSlayerHelm>(), ItemType<AncientGodSlayerChestplate>(), ItemType<AncientGodSlayerLeggings>(), ItemType<KnowledgeSentinels>() };
                string instructions = $"Use a [i:{summon}] in the Underworld";
                string despawn = CalamityUtils.ColorMessage("The Devourer's assassin has finished its easy task.", new Color(0xBA, 0x55, 0xD3));
                AddBoss(bossChecklist, calamity, "Signus", order, type, DownedSignus, summon, collection, instructions, despawn, () => true);
            }

            // Polterghast
            {
                BossDifficulty.TryGetValue("Polterghast", out float order);
                List<int> bosses = new List<int>() { NPCType<Polterghast>(), NPCType<PolterPhantom>() };
                int summon = ItemType<NecroplasmicBeacon>();
                List<int> collection = new List<int>() { ItemType<PolterghastTrophy>(), ItemType<PolterghastMask>(), ItemType<KnowledgePolterghast>() };
                string instructions = $"Kill 30 phantom spirits or use a [i:{summon}] in the Dungeon";
                string despawn = CalamityUtils.ColorMessage("The volatile spirits disperse throughout the depths of the dungeon.", new Color(0xB0, 0xE0, 0xE6));
                AddBoss(bossChecklist, calamity, "Polterghast", order, bosses, DownedPolterghast, summon, collection, instructions, despawn, () => true);
            }

            // Old Duke
            {
                BossDifficulty.TryGetValue("OldDuke", out float order);
                List<int> bosses = new List<int>() { NPCType<OldDuke>() };
                int summon = ItemType<BloodwormItem>();
                List<int> collection = new List<int>() { ItemType<OldDukeTrophy>(), ItemType<OldDukeMask>(), ItemType<KnowledgeOldDuke>() };
                string instructions = $"Defeat the Acid Rain event post-Polterghast or fish using a [i:{summon}] in the Sulphurous Sea";
                string despawn = CalamityUtils.ColorMessage("The old duke disappears amidst the acidic downpour.", new Color(0xF0, 0xE6, 0x8C));
                AddBoss(bossChecklist, calamity, "Old Duke", order, bosses, DownedBoomerDuke, summon, collection, instructions, despawn, () => true);
            }

            // Devourer of Gods
            {
                BossDifficulty.TryGetValue("DevourerOfGods", out float order);
                int type = NPCType<DevourerofGodsHead>();
                int summon = ItemType<CosmicWorm>();
                List<int> collection = new List<int>() { ItemType<DevourerofGodsTrophy>(), ItemType<DevourerofGodsMask>(), ItemType<KnowledgeDevourerofGods>() };
                string instructions = $"Use a [i:{summon}]";
                string despawn = CalamityUtils.ColorMessage("The Devourer of Gods has slain everyone and feasted on their essence.", new Color(0x00, 0xFF, 0xFF));
                string bossHeadTex = "CalamityMod/NPCs/DevourerofGods/DevourerofGodsHead_Head_Boss";
                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/DevourerofGods/DevourerofGods_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, "Devourer of Gods", order, type, DownedDoG, summon, collection, instructions, despawn, () => true, portrait, bossHeadTex);
            }

            // Yharon
            {
                BossDifficulty.TryGetValue("Yharon", out float order);
                int type = NPCType<Yharon>();
                int summon = ItemType<JungleDragonEgg>();
                List<int> collection = new List<int>() { ItemType<YharonTrophy>(), ItemType<YharonMask>(), ItemType<KnowledgeYharon>(), ItemType<ForgottenDragonEgg>(), ItemType<McNuggets>(), ItemType<FoxDrive>() };
                string instructions = $"Use a [i:{summon}] in the Jungle Biome";
                string despawn = CalamityUtils.ColorMessage("Yharon found you too weak to stay near your gravestone.", new Color(0xFF, 0xA5, 0x00));
                string bossLogTex = "CalamityMod/NPCs/Yharon/Yharon_Head_Boss";
                AddBoss(bossChecklist, calamity, "Yharon", order, type, DownedYharon, summon, collection, instructions, despawn, () => true, null, bossLogTex);
            }

            // Exo Mechs
            // Collection requires edits
            // Instructions require edits
            // Despawn requires edits
            {
                BossDifficulty.TryGetValue("ExoMechs", out float order);
                List<int> bosses = new List<int>() { NPCType<Apollo>(), NPCType<AresBody>(), NPCType<Artemis>(), NPCType<ThanatosHead>() };
                List<int> collection = new List<int>() { ItemType<AresTrophy>(), ItemType<ThanatosTrophy>(), ItemType<ArtemisTrophy>(), ItemType<ApolloTrophy>(), ItemType<DraedonMask>(), ItemType<AresMask>(), ItemType<ThanatosMask>(), ItemType<ArtemisMask>(), ItemType<ApolloMask>(), ItemType<KnowledgeExoMechs>() };
                string instructions = "By using a high-tech computer";
                string despawn = CalamityUtils.ColorMessage("An imperfection after all... what a shame.", new Color(0x7F, 0xFF, 0xD4));

                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = Request<Texture2D>("CalamityMod/NPCs/ExoMechs/ExoMechs_BossChecklist").Value;
                    float scale = 0.7f;
                    Vector2 centered = new Vector2(rect.Center.X - texture.Width * scale / 2, rect.Center.Y - texture.Height * scale / 2);
                    sb.Draw(texture, centered, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                };

                AddBoss(bossChecklist, calamity, "Exo Mechs", order, bosses, DownedExoMechs, null, collection, instructions, despawn, () => true, portrait);
            }

            // Supreme Calamitas
            {
                BossDifficulty.TryGetValue("SupremeCalamitas", out float order);
                int type = NPCType<SupremeCalamitas>();
                int summon1 = ItemType<AshesofCalamity>();
                int summon2 = ItemType<CeremonialUrn>();
                int altar = ItemType<AltarOfTheAccursedItem>();
                List<int> summons = new List<int>() { summon1, summon2 };
                List<int> collection = new List<int>() { ItemType<SupremeCalamitasTrophy>(), ItemType<SupremeCataclysmTrophy>(), ItemType<SupremeCatastropheTrophy>(), ItemType<AshenHorns>(), ItemType<SCalMask>(), ItemType<SCalRobes>(), ItemType<SCalBoots>(), ItemType<KnowledgeCalamitas>(), ItemType<BrimstoneJewel>(), ItemType<Levi>() };
                string instructions = $"Use [i:{summon1}] or a [i:{summon2}] as offering at an [i:{altar}]";
                string despawn = CalamityUtils.ColorMessage("Please don't waste my time.", new Color(0xFF, 0xA5, 0x00));
                string bossHeadTex = "CalamityMod/NPCs/SupremeCalamitas/HoodedHeadIcon";
                AddBoss(bossChecklist, calamity, "Supreme Calamitas", order, type, DownedSCal, summons, collection, instructions, despawn, () => true, null, bossHeadTex);
            }

            // Adult Eidolon Wyrm
            {
                BossDifficulty.TryGetValue("AdultEidolonWyrm", out float order);
                int type = NPCType<AdultEidolonWyrmHead>();
                int summon = ItemID.RodofDiscord;
                List<int> collection = new List<int>() { };
                string instructions = $"While in the Abyss, use an item that inflicts Chaos State";
                string despawn = CalamityUtils.ColorMessage("...", new Color(0x7F, 0xFF, 0xD4));
                string bossLogTex = "CalamityMod/NPCs/AdultEidolonWyrm/AdultEidolonWyrmHead_Head_Boss";

                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AdultEidolonWyrm/AdultEidolonWyrm_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddBoss(bossChecklist, calamity, "Adult Eidolon Wyrm", order, type, DownedAdultEidolonWyrm, summon, collection, instructions, despawn, () => true, portrait, bossLogTex);
            }
        }

        private static void AddCalamityInvasions(Mod bossChecklist, Mod calamity)
        {
            // Initial Acid Rain
            {
                InvasionDifficulty.TryGetValue("Acid Rain Initial", out float order);
                List<int> enemies = AcidRainEvent.PossibleEnemiesPreHM.Select(enemy => enemy.Key).ToList();
                int summon = ItemType<CausticTear>();
                List<int> collection = new List<int>() { ItemType<RadiatingCrystal>() };
                string instructions = $"Use a [i:{summon}] or wait for the invasion to occur naturally after the Eye of Cthulhu is defeated.";
                string iconTexture = "CalamityMod/ExtraTextures/UI/AcidRainIcon";
                //TODO: Fix the portrait scaling/sizing
                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Events/AcidRainT1_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddInvasion(bossChecklist, calamity, "Acid Rain", order, enemies, DownedAcidRainInitial, summon, collection, instructions, () => true, portrait, iconTexture);
            }
            // Post-Aquatic Scourge Acid Rain
            {
                InvasionDifficulty.TryGetValue("Acid Rain Aquatic Scourge", out float order);
                List<int> enemies = AcidRainEvent.PossibleEnemiesAS.Select(enemy => enemy.Key).ToList();
                enemies.Add(ModContent.NPCType<IrradiatedSlime>());
                enemies.AddRange(AcidRainEvent.PossibleMinibossesAS.Select(miniboss => miniboss.Key));
                List<int> summons = new List<int>() { ItemType<CausticTear>() };
                List<int> collection = new List<int>() { ItemType<RadiatingCrystal>() };
                string instructions = $"Use a [i:{ItemType<CausticTear>()}] or wait for the invasion to occur naturally after the Aquatic Scourge is defeated";
                string iconTexture = "CalamityMod/ExtraTextures/UI/AcidRainIcon";
                //TODO: Fix the portrait scaling/sizing
                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Events/AcidRainT2_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddInvasion(bossChecklist, calamity, "Acid Rain (Post-AS)", order, enemies, DownedAcidRainHardmode, summons, collection, instructions, () => true, portrait, iconTexture);
            }
            // Post-Polterghast Acid Rain
            {
                InvasionDifficulty.TryGetValue("Acid Rain Polterghast", out float order);
                List<int> enemies = AcidRainEvent.PossibleEnemiesPolter.Select(enemy => enemy.Key).ToList();
                enemies.AddRange(AcidRainEvent.PossibleMinibossesPolter.Select(miniboss => miniboss.Key));
                List<int> summons = new List<int>() { ItemType<CausticTear>() };
                List<int> collection = new List<int>() { ItemType<RadiatingCrystal>() };
                string instructions = $"Use a [i:{ItemType<CausticTear>()}] or wait for the invasion to occur naturally after the Polterghast is defeated";
                string iconTexture = "CalamityMod/ExtraTextures/UI/AcidRainIcon";
                //TODO: Fix the portrait scaling/sizing
                var portrait = (SpriteBatch sb, Rectangle rect, Color color) => {
                    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Events/AcidRainT3_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };
                AddInvasion(bossChecklist, calamity, "Acid Rain (Post-Polter)", order, enemies, DownedBoomerDuke, summons, collection, instructions, () => true, portrait, iconTexture);
            }
        }

        private static void AddCalamityBossLoot(Mod bossChecklist)
        {
            // King Slime
            AddLoot(bossChecklist, "KingSlime",
                new List<int>() { ItemType<CrownJewel>() },
                new List<int>() { ItemType<KnowledgeKingSlime>() }
            );

            // Eye of Cthulhu
            AddLoot(bossChecklist, "EyeofCthulhu",
                new List<int>() { ItemType<DeathstareRod>(), ItemType<TeardropCleaver>() },
                new List<int>() { ItemType<KnowledgeEyeofCthulhu>() }
            );

            // Eater of Worlds
            AddLoot(bossChecklist, "EaterofWorldsHead",
                null,
                new List<int>() { ItemType<KnowledgeEaterofWorlds>(), ItemType<KnowledgeCorruption>() }
            );

            // Brain of Cthulhu
            AddLoot(bossChecklist, "BrainofCthulhu",
                null,
                new List<int>() { ItemType<KnowledgeBrainofCthulhu>(), ItemType<KnowledgeCrimson>() }
            );

            // Queen Bee
            AddLoot(bossChecklist, "QueenBee",
                new List<int>() { ItemType<HardenedHoneycomb>(), ItemID.Stinger, ItemType<TheBee>() },
                new List<int>() { ItemType<KnowledgeQueenBee>() }
            );

            // Skeletron
            AddLoot(bossChecklist, "SkeletronHead",
                null,
                new List<int>() { ItemType<KnowledgeSkeletron>() }
            );

            // Wall of Flesh
            AddLoot(bossChecklist, "WallofFlesh",
                new List<int>() { ItemType<Meowthrower>(), ItemType<BlackHawkRemote>(), ItemType<BlastBarrel>(), ItemType<RogueEmblem>(), ItemType<Carnage>(), ItemID.CorruptionKey, ItemID.CrimsonKey },
                new List<int>() { ItemType<KnowledgeWallofFlesh>(), ItemType<KnowledgeUnderworld>(), ItemType<HermitsBoxofOneHundredMedicines>() }
            );

            // Queen Slime
            AddLoot(bossChecklist, "QueenSlime",
                new List<int>() { ItemID.HallowedKey }
            );

            // The Twins
            AddLoot(bossChecklist, "TheTwins",
                new List<int>() { ItemType<Arbalest>() },
                new List<int>() { ItemType<KnowledgeTwins>(), ItemType<KnowledgeMechs>() }
            );

            // The Destroyer
            AddLoot(bossChecklist, "TheDestroyer",
                new List<int>() { ItemType<KnowledgeDestroyer>(), ItemType<KnowledgeMechs>() }
            );

            // Skeletron Prime
            AddLoot(bossChecklist, "SkeletronPrime",
                null,
                new List<int>() { ItemType<KnowledgeSkeletronPrime>(), ItemType<KnowledgeMechs>() }
            );

            // Plantera
            AddLoot(bossChecklist, "Plantera",
                new List<int>() { ItemType<LivingShard>(), ItemType<BlossomFlux>(), ItemType<BloomStone>(), ItemID.JungleKey },
                new List<int>() { ItemType<KnowledgePlantera>() }
            );
            AddSummons(bossChecklist, "Plantera", new List<int>() { ItemType<Portabulb>() });

            // Golem
            AddLoot(bossChecklist, "Golem",
                new List<int>() { ItemType<EssenceofSunlight>(), ItemType<AegisBlade>() },
                new List<int>() { ItemType<KnowledgeGolem>() }
            );
            AddSummons(bossChecklist, "Golem", new List<int>() { ItemType<OldPowerCell>() });

            // Duke Fishron
            AddLoot(bossChecklist, "DukeFishron",
                new List<int>() { ItemType<DukesDecapitator>(), ItemType<BrinyBaron>() },
                new List<int>() { ItemType<KnowledgeDukeFishron>() }
            );

            // Betsy
            AddLoot(bossChecklist, "DD2Betsy",
                null,
                null
            );

            // Lunatic Cultist
            AddLoot(bossChecklist, "CultistBoss",
                null,
                new List<int>() { ItemType<KnowledgeLunaticCultist>(), ItemType<KnowledgeBloodMoon>() }
            );
            AddSummons(bossChecklist, "CultistBoss", new List<int>() { ItemType<EidolonTablet>() });

            // Moon Lord
            AddLoot(bossChecklist, "MoonLord",
                new List<int>() { ItemType<UtensilPoker>(), ItemType<CelestialOnion>() },
                new List<int>() { ItemType<KnowledgeMoonLord>() }
            );
        }

        private static void AddCalamityEventLoot(Mod bossChecklist)
        {
            // Blood Moon
            AddLoot(bossChecklist, "Blood Moon",
                new List<int>() { ItemType<BloodOrb>(), ItemType<BouncingEyeball>() },
                null
            );

            // Goblin Army
            AddLoot(bossChecklist, "Goblin Army",
                new List<int>() { ItemType<PlasmaRod>(), ItemType<TheFirstShadowflame>(), ItemType<BurningStrife>() },
                null
            );

            // Solar Eclipse
            AddLoot(bossChecklist, "Solar Eclipse",
                new List<int>() { ItemType<SolarVeil>(), ItemType<DefectiveSphere>(), ItemType<DarksunFragment>() },
                null
            );

            // Pumpkin Moon
            AddLoot(bossChecklist, "Pumpkin Moon",
                new List<int>() { ItemType<NightmareFuel>() },
                null
            );
            AddLoot(bossChecklist, "Pumpking",
                new List<int>() { ItemType<NightmareFuel>() },
                null
            );

            // Frost Moon
            AddLoot(bossChecklist, "Frost Moon",
                new List<int>() { ItemType<EndothermicEnergy>() },
                null
            );
            AddLoot(bossChecklist, "Ice Queen",
                new List<int>() { ItemType<EndothermicEnergy>() },
                null
            );

            // Martian Madness
            AddLoot(bossChecklist, "Martian Madness",
                new List<int>() { ItemType<Wingman>(), ItemType<ShockGrenade>(), ItemType<NullificationRifle>() },
                null
            );
            AddLoot(bossChecklist, "Martian Saucer",
                new List<int>() { ItemType<NullificationRifle>() },
                null
            );

            // Lunar Events
            AddLoot(bossChecklist, "Lunar Event",
                new List<int>() { ItemType<MeldBlob>() },
                null
            );
        }

        private static void FargosSupport()
        {
            Mod fargos = GetInstance<CalamityMod>().fargos;
            if (fargos is null)
                return;

            void AddToMutantShop(string bossName, string summonItemName, Func<bool> downed, int price)
            {
                BossDifficulty.TryGetValue(bossName, out float order);
                fargos.Call("AddSummon", order, "CalamityMod", summonItemName, downed, price);
            }

            fargos.Call("AbominationnClearEvents", "CalamityMod", AcidRainEvent.AcidRainEventIsOngoing, true);

            AddToMutantShop("OldDuke", "BloodwormItem", DownedBoomerDuke, Item.buyPrice(platinum: 2));
        }

        private static void CensusSupport()
        {
            Mod censusMod = GetInstance<CalamityMod>().census;
            if (censusMod != null)
            {
                censusMod.Call("TownNPCCondition", NPCType<SEAHOE>(), "Defeat a Giant Clam after defeating the Desert Scourge");
                censusMod.Call("TownNPCCondition", NPCType<THIEF>(), "Have a [i:" + ItemID.PlatinumCoin + "] in your inventory after defeating Skeletron");
                censusMod.Call("TownNPCCondition", NPCType<FAP>(), "Have [i:" + ItemType<FabsolsVodka>() + "] in your inventory in Hardmode");
                censusMod.Call("TownNPCCondition", NPCType<DILF>(), "Defeat Cryogen");
                censusMod.Call("TownNPCCondition", NPCType<WITCH>(), "Defeat Supreme Calamitas");
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
            RegisterSummon(ItemType<RustyBeaconPrototype>(), BuffType<RustyDroneBuff>(), ProjectileType<RustyDrone>());
            RegisterSummon(ItemType<SeaboundStaff>(), BuffType<BrittleStar>(), ProjectileType<BrittleStarMinion>());
            RegisterSummon(ItemType<MagicalConch>(), BuffType<HermitCrab>(), ProjectileType<HermitCrabMinion>());
            RegisterSummon(ItemType<DeathstareRod>(), BuffType<MiniatureEyeofCthulhu>(), ProjectileType<DeathstareEyeball>());
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
            RegisterSummon(ItemType<ColdDivinity>(), BuffType<ColdDivinityBuff>(), ProjectileType<ColdDivinityPointyThing>());
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

            sAssociation.Call("AddMinionInfo", ItemType<EntropysVigil>(), BuffType<EntropysVigilBuff>(), new List<int>() { ProjectileType<Calamitamini>(), ProjectileType<Cataclymini>(), ProjectileType<Catastromini>()}, new List<float>() {1-(1f/3f), 2f/3f, 2f/3f});
            //Entropy's Vigil is a bruh moment
            sAssociation.Call("AddMinionInfo", ItemType<ResurrectionButterfly>(), BuffType<ResurrectionButterflyBuff>(), new List<int>() { ProjectileType<PinkButterfly>(), ProjectileType<PurpleButterfly>()});
        }
    }
}
