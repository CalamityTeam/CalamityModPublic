using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class Phantoplasm : ModItem
{
	public override void SetStaticDefaults()
	{
 		DisplayName.SetDefault("Phantoplasm");
 		Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 5));
 	}
	
	public override void SetDefaults()
	{
		item.width = 12;
		item.height = 12;
		item.maxStack = 999;
		item.value = 97500;
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
	
	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(200, 200, 200, 0);
	}
}}