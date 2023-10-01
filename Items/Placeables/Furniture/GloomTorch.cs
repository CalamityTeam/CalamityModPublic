using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class GloomTorch : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
			ItemID.Sets.Torches[Item.type] = true;
            ItemID.Sets.SingleUseInGamepad[Type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.holdStyle = 1;
            Item.noWet = true;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.Crags.GloomTorch>();
            Item.flame = true;
            Item.value = 500;
        }

        public override void HoldItem(Player player)
        {
            bool killTorch = Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) || Item.wet;
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
                
            if (!killTorch)
                Lighting.AddLight(position, 0.9f, 1.2f, 0.3f);
        }

        public override void PostUpdate()
        {
            if (!Item.wet)
                Lighting.AddLight((int)((Item.position.X + Item.width / 2) / 16f), (int)((Item.position.Y + Item.height / 2) / 16f), 0.5f, 0.75f, 1.2f);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
            AddIngredient(ItemID.Torch, 3).
            AddIngredient<Items.Placeables.ScorchedBone>().
            Register();
        }
    }
}
