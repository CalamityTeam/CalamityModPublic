using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class TheCommunity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Community");
            Tooltip.SetDefault("The heart of (most of) the Terraria community\n" +
                "Starts off with weak buffs to all of your stats\n" +
                "The stat buffs become more powerful as you progress\n" +
                "Reduces the DoT effects of harmful debuffs inflicted on you\n" +
                "Thank you to all of my supporters who made this mod a reality");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 10));
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 64;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.Rainbow;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.community = true;
        }

        // Community and Shattered Community are mutually exclusive
        public override bool CanEquipAccessory(Player player, int slot) => !player.Calamity().shatteredCommunity;
	}
}
