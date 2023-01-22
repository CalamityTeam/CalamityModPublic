using System.Collections.Generic;
using CalamityMod.CustomRecipes;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static CalamityMod.World.CalamityWorld;

namespace CalamityMod
{
    public class MiscWorldStateSystem : ModSystem
    {
        #region Resetting
        public static void ResetWorldData()
        {
            NPC.LunarShieldPowerExpert = 100;

            CalamityGlobalNPC.holyBoss = -1;
            CalamityGlobalNPC.doughnutBoss = -1;
            CalamityGlobalNPC.doughnutBossDefender = -1;
            CalamityGlobalNPC.doughnutBossHealer = -1;
            CalamityGlobalNPC.voidBoss = -1;
            CalamityGlobalNPC.energyFlame = -1;
            CalamityGlobalNPC.hiveMind = -1;
            CalamityGlobalNPC.astrumAureus = -1;
            CalamityGlobalNPC.scavenger = -1;

            for (int i = 0; i < CalamityGlobalNPC.bobbitWormBottom.Length; i++)
                CalamityGlobalNPC.bobbitWormBottom[i] = -1;

            CalamityGlobalNPC.DoGHead = -1;
            CalamityGlobalNPC.SCal = -1;
            CalamityGlobalNPC.ghostBoss = -1;
            CalamityGlobalNPC.laserEye = -1;
            CalamityGlobalNPC.fireEye = -1;
            CalamityGlobalNPC.brimstoneElemental = -1;
            CalamityGlobalNPC.signus = -1;
            CalamityGlobalNPC.draedonExoMechPrimePlasmaCannon = -1;
            CalamityGlobalNPC.draedonExoMechPrime = -1;
            CalamityGlobalNPC.draedonExoMechTwinGreen = -1;
            CalamityGlobalNPC.draedonExoMechTwinRed = -1;
            CalamityGlobalNPC.draedonExoMechWorm = -1;
            CalamityGlobalNPC.adultEidolonWyrmHead = -1;
            BossRushEvent.BossRushStage = 0;
            ArmoredDiggerSpawnCooldown = 0;
            BossRushEvent.BossRushActive = false;
            BossRushEvent.BossRushSpawnCountdown = 180;
            BossRushEvent.HostileProjectileKillCounter = 0;
            CustomTemple.NewAlterPosition = Point.Zero;
            Abyss.AbyssChasmBottom = 0;
            SulphurousSea.YStart = 0;
            Abyss.AtLeftSideOfWorld = false;

            spawnedBandit = false;
            spawnedCirrus = false;
            foundHomePermafrost = false;

            catName = false;
            dogName = false;
            bunnyName = false;

            onionMode = false;
            revenge = false;
            TalkedToDraedon = false;
            death = false;
            armageddon = false;
            AcidRainEvent.AcidRainEventIsOngoing = false;
            AcidRainEvent.CountdownUntilForcedAcidRain = 0;
        }

        public override void OnWorldLoad() => ResetWorldData();

        public override void OnWorldUnload() => ResetWorldData();
        #endregion

