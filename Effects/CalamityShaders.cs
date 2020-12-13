using CalamityMod.Skies;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Effects
{
    public class CalamityShaders
    {
        public static Effect AstralFogShader;
        public static Effect LightShader;
        public static Effect TentacleShader;
        public static Effect LightDistortionShader;
        public static Effect PhaseslayerRipShader;
        public static Effect FabstaffRayShader;
        public static Effect ScarletDevilShader;

        public static void LoadShaders()
        {
            if (Main.dedServ)
                return;
            AstralFogShader = CalamityMod.Instance.GetEffect("Effects/CustomShader");
            LightShader = CalamityMod.Instance.GetEffect("Effects/LightBurstShader");
            TentacleShader = CalamityMod.Instance.GetEffect("Effects/TentacleShader");
            LightDistortionShader = CalamityMod.Instance.GetEffect("Effects/DistortionShader");
            PhaseslayerRipShader = CalamityMod.Instance.GetEffect("Effects/PhaseslayerRipShader");
            FabstaffRayShader = CalamityMod.Instance.GetEffect("Effects/FabstaffStreak");
            ScarletDevilShader = CalamityMod.Instance.GetEffect("Effects/ScarletDevilStreak");

            Filters.Scene["CalamityMod:Astral"] = new Filter(new AstralScreenShaderData(new Ref<Effect>(AstralFogShader), "AstralPass").UseColor(0.18f, 0.08f, 0.24f), EffectPriority.VeryHigh);

            Filters.Scene["CalamityMod:LightBurst"] = new Filter(new ScreenShaderData(new Ref<Effect>(LightShader), "BurstPass"), EffectPriority.VeryHigh);
            Filters.Scene["CalamityMod:LightBurst"].Load();

            GameShaders.Misc["CalamityMod:SubsumingTentacle"] = new MiscShaderData(new Ref<Effect>(TentacleShader), "BurstPass");
            GameShaders.Misc["CalamityMod:LightDistortion"] = new MiscShaderData(new Ref<Effect>(LightDistortionShader), "DistortionPass");
            GameShaders.Misc["CalamityMod:PhaseslayerRipEffect"] = new MiscShaderData(new Ref<Effect>(PhaseslayerRipShader), "TrailPass");
            GameShaders.Misc["CalamityMod:FabstaffRay"] = new MiscShaderData(new Ref<Effect>(FabstaffRayShader), "TrailPass");
            GameShaders.Misc["CalamityMod:OverpoweredTouhouSpearShader"] = new MiscShaderData(new Ref<Effect>(ScarletDevilShader), "TrailPass");
        }
    }
}
