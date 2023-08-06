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
        public int CirrusIndex;

        public DrunkCrabScreenShaderData(string passName)
            : base(passName)
        {
        }

        public void UpdateBossIndex()
        {
            int CrabType = ModContent.NPCType<Crabulon>();
            int CirrusType = ModContent.NPCType<SupremeCalamitas>();
            bool shouldCheckForCirrus = false;
            bool shouldForceDeactivateCirrusShader = false;
            if (CalamityGlobalNPC.SCal != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().cirrus)
                    {
                        shouldCheckForCirrus = Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().gettingTired5;
                        shouldForceDeactivateCirrusShader = Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().giveUpCounter <= 1;
                    }
                }
            }

            if (shouldCheckForCirrus)
            {
                if (CirrusIndex >= 0 && Main.npc[CirrusIndex].active && Main.npc[CirrusIndex].type == CirrusType && !shouldForceDeactivateCirrusShader)
                    return;

                CirrusIndex = -1;

                if (!shouldForceDeactivateCirrusShader)
                {
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].type == CirrusType)
                        {
                            CirrusIndex = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (CrabIndex >= 0 && Main.npc[CrabIndex].active && Main.npc[CrabIndex].type == CrabType)
                    return;

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
        }

        public override void Update(GameTime gameTime)
        {
            bool shouldCheckForCirrus = false;
            if (CalamityGlobalNPC.SCal != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().cirrus)
                        shouldCheckForCirrus = Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().gettingTired5;
                }
            }

            if (shouldCheckForCirrus)
            {
                if (CirrusIndex == -1)
                {
                    UpdateBossIndex();
                    if (CirrusIndex == -1)
                        Filters.Scene["CalamityMod:DrunkCrabulon"].Deactivate();
                }
            }
            else
            {
                if (CrabIndex == -1 || !Main.zenithWorld)
                {
                    UpdateBossIndex();
                    if (CrabIndex == -1 || !Main.zenithWorld)
                        Filters.Scene["CalamityMod:DrunkCrabulon"].Deactivate();
                }
            }
        }

        public override void Apply()
        {
            UpdateBossIndex();

            bool shouldCheckForCirrus = false;
            if (CalamityGlobalNPC.SCal != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().cirrus)
                        shouldCheckForCirrus = Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().gettingTired5;
                }
            }

            if (shouldCheckForCirrus)
            {
                if (CirrusIndex != -1)
                    UseTargetPosition(Main.npc[CirrusIndex].Center);
            }
            else
            {
                if (CrabIndex != -1)
                    UseTargetPosition(Main.npc[CrabIndex].Center);
            }

            base.Apply();
        }
    }
}
