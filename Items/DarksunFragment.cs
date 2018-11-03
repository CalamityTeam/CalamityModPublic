using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class DarksunFragment : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Darksun Fragment");
		Tooltip.SetDefault("A shard of lunar and solar energy");
	}
		
	public override void Update(ref float gravity, ref float maxFallSpeed)
    {
		maxFallSpeed *= 0f;
        float num = (float)Main.rand.Next(90, 111) * 0.01f;
        num *= Main.essScale;
        Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.5f * num, 0.5f * num, 0.5f * num);
    }
	
	public override void SetDefaults()
	{
		item.width = 20;
		item.height = 20;
		item.maxStack = 999;
		item.value = 80000;
	}
	
	public override void ModifyTooltips(List<TooltipLine> list)
	{
    	foreach (TooltipLine line2 in list)
	    {
	        if (line2.mod == "Terraria" && line2.Name == "ItemName")
	        {
	            line2.overrideColor = new Color(108, 45, 199);
	        }
	    }
	}
}}