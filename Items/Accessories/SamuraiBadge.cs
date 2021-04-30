using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SamuraiBadge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Samurai Badge");
            Tooltip.SetDefault("Increases melee damage, true melee damage and melee speed the closer you are to enemies\n" +
				"Max boost is 30% increased melee damage, true melee damage and melee speed");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
			item.rare = ItemRarityID.Purple;
			item.Calamity().customRarity = CalamityRarity.Turquoise;
			item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.accessory = true;
			item.Calamity().challengeDrop = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.badgeOfBraveryRare = true;
        }
    }
}
