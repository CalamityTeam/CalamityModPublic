using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Signus
{
    public class SignusScreenShaderData : ScreenShaderData
    {
        private int SignusIndex;

        public SignusScreenShaderData(string passName)
            : base(passName)
        {
        }

        private void UpdateSIndex()
        {
            int SignusType = ModContent.NPCType<Signus>();
            if (SignusIndex >= 0 && Main.npc[SignusIndex].active && Main.npc[SignusIndex].type == SignusType)
            {
                return;
            }
            SignusIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == SignusType)
                {
                    SignusIndex = i;
                    break;
                }
            }
        }

        public override void Apply()
        {
            UpdateSIndex();
            if (SignusIndex != -1)
            {
                UseTargetPosition(Main.npc[SignusIndex].Center);
            }
            base.Apply();
        }
    }
}
