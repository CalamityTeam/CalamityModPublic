using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Bumblebirb;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class BirbPheromones : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Exotic Pheromones");
            Tooltip.SetDefault("Attracts the failed draconic experiment\n" +
                "Summons The Dragonfolly when used in the jungle\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneJungle && !NPC.AnyNPCs(ModContent.NPCType<Bumblefuck>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Bumblefuck>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<Bumblefuck>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BarofLife>(5).
                AddIngredient(ItemID.FragmentSolar, 20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
