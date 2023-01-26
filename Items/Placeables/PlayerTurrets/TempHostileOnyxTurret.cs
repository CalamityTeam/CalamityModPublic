using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Plates;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.PlayerTurrets
{
    public class TempHostileOnyxTurret : ModItem
    {
        public override string Texture => "CalamityMod/Items/Placeables/PlayerTurrets/OnyxTurret";
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            Tooltip.SetDefault("Shoots a shotgun spread of bullets at nearby players\n" +
                "If you see this item in a public release, tell the devs :)");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HostileOnyxTurret>());

            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(14).
                AddIngredient<DubiousPlating>(20).
                AddIngredient<Onyxplate>(10).
                AddIngredient<BlightedGel>(50).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
