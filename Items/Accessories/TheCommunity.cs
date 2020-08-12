using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

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
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 10));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = 7;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.Rainbow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.community = true;
        }
    }
}
