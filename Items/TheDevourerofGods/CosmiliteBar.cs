using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.TheDevourerofGods {
public class CosmiliteBar : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Cosmilite Bar");
		Tooltip.SetDefault("A chunk of highly-resistant cosmic steel");
		Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
	}
		
	public override void SetDefaults()
	{
		item.width = 15;
		item.height = 12;
		item.maxStack = 999;
		item.value = 698750;
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
	
	public override void Update(ref float gravity, ref float maxFallSpeed)
    {
        float num = (float)Main.rand.Next(90, 111) * 0.01f;
        num *= Main.essScale;
        Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.5f * num, 0f * num, 0.5f * num);
    }
}}