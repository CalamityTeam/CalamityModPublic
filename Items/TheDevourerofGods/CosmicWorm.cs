using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items.TheDevourerofGods
{
	public class CosmicWorm : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmic Worm");
			Tooltip.SetDefault("Summons the Devourer of Gods\n" +
                "Not consumable");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.consumable = false;
		}
		
		public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 255, 0);
	            }
	        }
	    }
		
		public override bool CanUseItem(Player player)
		{
			return !NPC.AnyNPCs(mod.NPCType("DevourerofGodsHead")) && !NPC.AnyNPCs(mod.NPCType("DevourerofGodsHeadS")) && CalamityWorld.DoGSecondStageCountdown <= 0;
		}
		
		public override bool UseItem(Player player)
		{
            string key = "Mods.CalamityMod.EdgyBossText12";
            Color messageColor = Color.Cyan;
            if (Main.netMode == 0)
            {
                Main.NewText(Language.GetTextValue(key), messageColor);
            }
            else if (Main.netMode == 2)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }
            NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("DevourerofGodsHead"));
			Main.PlaySound(SoundID.Roar, player.position, 0);
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ArmoredShell", 3);
			recipe.AddIngredient(null, "TwistingNether");
			recipe.AddIngredient(null, "DarkPlasma");
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}