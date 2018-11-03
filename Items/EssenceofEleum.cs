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
public class EssenceofEleum : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Essence of Eleum");
		Tooltip.SetDefault("The essence of cold creatures");
	}
		
	public override void Update(ref float gravity, ref float maxFallSpeed)
    {
		maxFallSpeed *= 0f;
        float num = (float)Main.rand.Next(90, 111) * 0.01f;
        num *= Main.essScale;
        Lighting.AddLight((int)((item.position.X + (float)(item.width / 2)) / 16f), (int)((item.position.Y + (float)(item.height / 2)) / 16f), 0.15f * num, 0.05f * num, 0.5f * num);
    }
	
	public override void SetDefaults()
	{
		item.width = 8;
		item.height = 20;
		item.maxStack = 999;
		item.value = 25000;
		item.rare = 5;
	}
}}