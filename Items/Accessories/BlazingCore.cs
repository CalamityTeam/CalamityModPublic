using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BlazingCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Blazing Core");
            Tooltip.SetDefault("The searing core of the profaned goddess\n" +
                               "Being hit creates a miniature sun that lingers, dealing damage to nearby enemies\n" +
                               "The sun will slowly drag enemies into it\n" +
                               "Only one sun can be active at once\n" +
                               "Provides a moderate amount of light in the Abyss");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 46;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.blazingCore = true;
        }
    }
}
