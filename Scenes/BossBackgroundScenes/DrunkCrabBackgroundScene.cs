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
            
            // Case 2: Supreme Cirrus BH5.
            bool cirrusSpecialAttack = false;

            // Try to find Supreme Cirrus, if she exists. She might not.
            try
            {
                // Is there an index reference to Supreme Calamitas/Cirrus available? Is it valid? I sure hope so. Thanks TML
                if (CalamityGlobalNPC.SCal >= 0 && CalamityGlobalNPC.SCal < Main.maxNPCs && Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    NPC npc = Main.npc[CalamityGlobalNPC.SCal];
                    SupremeCalamitas supremeSomeone = npc.ModNPC<SupremeCalamitas>();
                    cirrusSpecialAttack = supremeSomeone is not null && supremeSomeone.cirrus && supremeSomeone.gettingTired5;
                }
            }
            catch
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText("Supreme Cirrus code attempted to crash the game. Did you do something weird?");
            }

            return cirrusSpecialAttack;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:DrunkCrabulon", isActive);
        }
    }
}