        #region Saving/Loading
        public override void SaveWorldData(TagCompound tag)
        {
            var downed = new List<string>();
            if (onionMode)
                downed.Add("onionMode");
            if (TalkedToDraedon)
                downed.Add("TalkedToDraedon");
            if (revenge)
                downed.Add("revenge");
            if (death)
                downed.Add("death");
            if (Abyss.AtLeftSideOfWorld)
                downed.Add("abyssSide");
            if (BossRushEvent.BossRushActive)
                downed.Add("bossRushActive");
            if (AcidRainEvent.AcidRainEventIsOngoing)
                downed.Add("acidRain");
            if (spawnedBandit)
                downed.Add("bandit");
            if (spawnedCirrus)
                downed.Add("drunkPrincess");
            if (foundHomePermafrost)
                downed.Add("archmageHome");

            #region Save Pet Names
            if (catName)
                downed.Add("catName");
            if (dogName)
                downed.Add("dogName");
            if (bunnyName)
                downed.Add("bunnyName");
			#endregion

            if (AcidRainEvent.HasTriedToSummonOldDuke)
                downed.Add("spawnedBoomer");
            if (AcidRainEvent.HasStartedAcidicDownpour)
                downed.Add("startDownpour");
            if (AcidRainEvent.HasBeenForceStartedByEoCDefeat)
                downed.Add("forcedRain");
            if (AcidRainEvent.OldDukeHasBeenEncountered)
                downed.Add("encounteredOldDuke");
            if (HasGeneratedLuminitePlanetoids)
                downed.Add("HasGeneratedLuminitePlanetoids");
            downed.AddWithCondition("IsWorldAfterDraedonUpdate", IsWorldAfterDraedonUpdate);

            downed.AddWithCondition("TinOreWorld", OreTypes[0] == TileID.Tin);
            downed.AddWithCondition("LeadOreWorld", OreTypes[1] == TileID.Lead);
            downed.AddWithCondition("TungstenOreWorld", OreTypes[2] == TileID.Tungsten);
            downed.AddWithCondition("PlatinumOreWorld", OreTypes[3] == TileID.Platinum);

            RecipeUnlockHandler.Save(downed);

            tag["downed"] = downed;
            tag["abyssChasmBottom"] = Abyss.AbyssChasmBottom;
            tag["SulphSeaYStart"] = SulphurousSea.YStart;
            tag["acidRainPoints"] = AcidRainEvent.AccumulatedKillPoints;
            tag["Reforges"] = Reforges;
            tag["MoneyStolenByBandit"] = MoneyStolenByBandit;

            tag["SunkenSeaLabCenter"] = SunkenSeaLabCenter;
            tag["PlanetoidLabCenter"] = PlanetoidLabCenter;
            tag["JungleLabCenter"] = JungleLabCenter;
            tag["HellLabCenter"] = HellLabCenter;
            tag["IceLabCenter"] = IceLabCenter;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            onionMode = downed.Contains("onionMode");
            TalkedToDraedon = downed.Contains("TalkedToDraedon");
            revenge = downed.Contains("revenge");
            death = downed.Contains("death");
            Abyss.AtLeftSideOfWorld = downed.Contains("abyssSide");
            BossRushEvent.BossRushActive = downed.Contains("bossRushActive");
            AcidRainEvent.AcidRainEventIsOngoing = downed.Contains("acidRain");
            spawnedBandit = downed.Contains("bandit");
            spawnedCirrus = downed.Contains("drunkPrincess");
            foundHomePermafrost = downed.Contains("archmageHome");

            #region Load Pet Names
            catName = downed.Contains("catName");
            dogName = downed.Contains("dogName");
            bunnyName = downed.Contains("bunnyName");
			#endregion

            AcidRainEvent.HasTriedToSummonOldDuke = downed.Contains("spawnedBoomer");
            AcidRainEvent.HasStartedAcidicDownpour = downed.Contains("startDownpour");
            AcidRainEvent.HasBeenForceStartedByEoCDefeat = downed.Contains("forcedRain");
            AcidRainEvent.OldDukeHasBeenEncountered = downed.Contains("encounteredOldDuke");
            HasGeneratedLuminitePlanetoids = downed.Contains("HasGeneratedLuminitePlanetoids");
            IsWorldAfterDraedonUpdate = downed.Contains("IsWorldAfterDraedonUpdate");

            OreTypes[0] = downed.Contains("TinOreWorld") ? TileID.Tin : TileID.Copper;
            OreTypes[1] = downed.Contains("LeadOreWorld") ? TileID.Lead : TileID.Iron;
            OreTypes[2] = downed.Contains("TungstenOreWorld") ? TileID.Tungsten : TileID.Silver;
            OreTypes[3] = downed.Contains("PlatinumOreWorld") ? TileID.Platinum : TileID.Gold;

            RecipeUnlockHandler.Load(downed);

            Abyss.AbyssChasmBottom = tag.GetInt("abyssChasmBottom");
            SulphurousSea.YStart = tag.GetInt("SulphSeaYStart");
            AcidRainEvent.AccumulatedKillPoints = tag.GetInt("acidRainPoints");
            Reforges = tag.GetInt("Reforges");
            MoneyStolenByBandit = tag.GetInt("MoneyStolenByBandit");

            SunkenSeaLabCenter = tag.Get<Vector2>("SunkenSeaLabCenter");
            PlanetoidLabCenter = tag.Get<Vector2>("PlanetoidLabCenter");
            JungleLabCenter = tag.Get<Vector2>("JungleLabCenter");
            HellLabCenter = tag.Get<Vector2>("HellLabCenter");
            IceLabCenter = tag.Get<Vector2>("IceLabCenter");
        }
        #endregion Saving/Loading
    }
}
