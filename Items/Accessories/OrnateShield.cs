using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Armor.Daedalus;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class OrnateShield : ModItem
    {
        public const int ShieldSlamIFrames = 12;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
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
            if ((player.armor[0].type == ModContent.ItemType<DaedalusHeadMagic>() || player.armor[0].type == ModContent.ItemType<DaedalusHeadSummon>() ||
                player.armor[0].type == ModContent.ItemType<DaedalusHeadMelee>() || player.armor[0].type == ModContent.ItemType<DaedalusHeadRanged>() ||
                player.armor[0].type == ModContent.ItemType<DaedalusHeadRogue>()) &&
                player.armor[1].type == ModContent.ItemType<DaedalusBreastplate>() && player.armor[2].type == ModContent.ItemType<DaedalusLeggings>())
            {
                player.endurance += 0.08f;
                player.statLifeMax2 += 20;
            }
            player.Calamity().DashID = OrnateShieldDash.ID;
            player.dash = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VerstaltiteBar>(5).
                AddIngredient(ItemID.CrystalShard, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
