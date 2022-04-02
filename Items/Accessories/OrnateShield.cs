using CalamityMod.Items.Materials;
using CalamityMod.Items.Armor;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class OrnateShield : ModItem
    {
        public const int ShieldSlamIFrames = 4;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ornate Shield");
            Tooltip.SetDefault("Boosted damage reduction and health while wearing the Daedalus armor\n" +
                "Grants a frost dash");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.defense = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if ((player.armor[0].type == ModContent.ItemType<DaedalusHat>() || player.armor[0].type == ModContent.ItemType<DaedalusHeadgear>() ||
                player.armor[0].type == ModContent.ItemType<DaedalusHelm>() || player.armor[0].type == ModContent.ItemType<DaedalusHelmet>() ||
                player.armor[0].type == ModContent.ItemType<DaedalusVisor>()) &&
                player.armor[1].type == ModContent.ItemType<DaedalusBreastplate>() && player.armor[2].type == ModContent.ItemType<DaedalusLeggings>())
            {
                player.endurance += 0.08f;
                player.statLifeMax2 += 20;
            }
            player.Calamity().dashMod = 6;
            player.dash = 0;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 5);
            recipe.AddIngredient(ItemID.CrystalShard, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
