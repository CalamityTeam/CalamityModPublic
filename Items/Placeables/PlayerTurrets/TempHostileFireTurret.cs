using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Plates;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.PlayerTurrets
{
    public class TempHostileFireTurret : ModItem
    {
        public override string Texture => "CalamityMod/Items/Placeables/PlayerTurrets/FireTurret";
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            Tooltip.SetDefault("Roasts nearby players with a flamethrower at full blast\n" +
                "If you see this item in a public release, tell the devs :)");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HostileFireTurret>());

            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(14).
                AddIngredient<DubiousPlating>(20).
                AddIngredient<Chaosplate>(10).
                AddIngredient<EssenceofChaos>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
