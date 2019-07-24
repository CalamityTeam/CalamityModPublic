using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.HiveMind
{
	public class Teratoma : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Teratoma");
			Tooltip.SetDefault("Summons the Hive Mind");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.maxStack = 20;
			item.rare = 3;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			return player.ZoneCorrupt && !NPC.AnyNPCs(mod.NPCType("HiveMind")) && !NPC.AnyNPCs(mod.NPCType("HiveMindP2"));
		}
		
		public override bool UseItem(Player player)
		{
            if (Main.netMode != 1)
            {
                int num = NPC.NewNPC((int)(player.position.X + (float)(Main.rand.Next(-100, 100))), (int)(player.position.Y - 150f), mod.NPCType("HiveMind"), 0, 0f, 0f, 0f, 0f, 255);
                Main.PlaySound(SoundID.Roar, player.position, 0);
            }
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RottenChunk, 9);
			recipe.AddIngredient(null, "TrueShadowScale", 5);
			recipe.AddIngredient(ItemID.DemoniteBar, 2);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}