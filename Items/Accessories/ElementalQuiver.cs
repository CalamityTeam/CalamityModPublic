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
            DisplayName.SetDefault("Elemental Quiver");
            Tooltip.SetDefault("15% increased ranged damage, 5% increased ranged critical strike chance, and 20% reduced ammo usage\n" +
                "5 increased defense, 2 increased life regen, and 15% increased pick speed\n" +
                "Greatly increases arrow speed and grants a 20% chance to not consume arrows");
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
            player.GetDamage(DamageClass.Ranged) += 0.15f;
            player.GetCritChance(DamageClass.Ranged) += 5;
            player.lifeRegen += 2;
            player.statDefense += 5;
            player.pickSpeed -= 0.15f;
            player.magicQuiver = true;
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.eQuiver = true; // Since splitting was removed, this has no effect.
            modPlayer.rangedAmmoCost *= 0.8f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MagicQuiver).
                AddIngredient<DaedalusEmblem>().
                AddIngredient(ItemID.LunarBar, 8).
                AddIngredient<GalacticaSingularity>(4).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
