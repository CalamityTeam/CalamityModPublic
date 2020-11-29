using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ReaverCap : ModItem
    {
		//Health and Regen Helm
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Headgear");
            Tooltip.SetDefault("+50 max life\n" +
			"Passively regenerates one health point every second");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 7;
            item.defense = 20; //43
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ReaverScaleMail>() && legs.type == ModContent.ItemType<ReaverCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.reaverSpore = true;
            player.setBonus = "5% increased damage and +3 life regen\n" +
				"Reduces the life regen lost from damage over time debuffs by 20%\n" +
                "All attacks have a small chance to steal life and speed up the rate of life regen";
            player.allDamage += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().reaverRegen = true;
            player.statLifeMax2 += 50;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 8);
			recipe.AddIngredient(ItemID.JungleSpores, 6);
			recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
