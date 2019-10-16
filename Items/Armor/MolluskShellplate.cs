using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Body)]
    public class MolluskShellplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mollusk Shellplate");
            Tooltip.SetDefault("10% increased damage and 6% increased critical strike chance\n" +
                               "15% decreased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 22;
            item.value = Item.buyPrice(0, 20, 0, 0);
            item.rare = 5;
            item.defense = 22;
        }

        public override void UpdateEquip(Player player)
        {
            player.allDamage += 0.1f;
            player.Calamity().AllCritBoost(6);
            player.moveSpeed -= 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 25);
            recipe.AddIngredient(ModContent.ItemType<MolluskHusk>(), 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
