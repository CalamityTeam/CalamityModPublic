using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeBrainofCthulhu : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Brain of Cthulhu");
            Tooltip.SetDefault("An eye and now a brain.\n" +
                "Most likely another abomination spawned from this inchoate mass of flesh.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.UseSound = SoundID.Item8;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.BrainofCthulhuTrophy).AddIngredient(ModContent.ItemType<PearlShard>(), 10).Register();
        }
    }
}
