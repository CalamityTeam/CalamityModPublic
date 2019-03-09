using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.DesertScourge
{
    public class AeroStone : ModItem
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aero Stone");
			Tooltip.SetDefault("One of the ancient relics\n" +
            	"Increases movement speed by 10%, jump speed by 200%, and all damage by 3%");
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
			CalamityCustomThrowingDamagePlayer modPlayer = CalamityCustomThrowingDamagePlayer.ModPlayer(player);
			Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0f, 0.425f, 0.425f);
        	player.moveSpeed += 0.1f;
        	player.jumpSpeedBoost += 2.0f;
			player.meleeDamage += 0.03f;
			player.magicDamage += 0.03f;
			player.rangedDamage += 0.03f;
			modPlayer.throwingDamage += 0.03f;
			player.minionDamage += 0.03f;
		}
    }
}
