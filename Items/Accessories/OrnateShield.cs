using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class OrnateShield : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int ShieldSlamDamage = 50;
        public const float ShieldSlamKnockback = 3f;
        public const int ShieldSlamIFrames = 12;

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
            // Ornate Shield ram dash
            player.Calamity().DashID = OrnateShieldDash.ID;
            player.dashType = 0;

            // Cold debuff immunities (Do not fear, this accessory is post Cryogen!)
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>(5).
                AddIngredient(ItemID.CrystalShard, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
