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
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/WyrmScream");
	        item.autoReuse = true;
	        item.shootSpeed = 5f;
	        item.shoot = mod.ProjectileType("EidolicWail");
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }
	}
}