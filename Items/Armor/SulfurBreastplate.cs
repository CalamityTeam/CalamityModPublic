using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class SulfurBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Breastplate");
            Tooltip.SetDefault("8% increased rogue damage and 5% increased rogue critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 20;
            Item.value = Item.buyPrice(0, 1, 15, 0);
            Item.defense = 6;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingDamage += 0.08f;
            player.Calamity().throwingCrit += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<UrchinStinger>(), 50).AddIngredient(ModContent.ItemType<Acidwood>(), 20).AddIngredient(ModContent.ItemType<SulphurousSand>(), 20).AddIngredient(ModContent.ItemType<SulfuricScale>(), 20).AddTile(TileID.Anvils).Register();
        }
    }
}
