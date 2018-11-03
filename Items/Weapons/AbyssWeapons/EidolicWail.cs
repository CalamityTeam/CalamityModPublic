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
	public class EidolicWail : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eidolic Wail");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 210;
	        item.magic = true;
	        item.mana = 10;
	        item.width = 60;
	        item.height = 60;
            item.useTime = 12;
            item.reuseDelay = 30;
            item.useAnimation = 36;
            item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 1f;
	        item.value = 3000000;
	        item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Idle/WyrmScream");
	        item.autoReuse = true;
	        item.shootSpeed = 5f;
	        item.shoot = mod.ProjectileType("EidolicWail");
	    }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
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
	}
}