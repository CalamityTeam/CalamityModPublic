using CalamityMod.Events;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class EidolonTablet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eidolon Tablet");
            Tooltip.SetDefault("Summons the Lunatic Cultist\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Cyan;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(NPCID.CultistBoss) && !NPC.LunarApocalypseIsUp && !NPC.AnyNPCs(NPCID.CultistTablet) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int npc = NPC.NewNPC((int)player.Center.X + 30, (int)player.Center.Y - 90, NPCID.CultistBoss, 1);
                Main.npc[npc].direction = Main.npc[npc].spriteDirection = Math.Sign(player.Center.X - player.Center.X - 30f);
                Main.npc[npc].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(npc);
            }
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, NPCID.CultistBoss);

            return true;
        }
    }
}
