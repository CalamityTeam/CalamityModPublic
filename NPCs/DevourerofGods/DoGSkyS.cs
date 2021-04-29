using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    public class DoGSkyS : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private int DoGIndex = -1;

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
            if (this.UpdateDoGIndex())
            {
                float x = 0f;
                if (this.DoGIndex != -1)
                {
                    x = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[this.DoGIndex].Center);
                }
                return (1f - Utils.SmoothStep(3000f, 6000f, x)) * 0.5f;
            }
            return 0.5f;
        }

        public override Color OnTileColor(Color inColor)
        {
            float intensity = this.GetIntensity();
            return new Color(Vector4.Lerp(new Vector4(0.5f, 0.8f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }

        private bool UpdateDoGIndex()
        {
            int DoGType = ModContent.NPCType<DevourerofGodsHeadS>();
            if (DoGIndex >= 0 && Main.npc[DoGIndex].active && Main.npc[DoGIndex].type == DoGType)
            {
                return true;
            }
            DoGIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == DoGType)
                {
                    DoGIndex = i;
                    break;
                }
            }
            return DoGIndex != -1;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0 && DoGIndex != -1)
            {
                if (Main.npc[DoGIndex].active)
                {
                    float intensity = this.GetIntensity();
                    if ((double)Main.npc[DoGIndex].life < (double)Main.npc[DoGIndex].lifeMax * 0.15 || CalamityWorld.death || CalamityWorld.malice)
                    {
                        spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                            Color.Black * (intensity + 0.5f));
                    }
                    else
                    {
                        spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                            (Main.npc[DoGIndex].ai[2] == 0f ? Color.Cyan : Color.Fuchsia) * intensity);
                    }
                }
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
