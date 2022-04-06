using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class DaedalusBreastplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Breastplate");
            Tooltip.SetDefault("3% increased damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.defense = 19; //41
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.03f;
            player.Calamity().AllCritBoost(3);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VerstaltiteBar>(15)
                .AddIngredient(ItemID.CrystalShard, 6)
                .AddIngredient<EssenceofEleum>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
