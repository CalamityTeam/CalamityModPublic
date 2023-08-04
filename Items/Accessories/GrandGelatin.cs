using System;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class GrandGelatin : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.GrandGelatin = true;
            player.moveSpeed += 0.1f;
            player.jumpSpeedBoost += 0.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CleansingJelly>().
                AddIngredient<LifeJelly>().
                AddIngredient<VitalJelly>().
                AddIngredient(ItemID.SoulofLight, 2).
                AddIngredient(ItemID.SoulofNight, 2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
