using CalamityMod.NPCs;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.World;
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
            int PermafrostType = ModContent.NPCType<SupremeCalamitas>();
            bool shouldCheckForPermafrost = false;
            bool shouldForceDeactivatePermafrostShader = false;
            if (CalamityGlobalNPC.SCal != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().permafrost)
                    {
                        shouldCheckForPermafrost = Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().gettingTired5;
                        shouldForceDeactivatePermafrostShader = Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().giveUpCounter <= 1;
                    }
                }
            }

            if (CrabIndex >= 0 && Main.npc[CrabIndex].active && Main.npc[CrabIndex].type == CrabType)
                return;

            CrabIndex = NPC.FindFirstNPC(CrabType);            
        }

        public override void Update(GameTime gameTime)
        {
            if (CrabIndex == -1 || !Main.zenithWorld)
            {
                UpdateBossIndex();
                if (CrabIndex == -1 || !Main.zenithWorld)
                    Filters.Scene["CalamityMod:DrunkCrabulon"].Deactivate();
            }            
        }

        public override void Apply()
        {
            UpdateBossIndex();

            if (CrabIndex != -1)
                    UseTargetPosition(Main.npc[CrabIndex].Center);

            base.Apply();
        }
    }
}
