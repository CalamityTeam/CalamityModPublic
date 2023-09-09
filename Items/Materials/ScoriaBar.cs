using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("CruptixBar")]
    public class ScoriaBar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Type] = 95; // Stardust Fragment
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ChaoticBarPlaced>());
            Item.value = Item.sellPrice(gold: 1, silver: 20);
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                TextureAssets.Item[Type].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 1f,
                drawOffset: new(5f, -12f)
            );
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScoriaOre>(4).
                AddTile(TileID.AdamantiteForge).
                Register();
        }
    }
}
