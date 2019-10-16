using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Legs)]
    public class GodSlayerLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Leggings");
            Tooltip.SetDefault("35% increased movement speed\n" +
                "10% increased damage and 6% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 45, 0, 0);
            item.defense = 35;
            item.Calamity().postMoonLordRarity = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.35f;
            player.allDamage += 0.1f;
            player.Calamity().AllCritBoost(6);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CosmiliteBar", 18);
            recipe.AddIngredient(null, "NightmareFuel", 9);
            recipe.AddIngredient(null, "EndothermicEnergy", 9);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
