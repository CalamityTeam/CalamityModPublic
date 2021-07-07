using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SamuraiBadge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Warbanner of the Sun");
            Tooltip.SetDefault("Increases melee damage, true melee damage and melee speed the closer you are to enemies\n" +
				"Max boost is 20% increased melee damage, true melee damage and melee speed");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 5));
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 78;
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
