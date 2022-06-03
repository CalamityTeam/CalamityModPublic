using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ElementalQuiver : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Elemental Quiver");
            Tooltip.SetDefault("15% increased ranged damage, 5% increased ranged critical strike chance and 20% reduced ammo usage\n" +
                "Grants a 20% chance to not consume arrows\n" +
                "Greatly increases all ranged projectile velocity");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage<RangedDamageClass>() += 0.15f;
            player.GetCritChance<RangedDamageClass>() += 5;
            player.magicQuiver = true;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.deadshotBrooch = true;
            modPlayer.rangedAmmoCost *= 0.8f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("QuiversGroup").
                AddIngredient<DeadshotBrooch>().
                AddIngredient(ItemID.LunarBar, 8).
                AddIngredient<GalacticaSingularity>(4).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
