using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class StatigelHeadgear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Statigel Headgear");
            Tooltip.SetDefault("10% increased ranged damage\n" +
                "7% increased ranged critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 5, 0, 0);
            item.rare = ItemRarityID.LightRed;
            item.defense = 7; //25
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StatigelArmor>() && legs.type == ModContent.ItemType<StatigelGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "When you take over 100 damage in one hit you become immune to damage for an extended period of time\n" +
                    "Grants an extra jump and increased jump height\n" +
                    "12% increased jump speed";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.statigelSet = true;
            modPlayer.statigelJump = true;
            Player.jumpHeight += 5;
            player.jumpSpeedBoost += 0.6f;
        }

        public override void UpdateEquip(Player player)
        {
            player.rangedDamage += 0.1f;
            player.rangedCrit += 7;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 5);
            recipe.AddIngredient(ItemID.HellstoneBar, 9);
            recipe.AddTile(ModContent.TileType<StaticRefiner>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
