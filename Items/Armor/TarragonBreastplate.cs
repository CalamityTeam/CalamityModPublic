using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class TarragonBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tarragon Breastplate");
            Tooltip.SetDefault("Breastplate of the exiler\n" +
                    "10% increased damage and 5% increased critical strike chance\n" +
                    "+2 life regen and +40 max life");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.lifeRegen = 2;
            item.value = Item.buyPrice(0, 40, 0, 0);
            item.defense = 37;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 40;
            player.allDamage += 0.1f;
            player.Calamity().AllCritBoost(5);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 15);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 18);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
