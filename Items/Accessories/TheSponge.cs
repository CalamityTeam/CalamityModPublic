using System;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [LegacyName("Sponge")]
    public class TheSponge : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public override string Texture => (DateTime.Now.Month == 4 && DateTime.Now.Day == 1) ? "CalamityMod/Items/Accessories/TheSpongeReal" : "CalamityMod/Items/Accessories/TheSponge";

        public static Asset<Texture2D> NoiseTex;
        // TODO -- Unique sounds for The Sponge
        public static readonly SoundStyle ShieldHurtSound = new("CalamityMod/Sounds/Custom/RoverDriveHit") { PitchVariance = 0.6f, Volume = 0.6f, MaxInstances = 0 };
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/RoverDriveActivate") { Volume = 0.85f };
        public static readonly SoundStyle BreakSound = new("CalamityMod/Sounds/Custom/RoverDriveBreak") { Volume = 0.75f };

        public static int ShieldDurabilityMax = 180;
        public static int ShieldRechargeDelay = CalamityUtils.SecondsToFrames(9); // was 6
        public static int TotalShieldRechargeTime = CalamityUtils.SecondsToFrames(6);

        // While active, The Sponge gives 30 defense and 10% DR
        public static int ShieldActiveDefense = 30;
        public static float ShieldActiveDamageReduction = 0.1f;


        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 30));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sponge = true;
            modPlayer.spongeShieldVisible = !hideVisual;

            if (modPlayer.SpongeShieldDurability > 0)
            {
                player.statDefense += ShieldActiveDefense;
                player.endurance += ShieldActiveDamageReduction;
            }
        }

        // In vanity, provides a visual shield but no actual functionality
        public override void UpdateVanity(Player player) => player.Calamity().spongeShieldVisible = true;

        // Renders the bubble shield over the item in the world.
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            // Will not render a shield if the April Fool's sprite is active.
            if (Texture == "CalamityMod/Items/Accessories/TheSponge")
            {
                Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/TheSpongeShield").Value;
                spriteBatch.Draw(tex, Item.Center - Main.screenPosition + new Vector2(0f, 0f), Main.itemAnimations[Item.type].GetFrame(tex), Color.Cyan * 0.5f, 0f, new Vector2(tex.Width / 2f, (tex.Height / 30f ) * 0.8f), 1f, SpriteEffects.None, 0);
            }
        }

        // TODO -- Is this necessary to block the shield in-inventory on April first?
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return Texture != "CalamityMod/Items/Accessories/TheSponge";
        }

        // Renders the bubble shield over the item in a player's inventory.
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // Will not render a shield if the April Fool's sprite is active.
            if (Texture == "CalamityMod/Items/Accessories/TheSponge")
            {
                float wantedScale = 0.85f;
                Vector2 drawOffset = new(-2f, -1f);

                CalamityUtils.DrawInventoryCustomScale(
                    spriteBatch,
                    texture: TextureAssets.Item[Type].Value,
                    position,
                    frame,
                    drawColor,
                    itemColor,
                    origin,
                    scale,
                    wantedScale,
                    drawOffset
                );
                Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/TheSpongeShield").Value;
                CalamityUtils.DrawInventoryCustomScale(
                    spriteBatch,
                    texture: tex,
                    position,
                    Main.itemAnimations[Item.type].GetFrame(tex),
                    Color.Cyan * 0.4f,
                    itemColor,
                    origin,
                    scale,
                    wantedScale,
                    drawOffset
                );
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RoverDrive>().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(20).
                AddIngredient<CosmiliteBar>(5).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }

        // Complex drawcode which draws Sponge shields on ALL players who have it available. Supposedly.
        // This is applied as IL (On hook) which draws right before Inferno Ring.
        internal static void DrawSpongeShields(On_Main.orig_DrawInfernoRings orig, Main mainObj)
        {
            // TODO -- Control flow analysis indicates that this hook is not stable (as it was copied from Rover Drive).
            // Sponge shields will be drawn for each player with the Sponge equipped, yes.
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
                bool isVanityOnly = modPlayer.spongeShieldVisible && !modPlayer.sponge;
                bool shieldExists = isVanityOnly || modPlayer.SpongeShieldDurability > 0;
                if (!modPlayer.spongeShieldVisible || modPlayer.drawnAnyShieldThisFrame || !shieldExists)
                    continue;

                // Scale the shield is drawn at. The Sponge shield gently grows and shrinks; it should be largely imperceptible.
                // The "i" parameter is to make different player's shields not be perfectly synced.
                float baseScale = 0.155f;
                float maxExtraScale = 0.025f;
                float extraScalePulseInterpolant = MathF.Pow(4f, MathF.Sin(Main.GlobalTimeWrappedHourly * 0.791f + i) - 1);
                float scale = baseScale + maxExtraScale * extraScalePulseInterpolant;

                if (!alreadyDrawnShieldForPlayer)
                {
                    // If in vanity, the shield is always projected as if it's at full strength.
                    float visualShieldStrength = 1f;
                    if (!isVanityOnly)
                    {
                        // Again, I believe there is no way this looks correct when two players have The Sponge equipped.
                        CalamityPlayer localModPlayer = Main.LocalPlayer.Calamity();
                        float shieldDurabilityRatio = localModPlayer.SpongeShieldDurability / (float)ShieldDurabilityMax;
                        visualShieldStrength = MathF.Pow(shieldDurabilityRatio, 0.5f);
                    }

                    // The scale used for the noise overlay also grows and shrinks
                    // This is intentionally out of sync with the shield, and intentionally desynced per player
                    // Don't put this anywhere less than 0.15f or higher than 0.75f. The higher it is, the denser / more zoomed out the noise overlay is.
                    // Changing this too quickly and/or too much makes the noise grow and shrink visibly, so be careful with that.
                    float noiseScale = MathHelper.Lerp(0.28f, 0.38f, 0.5f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 0.347f + i));

                    // Define shader parameters
                    Effect shieldEffect = Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
                    shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.0813f); // Scrolling speed of polygonal overlay
                    shieldEffect.Parameters["blowUpPower"].SetValue(3f);
                    shieldEffect.Parameters["blowUpSize"].SetValue(0.56f);
                    shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

                    // Shield opacity multiplier slightly changes, this is independent of current shield strength
                    float baseShieldOpacity = 0.9f + 0.1f * MathF.Sin(Main.GlobalTimeWrappedHourly * 1.95f);
                    float minShieldStrengthOpacityMultiplier = 0.25f;
                    float finalShieldOpacity = baseShieldOpacity * MathHelper.Lerp(minShieldStrengthOpacityMultiplier, 1f, visualShieldStrength);
                    shieldEffect.Parameters["shieldOpacity"].SetValue(finalShieldOpacity);
                    shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                    Color shieldColor, primaryEdgeColor, secondaryEdgeColor;
                    
                    // Outside of single player, the shield color is overridden if the player is on a team.
                    if (Main.netMode != NetmodeID.SinglePlayer && player.team != 0)
                    {
                        switch (Main.player[i].team)
                        {
                            // Red team
                            case 1:
                                shieldColor = new Color(163, 25, 26); // #A3191A
                                primaryEdgeColor = shieldColor;
                                secondaryEdgeColor = new Color(196, 51, 31); // #C4331F
                                break;

                            // Green team
                            case 2:
                                shieldColor = new Color(13, 143, 22); // #0D8F16
                                primaryEdgeColor = shieldColor;
                                secondaryEdgeColor = new Color(30, 201, 62); // #1EC93E
                                break;

                            // Blue team
                            case 3:
                                shieldColor = new Color(18, 57, 163); // #1239A3
                                primaryEdgeColor = shieldColor;
                                secondaryEdgeColor = new Color(38, 110, 191); // #266EBF
                                break;

                            // Yellow team
                            case 4:
                                shieldColor = new Color(194, 154, 10); // #C29A0A
                                primaryEdgeColor = shieldColor;
                                secondaryEdgeColor = new Color(230, 209, 25); // #E6D119
                                break;

                            // Pink team
                            case 5:
                                shieldColor = new Color(184, 22, 143); // #B8168F
                                primaryEdgeColor = shieldColor;
                                secondaryEdgeColor = new Color(220, 40, 213); // #DE28D5
                                break;

                            // Invalid team
                            default:
                                shieldColor = new Color(148, 162, 176); // #94A2B0
                                primaryEdgeColor = shieldColor;
                                secondaryEdgeColor = new Color(193, 197, 201); // #C1C5C9
                                break;
                        }
                    }

                    // Un-teamed / single player shield colors
                    else
                    {
                        shieldColor = new Color(24, 156, 204); // #189CCC
                        primaryEdgeColor = shieldColor;
                        secondaryEdgeColor = new Color(34, 224, 227); // #22E0E3
                    }

                    // Final shield edge color, which lerps about
                    Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, primaryEdgeColor, secondaryEdgeColor);

                    // Define shader parameters for shield color
                    shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
                    shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

                    // GOD I LOVE END BEGIN CAN THIS GAME PLEASE BE SWALLOWED BY THE FIRES OF HELL THANKS
                    // yes I copy pasted that comment, I hate end begin that much
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);
                }

                alreadyDrawnShieldForPlayer = true;
                modPlayer.drawnAnyShieldThisFrame = true;

                // Fetch shield noise overlay texture (this is the polygons fed to the shader)
                NoiseTex ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons");
                Vector2 pos = player.MountedCenter + player.gfxOffY * Vector2.UnitY - Main.screenPosition;
                Texture2D tex = NoiseTex.Value;
                Main.spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);
            }

            if (alreadyDrawnShieldForPlayer)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }

            orig(mainObj);
        }
    }
}
