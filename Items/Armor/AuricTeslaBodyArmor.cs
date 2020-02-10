using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class AuricTeslaBodyArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Body Armor");
            Tooltip.SetDefault("+100 max life\n" +
                       "25% increased movement speed\n" +
                       "Attacks have a 2% chance to do no damage to you\n" +
                       "8% increased damage and 5% increased critical strike chance\n" +
                       "You will freeze enemies near you when you are struck");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(1, 44, 0, 0);
            item.defense = 48;
            item.Calamity().customRarity = CalamityRarity.Rainbow;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fBarrier = true;
            modPlayer.godSlayerReflect = true;
            player.statLifeMax2 += 100;
            player.moveSpeed += 0.25f;
            player.allDamage += 0.08f;
            modPlayer.AllCritBoost(5);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaArmor>());
            recipe.AddIngredient(ModContent.ItemType<GodSlayerChestplate>());
            recipe.AddIngredient(ModContent.ItemType<BloodflareBodyArmor>());
            recipe.AddIngredient(ModContent.ItemType<TarragonBreastplate>());
			recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 18);
			recipe.AddIngredient(ModContent.ItemType<FrostBarrier>());
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
