using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    public class DoGScreenShaderData : ScreenShaderData
    {
        private int DoGIndex;

        public DoGScreenShaderData(string passName)
            : base(passName)
        {
        }

        private void UpdateDoGIndex()
        {
            int DoGType = ModContent.NPCType<DevourerofGodsHead>();
            if (DoGIndex >= 0 && Main.npc[DoGIndex].active && Main.npc[DoGIndex].type == DoGType)
            {
                return;
            }
            DoGIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == DoGType)
                {
                    DoGIndex = i;
                    break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (DoGIndex == -1)
            {
                UpdateDoGIndex();
                if (DoGIndex == -1)
                    Filters.Scene["CalamityMod:DevourerofGodsHead"].Deactivate();
            }
        }

        public override void Apply()
        {
            UpdateDoGIndex();
            if (DoGIndex != -1)
            {
                UseTargetPosition(Main.npc[DoGIndex].Center);
            }
            base.Apply();
        }
    }
}
