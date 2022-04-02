using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class FathomSwarmerBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fathom Swarmer Breastplate");
            Tooltip.SetDefault("6% increased damage reduction\n" +
                "6% increased minion damage\n" +
                "Boosted defense and regen increased while submerged in liquid\n" +
                "Reduces defense loss within the Abyss");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.defense = 22;
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.06f;
            player.endurance += 0.06f;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.statDefense += 10;
                player.lifeRegen += 5;
            }
            player.Calamity().fathomSwarmerBreastplate = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpiderBreastplate);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 12);
            recipe.AddIngredient(ModContent.ItemType<PlantyMush>(), 10);
            recipe.AddIngredient(ModContent.ItemType<AbyssGravel>(), 18);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
