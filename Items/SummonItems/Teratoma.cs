using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.HiveMind;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class Teratoma : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Teratoma");
            Tooltip.SetDefault("Summons the Hive Mind when used in the corruption\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.rare = ItemRarityID.Orange;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneCorrupt && !NPC.AnyNPCs(ModContent.NPCType<HiveMind>()) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<HiveMind>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<HiveMind>());

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RottenChunk, 13);
            recipe.AddIngredient(ModContent.ItemType<TrueShadowScale>(), 7);
            recipe.AddIngredient(ItemID.DemoniteBar, 3);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
