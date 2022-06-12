using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Calamitas
{
    public class CalSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private int CalIndex = -1;

        public override void Update(GameTime gameTime)
        {
            if (CalIndex == -1 || BossRushEvent.BossRushActive)
            {
                UpdateCalIndex();
                if (CalIndex == -1 || BossRushEvent.BossRushActive)
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
            if (this.UpdateCalIndex())
            {
                float x = 0f;
                if (this.CalIndex != -1)
                {
                    x = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[this.CalIndex].Center);
                }
                return (1f - Utils.SmoothStep(3000f, 6000f, x)) * intensity;
            }
            return 0f;
        }

        public override Color OnTileColor(Color inColor)
        {
            float intensity = this.GetIntensity();
            return new Color(Vector4.Lerp(new Vector4(0.5f, 0.8f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }

        private bool UpdateCalIndex()
        {
            int CalType = ModContent.NPCType<CalamitasClone>();
            if (CalIndex >= 0 && Main.npc[CalIndex].active && Main.npc[CalIndex].type == CalType)
            {
                return true;
            }
            CalIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == CalType)
                {
                    CalIndex = i;
                    break;
                }
            }
            //this.DoGIndex = DoGIndex;
            return CalIndex != -1;
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
