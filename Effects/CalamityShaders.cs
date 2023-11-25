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
        private const string ShaderPath = "Effects/";
        internal const string CalamityShaderPrefix = "CalamityMod:";

        // made by boffin. Source code not available. Creates fog in the astral infection
        internal static Effect AstralFogShader;

        //
        // All below shaders created by Dominic Karma
        //
        #region Dominic's Shaders

        // The Dance of Light's Blinding Light
        internal static Effect DanceOfLightBlindingShader;

        // Calamity accessory
        internal static Effect CalamityAccessoryMouseShader;

        // Subsuming Vortex tentacles
        internal static Effect SubsumingVortexTentacleShader;

        // Draedon teleporting shader
        internal static Effect DraedonTeleportShader;

        // UNKNOWN -- PROBABLY UNUSED
        internal static Effect LightDistortionShader;

        // Phaseslayer red energy trail
        internal static Effect PhaseslayerRipShader;

        // Generic UV-fade streak trail shader. Used by many things?
        internal static Effect FadedUVMapStreakShader;

        // Generic flaming streak trail shader. Used by many things?
        internal static Effect FlameStreakShader;

        // Generic solid trail shader. Used by many things?
        internal static Effect FadingSolidTrailShader;

        // Scarlet Devil main spear glowing trail. BEWARE: Is reused by many other items!
        internal static Effect ScarletDevilShader;

        // Yharon border suns (or flame pillars?) unsure. it's one or the other. maybe both
        internal static Effect BordernadoFireShader;

        // Photon Ripper hardlight teeth trail shader
        internal static Effect PrismCrystalShader;

        // Generic flame trail. BEWARE: Reused by Apollo, Artemis, Cosmilamp, Gem Tech broken gems, and Persecuted enchant portal demons
        internal static Effect ImpFlameTrailShader;

        // Supreme Witch, Calamitas' force field (which is her visual hitbox)
        internal static Effect SCalShieldShader;

        // Rancor's alchemical / satanic circle which floats near the player. The laser is emitted from here
        internal static Effect RancorMagicCircleShader;

        // Generic colored glow shader. BEWARE: Used by about a billion things, including Ares telegraphs, the Biome Blade line, etc.
        internal static Effect BasicTintShader;

        // Generic "pie chart" shader that renders only certain arc sectors of things. Used for the Cooldown Rack and various other visuals.
        internal static Effect CircularBarShader;

        // Cooldown Rack shader used to draw the cooldown's icon appropriately.
        internal static Effect CircularBarSpriteShader;

        // Devourer of Gods' death animation where he disintegrates into purple energy stuff.
        internal static Effect DoGDisintegrationShader;

        // Art Attack's main projectile trail shader (Visually designed to match Paper Mario)
        internal static Effect ArtAttackTrailShader;

        // Generic AoE telegraph. Used for Ares' nukes to denote their area of effect.
        internal static Effect CircularAoETelegraph;

        // Clips a sprite along a fixed plane. Used by Stream Gouge to have half-spears come out of portals.
        internal static Effect IntersectionClipShader;

        // Used by Dom's Bladecrest Oathsword. Appears to govern the swing animation.
        internal static Effect LocalLinearTransformationShader;

        // UNUSED -- Probably leftover from Dominic's experiments with applying shaders to primitives (arbitrary GPU-rendered triangles)
        internal static Effect BasicPrimitiveShader;

        // Artemis Ohio Beam. Also used by get fixed boi Nuclear Terror's "G-FUEL BEAM"
        internal static Effect ArtemisLaserShader;

        // Exoblade's melee slash trails. Also used by Terratomere
        internal static Effect ExobladeSlashShader;

        // Exoblade's projectile on-hit "aniume slash marks". Also used by Terratomere
        internal static Effect ExobladePierceShader;

        // Used by Subsuming Vortex's various vortices. Draws the main vortices
        internal static Effect ExoVortexShader;

        // Used by Subsuming Vortex's side vortices. Draws the swirling energy tendrils.
        internal static Effect SideStreakTrailShader;

        // Used by Heavenly Gale's hardlight arrows.
        internal static Effect HeavenlyGaleTrailShader;

        // Used by Heavenly Gale's exo lightning strikes. Also used by Stormfront Razor lightning.
        internal static Effect HeavenlyGaleLightningShader;

        // Used in the Codebreaker on Draedon's sprite while communicating with him.
        internal static Effect BlueStaticShader;

        // Used by Acid Eels, presumably to have their snaking movements look more smooth.
        internal static Effect PrimTextureOverlayShader;

        // Used as a default for primitive drawing when no specific shader is supplied. This shader simply renders the vertex color data without modification.
        internal static Effect StandardPrimitiveShader;

        // Used by Devourer of Gods. Renders the portal that he escapes through at the end of phase 1.
        internal static Effect DoGPortalShader;

        // Metaballs. See the MetaballManager class for comments on how this system works.
        // These shaders are leveraged to render the results of the metaball simulation to the screen.
        // The "Base" shader draws the particles themselves.
        // The "Additive" shader actually FUSES the particles into their blobby mess.
        //
        // Backing textures vary. The primary use of this system is Gruesome Eminence.
        internal static Effect MetaballEdgeShader;
        internal static Effect AdditiveMetaballEdgeShader;

        // Used to render the results of Navier-Stokes fluid simulations.
        internal static Effect FluidShaders;
        #endregion

        //
        // All below shaders were added to Calamity by IbanPlay
        // Authorship between IbanPlay and other SLR devs unknown
        //
        #region Iban's Shaders

        // Used by Coral Spout and Titanium Railgun while charging to indicate current projectile spread.
        internal static Effect SpreadTelegraph;

        // Used by Wulfrum Screwdriver and Titanium Railgun. Renders a pixelly "laser sight".
        internal static Effect PixelatedSightLine;

        // Used by the Wulfrum Treasure Pinger to highlight tiles in range
        internal static Effect WulfrumTilePing;

        // Used by the Wulfrum Scaffold Kit to highlight locations where Scaffold tiles will be placed
        internal static Effect WulfrumScaffoldSelection;

        // Used to render the Rover Drive's force field around the player
        internal static Effect RoverDriveShield;

        // UNUSED -- Probably leftover from Iban's work on the Biome Blade and Ark lines.
        internal static Effect RotateSprite;

        // Used on Exoblade to draw the sword swinging dramatically. This was the edit Iban made to Dom's Exoblade.
        internal static Effect SwingSprite;
        #endregion

        //
        // All below shaders created by Ozzatron
        //
        #region Ozz's Shaders

        // Used on players and NPCs when they have the Miracle Blight debuff.
        internal static Effect MiracleBlight;
        #endregion

        #region Aqua's Shaders
        internal static Effect CircularGradientWithEdge;
        internal static Effect WavyOpacity;
        #endregion

        //
        // All below shaders were added or created by Amber
        // Authorship for the PrimitiveClearShader goes to Toasty
        //

        #region Amber's Shaders
        internal static Effect PrimitiveClearShader;
        internal static Effect HolyInfernoShader;
        internal static Effect DeerclopsShadowShader;
        #endregion

        // Shorthand to register a loaded shader in Terraria's graphics engine
        // All shaders registered this way are accessible under GameShaders.Misc
        // They will use the prefix described above
        private static void RegisterMiscShader(Effect shader, string passName, string registrationName)
        {
            Ref<Effect> shaderPointer = new(shader);
            MiscShaderData passParamRegistration = new(shaderPointer, passName);
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
        private static void RegisterScreenShader(Effect shader, string passName, string registrationName, EffectPriority priority = EffectPriority.High)
        {
            Ref<Effect> shaderPointer = new(shader);
            ScreenShaderData passParamRegistration = new(shaderPointer, passName);
            RegisterSceneFilter(passParamRegistration, registrationName, priority);
        }

        public static void LoadShaders()
        {
            if (Main.dedServ)
                return;

            AssetRepository calAss = CalamityMod.Instance.Assets;

            // Shorthand to load shaders immediately.
            // Strings provided to LoadShader are the .xnb file paths.
            Effect LoadShader(string path) => calAss.Request<Effect>($"{ShaderPath}{path}", AssetRequestMode.ImmediateLoad).Value;

            //
            // Loading and registering each individual compiled shader for use.
            //

            AstralFogShader = LoadShader("AstralFogShader");
            var astralPassReg = new AstralScreenShaderData(new Ref<Effect>(AstralFogShader), "AstralPass").UseColor(0.18f, 0.08f, 0.24f);
            RegisterSceneFilter(astralPassReg, "Astral", EffectPriority.VeryHigh);

            #region Loading Dominic's Shaders

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

            // These two shaders are often (but not always) used together.
            MetaballEdgeShader = LoadShader("Metaballs/MetaballEdgeShader");
            RegisterMiscShader(MetaballEdgeShader, "ParticlePass", "MetaballEdge");

            AdditiveMetaballEdgeShader = LoadShader("Metaballs/AdditiveMetaballEdgeShader");
            RegisterMiscShader(AdditiveMetaballEdgeShader, "ParticlePass", "AdditiveMetaballEdge");

            // This shader is not registered with the game but is invoked directly to render the results of fluid simulation.
            FluidShaders = LoadShader("FluidShaders");
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
            WavyOpacity = LoadShader("WavyOpacity");
            RegisterMiscShader(WavyOpacity, "WavyOpacityPass", "WavyOpacity");
            #endregion

            #region Loading Amber's Shaders
            PrimitiveClearShader = LoadShader("PrimitiveClearShader");
            RegisterScreenShader(PrimitiveClearShader, "AutoloadPass", "PrimitiveClearShader");

            HolyInfernoShader = LoadShader("ScreenShaders/HolyInfernoShader");
            RegisterMiscShader(HolyInfernoShader, "InfernoPass", "HolyInfernoShader");

            DeerclopsShadowShader = LoadShader("ScreenShaders/DeerclopsShadowShader");
            RegisterMiscShader(DeerclopsShadowShader, "ShadowPass", "DeerclopsShadowShader");

            #endregion
        }
    }
}
