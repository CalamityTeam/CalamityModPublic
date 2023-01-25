using CalamityMod.NPCs.Crabulon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Skies
{
    public class DrunkCrabScreenShaderData : ScreenShaderData
    {
        public int CrabIndex;

        public DrunkCrabScreenShaderData(string passName)
            : base(passName)
        {
        }

        public void UpdateBossIndex()
        {
            int CrabType = ModContent.NPCType<Crabulon>();
            if (CrabIndex >= 0 && Main.npc[CrabIndex].active && Main.npc[CrabIndex].type == CrabType)
            {
                return;
            }
            CrabIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == CrabType)
                {
                    CrabIndex = i;
                    break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (CrabIndex == -1 || !CalamityMod.Instance.legendaryMode)
            {
                UpdateBossIndex();
                if (CrabIndex == -1 || !CalamityMod.Instance.legendaryMode)
                    Filters.Scene["CalamityMod:DrunkCrabulon"].Deactivate();
            }
        }

        public override void Apply()
        {
            UpdateBossIndex();
            if (CrabIndex != -1)
            {
                UseTargetPosition(Main.npc[CrabIndex].Center);
            }
            base.Apply();
        }
    }
}
