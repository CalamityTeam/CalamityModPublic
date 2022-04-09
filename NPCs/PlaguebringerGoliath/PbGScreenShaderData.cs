using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlaguebringerGoliath
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
            int PbGType = ModContent.NPCType<PlaguebringerGoliath>();
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

        public override void Update(GameTime gameTime)
        {
            if (PbGIndex == -1)
            {
                UpdatePbGIndex();
                if (PbGIndex == -1)
                    Filters.Scene["CalamityMod:PlaguebringerGoliath"].Deactivate();
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
