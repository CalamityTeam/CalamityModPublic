using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.PlaguebringerGoliath;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class Abomination : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abombination");
            Tooltip.SetDefault("Calls in the airborne abomination\n" +
                "Summons the Plaguebringer Goliath when used in the jungle\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.Yellow;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneJungle && !NPC.AnyNPCs(ModContent.NPCType<PlaguebringerGoliath>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<PlaguebringerGoliath>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<PlaguebringerGoliath>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 20).AddIngredient(ItemID.IronBar, 8).AddIngredient(ItemID.Stinger, 5).AddIngredient(ItemID.Obsidian, 3).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
