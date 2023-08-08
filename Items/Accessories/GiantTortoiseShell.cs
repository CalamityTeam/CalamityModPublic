using CalamityMod.Buffs.StatDebuffs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Materials;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class GiantTortoiseShell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.defense = 15;
            Item.width = 20;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.tortShell = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GiantShell>().
                AddIngredient(ItemID.TurtleShell).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
