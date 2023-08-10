using CalamityMod.Events;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class EidolonTablet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetStaticDefaults()
        {
           	NPCID.Sets.MPAllowedEnemies[NPCID.CultistBoss] = true;
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 18; // Bloody Tear (1 below Celestial Sigil fsr)
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
		}

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(NPCID.CultistBoss) && !NPC.LunarApocalypseIsUp && !NPC.AnyNPCs(NPCID.CultistTablet) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int npc = NPC.NewNPC(new EntitySource_BossSpawn(player), (int)player.Center.X + 30, (int)player.Center.Y - 90, NPCID.CultistBoss, 1);
                Main.npc[npc].direction = Main.npc[npc].spriteDirection = Math.Sign(player.Center.X - player.Center.X - 30f);
                Main.npc[npc].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(npc);
            }
            else
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, NPCID.CultistBoss);

            return true;
        }
    }
}
