using CalamityMod.NPCs;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace CalamityMod.Graphics.Drawers
{
    public class HolyInfernoDrawer : BaseDrawer
    {
        #region Fields/Properties

        public override DrawerLayer Layer => DrawerLayer.BeforeTiles;

        public Providence Provi => (Main.npc[CalamityGlobalNPC.holyBoss].ModNPC as Providence);

        //Should only draw if not in the main menu, provi is active and the boolean for drawing the border is true.
        public override bool ShouldDraw => !Main.gameMenu && CalamityGlobalNPC.holyBoss != -1 && 
            Main.npc[CalamityGlobalNPC.holyBoss].active && Providence.shouldDrawInfernoBorder;
        #endregion

        #region Methods
        public override void DrawToTarget(SpriteBatch spriteBatch)
        {
            var npc = Main.npc[CalamityGlobalNPC.holyBoss];
            var borderDistance = Providence.borderRadius;
            if (npc.HasValidTarget)
            {
                var target = Main.player[Main.myPlayer];
                var holyInfernoIntensity = target.Calamity().holyInfernoFadeIntensity;

                //Begin drawing the inferno
                var blackTile = TextureAssets.MagicPixel;
                var diagonalNoise = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/HarshNoise");
                var upwardPerlinNoise = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Perlin");
                var upwardNoise = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/MeltyNoise");

                var shader = GameShaders.Misc["CalamityMod:HolyInfernoShader"].Shader;
                shader.Parameters["colorMult"].SetValue(Main.dayTime ? 7.35f : 7.65f); //I want you to know it took considerable restraint to deliberately misspell colour.
                shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
                shader.Parameters["radius"].SetValue(borderDistance);
                shader.Parameters["anchorPoint"].SetValue(npc.Center);
                shader.Parameters["screenPosition"].SetValue(Main.screenPosition);
                shader.Parameters["screenSize"].SetValue(Main.ScreenSize.ToVector2());
                shader.Parameters["burnIntensity"].SetValue(holyInfernoIntensity);
                shader.Parameters["playerPosition"].SetValue(target.Center);
                shader.Parameters["maxOpacity"].SetValue(1f);
                shader.Parameters["day"].SetValue(Main.dayTime);

                spriteBatch.GraphicsDevice.Textures[1] = diagonalNoise.Value;
                spriteBatch.GraphicsDevice.Textures[2] = upwardNoise.Value;
                spriteBatch.GraphicsDevice.Textures[3] = upwardPerlinNoise.Value;

                //Manual end begin for the sampler state
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, shader, Main.GameViewMatrix.TransformationMatrix);
                Rectangle rekt = new(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight);
                spriteBatch.Draw(blackTile.Value, rekt, null, default, 0f, blackTile.Value.Size() * 0.5f, 0, 0f);
                //Inferno drawing complete
                spriteBatch.ExitShaderRegion();
            }
        }
        #endregion
    }
}
