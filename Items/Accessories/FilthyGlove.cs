using CalamityMod.Items.Materials;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(new EquipType[] { EquipType.HandsOn, EquipType.HandsOff } )]
    public class FilthyGlove : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.filthyGlove = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DemoniteBar, 2).
                AddIngredient<RottenMatter>(6).
                AddIngredient(ItemID.RottenChunk, 2).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
