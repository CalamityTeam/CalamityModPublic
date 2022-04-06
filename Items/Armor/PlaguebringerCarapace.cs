using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class PlaguebringerCarapace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguebringer Carapace");
            Tooltip.SetDefault("Reduces the damage caused to you by the plague\n" +
                "12% increased minion damage\n" +
                "Friendly bees inflict the plague");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.defense = 17;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().plaguebringerCarapace = true;
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.Calamity().reducedPlagueDmg = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BeeBreastplate)
                .AddIngredient<AlchemicalFlask>(2)
                .AddIngredient<PlagueCellCluster>(7)
                .AddIngredient<InfectedArmorPlating>(7)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
