using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeWallofFlesh")]
    public class LoreWallofFlesh : LoreItem
    {
        public override string Lore =>
@"To contain the essence of a slain God is no small thing. It is rather a towering, ghastly construct.
The Wall was lashed together with foul sinew and fouler magics, forming a rudimentary prison of flesh.
It served its purpose: halting the diffusion of undue divine influence.
Were it not for this alchemical breakthrough, the very world I fought for may have been lost in the carnage I wrought.
My methods have since evolved. I need not contain such essences, when they can be devoured.
May you channel my valor in combating the resulting outpour of energies.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Wall of Flesh");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WallofFleshTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
