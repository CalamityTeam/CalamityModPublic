using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class AEWScreenShaderData : ScreenShaderData
    {
        private int AEWIndex;

        public AEWScreenShaderData(string passName)
            : base(passName)
        {
        }

        private void UpdateAEWIndex()
        {
            int AEWType = ModContent.NPCType<EidolonWyrmHeadHuge>();
            if (AEWIndex >= 0 && Main.npc[AEWIndex].active && Main.npc[AEWIndex].type == AEWType)
            {
                return;
            }
            AEWIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == AEWType)
                {
                    AEWIndex = i;
                    break;
                }
            }
        }

        public override void Apply()
        {
            UpdateAEWIndex();
            if (AEWIndex != -1)
            {
                UseTargetPosition(Main.npc[AEWIndex].Center);
            }
            base.Apply();
        }
    }
}
