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
        public static Effect SCalMouseShader;
        public static Effect TentacleShader;
        public static Effect TeleportDisplacementShader;
        public static Effect LightDistortionShader;
        public static Effect PhaseslayerRipShader;
        public static Effect FadedUVMapStreakShader;
        public static Effect FlameStreakShader;
        public static Effect FadingSolidTrailShader;
        public static Effect ScarletDevilShader;
        public static Effect BordernadoFireShader;
        public static Effect PrismCrystalShader;
        public static Effect ImpFlameTrailShader;
        public static Effect SCalShieldShader;
        public static Effect RancorMagicCircleShader;
        public static Effect BasicTintShader;
        public static Effect CircularBarShader;
        public static Effect CircularBarSpriteShader;
        public static Effect DoGDisintegrationShader;
        public static Effect ArtAttackTrailShader;
        public static Effect CircularAoETelegraph;
        public static Effect IntersectionClipShader;
        public static Effect LocalLinearTransformationShader;

        public static Effect BaseFusableParticleEdgeShader;
        public static Effect AdditiveFusableParticleEdgeShader;

        public static Effect DoGPortalShader;

        public static void LoadShaders()
        {
            if (Main.dedServ)
                return;

            AstralFogShader = CalamityMod.Instance.GetEffect("Effects/CustomShader");
            LightShader = CalamityMod.Instance.GetEffect("Effects/LightBurstShader");
            TentacleShader = CalamityMod.Instance.GetEffect("Effects/TentacleShader");
            TeleportDisplacementShader = CalamityMod.Instance.GetEffect("Effects/TeleportDisplacementShader");
            SCalMouseShader = CalamityMod.Instance.GetEffect("Effects/SCalMouseShader");
            LightDistortionShader = CalamityMod.Instance.GetEffect("Effects/DistortionShader");
            PhaseslayerRipShader = CalamityMod.Instance.GetEffect("Effects/PhaseslayerRipShader");
            ScarletDevilShader = CalamityMod.Instance.GetEffect("Effects/ScarletDevilStreak");
            BordernadoFireShader = CalamityMod.Instance.GetEffect("Effects/BordernadoFire");
            PrismCrystalShader = CalamityMod.Instance.GetEffect("Effects/PrismCrystalStreak");
            FadedUVMapStreakShader = CalamityMod.Instance.GetEffect("Effects/FadedUVMapStreak");
            FlameStreakShader = CalamityMod.Instance.GetEffect("Effects/Flame");
            FadingSolidTrailShader = CalamityMod.Instance.GetEffect("Effects/FadingSolidTrail");
            ImpFlameTrailShader = CalamityMod.Instance.GetEffect("Effects/ImpFlameTrail");
            SCalShieldShader = CalamityMod.Instance.GetEffect("Effects/SupremeShieldShader");
            RancorMagicCircleShader = CalamityMod.Instance.GetEffect("Effects/RancorMagicCircle");
            BasicTintShader = CalamityMod.Instance.GetEffect("Effects/BasicTint");
            CircularBarShader = CalamityMod.Instance.GetEffect("Effects/CircularBarShader");
            CircularBarSpriteShader = CalamityMod.Instance.GetEffect("Effects/CircularBarSpriteShader");
            DoGDisintegrationShader = CalamityMod.Instance.GetEffect("Effects/DoGDisintegration");
            ArtAttackTrailShader = CalamityMod.Instance.GetEffect("Effects/ArtAttackTrail");
            CircularAoETelegraph = CalamityMod.Instance.GetEffect("Effects/CircularAoETelegraph");
            IntersectionClipShader = CalamityMod.Instance.GetEffect("Effects/IntersectionClipShader");
            LocalLinearTransformationShader = CalamityMod.Instance.GetEffect("Effects/LocalLinearTransformationShader");

            BaseFusableParticleEdgeShader = CalamityMod.Instance.GetEffect("Effects/ParticleFusion/BaseFusableParticleEdgeShader");
            AdditiveFusableParticleEdgeShader = CalamityMod.Instance.GetEffect("Effects/ParticleFusion/AdditiveFusableParticleEdgeShader");

            DoGPortalShader = CalamityMod.Instance.GetEffect("Effects/ScreenShaders/DoGPortalShader");

            Filters.Scene["CalamityMod:Astral"] = new Filter(new AstralScreenShaderData(new Ref<Effect>(AstralFogShader), "AstralPass").UseColor(0.18f, 0.08f, 0.24f), EffectPriority.VeryHigh);

            Filters.Scene["CalamityMod:LightBurst"] = new Filter(new ScreenShaderData(new Ref<Effect>(LightShader), "BurstPass"), EffectPriority.VeryHigh);
            Filters.Scene["CalamityMod:LightBurst"].Load();

            GameShaders.Misc["CalamityMod:FireMouse"] = new MiscShaderData(new Ref<Effect>(SCalMouseShader), "DyePass");
            GameShaders.Misc["CalamityMod:SubsumingTentacle"] = new MiscShaderData(new Ref<Effect>(TentacleShader), "BurstPass");
            GameShaders.Misc["CalamityMod:TeleportDisplacement"] = new MiscShaderData(new Ref<Effect>(TeleportDisplacementShader), "GlitchPass");
            GameShaders.Misc["CalamityMod:LightDistortion"] = new MiscShaderData(new Ref<Effect>(LightDistortionShader), "DistortionPass");
            GameShaders.Misc["CalamityMod:PhaseslayerRipEffect"] = new MiscShaderData(new Ref<Effect>(PhaseslayerRipShader), "TrailPass");
            GameShaders.Misc["CalamityMod:TrailStreak"] = new MiscShaderData(new Ref<Effect>(FadedUVMapStreakShader), "TrailPass");
            GameShaders.Misc["CalamityMod:Flame"] = new MiscShaderData(new Ref<Effect>(FlameStreakShader), "TrailPass");
            GameShaders.Misc["CalamityMod:FadingSolidTrail"] = new MiscShaderData(new Ref<Effect>(FadingSolidTrailShader), "TrailPass");
            GameShaders.Misc["CalamityMod:OverpoweredTouhouSpearShader"] = new MiscShaderData(new Ref<Effect>(ScarletDevilShader), "TrailPass");
            GameShaders.Misc["CalamityMod:Bordernado"] = new MiscShaderData(new Ref<Effect>(BordernadoFireShader), "TrailPass");
            GameShaders.Misc["CalamityMod:PrismaticStreak"] = new MiscShaderData(new Ref<Effect>(PrismCrystalShader), "TrailPass");
            GameShaders.Misc["CalamityMod:ImpFlameTrail"] = new MiscShaderData(new Ref<Effect>(ImpFlameTrailShader), "TrailPass");
            GameShaders.Misc["CalamityMod:SupremeShield"] = new MiscShaderData(new Ref<Effect>(SCalShieldShader), "ShieldPass");
            GameShaders.Misc["CalamityMod:RancorMagicCircle"] = new MiscShaderData(new Ref<Effect>(RancorMagicCircleShader), "ShieldPass");
            GameShaders.Misc["CalamityMod:BasicTint"] = new MiscShaderData(new Ref<Effect>(BasicTintShader), "TintPass");
            GameShaders.Misc["CalamityMod:CircularBarShader"] = new MiscShaderData(new Ref<Effect>(CircularBarShader), "Pass0");
            GameShaders.Misc["CalamityMod:CircularBarSpriteShader"] = new MiscShaderData(new Ref<Effect>(CircularBarSpriteShader), "Pass0");
            GameShaders.Misc["CalamityMod:DoGDisintegration"] = new MiscShaderData(new Ref<Effect>(DoGDisintegrationShader), "DisintegrationPass");
            GameShaders.Misc["CalamityMod:ArtAttack"] = new MiscShaderData(new Ref<Effect>(ArtAttackTrailShader), "TrailPass");
            GameShaders.Misc["CalamityMod:CircularAoETelegraph"] = new MiscShaderData(new Ref<Effect>(CircularAoETelegraph), "TelegraphPass");
            GameShaders.Misc["CalamityMod:IntersectionClip"] = new MiscShaderData(new Ref<Effect>(IntersectionClipShader), "ClipPass");
            GameShaders.Misc["CalamityMod:LinearTransformation"] = new MiscShaderData(new Ref<Effect>(LocalLinearTransformationShader), "TransformationPass");

            GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"] = new MiscShaderData(new Ref<Effect>(BaseFusableParticleEdgeShader), "ParticlePass");
            GameShaders.Misc["CalamityMod:AdditiveFusableParticleEdge"] = new MiscShaderData(new Ref<Effect>(AdditiveFusableParticleEdgeShader), "ParticlePass");

            GameShaders.Misc["CalamityMod:DoGPortal"] = new MiscShaderData(new Ref<Effect>(DoGPortalShader), "ScreenPass");
        }
    }
}
