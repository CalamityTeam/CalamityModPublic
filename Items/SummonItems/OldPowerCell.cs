using CalamityMod.Events;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class OldPowerCell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override string Texture => $"Terraria/Images/Item_{ItemID.LihzahrdPowerCell}";

        public override void SetStaticDefaults()
        {
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
                int playerTileX = (int)player.Center.X / 16;
                int playerTileY = (int)player.Center.Y / 16;
                Tile tile = Framing.GetTileSafely(playerTileX, playerTileY);
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
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, player.whoAmI, NPCID.Golem);

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
