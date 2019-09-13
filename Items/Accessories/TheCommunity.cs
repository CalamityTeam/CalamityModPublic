using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class TheCommunity : ModItem
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Community");
			Tooltip.SetDefault("The heart of (most of) the Terraria community\n" +
            	"Legendary Accessory\n" +
            	"Starts off with weak buffs to all of your stats\n" +
            	"The stat buffs become more powerful as you progress\n" +
            	"Reduces the DoT effects of harmful debuffs inflicted on you\n" +
            	"Boosts your maximum flight time by 15%\n" +
            	"Thank you to all of my supporters that made this mod a reality\n" +
                "Revengeance drop");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 15));
		}

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.accessory = true;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 20;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
		{
        	CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.community = true;
		}
    }
}
