using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.SupremeCalamitas
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
                "Not consumable");
		}

		public override void SetDefaults()
		{
			item.width = 40;
			item.height = 40;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = false;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 15;
		}

		public override bool CanUseItem(Player player)
		{
			return !NPC.AnyNPCs(mod.NPCType("SupremeCalamitas")) && CalamityWorld.downedBossAny;
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
						if (Main.tile[i, j].type == mod.TileType("ArenaTile"))
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
			NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("SupremeCalamitas"));
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AuricOre", 50);
            recipe.AddIngredient(null, "NightmareFuel", 30);
        	recipe.AddIngredient(null, "EndothermicEnergy", 30);
        	recipe.AddIngredient(null, "DarksunFragment", 25);
			recipe.AddIngredient(null, "CosmiliteBar", 15);
			recipe.AddIngredient(null, "Phantoplasm", 15);
            recipe.AddIngredient(null, "HellcasterFragment", 5);
            recipe.AddIngredient(null, "BlightedEyeball");
			recipe.AddTile(null, "DraedonsForge");
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
