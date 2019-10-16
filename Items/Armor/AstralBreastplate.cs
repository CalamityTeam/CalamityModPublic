using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Body)]
    public class AstralBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Breastplate");
            Tooltip.SetDefault("+20 max mana and life\n" +
                               "+3 max minions\n" +
                               "Creature detection");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 32, 0, 0);
            item.rare = 9;
            item.defense = 25;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.statManaMax2 += 20;
            player.maxMinions += 3;
            player.detectCreature = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AstralBar", 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
