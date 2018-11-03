using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons {
public class Waraxe : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Waraxe");
	}

    public override void SetDefaults()
    {
        item.damage = 22;
        item.melee = true;
        item.width = 32;
        item.height = 30;
        item.useTime = 22;
        item.useAnimation = 22;
        item.useTurn = true;
        item.axe = 10;
        item.useStyle = 1;
        item.knockBack = 5.25f;
        item.value = 50000;
        item.rare = 1;
        item.UseSound = SoundID.Item1;
        item.autoReuse = true;
    }
}}