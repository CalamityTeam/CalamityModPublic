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
            Item.width = 36;
            Item.height = 36;
            Item.maxStack = 99;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Materials/CoreofCalamityGlow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CoreofCinder>(), 3).AddIngredient(ModContent.ItemType<CoreofEleum>(), 3).AddIngredient(ModContent.ItemType<CoreofChaos>(), 3).AddIngredient(ModContent.ItemType<CalamityDust>()).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
