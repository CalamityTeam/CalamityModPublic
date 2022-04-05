using System.IO;
using CalamityMod.CustomRecipes;
using CalamityMod.Events;
using Terraria;
using Terraria.ModLoader;
using static CalamityMod.DownedBossSystem;
using static CalamityMod.World.CalamityWorld;

namespace CalamityMod.Systems
{
    public class WorldSyncingSystem : ModSystem
    {
        #region NetSend
        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = downedDesertScourge;
            flags[1] = downedHiveMind;
            flags[2] = downedPerforator;
            flags[3] = downedSlimeGod;
            flags[4] = downedCryogen;
            flags[5] = downedBrimstoneElemental;
            flags[6] = downedCalamitas;
            flags[7] = downedLeviathan;

            BitsByte flags2 = new BitsByte();
            flags2[0] = downedDoG;
            flags2[1] = downedPlaguebringer;
            flags2[2] = downedGuardians;
            flags2[3] = downedProvidence;
            flags2[4] = downedSentinel1;
            flags2[5] = downedSentinel2;
            flags2[6] = downedSentinel3;
            flags2[7] = downedYharon;

            // Don't write meaningful values for the now-unused vanilla boss booleans
            BitsByte flags3 = new BitsByte();
            flags3[0] = downedSCal;
            flags3[1] = downedBumble;
            flags3[2] = downedCrabulon;
            flags3[3] = downedBetsy;
            flags3[4] = downedScavenger;
            flags3[5] = false;
            flags3[6] = false;
            flags3[7] = false;

            BitsByte flags4 = new BitsByte();
            flags4[0] = false;
            flags4[1] = false;
            flags4[2] = false;
            flags4[3] = false;
            flags4[4] = false;
            flags4[5] = false;
            flags4[6] = onionMode;
            flags4[7] = revenge;

            BitsByte flags5 = new BitsByte();
            flags5[0] = downedStarGod;
            flags5[1] = spawnedBandit;
            flags5[2] = spawnedCirrus;
            flags5[3] = startAcidicDownpour;
            flags5[4] = false;
            flags5[5] = downedPolterghast;
            flags5[6] = death;
            flags5[7] = downedGSS;

            BitsByte flags6 = new BitsByte();
            flags6[0] = abyssSide;
            flags6[1] = downedAquaticScourge;
            flags6[2] = downedAstrageldon;
            flags6[3] = false;
            flags6[4] = armageddon;
            flags6[5] = false;
            flags6[6] = false;
            flags6[7] = false;

            BitsByte flags7 = new BitsByte();
            flags7[0] = BossRushEvent.BossRushActive;
            flags7[1] = downedBoomerDuke;
            flags7[2] = downedCLAM;
            flags7[3] = false;
            flags7[4] = rainingAcid;
            flags7[5] = downedEoCAcidRain;
            flags7[6] = downedAquaticScourgeAcidRain;
            flags7[7] = triedToSummonOldDuke;

            BitsByte flags8 = new BitsByte();
            flags8[0] = forcedRainAlready;
            flags8[1] = forcedDownpourWithTear;
            flags8[2] = downedSecondSentinels;
            flags8[3] = foundHomePermafrost;
            flags8[4] = downedCLAMHardMode;
            flags8[5] = guideName;
            flags8[6] = wizardName;
            flags8[7] = steampunkerName;

            BitsByte flags9 = new BitsByte();
            flags9[0] = stylistName;
            flags9[1] = witchDoctorName;
            flags9[2] = taxCollectorName;
            flags9[3] = pirateName;
            flags9[4] = mechanicName;
            flags9[5] = armsDealerName;
            flags9[6] = dryadName;
            flags9[7] = nurseName;

            BitsByte flags10 = new BitsByte();
            flags10[0] = anglerName;
            flags10[1] = clothierName;
            flags10[2] = encounteredOldDuke;
            flags10[3] = travelingMerchantName;
            flags10[4] = false;
            flags10[5] = false;
            flags10[6] = false;
            flags10[7] = false;

            BitsByte flags11 = new BitsByte();
            flags11[0] = malice;
            flags11[1] = HasGeneratedLuminitePlanetoids;
            flags11[2] = downedAdultEidolonWyrm;
            flags11[3] = downedExoMechs;
            flags11[4] = downedAres;
            flags11[5] = downedThanatos;
            flags11[6] = downedArtemisAndApollo;
            flags11[7] = TalkedToDraedon;

            writer.Write(flags);
            writer.Write(flags2);
            writer.Write(flags3);
            writer.Write(flags4);
            writer.Write(flags5);
            writer.Write(flags6);
            writer.Write(flags7);
            writer.Write(flags8);
            writer.Write(flags9);
            writer.Write(flags10);
            writer.Write(flags11);

            RecipeUnlockHandler.SendData(writer);

