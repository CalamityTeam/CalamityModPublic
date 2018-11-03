using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    public class TheCommunity : ModItem
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Community");
			Tooltip.SetDefault("The heart of (most of) the Terraria community\n" +
            	"Legendary Accessory\n" +
            	"Starts off with weak buffs to all of your stats\n" +
            	"The stat buffs become more powerful as you progress\n" +
            	"Reduces the DoT effects of harmful debuffs inflicted on you\n" +
            	"Boosts your maximum flight time by 15%\n" +
            	"Thank you to all of my supporters that made this mod a reality\n" +
                "Revengeance drop");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 15));
		}
    	
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = 10000000;
			item.accessory = true;
        }
        
        public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
	            }
	        }
	    }
        
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
        	CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.community = true;
		}
    }
}
