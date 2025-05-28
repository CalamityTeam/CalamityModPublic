using CalamityMod.NPCs;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.SupremeCalamitas;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DrunkCrabBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            // Case 1: Zenith Seed Crabulon.
            if (Main.zenithWorld && NPC.AnyNPCs(ModContent.NPCType<Crabulon>()))
                return true;

            // Case 2: Supreme Permafrost BH5.
            bool permafrostSpecialAttack = false;

            // Try to find Supreme Permafrost, if he exists. He might not.
            try
            {
                // Is there an index reference to Supreme Calamitas/Permafrost available? Is it valid? I sure hope so. Thanks TML
                if (CalamityGlobalNPC.SCal >= 0 && CalamityGlobalNPC.SCal < Main.maxNPCs && Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    NPC npc = Main.npc[CalamityGlobalNPC.SCal];
                    SupremeCalamitas supremeSomeone = npc.ModNPC<SupremeCalamitas>();
                    permafrostSpecialAttack = supremeSomeone is not null && supremeSomeone.permafrost && supremeSomeone.gettingTired5;
                }
            }
            catch
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText("Supreme Permafrost code attempted to crash the game. Did you do something weird?");
            }

            return permafrostSpecialAttack;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:DrunkCrabulon", isActive);
        }
    }
}
