using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class DriedSeafood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Medallion");
            Tooltip.SetDefault("Summons the Desert Scourge when used in the desert\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneDesert && !NPC.AnyNPCs(ModContent.NPCType<DesertScourgeHead>()) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertScourgeHead>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertScourgeHead>());

            if (CalamityWorld.revenge)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                else
                    NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
                else
                    NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<DesertNuisanceHead>());
            }

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SandBlock, 25);
            recipe.AddIngredient(ItemID.Cactus, 15);
            recipe.AddIngredient(ItemID.AntlionMandible, 4);
            recipe.AddIngredient(ModContent.ItemType<StormlionMandible>(), 2);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
