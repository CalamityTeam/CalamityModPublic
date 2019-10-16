using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
{
    public class CalScreenShaderData : ScreenShaderData
    {
        private int CalIndex;

        public CalScreenShaderData(string passName)
            : base(passName)
        {
        }

        private void UpdateCalIndex()
        {
            int CalType = ModLoader.GetMod("CalamityMod").NPCType("CalamitasRun3");
            if (CalIndex >= 0 && Main.npc[CalIndex].active && Main.npc[CalIndex].type == CalType)
            {
                return;
            }
            CalIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == CalType)
                {
                    CalIndex = i;
                    break;
                }
            }
        }

        public override void Apply()
        {
            UpdateCalIndex();
            if (CalIndex != -1)
            {
                UseTargetPosition(Main.npc[CalIndex].Center);
            }
            base.Apply();
        }
    }
}
