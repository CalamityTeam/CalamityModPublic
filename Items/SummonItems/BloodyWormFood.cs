using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Perforator;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class BloodyWormFood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Worm Food");
            Tooltip.SetDefault("Summons the Perforator Hive when used in the crimson\n" +
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
            return player.ZoneCrimson && !NPC.AnyNPCs(ModContent.NPCType<PerforatorHive>()) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<PerforatorHive>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<PerforatorHive>());

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Vertebrae, 13);
            recipe.AddIngredient(ModContent.ItemType<BloodSample>(), 7);
            recipe.AddIngredient(ItemID.CrimtaneBar, 3);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
