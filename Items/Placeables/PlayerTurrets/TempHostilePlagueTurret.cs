using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Plates;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.PlayerTurrets
{
    public class TempHostilePlagueTurret : ModItem
    {
        public override string Texture => "CalamityMod/Items/Placeables/PlayerTurrets/PlagueTurret";
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            Tooltip.SetDefault("Ejects homing plague missiles towards nearby players\n" +
                "If you see this item in a public release, tell the devs :)");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HostilePlagueTurret>());

            Item.value = Item.buyPrice(0, 20, 0, 0);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(14).
                AddIngredient<DubiousPlating>(20).
                AddIngredient<Plagueplate>(10).
                AddIngredient<InfectedArmorPlating>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
