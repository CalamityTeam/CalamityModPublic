using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class LifeJelly : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.lifejelly = true;
        }
    }
}
