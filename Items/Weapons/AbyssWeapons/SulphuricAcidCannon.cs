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
            item.value = 3000000;
	        item.UseSound = SoundID.Item95;
	        item.shoot = mod.ProjectileType("SulphuricAcidCannon2");
	        item.shootSpeed = 7f;
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

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }
    }
}