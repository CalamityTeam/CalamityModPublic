using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items {
public class StormlionMandible : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Stormlion Mandible");
	}
		
	public override void SetDefaults()
	{
		item.width = 12;
		item.height = 24;
		item.maxStack = 999;
		item.value = 3000;
		item.rare = 2;
	}
}}