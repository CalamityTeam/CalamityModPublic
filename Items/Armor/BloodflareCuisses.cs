using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Legs)]
    public class BloodflareCuisses : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Cuisses");
            Tooltip.SetDefault("30% increased movement speed, 10% increased damage and 7% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.defense = 29;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.3f;
            player.allDamage += 0.1f;
            player.Calamity().AllCritBoost(7);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodstoneCore", 13);
            recipe.AddIngredient(null, "RuinousSoul", 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
