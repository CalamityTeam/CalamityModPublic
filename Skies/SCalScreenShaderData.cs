using System;
using CalamityMod.NPCs.SupremeCalamitas;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Skies
{
    public class SCalScreenShaderData : ScreenShaderData
    {
        private int SCalIndex;

        public SCalScreenShaderData(string passName)
            : base(passName)
        {
        }

        private void UpdateSCalIndex()
        {
            int SCalType = ModContent.NPCType<SupremeCalamitas>();
            if (SCalIndex >= 0 && Main.npc[SCalIndex].active && Main.npc[SCalIndex].type == SCalType)
            {
                return;
            }
            SCalIndex = NPC.FindFirstNPC(SCalType);
        }

        public override void Update(GameTime gameTime)
        {
            if (SCalIndex == -1)
            {
                UpdateSCalIndex();
                if (SCalIndex == -1)
                    Filters.Scene["CalamityMod:SupremeCalamitas"].Deactivate(Array.Empty<object>());
            }
        }

        public override void Apply()
        {
            UpdateSCalIndex();
            if (SCalIndex != -1)
            {
                UseTargetPosition(Main.npc[SCalIndex].Center);

                if (Main.npc[SCalIndex].ModNPC<SupremeCalamitas>().permafrost)
                    Filters.Scene["CalamityMod:SupremeCalamitas"].GetShader().UseColor(0.9f, 0.3f, 0.9f);
            }
            base.Apply();
        }
    }
}
