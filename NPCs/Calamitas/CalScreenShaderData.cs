using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Calamitas
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
            int CalType = ModContent.NPCType<CalamitasClone>();
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

        public override void Update(GameTime gameTime)
        {
            if (CalIndex == -1 || BossRushEvent.BossRushActive)
            {
                UpdateCalIndex();
                if (CalIndex == -1 || BossRushEvent.BossRushActive)
                    Filters.Scene["CalamityMod:CalamitasRun3"].Deactivate();
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
