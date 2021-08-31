using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class BrimflameRobes : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimflame Robes");
            Tooltip.SetDefault("5% increased magic damage and critical strike chance\n" +
                "Grants obsidian rose effects");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.magicDamage += 0.05f;
            player.magicCrit += 5;
            player.lavaRose = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>(), 20);
            recipe.AddIngredient(ModContent.ItemType<UnholyCore>(), 4);
            recipe.AddIngredient(ItemID.ObsidianRose);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
