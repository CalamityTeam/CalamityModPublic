using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class AtaxiaArmor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydrothermic Armor");
            Tooltip.SetDefault("+20 max life\n" +
                "8% increased damage and 4% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 24, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.statLifeMax2 += 20;
            player.GetDamage<GenericDamageClass>() += 0.08f;
            player.Calamity().AllCritBoost(4);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CruptixBar>(15)
                .AddIngredient(ItemID.HellstoneBar, 8)
                .AddIngredient<CoreofChaos>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
