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
            item.rare = 4;
            item.defense = 7; //25
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StatigelArmor>() && legs.type == ModContent.ItemType<StatigelGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
			string jumpSpeedBonus = player.autoJump ? "7.5" : "30";
			player.setBonus = "When you take over 100 damage in one hit you become immune to damage for an extended period of time\n" +
					"Grants an extra jump and increased jump height\n" +
					jumpSpeedBonus + "% increased jump speed";
			CalamityPlayer modPlayer = player.Calamity();
			modPlayer.statigelSet = true;
			modPlayer.statigelJump = true;
			Player.jumpHeight += 5;
			player.jumpSpeedBoost += player.autoJump ? 0.375f : 1.5f;
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
