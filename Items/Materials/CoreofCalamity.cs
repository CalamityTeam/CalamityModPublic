using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class CoreofCalamity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Core of Calamity");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 36;
            item.maxStack = 99;
            item.value = Item.sellPrice(gold: 4);
            item.rare = ItemRarityID.Yellow;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Materials/CoreofCalamityGlow"));
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 3);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 3);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 3);
            recipe.AddIngredient(ModContent.ItemType<CalamityDust>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
