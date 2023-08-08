using CalamityMod.NPCs.StormWeaver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.Skies
{
    public class StormWeaverFlashSky : CustomSky
    {
        public int StormWeaverHeadIndex = -1;

        public override void Update(GameTime gameTime)
        {
            int weaverType = ModContent.NPCType<StormWeaverHead>();
            if (StormWeaverHeadIndex >= 0 && Main.npc[StormWeaverHeadIndex].active && Main.npc[StormWeaverHeadIndex].type == weaverType)
                return;

            StormWeaverHeadIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == weaverType)
                {
                    StormWeaverHeadIndex = i;
                    break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth < float.MaxValue || !Main.npc.IndexInRange(StormWeaverHeadIndex))
                return;

            // Draw lightning in the background based on TextureAssets.MagicPixel.
            // It is a long, white vertical strip that exists for some reason.
            // This lightning effect is achieved by expanding this to fit the entire background and then drawing it as a distinct element.
            Texture2D white = TextureAssets.MagicPixel.Value;
            float lightningFlashPower = (Main.npc[StormWeaverHeadIndex].ModNPC as StormWeaverHead).lightning;
            Vector2 scale = new Vector2(Main.screenWidth * 1.1f / white.Width, Main.screenHeight * 1.1f / white.Height);
            Vector2 screenCenter = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            Color drawColor = Color.White * MathHelper.Lerp(0f, 0.88f, lightningFlashPower);
            Vector2 origin = white.Size() * 0.5f;

            for (int i = 0; i < 2; i++)
                spriteBatch.Draw(white, screenCenter, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        }

        public override void Reset() { }

        public override void Activate(Vector2 position, params object[] args) { }

        public override void Deactivate(params object[] args) { }

        public override bool IsActive() => StormWeaverHeadIndex != -1 && !Main.gameMenu;
    }
}
