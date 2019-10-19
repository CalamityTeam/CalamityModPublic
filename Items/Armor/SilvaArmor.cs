using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class SilvaArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Armor");
            Tooltip.SetDefault("+80 max life\n" +
                       "20% increased movement speed\n" +
                       "12% increased damage and 8% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 72, 0, 0);
            item.defense = 44;
            item.Calamity().postMoonLordRarity = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 80;
            player.moveSpeed += 0.2f;
            player.allDamage += 0.12f;
            player.Calamity().AllCritBoost(8);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 10);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Tenebris>(), 12);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 16);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 16);
            recipe.AddIngredient(ModContent.ItemType<LeadCore>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
