using CalamityMod.Items.Placeables.FurnitureExo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class ExoPrism : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Prism");
            Tooltip.SetDefault("Fractal energies shimmer across its surface");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 52;
            item.maxStack = 999;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(gold: 60);
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public void DrawBackAfterimage(SpriteBatch spriteBatch, Vector2 baseDrawPosition, Rectangle frame, float baseScale)
        {
            float pulse = Main.GlobalTime * 0.75f % 1f;
            float outwardnessFactor = MathHelper.Lerp(0.9f, 1.3f, pulse);
            Color drawColor = Color.MintCream * (1f - pulse) * 0.27f;
            drawColor.A = 0;
            for (int i = 0; i < 4; i++)
            {
                float scale = baseScale * outwardnessFactor;
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 4f).ToRotationVector2() * 4f;
                spriteBatch.Draw(Main.itemTexture[item.type], drawPosition, frame, drawColor, item.velocity.X * 0.2f, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Rectangle frame = Main.itemTexture[item.type].Frame();
            DrawBackAfterimage(spriteBatch, item.position - Main.screenPosition + frame.Size() * 0.5f, frame, scale);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 0.6f * brightness, 0.64f * brightness, 0.6f * brightness);

            if (Main.rand.NextBool(3))
            {
                Dust exoShine = Dust.NewDustDirect(item.position, (int)(item.width * item.scale), (int)(item.height * item.scale * 0.6f), 204);
                exoShine.velocity = Vector2.Lerp(Main.rand.NextVector2Unit(), -Vector2.UnitY, 0.5f) * Main.rand.NextFloat(1.2f, 1.8f);
                exoShine.fadeIn = 0.7f;
                exoShine.noGravity = true;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ExoPlatform>(), 2);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
