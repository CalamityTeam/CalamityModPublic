using CalamityMod.CalPlayer;
using CalamityMod.World;
using System.Collections.Generic;
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
            DisplayName.SetDefault("Blazing Core");
            Tooltip.SetDefault("The searing core of the profaned goddess\n" +
                               "10% damage reduction\n" +
                               "Being hit creates a miniature sun that lingers, dealing damage to nearby enemies\n" +
                               "The sun will slowly drag enemies into it\n" +
                               "Only one sun can be active at once\n" +
							   "Provides a moderate amount of light in the Abyss");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 6));
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 46;
            item.accessory = true;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.blazingCore = true;
        }
    }
}
