using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class BlightedLens : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Blighted Lens");
	}
	
	public override void SetDefaults()
	{
		item.width = 16;
		item.height = 22;
		item.maxStack = 999;
		item.value = 5000;
		item.rare = 2;
	}
}}