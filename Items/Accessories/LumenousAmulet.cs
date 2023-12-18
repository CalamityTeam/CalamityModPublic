using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class LumenousAmulet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.lumenousAmulet = true;
            player.buffImmune[ModContent.BuffType<RiptideDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<CrushDepth>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AbyssalAmulet>().
                AddIngredient<Lumenyl>(15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
