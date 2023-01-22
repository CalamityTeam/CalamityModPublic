using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeProfanedGuardians")]
    public class LoreProfanedGuardians : LoreItem
    {
        public override string Lore =>
@"The Guardians are rather simple constructs, extensions of the Profaned Goddess’ power.
They are given partial autonomy to hunt down threats and are rarely seen outside of temples sanctified in her name.
She has been attempting to expand her domain, and it is no surprise she sees you as her largest threat.
After all, it was you that finished off the star-spawned horror that catalyzed the downfall of the Dragons.
Draw her out from hiding. Have no mercy, for the Profaned Goddess shows none herself.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Profaned Guardians");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Purple;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ProfanedGuardianTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
