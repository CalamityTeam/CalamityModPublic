using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;
using Terraria.Graphics.Shaders;
using System;

namespace CalamityMod.Items.Materials
{
    public class PlasmaDriveCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prototype Plasma Drive Core");
            Tooltip.SetDefault("Despite all the time it spent in storage, its furnace still burns strong");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 0, 0);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = Main.itemTexture[item.type];

            //Give it an outline. Make it look really important and shiny. The player must not confuse this for a random material
            spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
            Color outlineColor = Color.Lerp(Color.Cyan, Color.Orange, (float)Math.Sin(Main.GlobalTime * 2f) * 0.5f + 0.5f);
            Vector3 outlineHSL = Main.rgbToHsl(outlineColor); //BasicTint uses the opposite hue i guess? or smth is fucked with the way shaders get their colors. anyways, we invert it

            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(1f);
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1 - outlineHSL.X, outlineHSL.Y, outlineHSL.Z));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            float outlineWidth = MathHelper.Lerp(2, 3, (float)Math.Sin(Main.GlobalTime * 2f) * 0.5f + 0.5f);

            for (float i = 0; i < 1; i += 0.25f)
            {
               spriteBatch.Draw(tex, position + (i * MathHelper.TwoPi).ToRotationVector2() * outlineWidth * scale, frame, outlineColor, 0f, origin, scale, 0f, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

            return true;
        }
    }
}
