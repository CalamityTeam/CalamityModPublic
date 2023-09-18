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
            Item.width = 34;
            Item.height = 52;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.GrandGelatin = true;
            player.moveSpeed += 0.12f;
            player.jumpSpeedBoost += 0.6f;
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
