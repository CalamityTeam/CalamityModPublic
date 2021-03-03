using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ArcanumoftheVoid : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arcanum of the Void");
            Tooltip.SetDefault("You reflect projectiles when they hit you\n" +
                            "Reflected projectiles deal 5 times damage to enemies and no damage to you\n" +
							"This effect has a 90 second cooldown and does not work while a non-dash dodge is on cooldown");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.accessory = true;
			item.value = CalamityGlobalItem.Rarity12BuyPrice;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.Turquoise;
			item.expert = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.projRef = true;
        }
    }
}
