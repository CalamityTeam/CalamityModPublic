using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Leviathan {
public class EnchantedPearl : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Enchanted Pearl");
		Tooltip.SetDefault("Increases fishing skill\nCrate potion effect, does not stack with crate potions");
	}
	
	public override void SetDefaults()
	{
		item.width = 26;
		item.height = 26;
		item.value = 150000;
		item.rare = 7;
		item.accessory = true;
	}
	
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.fishingSkill += 10;
		player.cratePotion = true;
	}
}}