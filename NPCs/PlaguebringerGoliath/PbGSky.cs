using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlaguebringerGoliath
{
    public class PbGSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private int PbGIndex = -1;

        public override void Update(GameTime gameTime)
        {
            if (PbGIndex == -1)
            {
                UpdatePbGIndex();
                if (PbGIndex == -1)
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
            if (this.UpdatePbGIndex())
            {
                float x = 0f;
                if (this.PbGIndex != -1)
                {
                    x = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[this.PbGIndex].Center);
                }
                return 1f - Utils.SmoothStep(3000f, 6000f, x);
            }
            return 0f;
        }

        public override Color OnTileColor(Color inColor)
        {
            float intensity = this.GetIntensity();
            return new Color(Vector4.Lerp(new Vector4(0.5f, 0.8f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }

        private bool UpdatePbGIndex()
        {
            int PbGType = ModContent.NPCType<PlaguebringerGoliath>();
            if (PbGIndex >= 0 && Main.npc[PbGIndex].active && Main.npc[PbGIndex].type == PbGType)
            {
                return true;
            }
            PbGIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == PbGType)
                {
                    PbGIndex = i;
                    break;
                }
            }
            //this.DoGIndex = DoGIndex;
            return PbGIndex != -1;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                float intensity = this.GetIntensity();
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * intensity);
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
