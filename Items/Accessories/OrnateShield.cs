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
            Item.width = 36;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 8;
            Item.accessory = true;
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
            CreateRecipe()
                .AddIngredient<VerstaltiteBar>(5)
                .AddIngredient(ItemID.CrystalShard, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
