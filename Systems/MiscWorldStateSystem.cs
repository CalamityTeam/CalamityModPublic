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
            DoGSecondStageCountdown = 0;
            ArmoredDiggerSpawnCooldown = 0;
            BossRushEvent.BossRushActive = false;
            BossRushEvent.BossRushSpawnCountdown = 180;
            BossRushEvent.HostileProjectileKillCounter = 0;
            newAltarX = 0;
            newAltarY = 0;
            Abyss.AbyssChasmBottom = 0;
            Abyss.AtLeftSideOfWorld = false;

            spawnedBandit = false;
            spawnedCirrus = false;
            foundHomePermafrost = false;

            anglerName = false;
            armsDealerName = false;
            clothierName = false;
            cyborgName = false;
            demolitionistName = false;
            dryadName = false;
            dyeTraderName = false;
            goblinTinkererName = false;
            guideName = false;
            mechanicName = false;
            merchantName = false;
            nurseName = false;
            painterName = false;
            partyGirlName = false;
            pirateName = false;
            skeletonMerchantName = false;
            steampunkerName = false;
            stylistName = false;
            tavernkeepName = false;
            taxCollectorName = false;
            travelingMerchantName = false;
            truffleName = false;
            witchDoctorName = false;
            wizardName = false;
            onionMode = false;
            revenge = false;
            TalkedToDraedon = false;
            death = false;
            armageddon = false;
            malice = false;
            rainingAcid = false;
            forceRainTimer = 0;
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
            if (malice)
                downed.Add("malice");
            if (Abyss.AtLeftSideOfWorld)
                downed.Add("abyssSide");
            if (BossRushEvent.BossRushActive)
                downed.Add("bossRushActive");
            if (rainingAcid)
                downed.Add("acidRain");
            if (spawnedBandit)
                downed.Add("bandit");
            if (spawnedCirrus)
                downed.Add("drunkPrincess");
            if (foundHomePermafrost)
                downed.Add("archmageHome");

            #region Save NPC Names
            if (anglerName)
                downed.Add("anglerName");
            if (armsDealerName)
                downed.Add("armsDealerName");
            if (clothierName)
                downed.Add("clothierName");
            if (cyborgName)
                downed.Add("cyborgName");
            if (demolitionistName)
                downed.Add("demolitionistName");
            if (dryadName)
                downed.Add("dryadName");
            if (dyeTraderName)
                downed.Add("dyeTraderName");
            if (goblinTinkererName)
                downed.Add("goblinTinkererName");
            if (guideName)
                downed.Add("guideName");
            if (mechanicName)
                downed.Add("mechanicName");
            if (merchantName)
                downed.Add("merchantName");
            if (nurseName)
                downed.Add("nurseName");
            if (painterName)
                downed.Add("painterName");
            if (partyGirlName)
                downed.Add("partyGirlName");
            if (pirateName)
                downed.Add("pirateName");
            if (skeletonMerchantName)
                downed.Add("skeletonMerchantName");
            if (steampunkerName)
                downed.Add("steampunkerName");
            if (stylistName)
                downed.Add("stylistName");
            if (tavernkeepName)
                downed.Add("tavernkeepName");
            if (taxCollectorName)
                downed.Add("taxCollectorName");
            if (travelingMerchantName)
                downed.Add("travelingMerchantName");
            if (truffleName)
                downed.Add("truffleName");
            if (witchDoctorName)
                downed.Add("witchDoctorName");
            if (wizardName)
                downed.Add("wizardName");
            #endregion

            if (triedToSummonOldDuke)
                downed.Add("spawnedBoomer");
            if (startAcidicDownpour)
                downed.Add("startDownpour");
            if (forcedRainAlready)
                downed.Add("forcedRain");
            if (forcedDownpourWithTear)
                downed.Add("forcedTear");
            if (encounteredOldDuke)
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
            tag["acidRainPoints"] = acidRainPoints;
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
            TalkedToDraedon = downed.Contains("TalkedToDraedon");
            onionMode = downed.Contains("onionMode");
            revenge = downed.Contains("revenge");
            death = downed.Contains("death");
            malice = downed.Contains("malice");
            Abyss.AtLeftSideOfWorld = downed.Contains("abyssSide");
            BossRushEvent.BossRushActive = downed.Contains("bossRushActive");
            rainingAcid = downed.Contains("acidRain");

            spawnedBandit = downed.Contains("bandit");
            spawnedCirrus = downed.Contains("drunkPrincess");
            foundHomePermafrost = downed.Contains("archmageHome");

            #region Load NPC Names
            anglerName = downed.Contains("anglerName");
            armsDealerName = downed.Contains("armsDealerName");
            clothierName = downed.Contains("clothierName");
            cyborgName = downed.Contains("cyborgName");
            demolitionistName = downed.Contains("demolitionistName");
            dryadName = downed.Contains("dryadName");
            dyeTraderName = downed.Contains("dyeTraderName");
            goblinTinkererName = downed.Contains("goblinTinkererName");
            guideName = downed.Contains("guideName");
            mechanicName = downed.Contains("mechanicName");
            merchantName = downed.Contains("merchantName");
            nurseName = downed.Contains("nurseName");
            painterName = downed.Contains("painterName");
            partyGirlName = downed.Contains("partyGirlName");
            pirateName = downed.Contains("pirateName");
            skeletonMerchantName = downed.Contains("skeletonMerchantName");
            steampunkerName = downed.Contains("steampunkerName");
            stylistName = downed.Contains("stylistName");
            tavernkeepName = downed.Contains("tavernkeepName");
            taxCollectorName = downed.Contains("taxCollectorName");
            travelingMerchantName = downed.Contains("travelingMerchantName");
            truffleName = downed.Contains("truffleName");
            witchDoctorName = downed.Contains("witchDoctorName");
            wizardName = downed.Contains("wizardName");
            #endregion

            triedToSummonOldDuke = downed.Contains("spawnedBoomer");
            startAcidicDownpour = downed.Contains("startDownpour");
            forcedRainAlready = downed.Contains("forcedRain");
            forcedDownpourWithTear = downed.Contains("forcedTear");
            encounteredOldDuke = downed.Contains("encounteredOldDuke");
            HasGeneratedLuminitePlanetoids = downed.Contains("HasGeneratedLuminitePlanetoids");
            IsWorldAfterDraedonUpdate = downed.Contains("IsWorldAfterDraedonUpdate");

            OreTypes[0] = downed.Contains("TinOreWorld") ? TileID.Tin : TileID.Copper;
            OreTypes[1] = downed.Contains("LeadOreWorld") ? TileID.Lead : TileID.Iron;
            OreTypes[2] = downed.Contains("TungstenOreWorld") ? TileID.Tungsten : TileID.Silver;
            OreTypes[3] = downed.Contains("PlatinumOreWorld") ? TileID.Platinum : TileID.Gold;

            RecipeUnlockHandler.Load(downed);

            Abyss.AbyssChasmBottom = tag.GetInt("abyssChasmBottom");
            acidRainPoints = tag.GetInt("acidRainPoints");
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
