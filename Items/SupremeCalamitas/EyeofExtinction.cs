using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
			return !NPC.AnyNPCs(mod.NPCType("SupremeCalamitas"));
		}
		
		public override bool UseItem(Player player)
		{
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