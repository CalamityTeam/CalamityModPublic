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
    public class LumenousAmulet : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Lumenous Amulet");
            Tooltip.SetDefault("Attacks inflict the Crush Depth debuff\n" +
                "Grants immunity to the Crush Depth debuff\n" +
                "While in the abyss you gain 25% increased max life\n" +
                "Provides a moderate amount of light in the abyss");
        }

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
            modPlayer.abyssalAmulet = true;
            modPlayer.lumenousAmulet = true;
            player.buffImmune[ModContent.BuffType<CrushDepth>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AbyssalAmulet>().
                AddIngredient<Lumenyl>(15).
                AddIngredient<Tenebris>(5).
                AddIngredient<SeaPrism>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
