using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class MiracleMatter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Miracle Matter");
            Tooltip.SetDefault("Its amorphous form contains untold potential\n" + "One is required for every Exo Weapon");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.height = 64;
            item.maxStack = 999;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(platinum: 6, gold: 50);
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public void DrawBackAfterimage(SpriteBatch spriteBatch, Vector2 baseDrawPosition, Rectangle frame, float baseScale)
        {
            if (item.velocity.X != 0f)
                return;

            float pulse = (float)Math.Cos(1.61803398875f * Main.GlobalTime * 2f) + (float)Math.Cos(Math.E * Main.GlobalTime * 1.7f);
            pulse = pulse * 0.25f + 0.5f;

            // Sharpen the pulse with a power to give erratic fire bursts.
            pulse = (float)Math.Pow(pulse, 3D);

            float outwardnessFactor = MathHelper.Lerp(-0.3f, 1.2f, pulse);
            Color drawColor = Color.Lerp(new Color(255, 218, 99), new Color(249, 134, 44), pulse);
            drawColor *= MathHelper.Lerp(0.35f, 0.67f, CalamityUtils.Convert01To010(pulse));
            drawColor.A = 25;
            for (int i = 0; i < 8; i++)
            {
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 8f).ToRotationVector2() * outwardnessFactor * baseScale * 8f;
                spriteBatch.Draw(Main.itemTexture[item.type], drawPosition, frame, drawColor, 0f, Vector2.Zero, baseScale, SpriteEffects.None, 0f);
            }
        }


        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Rectangle frame = Main.itemTexture[item.type].Frame();
            DrawBackAfterimage(spriteBatch, item.position - Main.screenPosition, frame, scale);
            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            item.velocity.X = 0f;
            DrawBackAfterimage(spriteBatch, position, frame, scale);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 0.94f * brightness, 0.95f * brightness, 0.56f * brightness);

            if (Main.rand.NextBool(3))
            {
                Dust exoFlame = Dust.NewDustDirect(item.position, (int)(item.width * item.scale), (int)(item.height * item.scale * 0.6f), 6);
                exoFlame.velocity = Vector2.Lerp(Main.rand.NextVector2Unit(), -Vector2.UnitY, 0.5f) * Main.rand.NextFloat(1.8f, 2.6f);
                exoFlame.scale *= Main.rand.NextFloat(0.85f, 1.15f);
                exoFlame.fadeIn = 0.9f;
                exoFlame.noGravity = true;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ExoPrism>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BarofLife>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 1);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 1);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 3);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
