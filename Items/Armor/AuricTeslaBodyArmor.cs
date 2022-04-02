using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
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
                       "8% increased damage and 5% increased critical strike chance\n" +
                       "You will freeze enemies near you when you are struck");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 34;
            item.value = Item.buyPrice(1, 44, 0, 0);
            item.defense = 48;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fBarrier = true;
            player.buffImmune[ModContent.BuffType<Irradiated>()] = true;
            player.statLifeMax2 += 100;
            player.allDamage += 0.08f;
            modPlayer.AllCritBoost(5);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GodSlayerChestplate>());
            recipe.AddIngredient(ModContent.ItemType<BloodflareBodyArmor>());
            recipe.AddIngredient(ModContent.ItemType<TarragonBreastplate>());
            recipe.AddIngredient(ModContent.ItemType<FrostBarrier>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 18);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaArmor>());
            recipe.AddIngredient(ModContent.ItemType<BloodflareBodyArmor>());
            recipe.AddIngredient(ModContent.ItemType<TarragonBreastplate>());
            recipe.AddIngredient(ModContent.ItemType<FrostBarrier>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 18);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
