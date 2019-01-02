using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Calamitas
{
    public class ChaosStone : ModItem
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chaos Stone");
			Tooltip.SetDefault("One of the ancient relics\n" +
            	"Increases max mana by 50, all damage by 3%, and reduces mana usage by 5%");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 4));
		}
    	
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 5;
			item.accessory = true;
        }
        
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
        	Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.85f, 0f, 0f);
			player.statManaMax2 += 50;
			player.manaCost *= 0.95f;
			player.meleeDamage += 0.03f;
			player.magicDamage += 0.03f;
			player.rangedDamage += 0.03f;
            CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.03f;
			player.minionDamage += 0.03f;
		}
    }
}
