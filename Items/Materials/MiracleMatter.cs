using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class MiracleMatter : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
			ItemID.Sets.SortingPriorityMaterials[Type] = 122;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 64;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(platinum: 6, gold: 50);
            Item.rare = ModContent.RarityType<Violet>();
        }

        public void DrawBackAfterimage(SpriteBatch spriteBatch, Vector2 baseDrawPosition, Rectangle frame, float baseScale)
        {
            if (Item.velocity.X != 0f)
                return;

            float pulse = (float)Math.Cos(1.61803398875f * Main.GlobalTimeWrappedHourly * 2f) + (float)Math.Cos(Math.E * Main.GlobalTimeWrappedHourly * 1.7f);
            pulse = pulse * 0.25f + 0.5f;

            // Sharpen the pulse with a power to give erratic fire bursts.
            pulse = (float)Math.Pow(pulse, 3D);

            float outwardnessFactor = MathHelper.Lerp(-0.3f, 1.2f, pulse);
            Color drawColor = Color.Lerp(new Color(255, 218, 99), new Color(249, 134, 44), pulse);
            drawColor *= MathHelper.Lerp(0.35f, 0.67f, CalamityUtils.Convert01To010(pulse));
            drawColor.A = 25;
            float drawPositionOffset = outwardnessFactor * baseScale * 8f;
            for (int i = 0; i < 8; i++)
            {
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 8f).ToRotationVector2() * drawPositionOffset;
                spriteBatch.Draw(TextureAssets.Item[Item.type].Value, drawPosition, frame, drawColor, 0f, Vector2.Zero, baseScale, SpriteEffects.None, 0f);
            }
        }


        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Rectangle frame = TextureAssets.Item[Item.type].Value.Frame();
            DrawBackAfterimage(spriteBatch, Item.position - Main.screenPosition, frame, scale);
            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Item.velocity.X = 0f;
            DrawBackAfterimage(spriteBatch, position - frame.Size() * 0.25f, frame, scale);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 0.94f * brightness, 0.95f * brightness, 0.56f * brightness);

            if (Main.rand.NextBool(3))
            {
                Dust exoFlame = Dust.NewDustDirect(Item.position, (int)(Item.width * Item.scale), (int)(Item.height * Item.scale * 0.6f), 6);
                exoFlame.velocity = Vector2.Lerp(Main.rand.NextVector2Unit(), -Vector2.UnitY, 0.5f) * Main.rand.NextFloat(1.8f, 2.6f);
                exoFlame.scale *= Main.rand.NextFloat(0.85f, 1.15f);
                exoFlame.fadeIn = 0.9f;
                exoFlame.noGravity = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ExoPrism>(5).
                AddIngredient<AuricBar>(5).
                AddIngredient<LifeAlloy>().
                AddIngredient<CoreofCalamity>().
                AddIngredient<AscendantSpiritEssence>().
                AddIngredient<GalacticaSingularity>(3).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
