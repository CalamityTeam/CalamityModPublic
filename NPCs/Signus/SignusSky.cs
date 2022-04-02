using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Signus
{
    public class SignusSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private int SignusIndex = -1;

        public override void Update(GameTime gameTime)
        {
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
            if (this.UpdateSIndex())
            {
                float x = 0f;
                if (this.SignusIndex != -1)
                {
                    x = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[this.SignusIndex].Center);
                }

                float maxIntensity = 0.1f;
                if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
                {
                    maxIntensity = 1f - (float)Main.npc[this.SignusIndex].life / (float)Main.npc[this.SignusIndex].lifeMax;
                }
                return (1f - Utils.SmoothStep(3000f, 6000f, x)) * maxIntensity;
            }
            return 0f;
        }

        public override Color OnTileColor(Color inColor)
        {
            float intensity = this.GetIntensity();
            return new Color(Vector4.Lerp(new Vector4(0.5f, 0.8f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }

        private bool UpdateSIndex()
        {
            int SignusType = ModContent.NPCType<Signus>();
            if (SignusIndex >= 0 && Main.npc[SignusIndex].active && Main.npc[SignusIndex].type == SignusType)
            {
                return true;
            }
            SignusIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == SignusType)
                {
                    SignusIndex = i;
                    break;
                }
            }
            return SignusIndex != -1;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                float intensity = this.GetIntensity();
                spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * intensity);
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
