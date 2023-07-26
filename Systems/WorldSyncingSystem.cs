using System.IO;
using CalamityMod.CustomRecipes;
using CalamityMod.Events;
using CalamityMod.World;
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
            flags[6] = downedCalamitasClone;
            flags[7] = downedLeviathan;

            BitsByte flags2 = new BitsByte();
            flags2[0] = downedDoG;
            flags2[1] = downedPlaguebringer;
            flags2[2] = downedGuardians;
            flags2[3] = downedProvidence;
            flags2[4] = downedCeaselessVoid;
            flags2[5] = downedStormWeaver;
            flags2[6] = downedSignus;
            flags2[7] = downedYharon;

            // Don't write meaningful values for the now-unused vanilla boss booleans
            BitsByte flags3 = new BitsByte();
            flags3[0] = downedCalamitas;
            flags3[1] = downedDragonfolly;
            flags3[2] = downedCrabulon;
            flags3[3] = downedBetsy;
            flags3[4] = downedRavager;
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
            flags5[0] = downedAstrumDeus;
            flags5[1] = spawnedBandit;
            flags5[2] = spawnedCirrus;
            flags5[3] = AcidRainEvent.HasStartedAcidicDownpour;
            flags5[4] = false;
            flags5[5] = downedPolterghast;
            flags5[6] = death;
            flags5[7] = downedGSS;

            BitsByte flags6 = new BitsByte();
            flags6[0] = Abyss.AtLeftSideOfWorld;
            flags6[1] = downedAquaticScourge;
            flags6[2] = downedAstrumAureus;
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
            flags7[4] = AcidRainEvent.AcidRainEventIsOngoing;
            flags7[5] = downedEoCAcidRain;
            flags7[6] = downedAquaticScourgeAcidRain;
            flags7[7] = AcidRainEvent.HasTriedToSummonOldDuke;

            BitsByte flags8 = new BitsByte();
            flags8[0] = AcidRainEvent.HasBeenForceStartedByEoCDefeat;
            flags8[1] = false;
            flags8[2] = downedSecondSentinels;
            flags8[3] = foundHomePermafrost;
            flags8[4] = downedCLAMHardMode;
            flags8[5] = catName;
            flags8[6] = dogName;
            flags8[7] = bunnyName;

            BitsByte flags9 = new BitsByte();
            flags9[0] = false;
            flags9[1] = false;
            flags9[2] = false;
            flags9[3] = false;
            flags9[4] = false;
            flags9[5] = false;
            flags9[6] = false;
            flags9[7] = false;

            BitsByte flags10 = new BitsByte();
            flags10[0] = false;
            flags10[1] = false;
            flags10[2] = AcidRainEvent.OldDukeHasBeenEncountered;
            flags10[3] = false;
            flags10[4] = false;
            flags10[5] = false;
            flags10[6] = false;
            flags10[7] = false;

            BitsByte flags11 = new BitsByte();
            flags11[0] = false;
            flags11[1] = HasGeneratedLuminitePlanetoids;
            flags11[2] = downedPrimordialWyrm;
            flags11[3] = downedExoMechs;
            flags11[4] = downedAres;
            flags11[5] = downedThanatos;
            flags11[6] = downedArtemisAndApollo;
            flags11[7] = TalkedToDraedon;

            BitsByte flags12 = new BitsByte();
            flags12[0] = downedCragmawMire;
            flags12[1] = downedMauler;
            flags12[2] = downedNuclearTerror;
            flags12[3] = downedBossRush;
            flags12[4] = DraedonMechdusa;

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
            writer.Write(flags12);

            RecipeUnlockHandler.SendData(writer);

            writer.Write(Abyss.AbyssChasmBottom);
            writer.Write(SulphurousSea.YStart);
            writer.Write(AstralBiome.YStart);
            writer.Write(AcidRainEvent.AccumulatedKillPoints);
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
            writer.WriteVector2(CavernLabCenter);
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
            downedCalamitasClone = flags[6];
            downedLeviathan = flags[7];

            BitsByte flags2 = reader.ReadByte();
            downedDoG = flags2[0];
            downedPlaguebringer = flags2[1];
            downedGuardians = flags2[2];
            downedProvidence = flags2[3];
            downedCeaselessVoid = flags2[4];
            downedStormWeaver = flags2[5];
            downedSignus = flags2[6];
            downedYharon = flags2[7];

            // Explicitly discard the now-unused vanilla boss booleans
            BitsByte flags3 = reader.ReadByte();
            downedCalamitas = flags3[0];
            downedDragonfolly = flags3[1];
            downedCrabulon = flags3[2];
            downedBetsy = flags3[3];
            downedRavager = flags3[4];
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
            downedAstrumDeus = flags5[0];
            spawnedBandit = flags5[1];
            spawnedCirrus = flags5[2];
            AcidRainEvent.HasStartedAcidicDownpour = flags5[3];
            _ = flags5[4];
            downedPolterghast = flags5[5];
            death = flags5[6];
            downedGSS = flags5[7];

            BitsByte flags6 = reader.ReadByte();
            Abyss.AtLeftSideOfWorld = flags6[0];
            downedAquaticScourge = flags6[1];
            downedAstrumAureus = flags6[2];
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
            AcidRainEvent.AcidRainEventIsOngoing = flags7[4];
            downedEoCAcidRain = flags7[5];
            downedAquaticScourgeAcidRain = flags7[6];
            AcidRainEvent.HasTriedToSummonOldDuke = flags7[7];

            BitsByte flags8 = reader.ReadByte();
            AcidRainEvent.HasBeenForceStartedByEoCDefeat = flags8[0];
            _ = flags8[1];
            downedSecondSentinels = flags8[2];
            foundHomePermafrost = flags8[3];
            downedCLAMHardMode = flags8[4];
            catName = flags8[5];
            dogName = flags8[6];
            bunnyName = flags8[7];

            BitsByte flags9 = reader.ReadByte();
            _ = flags9[0];
            _ = flags9[1];
            _ = flags9[2];
            _ = flags9[3];
            _ = flags9[4];
            _ = flags9[5];
            _ = flags9[6];
            _ = flags9[7];

            BitsByte flags10 = reader.ReadByte();
            _ = flags10[0];
            _ = flags10[1];
            AcidRainEvent.OldDukeHasBeenEncountered = flags10[2];
            _ = flags10[3];
            _ = flags10[4];
            _ = flags10[5];
            _ = flags10[6];
            _ = flags10[7];

            BitsByte flags11 = reader.ReadByte();
            _ = flags11[0];
            HasGeneratedLuminitePlanetoids = flags11[1];
            downedPrimordialWyrm = flags11[2];
            downedExoMechs = flags11[3];
            downedAres = flags11[4];
            downedThanatos = flags11[5];
            downedArtemisAndApollo = flags11[6];
            TalkedToDraedon = flags11[7];

            BitsByte flags12 = reader.ReadByte();
            downedCragmawMire = flags12[0];
            downedMauler = flags12[1];
            downedNuclearTerror = flags12[2];
            downedBossRush = flags12[3];
            DraedonMechdusa = flags12[4];

            RecipeUnlockHandler.ReceiveData(reader);

            Abyss.AbyssChasmBottom = reader.ReadInt32();
            SulphurousSea.YStart = reader.ReadInt32();
            AstralBiome.YStart = reader.ReadInt32();
            AcidRainEvent.AccumulatedKillPoints = reader.ReadInt32();
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
            CavernLabCenter = reader.ReadVector2();
        }
        #endregion
    }
}