            writer.Write(abyssChasmBottom);
            writer.Write(acidRainPoints);
            writer.Write(Reforges);
            writer.Write(MoneyStolenByBandit);
            writer.Write(DraedonSummonCountdown);
            writer.Write((int)DraedonMechToSummon);
            writer.WriteVector2(DraedonSummonPosition);
            writer.WriteVector2(SunkenSeaLabCenter);
            writer.WriteVector2(PlanetoidLabCenter);
            writer.WriteVector2(JungleLabCenter);
            writer.WriteVector2(HellLabCenter);
            writer.WriteVector2(IceLabCenter);
        }
        #endregion

        #region NetReceive
        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedDesertScourge = flags[0];
            downedHiveMind = flags[1];
            downedPerforator = flags[2];
            downedSlimeGod = flags[3];
            downedCryogen = flags[4];
            downedBrimstoneElemental = flags[5];
            downedCalamitas = flags[6];
            downedLeviathan = flags[7];

            BitsByte flags2 = reader.ReadByte();
            downedDoG = flags2[0];
            downedPlaguebringer = flags2[1];
            downedGuardians = flags2[2];
            downedProvidence = flags2[3];
            downedSentinel1 = flags2[4];
            downedSentinel2 = flags2[5];
            downedSentinel3 = flags2[6];
            downedYharon = flags2[7];

            // Explicitly discard the now-unused vanilla boss booleans
            BitsByte flags3 = reader.ReadByte();
            downedSCal = flags3[0];
            downedBumble = flags3[1];
            downedCrabulon = flags3[2];
            downedBetsy = flags3[3];
            downedScavenger = flags3[4];
            _ = flags3[5];
            _ = flags3[6];
            _ = flags3[7];

            BitsByte flags4 = reader.ReadByte();
            _ = flags4[0];
            _ = flags4[1];
            _ = flags4[2];
            _ = flags4[3];
            _ = flags4[4];
            _ = flags4[5];
            onionMode = flags4[6];
            revenge = flags4[7];

            BitsByte flags5 = reader.ReadByte();
            downedStarGod = flags5[0];
            spawnedBandit = flags5[1];
            spawnedCirrus = flags5[2];
            startAcidicDownpour = flags5[3];
            _ = flags5[4];
            downedPolterghast = flags5[5];
            death = flags5[6];
            downedGSS = flags5[7];

            BitsByte flags6 = reader.ReadByte();
            abyssSide = flags6[0];
            downedAquaticScourge = flags6[1];
            downedAstrageldon = flags6[2];
            _ = flags6[3];
            armageddon = flags6[4];
            _ = flags6[5];
            _ = flags6[6];
            _ = flags6[7];

            BitsByte flags7 = reader.ReadByte();
            BossRushEvent.BossRushActive = flags7[0];
            downedBoomerDuke = flags7[1];
            downedCLAM = flags7[2];
            _ = flags7[3];
            rainingAcid = flags7[4];
            downedEoCAcidRain = flags7[5];
            downedAquaticScourgeAcidRain = flags7[6];
            triedToSummonOldDuke = flags7[7];

            BitsByte flags8 = reader.ReadByte();
            forcedRainAlready = flags8[0];
            forcedDownpourWithTear = flags8[1];
            downedSecondSentinels = flags8[2];
            foundHomePermafrost = flags8[3];
            downedCLAMHardMode = flags8[4];
            guideName = flags8[5];
            wizardName = flags8[6];
            steampunkerName = flags8[7];

            BitsByte flags9 = reader.ReadByte();
            stylistName = flags9[0];
            witchDoctorName = flags9[1];
            taxCollectorName = flags9[2];
            pirateName = flags9[3];
            mechanicName = flags9[4];
            armsDealerName = flags9[5];
            dryadName = flags9[6];
            nurseName = flags9[7];

            BitsByte flags10 = reader.ReadByte();
            anglerName = flags10[0];
            clothierName = flags10[1];
            encounteredOldDuke = flags10[2];
            travelingMerchantName = flags10[3];
            _ = flags10[4];
            _ = flags10[5];
            _ = flags10[6];
            _ = flags10[7];

            BitsByte flags11 = reader.ReadByte();
            malice = flags11[0];
            HasGeneratedLuminitePlanetoids = flags11[1];
            downedAdultEidolonWyrm = flags11[2];
            downedExoMechs = flags11[3];
            downedAres = flags11[4];
            downedThanatos = flags11[5];
            downedArtemisAndApollo = flags11[6];
            TalkedToDraedon = flags11[7];

            RecipeUnlockHandler.ReceiveData(reader);

            abyssChasmBottom = reader.ReadInt32();
            acidRainPoints = reader.ReadInt32();
            Reforges = reader.ReadInt32();
            MoneyStolenByBandit = reader.ReadInt32();
            DraedonSummonCountdown = reader.ReadInt32();
            DraedonMechToSummon = (ExoMech)reader.ReadInt32();
            DraedonSummonPosition = reader.ReadVector2();
            SunkenSeaLabCenter = reader.ReadVector2();
            PlanetoidLabCenter = reader.ReadVector2();
            JungleLabCenter = reader.ReadVector2();
            HellLabCenter = reader.ReadVector2();
            IceLabCenter = reader.ReadVector2();
        }
        #endregion
    }
}
