using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class BloodyWormScarf : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Worm Scarf");
            Tooltip.SetDefault("10% increased damage reduction and increased melee stats");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 42;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.1f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
            player.endurance += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodyWormTooth>().
                AddIngredient(ItemID.WormScarf).
                AddIngredient(ItemID.SoulofNight, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
