using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Cryogen
{
    public class CryoStone : ModItem
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cryo Stone");
			Tooltip.SetDefault("One of the ancient relics\n" +
            	"Increases damage reduction by 5% and all damage and crit chance by 2%");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 5));
		}
    	
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = 500000;
            item.rare = 5;
            item.defense = 6;
			item.accessory = true;
        }
        
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
        	Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0f, 0.25f, 0.6f);
			player.endurance += 0.05f;
			player.meleeCrit += 2;
			player.meleeDamage += 0.02f;
			player.magicCrit += 2;
			player.magicDamage += 0.02f;
			player.rangedCrit += 2;
			player.rangedDamage += 0.02f;
			player.thrownCrit += 2;
			player.thrownDamage += 0.02f;
			player.minionDamage += 0.02f;
		}
    }
}
