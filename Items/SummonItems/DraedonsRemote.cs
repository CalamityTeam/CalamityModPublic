using CalamityMod.Events;
using CalamityMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class DraedonsRemote : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon's Remote");
            Tooltip.SetDefault("Mayhem...\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.rare = ItemRarityID.Yellow;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !NPC.AnyNPCs(NPCID.TheDestroyer) && !NPC.AnyNPCs(NPCID.SkeletronPrime) && !NPC.AnyNPCs(NPCID.Spazmatism) && !NPC.AnyNPCs(NPCID.Retinazer) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            CalamityGlobalNPC.DraedonMayhem = true;
            CalamityNetcode.SyncWorld();
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnOnPlayer(player.whoAmI, NPCID.TheDestroyer);
                NPC.SpawnOnPlayer(player.whoAmI, NPCID.SkeletronPrime);
                NPC.SpawnOnPlayer(player.whoAmI, NPCID.Spazmatism);
                NPC.SpawnOnPlayer(player.whoAmI, NPCID.Retinazer);
            }
            else
            {
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, NPCID.TheDestroyer);
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, NPCID.SkeletronPrime);
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, NPCID.Spazmatism);
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, NPCID.Retinazer);
            }

            return true;
        }
    }
}
