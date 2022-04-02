using CalamityMod.Events;
using CalamityMod.NPCs.Providence;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class ProfanedCoreUnlimited : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Core");
            Tooltip.SetDefault("The core of the unholy flame\n" +
                "Summons Providence when used in the hallow or underworld\n" +
                "Should be used during daytime\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
            item.rare = ItemRarityID.Purple;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Providence>()) && (player.ZoneHoly || player.ZoneUnderworldHeight) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceSpawn"), (int)player.position.X, (int)player.position.Y);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int npc = NPC.NewNPC((int)(player.position.X + Main.rand.Next(-500, 501)), (int)(player.position.Y - 250f), ModContent.NPCType<Providence>(), 1);
                Main.npc[npc].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(npc);
            }
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<Providence>());

            return true;
        }
    }
}
