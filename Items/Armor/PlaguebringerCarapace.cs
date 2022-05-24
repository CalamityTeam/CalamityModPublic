using CalamityMod.Buffs.DamageOverTime;
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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Plaguebringer Carapace");
            Tooltip.SetDefault("Grants immunity to the Plague\n" +
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
            player.GetDamage<SummonDamageClass>() += 0.12f;
            player.buffImmune[ModContent.BuffType<Plague>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BeeBreastplate).
                AddIngredient<AlchemicalFlask>(2).
                AddIngredient<PlagueCellCluster>(7).
                AddIngredient<InfectedArmorPlating>(7).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
