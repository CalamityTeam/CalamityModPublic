using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Bumblefuck {
public class EffulgentFeather : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Effulgent Feather");
		Tooltip.SetDefault("It vibrates with fluffy golden energy");
		Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(3, 12));
	}
	
	public override void SetDefaults()
	{
		item.width = 24;
		item.height = 24;
		item.maxStack = 999;
		item.value = 150000;
	}
	
	public override void ModifyTooltips(List<TooltipLine> list)
    {
        foreach (TooltipLine line2 in list)
        {
            if (line2.mod == "Terraria" && line2.Name == "ItemName")
            {
                line2.overrideColor = new Color(43, 96, 222);
            }
        }
    }
}}