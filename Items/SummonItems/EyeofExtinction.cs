using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Tiles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
	public class EyeofExtinction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of Extinction");
            Tooltip.SetDefault("Death\n" +
                "Summons Supreme Calamitas\n" +
                "Creates a large square arena of blocks around your player\n" +
                "Your player is the CENTER of the arena so be sure to use this item in a good location\n" +
                "During the battle, heart pickups will heal half as much HP\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            int surface = (int)Main.worldSurface;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < surface; j++)
                {
                    if (Main.tile[i, j] != null)
                    {
                        if (Main.tile[i, j].type == ModContent.TileType<ArenaTile>())
                        {
                            WorldGen.KillTile(i, j, false, false, false);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
                            }
                            else
                            {
                                WorldGen.SquareTileFrame(i, j, true);
                            }
                        }
                    }
                }
            }

            Main.PlaySound(SoundID.Roar, player.position, 0);
			if (Main.netMode != NetmodeID.MultiplayerClient)
				NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<SupremeCalamitas>());
			else
				NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<SupremeCalamitas>());

			return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BlightedEyeball>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
