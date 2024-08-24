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
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public sealed class CalamityNetImportantNPC : GlobalNPC
    {
        private static HashSet<int> typesToUpdate;

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
            #region Pre Hardmode
            MarkNPCToNetImportant<DesertScourgeHead>();
            MarkNPCToNetImportant<DesertScourgeBody>();
            MarkNPCToNetImportant<DesertScourgeTail>();

            MarkNPCToNetImportant<DesertNuisanceHead>();
            MarkNPCToNetImportant<DesertNuisanceBody>();
            MarkNPCToNetImportant<DesertNuisanceTail>();

            MarkNPCToNetImportant<DesertNuisanceHeadYoung>();
            MarkNPCToNetImportant<DesertNuisanceBodyYoung>();
            MarkNPCToNetImportant<DesertNuisanceTailYoung>();

            MarkNPCToNetImportant<PerforatorHeadSmall>();
            MarkNPCToNetImportant<PerforatorBodySmall>();
            MarkNPCToNetImportant<PerforatorTailSmall>();

            MarkNPCToNetImportant<PerforatorHeadMedium>();
            MarkNPCToNetImportant<PerforatorBodyMedium>();
            MarkNPCToNetImportant<PerforatorTailMedium>();

            MarkNPCToNetImportant<PerforatorHeadLarge>();
            MarkNPCToNetImportant<PerforatorBodyLarge>();
            MarkNPCToNetImportant<PerforatorTailLarge>();
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
            MarkNPCToNetImportant<CosmicGuardianHead>();
            MarkNPCToNetImportant<CosmicGuardianBody>();
            MarkNPCToNetImportant<CosmicGuardianTail>();

            MarkNPCToNetImportant<DevourerofGodsHead>();
            MarkNPCToNetImportant<DevourerofGodsBody>();
            MarkNPCToNetImportant<DevourerofGodsTail>();

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

            MarkNPCToNetImportant<EidolonWyrmHead>();
            MarkNPCToNetImportant<EidolonWyrmBody>();
            MarkNPCToNetImportant<EidolonWyrmBodyAlt>();
            MarkNPCToNetImportant<EidolonWyrmTail>();

            MarkNPCToNetImportant<GulperEelHead>();
            MarkNPCToNetImportant<GulperEelBody>();
            MarkNPCToNetImportant<GulperEelBodyAlt>();
            MarkNPCToNetImportant<GulperEelTail>();

            MarkNPCToNetImportant<OarfishHead>();
            MarkNPCToNetImportant<OarfishBody>();
            MarkNPCToNetImportant<OarfishTail>();

            MarkNPCToNetImportant<PrimordialWyrmHead>();
            MarkNPCToNetImportant<PrimordialWyrmBody>();
            MarkNPCToNetImportant<PrimordialWyrmBodyAlt>();
            MarkNPCToNetImportant<PrimordialWyrmTail>();
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

            if (Main.GameUpdateCount % 30 != 0)
                return;

            if (!typesToUpdate.Contains(npc.type))
                return;

            foreach (var player in Main.ActivePlayers)
            {
                // Exclude server client from distance check
                if (player.whoAmI == Main.myPlayer)
                    continue;

                // distance between 1000~1500 update with 8 tick period
                // and distance over 1500 will never update
                // So we forcely update NPC distanced over 1500 with 30 tick period
                float distance = CalamityUtils.ManhattanDistance(player.Center, npc.Center);
                if (distance <= 1499.0f)
                    continue;

                NetMessage.SendData(MessageID.SyncNPC, player.whoAmI, -1, null, npc.whoAmI);
            }
        }

        private void MarkNPCToNetImportant<NPCType>() where NPCType : ModNPC
        {
            int type = ModContent.NPCType<NPCType>();
            typesToUpdate.Add(type);
        }
    }
}
