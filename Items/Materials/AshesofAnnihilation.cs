using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("CalamitousEssence")]
    public class AshesofAnnihilation : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(7, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
			ItemID.Sets.SortingPriorityMaterials[Type] = 123;
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 56;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(gold: 66, silver: 66, copper: 66);
            Item.rare = ModContent.RarityType<Violet>();
        }
        public void DrawPulsingAfterimage(SpriteBatch spriteBatch, Vector2 baseDrawPosition, Rectangle frame, float baseScale)
        {
            float pulse = Main.GlobalTimeWrappedHourly * 0.68f % 1f;
            float outwardness = pulse * baseScale * 8f;
            Color drawColor = Color.BlueViolet * (float)Math.Sqrt(1f - pulse) * 0.7f;
            drawColor.A = 0;
            float scale = baseScale * MathHelper.Lerp(1f, 1.45f, pulse);
            Vector2 drawPositionOffset = Vector2.UnitY * 2f;
            float velocity = Item.velocity.X * 0.2f;
            Vector2 origin = frame.Size() * 0.5f;
            for (int i = 0; i < 4; i++)
            {
                Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 4f).ToRotationVector2() * outwardness - drawPositionOffset;
                spriteBatch.Draw(TextureAssets.Item[Item.type].Value, drawPosition, frame, drawColor, velocity, origin, scale, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawPulsingAfterimage(spriteBatch, position, frame, scale);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Rectangle frame = Main.itemAnimations[Item.type].GetFrame(TextureAssets.Item[Item.type].Value);
            DrawPulsingAfterimage(spriteBatch, Item.position - Main.screenPosition + frame.Size() * 0.5f, frame, scale);
            return true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 0.34f * brightness, 0.08f * brightness, 0.155f * brightness);
        }
    }
}
