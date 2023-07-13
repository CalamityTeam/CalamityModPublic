using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Providence
{
    public class ProvSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private int ProvIndex = -1;

        public override void Update(GameTime gameTime)
        {
            if (ProvIndex == -1)
            {
                UpdatePIndex();
                if (ProvIndex == -1)
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
            if (UpdatePIndex())
            {
                float x = 0f;
                if (ProvIndex != -1)
                    x = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[ProvIndex].Center);

                float spawnAnimationTimer = 180f;
                float intensityScalar = 0.25f;
                if (Main.npc[ProvIndex].Calamity().newAI[3] < spawnAnimationTimer)
                    intensityScalar = MathHelper.Lerp(0f, intensityScalar, Main.npc[ProvIndex].Calamity().newAI[3] / spawnAnimationTimer);

                return (Main.player[Main.myPlayer].HasBuff(ModContent.BuffType<HolyInferno>()) ? 0.75f : (1f - Utils.SmoothStep(3000f, 6000f, x)) * intensityScalar);
            }
            return 0f;
        }

        public override Color OnTileColor(Color inColor)
        {
            float intensity = GetIntensity();
            return new Color(Vector4.Lerp(new Vector4(0.5f, 0.8f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }

        private bool UpdatePIndex()
        {
            int ProvType = ModContent.NPCType<Providence>();
            if (ProvIndex >= 0 && Main.npc[ProvIndex].active && Main.npc[ProvIndex].type == ProvType)
            {
                return true;
            }
            ProvIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ProvType)
                {
                    ProvIndex = i;
                    break;
                }
            }
            return ProvIndex != -1;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                float intensity = GetIntensity();
                Color color = Main.zenithWorld ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB) : Main.dayTime ? new Color(255, 200, 100) : new Color(100, 150, 255);
                spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), color * intensity);
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
