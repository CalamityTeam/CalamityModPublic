using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Events
{
    public class BossRushScreenShader : ScreenShaderData
    {
        public BossRushScreenShader(string passName) : base(passName) { }

        public override void Apply()
        {
            UseTargetPosition(Main.LocalPlayer.Center);
            base.Apply();
        }

        public override void Update(GameTime gameTime)
        {
            UseColor(Color.Lerp(BossRushSky.GeneralColor, Color.Black, BossRushEvent.WhiteDimness));
        }
    }
}
