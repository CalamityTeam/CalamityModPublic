using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("DaedalusEmblem")]
    public class DeadshotBrooch : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Deadshot Brooch");
            Tooltip.SetDefault("10% increased ranged damage, 5% increased ranged critical strike chance and 20% reduced ammo usage\n" +
                "Greatly increases all ranged projectile velocity");
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 40;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().deadshotBrooch = true;
            player.Calamity().rangedAmmoCost *= 0.8f;
            player.GetDamage<RangedDamageClass>() += 0.1f;
            player.GetCritChance<RangedDamageClass>() += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RangerEmblem).
                AddIngredient<CoreofCalamity>(2).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
