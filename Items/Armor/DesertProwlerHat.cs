using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DesertProwlerHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Prowler Hat");
            Tooltip.SetDefault("4% increased ranged critical strike chance and 20% chance to not consume ammo");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 1; //6
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DesertProwlerShirt>() && legs.type == ModContent.ItemType<DesertProwlerPants>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Ranged attacks deal an extra 1 flat damage\n" +
            "Ranged crits can rarely whip up a sandstorm";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.desertProwler = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 4;
            player.ammoCost80 = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertFeather>(2).
                AddIngredient(ItemID.Silk, 8).
                AddTile(TileID.Loom).
                Register();
        }
    }
}
