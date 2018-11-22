using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Providence {
public class MoltenAmputator : ModItem
{
	public override void SetStaticDefaults()
	{
		DisplayName.SetDefault("Molten Amputator");
	}

	public override void SetDefaults()
	{
		item.width = 60;  //The width of the .png file in pixels divided by 2.
		item.damage = 220;  //Keep this reasonable please.
		item.noMelee = true;  //Dictates whether this is a melee-class weapon.
		item.noUseGraphic = true;
		item.autoReuse = true;
		item.useAnimation = 18;
		item.useStyle = 1;
		item.useTime = 18;
		item.knockBack = 9f;  //Ranges from 1 to 9.
		item.UseSound = SoundID.Item1;
		item.thrown = true;  //Dictates whether the weapon can be "auto-fired".
		item.height = 60;  //The height of the .png file in pixels divided by 2.
		item.value = 5000000;  //Value is calculated in copper coins.
		item.shoot = mod.ProjectileType("MoltenAmputator");
		item.shootSpeed = 12f;
	}
	
	public override void ModifyTooltips(List<TooltipLine> list)
    {
        foreach (TooltipLine line2 in list)
        {
            if (line2.mod == "Terraria" && line2.Name == "ItemName")
            {
                line2.overrideColor = new Color(0, 255, 200);
            }
        }
    }
}}
