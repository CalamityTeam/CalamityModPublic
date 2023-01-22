using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeLunaticCultist")]
    public class LorePrelude : LoreItem
    {
        public override string Lore =>
@"In ages past, now named the Draconic Era, the majestic Dragons protected Terraria from all threats.
Their famed might was put to the ultimate test by an aberrant behemoth from beyond the stars.
Fighting with all their strength, the Dragons could wound and weaken it, but not destroy it.
Lacking options, they tore the monster down to a shadow of its former self, and sealed it away.
What is left of it now lies imprisoned in the Moon, as far away as the Dragons could banish it.
Much of dragonkind was lost as casualties in that struggle, and they never recovered.
Zeratros himself was gravely injured. It seemed his power, along with his life, would be lost forever.
One mortal, sworn to the service of the Dragons, rose in determination to save their virtuous King.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Prelude");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AncientCultistTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
