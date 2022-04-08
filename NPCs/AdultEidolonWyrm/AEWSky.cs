using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AdultEidolonWyrm
{
    public class AEWSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private int AEWIndex = -1;

        public override void Update(GameTime gameTime)
        {
            if (isActive && intensity < 1f)
                intensity += 0.01f;
            else if (!isActive && intensity > 0f)
                intensity -= 0.01f;
        }

        private float GetIntensity()
        {
            if (UpdateAEWIndex())
            {
                float x = 0f;
                if (AEWIndex != -1)
                    x = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[AEWIndex].Center);

                return 1f - Utils.SmoothStep(3000f, 6000f, x);
            }
            return 0f;
        }

        public override Color OnTileColor(Color inColor)
        {
            float intensity = GetIntensity();
            return new Color(Vector4.Lerp(new Vector4(0.5f, 0.8f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }

        private bool UpdateAEWIndex()
        {
            int AEWType = ModContent.NPCType<EidolonWyrmHeadHuge>();
            if (AEWIndex >= 0 && Main.npc[AEWIndex].active && Main.npc[AEWIndex].type == AEWType)
            {
                return true;
            }
            AEWIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == AEWType)
                {
                    AEWIndex = i;
                    break;
                }
            }
            return AEWIndex != -1;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                float intensity = GetIntensity();
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 15) * intensity);
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
