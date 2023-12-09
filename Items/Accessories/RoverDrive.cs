using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RoverDrive : ModItem, ILocalizedModType, IDyeableShaderRenderer
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static Asset<Texture2D> NoiseTex;
        public static readonly SoundStyle ShieldHurtSound = new("CalamityMod/Sounds/Custom/RoverDriveHit") { PitchVariance = 0.6f, Volume = 0.6f, MaxInstances = 0 };
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/RoverDriveActivate") { Volume = 0.85f };
        public static readonly SoundStyle BreakSound = new("CalamityMod/Sounds/Custom/RoverDriveBreak") { Volume = 0.75f };

        public static int ShieldDurabilityMax = 20;
        public static int ShieldRechargeTime = CalamityUtils.SecondsToFrames(10);

        // While active, Rover Drive gives 10 defense
        public static int ShieldDefenseBoost = 10;

        // Interface stuff.
        public float RenderDepth => IDyeableShaderRenderer.RoverDriveDepth;

        public bool ShouldDrawDyeableShader
        {
            get
            {
                bool result = false;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player is null || !player.active || player.outOfRange || player.dead)
                       continue;

                    CalamityPlayer modPlayer = player.Calamity();

                    // Do not render the shield if its visibility is off (or it does not exist)
                    bool isVanityOnly = modPlayer.roverDriveShieldVisible && !modPlayer.roverDrive;
                    bool shieldExists = isVanityOnly || modPlayer.RoverDriveShieldDurability > 0;
                    bool shouldntDraw = (!modPlayer.roverDriveShieldVisible || modPlayer.drawnAnyShieldThisFrame || !shieldExists);
                    result |= !shouldntDraw;
                }
                return result;
            }
        }

        // Allows item to be extractinated and specifies custom behavior instead of copying an existing item
        public override void SetStaticDefaults() => ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
            Item.MakeUsableWithChlorophyteExtractinator();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();

            modPlayer.roverDrive = true;
            modPlayer.roverDriveShieldVisible = !hideVisual;

            if (modPlayer.RoverDriveShieldDurability > 0)
                player.statDefense += ShieldDefenseBoost;
        }

        // In vanity, provides a visual shield but no actual functionality
        public override void UpdateVanity(Player player) => player.Calamity().roverDriveShieldVisible = true;

        // Scrappable for 3-5 wulfrum scrap or a 20% chance to get an energy core
        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            resultType = ModContent.ItemType<WulfrumMetalScrap>();
            resultStack = Main.rand.Next(3, 6);

            if (Main.rand.NextFloat() > 0.8f)
            {
                resultStack = 1;
                resultType = ModContent.ItemType<EnergyCore>();
            }
        }

        // Complex drawcode which draws Rover Drive shields on ALL players who have it available. Supposedly.
        // This is applied as IL (On hook) which draws right before Inferno Ring.
        public void DrawDyeableShader(SpriteBatch spriteBatch)
        {
            // TODO -- Control flow analysis indicates that this hook is not stable.
            // Rover Drive shields will be drawn for each player with Rover Drive, yes.
            // But there is no guarantee that the shields will be in the right condition for each player.
            // Visibility is not net synced, for example.
            bool alreadyDrawnShieldForPlayer = false;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player is null || !player.active || player.outOfRange || player.dead)
                    continue;

                CalamityPlayer modPlayer = player.Calamity();

                // Do not render the shield if its visibility is off (or it does not exist)
                bool isVanityOnly = modPlayer.roverDriveShieldVisible && !modPlayer.roverDrive;
                bool shieldExists = isVanityOnly || modPlayer.RoverDriveShieldDurability > 0;
                if (!modPlayer.roverDriveShieldVisible || modPlayer.drawnAnyShieldThisFrame || !shieldExists)
                    continue;

                // The shield very gently grows and shrinks
                float scale = 0.15f + 0.03f * (0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f + i * 0.2f));

                if (!alreadyDrawnShieldForPlayer)
                {
                    // If in vanity, the shield is always projected as if it's at full strength.
                    float shieldStrength = 1f;
                    if (!isVanityOnly)
                    {
                        // Again, I believe there is no way this looks correct when two players have Rover Drive equipped.
                        CalamityPlayer localModPlayer = Main.LocalPlayer.Calamity();
                        float shieldDurabilityRatio = localModPlayer.RoverDriveShieldDurability / (float)ShieldDurabilityMax;
                        shieldStrength = MathF.Pow(shieldDurabilityRatio, 0.5f);
                    }

                    // Noise scale also grows and shrinks, although out of sync with the shield
                    float noiseScale = MathHelper.Lerp(0.4f, 0.8f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.3f) * 0.5f + 0.5f);

                    // Define shader parameters
                    Effect shieldEffect = Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
                    shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.24f);
                    shieldEffect.Parameters["blowUpPower"].SetValue(2.5f);
                    shieldEffect.Parameters["blowUpSize"].SetValue(0.5f);
                    shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

                    // Shield opacity multiplier slightly changes, this is independent of current shield strength
                    float baseShieldOpacity = 0.9f + 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f);
                    shieldEffect.Parameters["shieldOpacity"].SetValue(baseShieldOpacity * (0.5f + 0.5f * shieldStrength));
                    shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                    // Get the shield color.
                    Color blueTint = new Color(51, 102, 255);
                    Color cyanTint = new Color(71, 202, 255);
                    Color wulfGreen = new Color(194, 255, 67) * 0.8f;
                    Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, blueTint, cyanTint, wulfGreen);
                    Color shieldColor = blueTint;
                    

                    // Define shader parameters for shield color
                    shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
                    shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

                    // GOD I LOVE END BEGIN CAN THIS GAME PLEASE BE SWALLOWED BY THE FIRES OF HELL THANKS
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);

                }

                alreadyDrawnShieldForPlayer = true;
                modPlayer.drawnAnyShieldThisFrame = true;

                // Fetch shield noise overlay texture (this is the techy overlay fed to the shader)
                NoiseTex ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise");
                Vector2 pos = player.MountedCenter + player.gfxOffY * Vector2.UnitY - Main.screenPosition;
                Texture2D tex = NoiseTex.Value;
                spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);
            }

            if (alreadyDrawnShieldForPlayer)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
        }
    }
}
