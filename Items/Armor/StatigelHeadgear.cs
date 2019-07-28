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
			item.rare = 4;
            item.defense = 7; //25
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("StatigelArmor") && legs.type == mod.ItemType("StatigelGreaves");
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "When you take over 100 damage in one hit you become immune to damage for an extended period of time\n" +
                "Grants an extra jump and increased jump height";
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.statigelSet = true;
            player.doubleJumpSail = true;
            player.jumpBoost = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.rangedDamage += 0.1f;
            player.rangedCrit += 7;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "PurifiedGel", 5);
            recipe.AddIngredient(ItemID.HellstoneBar, 9);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
