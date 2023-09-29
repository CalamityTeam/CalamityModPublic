using System;
using System.Linq;
using CalamityMod.CalPlayer;
using static CalamityMod.Items.Accessories.ProfanedSoulCrystal;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ProfanedSoulArtifact : ModItem, ILocalizedModType 
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static Asset<Texture2D> HeatTex;
        
        public static int ShieldRechargeDelay = CalamityUtils.SecondsToFrames(5);
        public static int TotalShieldRechargeTime = CalamityUtils.SecondsToFrames(2);

        public static int ShieldDurabilityMax = 25;
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        // Complex drawcode which draws Profaned Soul shields on ALL players who have it available. Supposedly.
        // This is applied as IL (On hook) which draws right before Inferno Ring.
        internal static void DrawProfanedSoulShields(On_Main.orig_DrawInfernoRings orig, Main mainObj)
        {
            // TODO -- Control flow analysis indicates that this hook is not stable (as it was copied from Rover Drive).
            // Profaned Soul shields will be drawn for each player with the Profaned Soul artifact/crystal, yes.
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
                bool isVanityOnly = modPlayer.pSoulShieldVisible && !modPlayer.pSoulArtifact;
                bool shouldNotDraw = modPlayer.andromedaState >= AndromedaPlayerState.LargeRobot; //I am not dealing with drawing that :taxevasion:
                bool shieldExists = isVanityOnly || modPlayer.pSoulShieldDurability > 0;
                if (!modPlayer.pSoulShieldVisible || modPlayer.drawnAnyShieldThisFrame || shouldNotDraw || !shieldExists)
                    continue;

                // Scale the shield is drawn at.
                // The "i" parameter is to make different player's shields not be perfectly synced.
                float scale = 0.15f + 0.03f * (0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f + i * 0.2f));

                if (!alreadyDrawnShieldForPlayer)
                {
                    CalamityPlayer localModPlayer = Main.LocalPlayer.Calamity();
                    DetermineTransformationEligibility(localModPlayer.Player);
                    var psState = (int)GetPscStateFor(localModPlayer.Player, localModPlayer.profanedCrystalAnim >= 0);
                    var psc = localModPlayer.profanedCrystalBuffs || (localModPlayer.profanedCrystalAnim >= 0 && psState >= (int)ProfanedSoulCrystalState.Buffs);
                    
                    scale += ((ProfanedSoulCrystalState)psState) == ProfanedSoulCrystalState.Empowered ? 0.05f : psc ? 0.025f : 0f;
                    if (localModPlayer.profanedCrystalAnim >= 0)
                        scale = MathHelper.Lerp(0f, scale, ((float)maxPscAnimTime - (float)localModPlayer.profanedCrystalAnim) / (float)maxPscAnimTime); //i don't like casting this many times, i'm not a fucking wizard
                    
                    float visualShieldStrength = 1f;
                    if (!isVanityOnly)
                    {
                        float max = psc ? ProfanedSoulCrystal.ShieldDurabilityMax : ShieldDurabilityMax;
                        float shieldDurabilityRatio = localModPlayer.pSoulShieldDurability / max;
                        visualShieldStrength = MathF.Pow(shieldDurabilityRatio, 0.5f);
                    }

                    // The scale used for the noise overlay polygons also grows and shrinks
                    // This is intentionally out of sync with the shield, and intentionally desynced per player
                    // Don't put this anywhere less than 0.25f or higher than 1f. The higher it is, the denser / more zoomed out the noise overlay is.
                    float noiseScale = MathHelper.Lerp(0.4f, 0.8f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.3f) * 0.5f + 0.5f);

                    // Define shader parameters
                    Effect shieldEffect = Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
                    shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.058f); // Scrolling speed of polygonal overlay
                    shieldEffect.Parameters["blowUpPower"].SetValue(2.8f);
                    shieldEffect.Parameters["blowUpSize"].SetValue(0.4f);
                    shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

                    // Shield opacity multiplier slightly changes, this is independent of current shield strength
                    float baseShieldOpacity = 0.9f + 0.1f * MathF.Sin(Main.GlobalTimeWrappedHourly * 1.95f);
                    float minShieldStrengthOpacityMultiplier = 0.5f;
                    float finalShieldOpacity = baseShieldOpacity * MathHelper.Lerp(minShieldStrengthOpacityMultiplier, 1f, visualShieldStrength);
                    shieldEffect.Parameters["shieldOpacity"].SetValue(finalShieldOpacity);
                    shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                    // Profaned Soul shields are not team specific
                    
                    Color shieldColor = GetColorForPsc(psState, Main.dayTime);
                    if (psState >= (int)(ProfanedSoulCrystalState.Buffs))
                    {
                        bool tester = contributorNames.Any(name => name.Equals(localModPlayer.Player.name));
                        shieldColor = tester ? CalamityUtils.ColorSwap(new Color(255, 166, 0), new Color(25, 250, 25) * 0.8f, 6f) :
                        GetLerpedColorForPsc(modPlayer);
                    }
                    
                    Color primaryEdgeColor = new Color(230, 199, 102) * 0.8f;
                    Color secondaryEdgeColor = new Color(249, 231, 217) * 0.8f;
                    Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, primaryEdgeColor, secondaryEdgeColor);

                    // Define shader parameters for shield color
                    shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
                    shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

                    // GOD I LOVE END BEGIN CAN THIS GAME PLEASE BE SWALLOWED BY THE FIRES OF HELL THANKS
                    // yes I copy pasted that comment, I hate end begin that much
                    // I also copy pasted that comment, this time for consistency :hdfailure:
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);
                    // Fetch shield heat overlay texture (this is the neutrons fed to the shader)
                    HeatTex ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons2");
                    Vector2 pos = player.MountedCenter + player.gfxOffY * Vector2.UnitY - Main.screenPosition;
                    Texture2D tex = HeatTex.Value;
                    Main.spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);
                    
                    //The border circle MUST be drawn after otherwise it becomes visually fucked.
                    float shieldScale = scale * 1.75f;
                    Texture2D shieldTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleOpenCircle").Value;
                    Rectangle shieldFrame = shieldTexture.Frame();
                    Vector2 origin = shieldFrame.Size() * 0.5f;
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    Main.spriteBatch.Draw(shieldTexture, pos, shieldFrame, shieldColor * 0.5f, player.fullRotation, origin, shieldScale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(shieldTexture, pos, shieldFrame, secondaryEdgeColor * 0.5f, player.fullRotation, origin, shieldScale * 0.95f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(shieldTexture, pos, shieldFrame, shieldColor * 0.5f, player.fullRotation, origin, shieldScale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(shieldTexture, pos, shieldFrame, secondaryEdgeColor * 0.5f, player.fullRotation, origin, shieldScale * 0.95f, SpriteEffects.None, 0f);
                }

                alreadyDrawnShieldForPlayer = true;
                modPlayer.drawnAnyShieldThisFrame = true;
            }

            if (alreadyDrawnShieldForPlayer)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }

            orig(mainObj);
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 40;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.Calamity().donorItem = true;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return incomingItem.type != ModContent.ItemType<ProfanedSoulCrystal>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.pSoulArtifact = true;
            modPlayer.pSoulShieldVisible = !hideVisual;
        }

        public override void UpdateVanity(Player player)
        {
            player.Calamity().pSoulShieldVisible = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ExodiumCluster>(25).
                AddIngredient<Havocplate>(25).
                AddIngredient<DivineGeode>(5).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
