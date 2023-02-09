using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class GloomTorch : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
			ItemID.Sets.Torches[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 18;
            Item.maxStack = 99;
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

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			// Vanilla usually matches sorting methods with the right type of item, but sometimes, like with torches, it doesn't. Make sure to set whichever items manually if need be.
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torches;
		}

        public override void HoldItem(Player player)
        {
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);
                
            if (!Item.wet)
                Lighting.AddLight(position, 0.9f, 1.2f, 0.3f);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight((int)((Item.position.X + Item.width / 2) / 16f), (int)((Item.position.Y + Item.height / 2) / 16f), 0.5f, 0.75f, 1.2f);
        }

		// This function doesn't even work....
        public override void AutoLightSelect(ref bool dryTorch, ref bool wetTorch, ref bool glowstick)
        {
            dryTorch = true;
            wetTorch = false;
            glowstick = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
            AddIngredient(ItemID.Torch, 3).
            AddIngredient(ModContent.ItemType<Items.Placeables.ScorchedBone>()).
            Register();
        }
    }
}
