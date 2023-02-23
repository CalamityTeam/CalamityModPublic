using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.Bumblebirb;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    [LegacyName("BirbPheromones")]
    public class ExoticPheromones : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Exotic Pheromones");
            Tooltip.SetDefault("Specifically formulated to attract draconic hybrids\n" +
                "Summons The Dragonfolly when used in the Jungle\n" +
                "Enrages outside the Jungle\n" +
                "Not consumable");
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 17; // Celestial Sigil
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

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneJungle && !NPC.AnyNPCs(ModContent.NPCType<Bumblefuck>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Bumblefuck>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<Bumblefuck>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LifeAlloy>(5).
                AddIngredient(ItemID.FragmentSolar, 20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
