using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Abyss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("LumenousAmulet")]
    [AutoloadEquip(EquipType.Neck)]
    public class DiamondOfTheDeep : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dOfTheDeep = true;
            modPlayer.dOfTheDeepVisual = !hideVisual;
            modPlayer.WaterDebuffMultiplier += 0.6f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaSpiritAmulet>().
                AddIngredient<AbyssGravel>(10).
                AddIngredient<DepthCells>(5).
                AddIngredient<PyreMantle>(10).
                AddIngredient<ScoriaBar>(5).
                AddIngredient<Voidstone>(10).
                AddIngredient<Lumenyl>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
