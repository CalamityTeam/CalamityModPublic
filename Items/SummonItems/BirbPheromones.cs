using CalamityMod.Events;
using CalamityMod.NPCs.Bumblebirb;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.SummonItems
{
    public class BirbPheromones : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exotic Pheromones");
            Tooltip.SetDefault("Attracts the failed draconic experiment");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.maxStack = 20;
            item.rare = 10;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneJungle && !NPC.AnyNPCs(ModContent.NPCType<Bumblefuck>()) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
			Main.PlaySound(SoundID.Roar, player.position, 0);
			if (Main.netMode != NetmodeID.MultiplayerClient)
				NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Bumblefuck>());
			else
				NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<Bumblefuck>());

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LunarBar, 3);
            recipe.AddIngredient(ItemID.FragmentSolar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
