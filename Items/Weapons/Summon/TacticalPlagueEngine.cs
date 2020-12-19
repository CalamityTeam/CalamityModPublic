using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class TacticalPlagueEngine : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tactical Plague Engine");
            Tooltip.SetDefault("Summons a plague jet to pummel your enemies into submission\n" +
                               "Jets will fire ammo from your inventory, 66% chance to not consume ammo\n" +
                               "Sometimes shoots a missile instead of a bullet");
        }

        public override void SetDefaults()
        {
            item.damage = 34;
            item.mana = 10;
            item.width = 28;
            item.height = 20;
            item.useTime = item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.knockBack = 0.5f;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item14;
            item.autoReuse = true;
            item.summon = true;
            item.shoot = ModContent.ProjectileType<TacticalPlagueJet>();
            item.shootSpeed = 16f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BlackHawkRemote>());
            recipe.AddIngredient(ModContent.ItemType<InfectedRemote>());
            recipe.AddIngredient(ModContent.ItemType<FuelCellBundle>());
            recipe.AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 15);
            recipe.AddIngredient(ModContent.ItemType<InfectedArmorPlating>(), 8);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
