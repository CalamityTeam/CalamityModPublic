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
        public static Effect FadedUVMapStreakShader;
        public static Effect FadingSolidTrailShader;
        public static Effect SCalShieldShader;

        public static void LoadShaders()
        {
            if (Main.dedServ)
                return;
            AstralFogShader = CalamityMod.Instance.GetEffect("Effects/CustomShader");
            LightShader = CalamityMod.Instance.GetEffect("Effects/LightBurstShader");
            TentacleShader = CalamityMod.Instance.GetEffect("Effects/TentacleShader");
            LightDistortionShader = CalamityMod.Instance.GetEffect("Effects/DistortionShader");
            FadedUVMapStreakShader = CalamityMod.Instance.GetEffect("Effects/FadedUVMapStreak");
            FadingSolidTrailShader = CalamityMod.Instance.GetEffect("Effects/FadingSolidTrail");
            SCalShieldShader = CalamityMod.Instance.GetEffect("Effects/SupremeShieldShader");

            Filters.Scene["CalamityMod:Astral"] = new Filter(new AstralScreenShaderData(new Ref<Effect>(AstralFogShader), "AstralPass").UseColor(0.18f, 0.08f, 0.24f), EffectPriority.VeryHigh);

            Filters.Scene["CalamityMod:LightBurst"] = new Filter(new ScreenShaderData(new Ref<Effect>(LightShader), "BurstPass"), EffectPriority.VeryHigh);
            Filters.Scene["CalamityMod:LightBurst"].Load();

            GameShaders.Misc["CalamityMod:SubsumingTentacle"] = new MiscShaderData(new Ref<Effect>(TentacleShader), "BurstPass");
            GameShaders.Misc["CalamityMod:LightDistortion"] = new MiscShaderData(new Ref<Effect>(LightDistortionShader), "DistortionPass");
            GameShaders.Misc["CalamityMod:TrailStreak"] = new MiscShaderData(new Ref<Effect>(FadedUVMapStreakShader), "TrailPass");
            GameShaders.Misc["CalamityMod:FadingSolidTrail"] = new MiscShaderData(new Ref<Effect>(FadingSolidTrailShader), "TrailPass");
            GameShaders.Misc["CalamityMod:SupremeShield"] = new MiscShaderData(new Ref<Effect>(SCalShieldShader), "ShieldPass");
        }
    }
}
