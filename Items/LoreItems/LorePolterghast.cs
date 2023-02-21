using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgePolterghast")]
    public class LorePolterghast : LoreItem
    {
        public override string Lore =>
@"The further my war dragged on, the further I sank into negligence. This specter is the crux of my failure.
I hid behind my excuses, calling them duties. Fighting the gods. Training. Ruling.
I had the time and resources to devote. I was simply paralyzed by apathy.
The scores of prisoners I kept in the dungeon I claimed perished alongside their jailors.
Within those hexed walls, none may know rest, and their souls coalesced into a formless monster.
Boiling with rage, wallowing in sorrow, screaming in madness. The amalgamation was uncontrollable.
The dragon cult was furious, their leader demanding I put the haunt down myself.
I did not answer. I had long since become deaf to the world outside my crusade. Do not fall as I have.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Polterghast");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PolterghastTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
