using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class GypsyPowder : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Gypsy Powder");
	}
		
	public override void SetDefaults()
	{
		item.width = 20;
		item.height = 20;
		item.maxStack = 999;
		item.value = 50000;
		item.rare = 5;
	}
}}