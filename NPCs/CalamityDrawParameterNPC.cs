using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
namespace CalamityMod.NPCs
{
    // Why this class is exist?
    // It because we want to avoid Threading Issues while We using NPC's properties on Draw Thread
    public sealed class CalamityDrawParameterNPC : GlobalNPC
    {
        public override bool InstancePerEntity => false;

        #region Draw Parameters
        public static bool[] DrawingMiracleBlight { get; private set; }
        public static bool[] DrawingPolarity { get; private set; }
        #endregion

        public override void Load()
        {
            DrawingMiracleBlight = new bool[Main.maxNPCs];
            DrawingPolarity = new bool[Main.maxNPCs];
        }

        public override void Unload()
        {
            DrawingMiracleBlight = null;
            DrawingPolarity = null;
        }

        public override void SetDefaults(NPC entity) => ResetParameters(entity);

        public override bool PreAI(NPC npc)
        {
            if (npc is null)
                return true;
            var whoAmI = npc.whoAmI;
            DrawingMiracleBlight[whoAmI] = ShouldDrawMiracleBlight(npc);
            DrawingPolarity[whoAmI] = ShouldDrawPolarity(npc);
            return true;
        }

        private static void ResetParameters(NPC npc)
        {
            if (npc is null)
                return;
            var whoAmI = npc.whoAmI;
            DrawingMiracleBlight[whoAmI] = false;
            DrawingPolarity[whoAmI] = false;
        }

        private static bool ShouldDrawMiracleBlight(NPC npc)
        {
            if (npc is null || !npc.active)
                return false;
            // Safety check for weird MP bug when getting global npcs.
            if (!npc.TryGetGlobalNPCSafer<CalamityGlobalNPC>(out var calNPC) || !npc.TryGetGlobalNPCSafer<CalamityPolarityNPC>(out var polNPC))
                return false;
            // Do not draw if the npc does not have miracle blight, or has the polarity effect.
            if (calNPC.miracleBlight <= 0 || polNPC.CurPolarity > 0f)
                return false;
            // Do not draw if the current player has the trippy effect.
            if (Main.LocalPlayer.Calamity().trippy)
                return false;
            return true;
        }

        private static bool ShouldDrawPolarity(NPC npc)
        {
            if (npc is null || !npc.active)
                return false;
            // Safety check for weird MP bug when getting global npcs.
            if (!npc.TryGetGlobalNPCSafer<CalamityGlobalNPC>(out var calNPC) || !npc.TryGetGlobalNPCSafer<CalamityPolarityNPC>(out var polNPC))
                return false;
            // I don't know who would be using this while also inflicting miracle blight, but in that rare case, do not draw these.
            if (calNPC.miracleBlight > 0)
                return false;
            // Do not draw if the npc doesn't have the polarity effect.
            if (polNPC.CurPolarity <= 0f)
                return false;
            return true;
        }
    }
}
