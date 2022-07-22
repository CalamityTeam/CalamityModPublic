using CalamityMod.Skies;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Effects
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
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
        public static Effect BasicPrimitiveShader;
        public static Effect ArtemisLaserShader;
        public static Effect ExobladeSlashShader;
        public static Effect ExobladePierceShader;

        public static Effect BaseFusableParticleEdgeShader;
        public static Effect AdditiveFusableParticleEdgeShader;

        public static Effect DoGPortalShader;

        public static Effect FluidShaders;

        public static void LoadShaders()
        {
            if (Main.dedServ)
                return;

            AstralFogShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/CustomShader", AssetRequestMode.ImmediateLoad).Value;
            LightShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/LightBurstShader", AssetRequestMode.ImmediateLoad).Value;
            TentacleShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/TentacleShader", AssetRequestMode.ImmediateLoad).Value;
            TeleportDisplacementShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/TeleportDisplacementShader", AssetRequestMode.ImmediateLoad).Value;
            SCalMouseShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/SCalMouseShader", AssetRequestMode.ImmediateLoad).Value;
            LightDistortionShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/DistortionShader", AssetRequestMode.ImmediateLoad).Value;
            PhaseslayerRipShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/PhaseslayerRipShader", AssetRequestMode.ImmediateLoad).Value;
            ScarletDevilShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/ScarletDevilStreak", AssetRequestMode.ImmediateLoad).Value;
            BordernadoFireShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/BordernadoFire", AssetRequestMode.ImmediateLoad).Value;
            PrismCrystalShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/PrismCrystalStreak", AssetRequestMode.ImmediateLoad).Value;
            FadedUVMapStreakShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/FadedUVMapStreak", AssetRequestMode.ImmediateLoad).Value;
            FlameStreakShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/Flame", AssetRequestMode.ImmediateLoad).Value;
            FadingSolidTrailShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/FadingSolidTrail", AssetRequestMode.ImmediateLoad).Value;
            ImpFlameTrailShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/ImpFlameTrail", AssetRequestMode.ImmediateLoad).Value;
            SCalShieldShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/SupremeShieldShader", AssetRequestMode.ImmediateLoad).Value;
            RancorMagicCircleShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/RancorMagicCircle", AssetRequestMode.ImmediateLoad).Value;
            BasicTintShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/BasicTint", AssetRequestMode.ImmediateLoad).Value;
            CircularBarShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/CircularBarShader", AssetRequestMode.ImmediateLoad).Value;
            CircularBarSpriteShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/CircularBarSpriteShader", AssetRequestMode.ImmediateLoad).Value;
            DoGDisintegrationShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/DoGDisintegration", AssetRequestMode.ImmediateLoad).Value;
            ArtAttackTrailShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/ArtAttackTrail", AssetRequestMode.ImmediateLoad).Value;
            CircularAoETelegraph = CalamityMod.Instance.Assets.Request<Effect>("Effects/CircularAoETelegraph", AssetRequestMode.ImmediateLoad).Value;
            IntersectionClipShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/IntersectionClipShader", AssetRequestMode.ImmediateLoad).Value;
            LocalLinearTransformationShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/LocalLinearTransformationShader", AssetRequestMode.ImmediateLoad).Value;
            BasicPrimitiveShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/BasicPrimitiveShader", AssetRequestMode.ImmediateLoad).Value;
            ArtemisLaserShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/ArtemisLaserShader", AssetRequestMode.ImmediateLoad).Value;
            ExobladeSlashShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/ExobladeSlashShader", AssetRequestMode.ImmediateLoad).Value;
            ExobladePierceShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/ExobladePierceShader", AssetRequestMode.ImmediateLoad).Value;

            BaseFusableParticleEdgeShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/ParticleFusion/BaseFusableParticleEdgeShader", AssetRequestMode.ImmediateLoad).Value;
            AdditiveFusableParticleEdgeShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/ParticleFusion/AdditiveFusableParticleEdgeShader", AssetRequestMode.ImmediateLoad).Value;

            DoGPortalShader = CalamityMod.Instance.Assets.Request<Effect>("Effects/ScreenShaders/DoGPortalShader", AssetRequestMode.ImmediateLoad).Value;

            FluidShaders = CalamityMod.Instance.Assets.Request<Effect>("Effects/FluidShaders", AssetRequestMode.ImmediateLoad).Value;

            Filters.Scene["CalamityMod:Astral"] = new Filter(new AstralScreenShaderData(new Ref<Effect>(AstralFogShader), "AstralPass").UseColor(0.18f, 0.08f, 0.24f), EffectPriority.VeryHigh);

            Filters.Scene["CalamityMod:LightBurst"] = new Filter(new ScreenShaderData(new Ref<Effect>(LightShader), "BurstPass"), EffectPriority.VeryHigh);
            Filters.Scene["CalamityMod:LightBurst"].Load();

            GameShaders.Misc["CalamityMod:FireMouse"] = new MiscShaderData(new Ref<Effect>(SCalMouseShader), "DyePass");
            GameShaders.Misc["CalamityMod:SubsumingTentacle"] = new MiscShaderData(new Ref<Effect>(TentacleShader), "BurstPass");
            GameShaders.Misc["CalamityMod:TeleportDisplacement"] = new MiscShaderData(new Ref<Effect>(TeleportDisplacementShader), "GlitchPass");
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
            GameShaders.Misc["CalamityMod:PrimitiveDrawer"] = new MiscShaderData(new Ref<Effect>(BasicPrimitiveShader), "TrailPass");
            GameShaders.Misc["CalamityMod:ArtemisLaser"] = new MiscShaderData(new Ref<Effect>(ArtemisLaserShader), "TrailPass");
            GameShaders.Misc["CalamityMod:ExobladeSlash"] = new MiscShaderData(new Ref<Effect>(ExobladeSlashShader), "TrailPass");
            GameShaders.Misc["CalamityMod:ExobladePierce"] = new MiscShaderData(new Ref<Effect>(ExobladePierceShader), "PiercePass");

            GameShaders.Misc["CalamityMod:BaseFusableParticleEdge"] = new MiscShaderData(new Ref<Effect>(BaseFusableParticleEdgeShader), "ParticlePass");
            GameShaders.Misc["CalamityMod:AdditiveFusableParticleEdge"] = new MiscShaderData(new Ref<Effect>(AdditiveFusableParticleEdgeShader), "ParticlePass");

            GameShaders.Misc["CalamityMod:DoGPortal"] = new MiscShaderData(new Ref<Effect>(DoGPortalShader), "ScreenPass");

            //A little experimenting courtesy of looking at how slr does it.
            var screenRef = new Ref<Effect>(CalamityMod.Instance.Assets.Request<Effect>("Effects/SpreadTelegraph", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["SpreadTelegraph"] = new Filter(new ScreenShaderData(screenRef, "TelegraphPass"), EffectPriority.High);
            Filters.Scene["SpreadTelegraph"].Load();

            screenRef = new Ref<Effect>(CalamityMod.Instance.Assets.Request<Effect>("Effects/PixelatedSightLine", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["PixelatedSightLine"] = new Filter(new ScreenShaderData(screenRef, "SightLinePass"), EffectPriority.High);
            Filters.Scene["PixelatedSightLine"].Load();

            screenRef = new Ref<Effect>(CalamityMod.Instance.Assets.Request<Effect>("Effects/WulfrumTilePing", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["WulfrumTilePing"] = new Filter(new ScreenShaderData(screenRef, "TilePingPass"), EffectPriority.High);
            Filters.Scene["WulfrumTilePing"].Load();

            screenRef = new Ref<Effect>(CalamityMod.Instance.Assets.Request<Effect>("Effects/WulfrumScaffoldSelection", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["WulfrumScaffoldSelection"] = new Filter(new ScreenShaderData(screenRef, "TilePingPass"), EffectPriority.High);
            Filters.Scene["WulfrumScaffoldSelection"].Load();

            screenRef = new Ref<Effect>(CalamityMod.Instance.Assets.Request<Effect>("Effects/RoverDriveShield", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["RoverDriveShield"] = new Filter(new ScreenShaderData(screenRef, "ShieldPass"), EffectPriority.High);
            Filters.Scene["RoverDriveShield"].Load();

            screenRef = new Ref<Effect>(CalamityMod.Instance.Assets.Request<Effect>("Effects/Compiler/SwingEffect", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["SwingRotationEffect"] = new Filter(new ScreenShaderData(screenRef, "SwingPass"), EffectPriority.High);
            Filters.Scene["SwingRotationEffect"].Load();
        }
    }
}
