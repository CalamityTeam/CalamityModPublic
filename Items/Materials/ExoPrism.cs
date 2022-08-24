using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class ExoPrism : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Exo Prism");
            Tooltip.SetDefault("Fractal energies shimmer across its surface");
			ItemID.Sets.SortingPriorityMaterials[Type] = 121;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 52;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 60);
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }
        public void DrawBackAfterimage(SpriteBatch spriteBatch, Vector2 baseDrawPosition, Rectangle frame, float baseScale)
        {
            float pulse = Main.GlobalTimeWrappedHourly * 0.75f % 1f;
            float outwardnessFactor = MathHelper.Lerp(0.9f, 1.3f, pulse);
            Color drawColor = Color.MintCream * (1f - pulse) * 0.27f;
            drawColor.A = 0;
            for (int i = 0; i < 4; i++)
            {
                float scale = baseScale * outwardnessFactor;
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 4f).ToRotationVector2() * 4f;
                spriteBatch.Draw(TextureAssets.Item[Item.type].Value, drawPosition, frame, drawColor, Item.velocity.X * 0.2f, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Rectangle frame = TextureAssets.Item[Item.type].Value.Frame();
            DrawBackAfterimage(spriteBatch, Item.position - Main.screenPosition + frame.Size() * 0.5f, frame, scale);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 0.6f * brightness, 0.64f * brightness, 0.6f * brightness);

            if (Main.rand.NextBool(3))
            {
                Dust exoShine = Dust.NewDustDirect(Item.position, (int)(Item.width * Item.scale), (int)(Item.height * Item.scale * 0.6f), 204);
                exoShine.velocity = Vector2.Lerp(Main.rand.NextVector2Unit(), -Vector2.UnitY, 0.5f) * Main.rand.NextFloat(1.2f, 1.8f);
                exoShine.fadeIn = 0.7f;
                exoShine.noGravity = true;
            }
        }
    }
}
