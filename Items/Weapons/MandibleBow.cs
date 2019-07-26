using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
//using TerrariaOverhaul;

namespace CalamityMod.Items.Weapons
{
	public class MandibleBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mandible Bow");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 13;
	        item.ranged = true;
	        item.width = 22;
	        item.height = 40;
	        item.useTime = 25;
	        item.useAnimation = 25;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 2f;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 1;
	        item.UseSound = SoundID.Item5;
	        item.autoReuse = false;
	        item.shoot = 10;
	        item.shootSpeed = 30f;
	        item.useAmmo = 40;
	    }

        /*public void OverhaulInit()
        {
            this.SetTag("bow");
        }*/
    }
}
