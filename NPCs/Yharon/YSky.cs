using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Yharon
{
    public class YSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private int YIndex = -1;

        public override void Update(GameTime gameTime)
        {
            if (YIndex == -1)
            {
                UpdateYIndex();
                if (YIndex == -1)
                    isActive = false;
            }

            if (isActive && intensity < 1f)
            {
                intensity += 0.01f;
            }
            else if (!isActive && intensity > 0f)
            {
                intensity -= 0.01f;
            }
        }

        private float GetIntensity()
        {
            if (this.UpdateYIndex())
            {
                float x = 0f;
                if (this.YIndex != -1)
                {
                    x = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[this.YIndex].Center);
                }
                return (1f - Utils.SmoothStep(3000f, 6000f, x)) * 0.66f;
            }
            return 0.66f;
        }

        public override Color OnTileColor(Color inColor)
        {
            float intensity = this.GetIntensity();
            return new Color(Vector4.Lerp(new Vector4(0.5f, 0.8f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }

        private bool UpdateYIndex()
        {
            int YType = ModContent.NPCType<Yharon>();
            if (YIndex >= 0 && Main.npc[YIndex].active && Main.npc[YIndex].type == YType)
            {
                return true;
            }
            YIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == YType)
                {
                    YIndex = i;
                    break;
                }
            }
            //this.DoGIndex = DoGIndex;
            return YIndex != -1;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                float intensity = this.GetIntensity();
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(77, 19, 0) * intensity);
            }
        }

        public override float GetCloudAlpha()
        {
            return 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive || intensity > 0f;
        }
    }
}
