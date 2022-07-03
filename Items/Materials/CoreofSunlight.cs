using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("CoreofCinder")]
    public class CoreofSunlight : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            ItemID.Sets.ItemNoGravity[Item.type] = true;

            DisplayName.SetDefault("Core of Sunlight");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Yellow;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Materials/CoreofSunlightGlow").Value);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 0.3f * brightness, 0.3f * brightness, 0.05f * brightness);
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient<EssenceofSunlight>().
                AddIngredient(ItemID.Ectoplasm).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
