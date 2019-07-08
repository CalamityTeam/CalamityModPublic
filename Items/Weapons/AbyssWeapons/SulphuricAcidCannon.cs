using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.AbyssWeapons
{
	public class SulphuricAcidCannon : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sulphuric Acid Cannon");
			Tooltip.SetDefault("Fires an acidic bubble that sticks to enemies and emits sulphuric gas");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 220;
	        item.ranged = true;
	        item.width = 90;
	        item.height = 30;
	        item.useTime = 18;
	        item.useAnimation = 18;
            item.useStyle = 5;
	        item.noMelee = true;
            item.knockBack = 6f;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item95;
	        item.shoot = mod.ProjectileType("SulphuricAcidCannon2");
	        item.shootSpeed = 7f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }
    }
}