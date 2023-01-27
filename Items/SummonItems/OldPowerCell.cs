using CalamityMod.Events;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class OldPowerCell : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Old Power Cell");
            Tooltip.SetDefault("Summons the Golem when used in the Jungle Temple\n" +
                "Enrages outside the Jungle Temple\n" +
                "Not consumable");
			NPCID.Sets.MPAllowedEnemies[NPCID.Golem] = true;
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 15; // Lihzahrd Power Cell
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.rare = ItemRarityID.Lime;
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
            bool canSummon = false;
            if (player.Center.Y > Main.worldSurface * 16.0)
            {
                int num = (int)player.Center.X / 16;
                int num2 = (int)player.Center.Y / 16;
                Tile tile = Framing.GetTileSafely(num, num2);
                if (tile.WallType == 87)
                    canSummon = true;
            }
            return canSummon && !NPC.AnyNPCs(NPCID.Golem) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, NPCID.Golem);
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, NPCID.Golem);

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarTabletFragment, 20).
                AddIngredient<EssenceofSunlight>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
