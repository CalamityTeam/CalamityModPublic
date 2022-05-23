using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class PlagueReaperVest : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Plague Reaper Vest");
            Tooltip.SetDefault("Grants immunity to the Plague\n" +
                "15% increased ranged damage and 5% increased ranged critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 24, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += 0.15f;
            player.GetCritChance<RangedDamageClass>() += 5;
            player.buffImmune[ModContent.BuffType<Plague>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.NecroBreastplate).
                AddIngredient<PlagueCellCluster>(29).
                AddIngredient(ItemID.Nanites, 19).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
