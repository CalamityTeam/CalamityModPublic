using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public sealed class CalamityNetImportantNPC : GlobalNPC
    {
        private static Dictionary<int, int> typesToUpdate;

        public override void Load()
        {
            typesToUpdate = new();
        }

        public override void Unload()
        {
            typesToUpdate?.Clear();
            typesToUpdate = null;
        }

        public override void SetStaticDefaults()
        {
            #region Vanilla Enemies
            MarkNPCToNetImportant(NPCID.EaterofWorldsHead);
            MarkNPCToNetImportant(NPCID.EaterofWorldsBody);
            MarkNPCToNetImportant(NPCID.EaterofWorldsTail);

            MarkNPCToNetImportant(NPCID.TheDestroyer);
            MarkNPCToNetImportant(NPCID.TheDestroyerBody);
            MarkNPCToNetImportant(NPCID.TheDestroyerTail);
            #endregion Vanilla Enemies



            #region Pre Hardmode
            MarkNPCToNetImportant<DesertScourgeHead>(netUpdateTickOffset: 1);
            MarkNPCToNetImportant<DesertScourgeBody>(netUpdateTickOffset: 1);
            MarkNPCToNetImportant<DesertScourgeTail>(netUpdateTickOffset: 1);

            MarkNPCToNetImportant<DesertNuisanceHead>(netUpdateTickOffset: 2);
            MarkNPCToNetImportant<DesertNuisanceBody>(netUpdateTickOffset: 2);
            MarkNPCToNetImportant<DesertNuisanceTail>(netUpdateTickOffset: 2);

            MarkNPCToNetImportant<DesertNuisanceHeadYoung>(netUpdateTickOffset: 3);
            MarkNPCToNetImportant<DesertNuisanceBodyYoung>(netUpdateTickOffset: 3);
            MarkNPCToNetImportant<DesertNuisanceTailYoung>(netUpdateTickOffset: 3);

            MarkNPCToNetImportant<PerforatorHeadSmall>(netUpdateTickOffset: 1);
            MarkNPCToNetImportant<PerforatorBodySmall>(netUpdateTickOffset: 1);
            MarkNPCToNetImportant<PerforatorTailSmall>(netUpdateTickOffset: 1);

            MarkNPCToNetImportant<PerforatorHeadMedium>(netUpdateTickOffset: 2);
            MarkNPCToNetImportant<PerforatorBodyMedium>(netUpdateTickOffset: 2);
            MarkNPCToNetImportant<PerforatorTailMedium>(netUpdateTickOffset: 2);

            MarkNPCToNetImportant<PerforatorHeadLarge>(netUpdateTickOffset: 3);
            MarkNPCToNetImportant<PerforatorBodyLarge>(netUpdateTickOffset: 3);
            MarkNPCToNetImportant<PerforatorTailLarge>(netUpdateTickOffset: 3);
            #endregion Pre Hardmode



            #region Hardmode
            MarkNPCToNetImportant<AquaticScourgeHead>();
            MarkNPCToNetImportant<AquaticScourgeBody>();
            MarkNPCToNetImportant<AquaticScourgeBodyAlt>();
            MarkNPCToNetImportant<AquaticScourgeTail>();

            MarkNPCToNetImportant<ArmoredDiggerHead>();
            MarkNPCToNetImportant<ArmoredDiggerBody>();
            MarkNPCToNetImportant<ArmoredDiggerTail>();

            MarkNPCToNetImportant<AstrumDeusHead>();
            MarkNPCToNetImportant<AstrumDeusBody>();
            MarkNPCToNetImportant<AstrumDeusTail>();

            MarkNPCToNetImportant<Cryogen.Cryogen>();
            MarkNPCToNetImportant<CryogenShield>();
            #endregion Hardmode



            #region Post ML
            MarkNPCToNetImportant<CosmicGuardianHead>(netUpdateTickOffset: 1);
            MarkNPCToNetImportant<CosmicGuardianBody>(netUpdateTickOffset: 1);
            MarkNPCToNetImportant<CosmicGuardianTail>(netUpdateTickOffset: 1);

            MarkNPCToNetImportant<DevourerofGodsHead>(netUpdateTickOffset: 2);
            MarkNPCToNetImportant<DevourerofGodsBody>(netUpdateTickOffset: 2);
            MarkNPCToNetImportant<DevourerofGodsTail>(netUpdateTickOffset: 2);

            MarkNPCToNetImportant<StormWeaverHead>();
            MarkNPCToNetImportant<StormWeaverBody>();
            MarkNPCToNetImportant<StormWeaverTail>();
            #endregion Post ML


            #region Calamitas Boss
            MarkNPCToNetImportant<SepulcherHead>();
            MarkNPCToNetImportant<SepulcherBody>();
            MarkNPCToNetImportant<SepulcherTail>();
            #endregion


            #region Draedon Boss
            MarkNPCToNetImportant<Draedon>();

            MarkNPCToNetImportant<ThanatosHead>();
            MarkNPCToNetImportant<ThanatosBody1>();
            MarkNPCToNetImportant<ThanatosBody2>();
            MarkNPCToNetImportant<ThanatosTail>();
            #endregion Draedon Boss



            #region Abyss
            MarkNPCToNetImportant<BobbitWormHead>();
            MarkNPCToNetImportant<BobbitWormSegment>();

            MarkNPCToNetImportant<EidolonWyrmHead>(netUpdateTickOffset: 1);
            MarkNPCToNetImportant<EidolonWyrmBody>(netUpdateTickOffset: 1);
            MarkNPCToNetImportant<EidolonWyrmBodyAlt>(netUpdateTickOffset: 1);
            MarkNPCToNetImportant<EidolonWyrmTail>(netUpdateTickOffset: 1);

            MarkNPCToNetImportant<GulperEelHead>();
            MarkNPCToNetImportant<GulperEelBody>();
            MarkNPCToNetImportant<GulperEelBodyAlt>();
            MarkNPCToNetImportant<GulperEelTail>();

            MarkNPCToNetImportant<OarfishHead>();
            MarkNPCToNetImportant<OarfishBody>();
            MarkNPCToNetImportant<OarfishTail>();

            MarkNPCToNetImportant<PrimordialWyrmHead>(netUpdateTickOffset: 2);
            MarkNPCToNetImportant<PrimordialWyrmBody>(netUpdateTickOffset: 2);
            MarkNPCToNetImportant<PrimordialWyrmBodyAlt>(netUpdateTickOffset: 2);
            MarkNPCToNetImportant<PrimordialWyrmTail>(netUpdateTickOffset: 2);
            #endregion Abyss
        }

        public override void PostAI(NPC npc)
        {
            // Only Server should update this!
            if (!Main.dedServ)
                return;

            // Obviously deactived npc is not on our interest (not sure if this is case though)
            if (!npc.active)
                return;

            if (!typesToUpdate.TryGetValue(npc.type, out var netUpdateTickOffset))
                return;

            if ((Main.GameUpdateCount + netUpdateTickOffset) % 45 != 0)
                return;

            foreach (var player in Main.ActivePlayers)
            {
                // distance between 1000~1500 update with 8 tick period
                // and distance over 1500 will never update
                // So we forcely update NPC distanced over 1500 with 45 tick period
                float distance = CalamityUtils.ManhattanDistance(player.position, npc.position);
                if (distance <= 1499.0f)
                    continue;

                npc.SyncNPCPosAndRotOnly(); //Light-weight version to sync it's position
            }
        }

        private void MarkNPCToNetImportant<NPCType>(int netUpdateTickOffset = 0) where NPCType : ModNPC
        {
            MarkNPCToNetImportant(ModContent.NPCType<NPCType>(), netUpdateTickOffset);
        }

        private void MarkNPCToNetImportant(int npcType, int netUpdateTickOffset = 0)
        {
            typesToUpdate[npcType] = netUpdateTickOffset;
        }
    }
}
