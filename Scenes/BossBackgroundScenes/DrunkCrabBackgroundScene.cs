using CalamityMod.NPCs;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DrunkCrabBackgroundScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player)
        {
            bool cirrusSpecialAttack = false;
            if (CalamityGlobalNPC.SCal != -1)
            {
                if (Main.npc[CalamityGlobalNPC.SCal].active)
                {
                    if (Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().cirrus)
                        cirrusSpecialAttack = Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().gettingTired5;
                }
            }

            return (NPC.AnyNPCs(ModContent.NPCType<Crabulon>()) && Main.zenithWorld) || cirrusSpecialAttack;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:DrunkCrabulon", isActive);
        }
    }
}
