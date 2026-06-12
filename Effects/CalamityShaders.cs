using CalamityMod.Skies;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Effects
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    // TODO: SOURCEGEN: Get rid of this loader and replace references with sourcegenned references
    
    [Autoload(Side = ModSide.Client)]
    public sealed class CalamityShaders : ModSystem
    {
        private const string ShaderPath = "Effects/";
        internal const string CalamityShaderPrefix = "CalamityMod:";

        // made by boffin. Source code not available. Creates fog in the astral infection
        internal static Asset<Effect> AstralFogShader;

        //
        // All below shaders created by Lucille Karma
        //
        #region Lucille's Shaders

        // The Dance of Light's Blinding Light
        internal static Asset<Effect> DanceOfLightBlindingShader;

        // Calamity accessory
        internal static Asset<Effect> CalamityAccessoryMouseShader;

        // Subsuming Vortex tentacles
        internal static Asset<Effect> SubsumingVortexTentacleShader;

        // Draedon teleporting shader
        internal static Asset<Effect> DraedonTeleportShader;

        // UNKNOWN -- PROBABLY UNUSED
        internal static Asset<Effect> LightDistortionShader;

        // Phaseslayer red energy trail
        internal static Asset<Effect> PhaseslayerRipShader;

        // Generic UV-fade streak trail shader. Used by many things?
        internal static Asset<Effect> FadedUVMapStreakShader;

        // Generic flaming streak trail shader. Used by many things?
        internal static Asset<Effect> FlameStreakShader;

        // Generic solid trail shader. Used by many things?
        internal static Asset<Effect> FadingSolidTrailShader;

        // Typical projectile javelin glowing trail. BEWARE: Is used by many items!
        internal static Asset<Effect> ScarletDevilShader;

        // Yharon border flame pillar trail.
        internal static Asset<Effect> BordernadoFireShader;

        // Photon Ripper hardlight teeth trail shader
        internal static Asset<Effect> PrismCrystalShader;

        // Generic flame trail. BEWARE: Reused by Apollo, Artemis, Cosmilamp, Gem Tech broken gems, and Persecuted enchant portal demons
        internal static Asset<Effect> ImpFlameTrailShader;

        // Supreme Witch, Calamitas' force field (which is her visual hitbox)
        internal static Asset<Effect> SCalShieldShader;

        // Rancor's alchemical / satanic circle which floats near the player. The laser is emitted from here
        internal static Asset<Effect> RancorMagicCircleShader;

        // Generic colored glow shader. BEWARE: Used by about a billion things, including Ares telegraphs, the Biome Blade line, etc.
        internal static Asset<Effect> BasicTintShader;

        // Generic "pie chart" shader that renders only certain arc sectors of things. Used for the Cooldown Rack and various other visuals.
        internal static Asset<Effect> CircularBarShader;

        // Cooldown Rack shader used to draw the cooldown's icon appropriately.
        internal static Asset<Effect> CircularBarSpriteShader;

        // Devourer of Gods' death animation where he disintegrates into purple energy stuff.
        internal static Asset<Effect> DoGDisintegrationShader;

        // Art Attack's main projectile trail shader (Visually designed to match Paper Mario)
        internal static Asset<Effect> ArtAttackTrailShader;

        // Generic AoE telegraph. Used for Ares' nukes to denote their area of effect.
        internal static Asset<Effect> CircularAoETelegraph;

        // Clips a sprite along a fixed plane. Used by Stream Gouge to have half-spears come out of portals.
        internal static Asset<Effect> IntersectionClipShader;

        // Was previously used by Dom's Bladecrest Oathsword. Appears to govern the swing animation.
        internal static Asset<Effect> LocalLinearTransformationShader;

        // UNUSED -- Probably leftover from Lucille's experiments with applying shaders to primitives (arbitrary GPU-rendered triangles)
        internal static Asset<Effect> BasicPrimitiveShader;

        // Artemis Ohio Beam. Also used by get fixed boi Nuclear Terror's "G-FUEL BEAM"
        internal static Asset<Effect> ArtemisLaserShader;

        // Exoblade's melee slash trails. Also used by Terratomere
        internal static Asset<Effect> ExobladeSlashShader;

        // Exoblade's projectile on-hit "anime slash marks". Also used by Terratomere
        internal static Asset<Effect> ExobladePierceShader;

        // Used by Subsuming Vortex's various vortices. Draws the main vortices. BEWARE: Reused by Burning Sea's fireball.
        internal static Asset<Effect> ExoVortexShader;

        // Used by Subsuming Vortex's small vortices. Draws the trailing energy tendrils.
        internal static Asset<Effect> SideStreakTrailShader;

        // Used by Heavenly Gale's hardlight arrows.
        internal static Asset<Effect> HeavenlyGaleTrailShader;

        // Used by Heavenly Gale's exo lightning strikes. Also used by Stormfront Razor lightning.
        internal static Asset<Effect> HeavenlyGaleLightningShader;

        // Used in the Codebreaker on Draedon's sprite while communicating with him.
        internal static Asset<Effect> BlueStaticShader;

        // Used by Acid Eels, presumably to have their snaking movements look more smooth.
        internal static Asset<Effect> PrimTextureOverlayShader;

        // Used as a default for primitive drawing when no specific shader is supplied. This shader simply renders the vertex color data without modification.
        internal static Asset<Effect> StandardPrimitiveShader;

        // Used by Devourer of Gods. Renders the portal that he escapes through at the end of phase 1.
        internal static Asset<Effect> DoGPortalShader;

        // Used to render all-encompassing fog in the Floral Paradise biome.
        internal static Asset<Effect> FogShader;

        // Used to render background water features in the Floral Paradise biome.
        internal static Asset<Effect> WaterfallShader;

        // Metaballs. See the MetaballManager class for comments on how this system works.
        // These shaders are leveraged to render the results of the metaball simulation to the screen.
        // The "Base" shader draws the particles themselves.
        // The "Additive" shader actually FUSES the particles into their blobby mess.
        //
        // Backing textures vary. The primary use of this system is Gruesome Eminence.
        internal static Asset<Effect> MetaballEdgeShader;
        internal static Asset<Effect> AdditiveMetaballEdgeShader;

        // Used to render the results of Navier-Stokes fluid simulations.
        internal static Asset<Effect> FluidShaders;

        // Used by projectiles fired by the Sylvestaff.
        internal static Asset<Effect> SylvestaffProjectileShader;

        // Used by the ribbons on the Sylvestaff.
        internal static Asset<Effect> SylvestaffRibbonShader;
        #endregion

        //
        // All below shaders were added to Calamity by IbanPlay
        // Authorship between IbanPlay and other SLR devs unknown
        //
        #region Iban's Shaders

        // Used by Coral Spout and Titanium Railgun while charging to indicate current projectile spread.
        internal static Asset<Effect> SpreadTelegraph;

        // Used by Wulfrum Screwdriver and Titanium Railgun. Renders a pixelly "laser sight".
        internal static Asset<Effect> PixelatedSightLine;

        // Used by the Wulfrum Treasure Pinger to highlight tiles in range
        internal static Asset<Effect> WulfrumTilePing;

        // Used by the Wulfrum Scaffold Kit to highlight locations where Scaffold tiles will be placed
        internal static Asset<Effect> WulfrumScaffoldSelection;

        // Used to render the Rover Drive's force field around the player
        internal static Asset<Effect> RoverDriveShield;

        // UNUSED -- Probably leftover from Iban's work on the Biome Blade and Ark lines.
        internal static Asset<Effect> RotateSprite;

        // Used on Exoblade to draw the sword swinging dramatically. This was the edit Iban made to Dom's Exoblade.
        internal static Asset<Effect> SwingSprite;
        #endregion

        //
        // All below shaders created by Ozzatron
        //
        #region Ozz's Shaders

        // Used on players and NPCs when they have the Miracle Blight debuff.
        internal static Asset<Effect> MiracleBlight;
        #endregion

        #region Aqua's Shaders
        internal static Asset<Effect> CircularGradientWithEdge;
        internal static Asset<Effect> GaleforceArrowTrailShader;
        internal static Asset<Effect> WavyOpacity;
        internal static Asset<Effect> HellBall;
        #endregion

        //
        // All below shaders were added or created by Amber
        // Authorship for the PrimitiveClearShader goes to Toasty
        //

        #region Amber's Shaders
        internal static Asset<Effect> PrimitiveClearShader;
        internal static Asset<Effect> HolyInfernoShader;
        #endregion

        #region YuH's Shaders
        internal static Asset<Effect> TeslaTrailShader;
        #endregion

        #region Doze's Shaders
        /// <summary>
        /// The shader used for Abyss's custom light system.
        /// Basically works by turning textures into an opacity mask
        /// </summary>
        internal static Asset<Effect> DozeLightingShader;
        /// <summary>
        /// Flips the screen. Used for Gravity Globe.
        /// </summary>
        internal static Asset<Effect> FlipScreenShader;
        /// <summary>
        /// Distorts a texture towards the center based on a provided noisemap. Meant to emulate the Terraria Otherworld barrier effects.
        /// Opacity is the intensity of this distortion, and Saturation is the speed the noisemap moves at.
        /// </summary>
        internal static Asset<Effect> OtherworldBarrierDistortionShader;
        #endregion

        //
        // All below shaders were created by fryzahh
        //
        #region fryzahh's Shaders
        // A simple shader which distorts an image using a provided texture of choice. 
        internal static Asset<Effect> BasicTextureDistortionShader;

        // The underwater rays seen at the top of the Sunken Sea Mod Menu.
        internal static Asset<Effect> UnderwaterRaysShader;

        // The blowing wind-like effect seen in the background during DoG's fight.
        internal static Asset<Effect> DoGDistortionWindsShader;

        // The rolling fog clouds seen behind the Distortion Winds effect during DoG's fight.
        internal static Asset<Effect> DoGBackgroundFogShader;

        // Allows a texture to control its opacity from the edges to the center in the shape of a circle.
        internal static Asset<Effect> CircularOpacityShader;

        // The shader effect imposed onto the static crack textures seen during DoG's fight. Fades out the texture from its edges to its center and 
        // allows it to lerp between two colors depending on the brightness of individual pixels along the sampled texture.
        internal static Asset<Effect> DoGRealityCrackShader;

        // The distorted circular effect seen emanating from the distortion rift during DoG's fight.
        internal static Asset<Effect> DoGRiftAuraShader;

        // The shader effect used for Voidragon's Abyssal Fire laser projectile.
        internal static Asset<Effect> AbyssalFireShader;

        // A simple chromatic abberation effect shader with support for custom abberation colors.
        internal static Asset<Effect> ChromaticAbberationShader;

        // Distorts a texture using a sine wave of specified amplitude and frequency either vertically or horizontally.
        internal static Asset<Effect> SineWaveDistortionShader;

        // The swirling aura effect seen around Horrible Hog while its idling.
        internal static Asset<Effect> HorribleHogAuraShader;
        #endregion

        #region Big E's Shaders
        internal static Asset<Effect> RadialBlur;

        internal static Asset<Effect> SeaPrismColorBlendingShader;
        internal static Asset<Effect> Dissolve;

        internal static Asset<Effect> BrainOfCthulhuForcefield;
        #endregion
        
        #region jasper's shaders
        internal static Asset<Effect> NanoblackSlashShader;

        // static gaussian bloom, cannot change parameters. do not reuse this if that binds you.
        internal static Asset<Effect> GaussianBloomShader;
        #endregion


        internal static Asset<Effect> SunkenSeaMenuLogoWater;

        public override void PostSetupContent()
        {
            AssetRepository calAss = CalamityMod.Instance.Assets;

            // Shorthand to load shaders immediately.
            // Strings provided to LoadShader are the .xnb file paths.
            Asset<Effect> LoadShader(string path) => calAss.Request<Effect>($"{ShaderPath}{path}", AssetRequestMode.ImmediateLoad);

            //
            // Loading and registering each individual compiled shader for use.
            //

            AstralFogShader = LoadShader("AstralFogShader");
            var astralPassReg = new AstralScreenShaderData(AstralFogShader, "AstralPass").UseColor(0.18f, 0.08f, 0.24f);
            RegisterSceneFilter(astralPassReg, "Astral", EffectPriority.VeryHigh);

            #region Loading Lucille's Shaders

            DanceOfLightBlindingShader = LoadShader("LightBurstShader");
            RegisterScreenShader(DanceOfLightBlindingShader, "BurstPass", "LightBurst");

            SubsumingVortexTentacleShader = LoadShader("TentacleShader");
            RegisterMiscShader(SubsumingVortexTentacleShader, "BurstPass", "SubsumingTentacle");

            DraedonTeleportShader = LoadShader("TeleportDisplacementShader");
            RegisterMiscShader(DraedonTeleportShader, "GlitchPass", "TeleportDisplacement");

            CalamityAccessoryMouseShader = LoadShader("SCalMouseShader");
            RegisterMiscShader(CalamityAccessoryMouseShader, "DyePass", "FireMouse");

            // THIS SHADER IS UNUSED.
            LightDistortionShader = LoadShader("DistortionShader");

            PhaseslayerRipShader = LoadShader("PhaseslayerRipShader");
            RegisterMiscShader(PhaseslayerRipShader, "TrailPass", "PhaseslayerRipEffect");

            FadedUVMapStreakShader = LoadShader("FadedUVMapStreak");
            RegisterMiscShader(FadedUVMapStreakShader, "TrailPass", "TrailStreak");

            FlameStreakShader = LoadShader("Flame");
            RegisterMiscShader(FlameStreakShader, "TrailPass", "Flame");

            FadingSolidTrailShader = LoadShader("FadingSolidTrail");
            RegisterMiscShader(FadingSolidTrailShader, "TrailPass", "FadingSolidTrail");

            ScarletDevilShader = LoadShader("ScarletDevilStreak");
            RegisterMiscShader(ScarletDevilShader, "TrailPass", "OverpoweredTouhouSpearShader");

            BordernadoFireShader = LoadShader("BordernadoFire");
            RegisterMiscShader(BordernadoFireShader, "TrailPass", "Bordernado");

            PrismCrystalShader = LoadShader("PrismCrystalStreak");
            RegisterMiscShader(PrismCrystalShader, "TrailPass", "PrismaticStreak");

            ImpFlameTrailShader = LoadShader("ImpFlameTrail");
            RegisterMiscShader(ImpFlameTrailShader, "TrailPass", "ImpFlameTrail");

            SCalShieldShader = LoadShader("SupremeShieldShader");
            RegisterMiscShader(SCalShieldShader, "ShieldPass", "SupremeShield");

            RancorMagicCircleShader = LoadShader("RancorMagicCircle");
            RegisterMiscShader(RancorMagicCircleShader, "ShieldPass", "RancorMagicCircle");

            BasicTintShader = LoadShader("BasicTint");
            RegisterMiscShader(BasicTintShader, "TintPass", "BasicTint");

            CircularBarShader = LoadShader("CircularBarShader");
            RegisterMiscShader(CircularBarShader, "Pass0", "CircularBarShader");

            CircularBarSpriteShader = LoadShader("CircularBarSpriteShader");
            RegisterMiscShader(CircularBarSpriteShader, "Pass0", "CircularBarSpriteShader");

            DoGDisintegrationShader = LoadShader("DoGDisintegration");
            RegisterMiscShader(DoGDisintegrationShader, "DisintegrationPass", "DoGDisintegration");

            ArtAttackTrailShader = LoadShader("ArtAttackTrail");
            RegisterMiscShader(ArtAttackTrailShader, "TrailPass", "ArtAttack");

            CircularAoETelegraph = LoadShader("CircularAoETelegraph");
            RegisterMiscShader(CircularAoETelegraph, "TelegraphPass", "CircularAoETelegraph");

            IntersectionClipShader = LoadShader("IntersectionClipShader");
            RegisterMiscShader(IntersectionClipShader, "ClipPass", "IntersectionClip");

            LocalLinearTransformationShader = LoadShader("LocalLinearTransformationShader");
            RegisterMiscShader(LocalLinearTransformationShader, "TransformationPass", "LinearTransformation");

            // NOTE: despite being registered, this shader is UNUSED
            BasicPrimitiveShader = LoadShader("BasicPrimitiveShader");
            RegisterMiscShader(BasicPrimitiveShader, "TrailPass", "PrimitiveDrawer");

            ArtemisLaserShader = LoadShader("ArtemisLaserShader");
            RegisterMiscShader(ArtemisLaserShader, "TrailPass", "ArtemisLaser");

            ExobladeSlashShader = LoadShader("ExobladeSlashShader");
            RegisterMiscShader(ExobladeSlashShader, "TrailPass", "ExobladeSlash");

            ExobladePierceShader = LoadShader("ExobladePierceShader");
            RegisterMiscShader(ExobladePierceShader, "PiercePass", "ExobladePierce");

            ExoVortexShader = LoadShader("ExoVortexShader");
            RegisterMiscShader(ExoVortexShader, "VortexPass", "ExoVortex");

            SideStreakTrailShader = LoadShader("SideStreakTrail");
            RegisterMiscShader(SideStreakTrailShader, "TrailPass", "SideStreakTrail");

            HeavenlyGaleTrailShader = LoadShader("HeavenlyGaleTrailShader");
            RegisterMiscShader(HeavenlyGaleTrailShader, "PiercePass", "HeavenlyGaleTrail");

            HeavenlyGaleLightningShader = LoadShader("HeavenlyGaleLightningShader");
            RegisterMiscShader(HeavenlyGaleLightningShader, "TrailPass", "HeavenlyGaleLightningArc");

            BlueStaticShader = LoadShader("BlueStaticShader");
            RegisterMiscShader(BlueStaticShader, "GlitchPass", "BlueStatic");

            PrimTextureOverlayShader = LoadShader("PrimTextureOverlayShader");
            RegisterMiscShader(PrimTextureOverlayShader, "TrailPass", "PrimitiveTexture");

            StandardPrimitiveShader = LoadShader("StandardPrimitiveShader");
            RegisterMiscShader(StandardPrimitiveShader, "PrimitivePass", "StandardPrimitiveShader");

            DoGPortalShader = LoadShader("ScreenShaders/DoGPortalShader");
            RegisterMiscShader(DoGPortalShader, "ScreenPass", "DoGPortal");

            FogShader = LoadShader("ScreenShaders/Fog");
            RegisterMiscShader(FogShader, "DyePass", "Fog");

            WaterfallShader = LoadShader("WaterfallShader");
            RegisterMiscShader(WaterfallShader, "TrailPass", "Waterfall");

            // These two shaders are often (but not always) used together.
            MetaballEdgeShader = LoadShader("Metaballs/MetaballEdgeShader");
            RegisterMiscShader(MetaballEdgeShader, "ParticlePass", "MetaballEdge");

            AdditiveMetaballEdgeShader = LoadShader("Metaballs/AdditiveMetaballEdgeShader");
            RegisterMiscShader(AdditiveMetaballEdgeShader, "ParticlePass", "AdditiveMetaballEdge");

            // This shader is not registered with the game but is invoked directly to render the results of fluid simulation.
            FluidShaders = LoadShader("FluidShaders");

            SylvestaffProjectileShader = LoadShader("SylvestaffProjectileShader");
            RegisterMiscShader(SylvestaffProjectileShader, "TrailPass", "SylvestaffProjectile");

            SylvestaffRibbonShader = LoadShader("SylvestaffRibbonShader");
            RegisterMiscShader(SylvestaffRibbonShader, "AutoloadPass", "SylvestaffRibbon");
            #endregion

            #region Loading Iban's Shaders

            SpreadTelegraph = LoadShader("SpreadTelegraph");
            RegisterScreenShader(SpreadTelegraph, "TelegraphPass", "SpreadTelegraph");

            PixelatedSightLine = LoadShader("PixelatedSightLine");
            RegisterScreenShader(PixelatedSightLine, "SightLinePass", "PixelatedSightLine");

            WulfrumTilePing = LoadShader("WulfrumTilePing");
            RegisterScreenShader(WulfrumTilePing, "TilePingPass", "WulfrumTilePing");

            WulfrumScaffoldSelection = LoadShader("WulfrumScaffoldSelection");
            RegisterScreenShader(WulfrumScaffoldSelection, "TilePingPass", "WulfrumScaffoldSelection");

            RoverDriveShield = LoadShader("RoverDriveShield");
            RegisterScreenShader(RoverDriveShield, "ShieldPass", "RoverDriveShield");

            // THIS SHADER IS UNUSED.
            RotateSprite = LoadShader("RotateSprite");
            RegisterScreenShader(RotateSprite, "RotationPass", "RotateSprite");

            SwingSprite = LoadShader("SwingSprite");
            RegisterScreenShader(SwingSprite, "SwingPass", "SwingSprite");
            #endregion

            #region Loading Ozz's Shaders

            MiracleBlight = LoadShader("MiracleBlight");
            RegisterMiscShader(MiracleBlight, "BlightPass", "MiracleBlight");
            #endregion

            #region Loading Aqua's Shaders
            CircularGradientWithEdge = LoadShader("CircularGradientWithEdge");
            RegisterMiscShader(CircularGradientWithEdge, "CircularGradientWithEdgePass", "CircularGradientWithEdge");

            GaleforceArrowTrailShader = LoadShader("GaleforceArrowTrail");
            RegisterMiscShader(ArtAttackTrailShader, "TrailPass", "GaleforceArrowTrail");

            WavyOpacity = LoadShader("WavyOpacity");
            RegisterMiscShader(WavyOpacity, "WavyOpacityPass", "WavyOpacity");

            HellBall = LoadShader("HellBall");
            RegisterScreenShader(HellBall, "HellBallPass", "HellBall");
            #endregion

            #region Loading Amber's Shaders
            PrimitiveClearShader = LoadShader("PrimitiveClearShader");
            RegisterScreenShader(PrimitiveClearShader, "AutoloadPass", "PrimitiveClearShader");

            HolyInfernoShader = LoadShader("ScreenShaders/HolyInfernoShader");
            RegisterMiscShader(HolyInfernoShader, "InfernoPass", "HolyInfernoShader");

            #endregion

            #region Loading YuH's Shaders
            TeslaTrailShader = LoadShader("TeslaTrail");
            RegisterMiscShader(TeslaTrailShader, "TrailPass", "TeslaTrail");
            #endregion

            #region Loading Doze's Shaders
            DozeLightingShader = LoadShader("DozeLightingShader");
            RegisterMiscShader(DozeLightingShader, "ShadowPass", "DozeLightingShader");

            FlipScreenShader = LoadShader("ScreenShaders/FlipScreen");
            RegisterScreenShader(FlipScreenShader, "FlipTheScreen", "FlipScreen",EffectPriority.VeryHigh);

            OtherworldBarrierDistortionShader = LoadShader("OtherworldBarrierDistortion");
            RegisterMiscShader(OtherworldBarrierDistortionShader, "DistortionPass", "OtherworldBarrierDistortion");
            #endregion

            #region Loading fryzahh's Shaders
            BasicTextureDistortionShader = LoadShader("BasicTextureDistortionShader");
            RegisterMiscShader(BasicTextureDistortionShader, "DistortionPass", "BasicTextureDistortion");

            UnderwaterRaysShader = LoadShader("UnderwaterRaysShader");
            RegisterMiscShader(UnderwaterRaysShader, "UnderwaterRayPass", "UnderwaterRays");

            DoGDistortionWindsShader = LoadShader("DoGDistortionWindsShader");
            RegisterMiscShader(DoGDistortionWindsShader, "DistortionWindsPass", "DoGDistortionWinds");

            DoGBackgroundFogShader = LoadShader("DoGBackgroundFogShader");
            RegisterMiscShader(DoGBackgroundFogShader, "DoGFogPass", "DoGBackgroundFog");

            CircularOpacityShader = LoadShader("CircularOpacityShader");
            RegisterMiscShader(CircularOpacityShader, "CircularOpacityPass", "CircularOpacity");

            DoGRealityCrackShader = LoadShader("DoGRealityCrackShader");
            RegisterMiscShader(DoGRealityCrackShader, "DoGRealityCrackPass", "DoGRealityCrack");

            DoGRiftAuraShader = LoadShader("DoGRiftAuraShader");
            RegisterMiscShader(DoGRiftAuraShader, "DoGRiftAuraPass", "DoGRiftAura");

            AbyssalFireShader = LoadShader("AbyssalFireShader");
            RegisterMiscShader(AbyssalFireShader, "LaserPass", "AbyssalFire");

            ChromaticAbberationShader = LoadShader("ChromaticAbberationShader");
            RegisterMiscShader(ChromaticAbberationShader, "ChormaAbberationPass", "ChromaticAbberation");

            SineWaveDistortionShader = LoadShader("SineWaveDistortionShader");
            RegisterMiscShader(SineWaveDistortionShader, "SinePass", "SineWaveDistortion");

            HorribleHogAuraShader = LoadShader("HorribleHogAuraShader");
            RegisterMiscShader(HorribleHogAuraShader, "ScaryAuraPass", "HorribleHogAura");
            #endregion

            #region Loading Big E's Shaders
            RadialBlur = LoadShader("ScreenShaders/RadialBlur");
            RegisterScreenShader(RadialBlur, "RadialBlurPass", "RadialBlurShader");

            SeaPrismColorBlendingShader = LoadShader("SeaPrismColorBlending");
            RegisterMiscShader(SeaPrismColorBlendingShader, "SeaPrismBlendingPass", "SeaPrismColorBlending");

            Dissolve = LoadShader("Dissolve");
            RegisterMiscShader(Dissolve, "DissolvePass", "Dissolve");

            BrainOfCthulhuForcefield = LoadShader("ScreenShaders/BrainOfCthulhuForcefield");
            RegisterScreenShader(BrainOfCthulhuForcefield, "BoCShieldPass", "BrainOfCthulhuForcefield");
            #endregion

            NanoblackSlashShader = LoadShader("SlashEffects/NanoblackSlash");
            RegisterMiscShader(NanoblackSlashShader, "SlashPass", "NanoblackSlash");

            GaussianBloomShader = LoadShader("SlashEffects/GaussianBloom");
            RegisterMiscShader(GaussianBloomShader, "BloomShader", "GaussianBloom");

            SunkenSeaMenuLogoWater = LoadShader("UI/SunkenSeaMenuLogoWater");
        }

        // Shorthand to register a loaded shader in Terraria's graphics engine
        // All shaders registered this way are accessible under GameShaders.Misc
        // They will use the prefix described above
        private static void RegisterMiscShader(Asset<Effect> shader, string passName, string registrationName)
        {
            MiscShaderData passParamRegistration = new(shader, passName);
            GameShaders.Misc[$"{CalamityShaderPrefix}{registrationName}"] = passParamRegistration;
        }

        private static void RegisterSceneFilter(ScreenShaderData passReg, string registrationName, EffectPriority priority = EffectPriority.High)
        {
            string prefixedRegistrationName = $"{CalamityShaderPrefix}{registrationName}";
            Filters.Scene[prefixedRegistrationName] = new Filter(passReg, priority);
            Filters.Scene[prefixedRegistrationName].Load();
        }

        // Shorthand to register a loaded shader in Terraria's graphics engine
        // All shaders registered this way are accessible under Filters.Scene
        // They will use the prefix described above
        private static void RegisterScreenShader(Asset<Effect> shader, string passName, string registrationName, EffectPriority priority = EffectPriority.High)
        {
            ScreenShaderData passParamRegistration = new(shader, passName);
            RegisterSceneFilter(passParamRegistration, registrationName, priority);
        }
    }
}
