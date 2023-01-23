using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCalamitas")]
    public class LoreCalamitas : LoreItem
    {
        public override string Lore =>
@"None have borne the brunt of misfortune quite like the Brimstone Witch, Calamitas.
When first I saw her, she was still a girl. Prostrated in my court, garbed in charred rags, trembling.
I could not grasp the unfathomable, raw power of the fell magics that coursed through her.
She could scant control it herself. Permafrost recognized this immediately. With a pained face, he counseled me to look after her.
The Witch entered his tutelage, and soon after, my service. She was ablaze with desire to douse the Gods in her wicked wrath.
Indeed, the faithful already quaked in her presence. Her name was a moniker of theirs, one uttered quietly in fear.
In my campaigns, I counted on her sheer capacity for annihilation as my ace in the hole.
No man, no army, no city, and no God could stand against her unbridled fury.
Eventually, the girl’s horrific sin was too much for her to bear. She left my side along with her mentor.
The weight of her deeds haunts her to this day. She despises me, and I cannot blame her.
Please, if you would, show her respect where I did not.";


        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Calamitas");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SupremeCalamitasTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
