using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class PlagueReaperStriders : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Reaper Striders");
            Tooltip.SetDefault("3% increased ranged critical strike chance\n" +
                "15% increased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 18, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 11;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 3;
            player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.NecroGreaves).
                AddIngredient<PlagueCellCluster>(21).
                AddIngredient(ItemID.Nanites, 17).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
