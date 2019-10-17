using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles;  

namespace CalamityMod.NPCs
{
    public class PbGScreenShaderData : ScreenShaderData
    {
        private int PbGIndex;

        public PbGScreenShaderData(string passName)
            : base(passName)
        {
        }

        private void UpdatePbGIndex()
        {
            int PbGType = ModLoader.GetMod("CalamityMod").NPCType("CalamitasRun3");
            if (PbGIndex >= 0 && Main.npc[PbGIndex].active && Main.npc[PbGIndex].type == PbGType)
            {
                return;
            }
            PbGIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == PbGType)
                {
                    PbGIndex = i;
                    break;
                }
            }
        }

        public override void Apply()
        {
            UpdatePbGIndex();
            if (PbGIndex != -1)
            {
                UseTargetPosition(Main.npc[PbGIndex].Center);
            }
            base.Apply();
        }
    }
}
