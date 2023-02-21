using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class LoreSignus : LoreItem
    {
        public override string Lore =>
@"An aberration that defies all explanation, borne of the Distortion and revered by the Onyx Kinsmen.
Almost all information about this entity is sourced from that enigmatic clan. All else is hearsay.
It has been reported to manifest in multiple places at once. Its capacity for deceit and ruthless cunning is peerless.
Statis' compatriot Braelor dueled me to a standstill. With our blades locked, the ronin lunged for the lethal blow.
The Devourer is not one for honor or loyalty. But he sensed weakness. Hesitation. An easy prey.
The serpent ensnared both my assailants in a dimensional vortex. He assured me they were as good as dead.
Yet, Statis must have struck a bargain with Signus, as he escaped his banishment unscathed.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Signus"); // Signus, the Blade Between?
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SignusTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
