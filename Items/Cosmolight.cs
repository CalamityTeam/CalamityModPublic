using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items
{
	public class Cosmolight : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cosmolight");
			Tooltip.SetDefault("Changes night to day and vice versa");
		}
		
		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 5;
			item.useAnimation = 20;
			item.useTime = 20;
			item.useStyle = 4;
			item.UseSound = SoundID.Item60;
			item.consumable = false;
		}

        public override bool CanUseItem(Player player)
        {
            return !CalamityGlobalNPC.AnyBossNPCS();
        }

        public override bool UseItem(Player player)
		{
			if (!Main.dayTime)
			{
				Main.time = 0.0;
				Main.dayTime = true;
    		}
    		else
    		{
    			Main.time = 0.0;
				Main.dayTime = false;
				Main.moonPhase++;
				if (Main.moonPhase >= 8)
				{
					Main.moonPhase = 0;
				}
    		}
    		if (Main.netMode == 2)
			{
				NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "Daylight");
			recipe.AddIngredient(null, "Moonlight");
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}