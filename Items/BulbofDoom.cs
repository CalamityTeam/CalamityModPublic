using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public class BulbofDoom : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Portable Bulb");
			Tooltip.SetDefault("Summons Plantera");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.maxStack = 20;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.rare = 7;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			return player.ZoneJungle && !NPC.AnyNPCs(NPCID.Plantera);
		}
		
		public override bool UseItem(Player player)
		{
			NPC.SpawnOnPlayer(player.whoAmI, NPCID.Plantera);
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "BarofLife", 3);
			recipe.AddIngredient(null, "LivingShard");
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}