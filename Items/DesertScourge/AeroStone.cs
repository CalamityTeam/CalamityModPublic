using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod;

namespace CalamityMod.Items.DesertScourge
{
    public class AeroStone : ModItem
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aero Stone");
			Tooltip.SetDefault("One of the ancient relics\n" +
            	"Increases movement speed by 10%, jump speed by 200%, and all damage and crit chance by 2%");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 8));
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
        	Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0f, 0.425f, 0.425f);
        	player.moveSpeed += 0.1f;
        	player.jumpSpeedBoost += 2.0f;
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
