using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class FearmongerPlateMail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fearmonger Plate Mail");
            Tooltip.SetDefault("+100 max life and 8% increased damage reduction\n" +
            "5% increased damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(gold: 60);
            item.defense = 50;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 100;
            player.endurance += 0.08f;
            player.allDamage += 0.05f;
            player.Calamity().AllCritBoost(5);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpookyBreastplate);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 12);
            recipe.AddIngredient(ItemID.SoulofFright, 12);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 3);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
