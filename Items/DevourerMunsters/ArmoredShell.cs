using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.DevourerMunsters {
public class ArmoredShell : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Armored Shell");
	}
	
	public override void SetDefaults()
	{
		item.width = 24;
		item.height = 30;
		item.maxStack = 999;
		item.value = 15000;
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
}}