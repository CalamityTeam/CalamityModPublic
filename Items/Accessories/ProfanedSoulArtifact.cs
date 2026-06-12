using System;
using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.World;
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
    public class ProfanedSoulArtifact : ModItem, ILocalizedModType, IDyeableShaderRenderer
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static Asset<Texture2D> HeatTex;

        public static int ShieldRechargeDelay = CalamityUtils.SecondsToFrames(5);
        public static int TotalShieldRechargeTime = CalamityUtils.SecondsToFrames(2);

        public static int ShieldDurabilityMax = 25;

        // Interface stuff.
        public int OwnerPlayer { get; set; }
        public float RenderDepth => IDyeableShaderRenderer.ProfanedSoulShieldDepth;

        public bool ShouldDrawDyeableShader
        {
            get
            {
                if (CalamityClientConfig.Instance.EnergyShieldOpacity <= 0.0f)
                    return false;

                if (OwnerPlayer < 0 || OwnerPlayer >= Main.maxPlayers)
                    return false;

                var player = Main.player[OwnerPlayer];
                if (player is null)
                    return false;

                if (player.outOfRange || player.dead)
                    return false;

                CalamityPlayer modPlayer = player.Calamity();
                if (modPlayer.drawingParameters.ProfanedShieldCharge <= 0.0f)
                    return false;

                return true;
            }
        }

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<HolyFlames>()];
        }

        public void DrawDyeableShader(SpriteBatch spriteBatch) => DrawProfanedSoulShields(OwnerPlayer);

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 40;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
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

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string adrenTooltip = CalamityWorld.revenge ? this.GetLocalizedValue("ShieldAdren") : "";
            tooltips.FindAndReplace("[ADREN]", adrenTooltip);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DivineGeode>(5).
                AddIngredient<Havocplate>(25).
                AddIngredient<ExodiumCluster>(25).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        // Complex drawcode which draws Profaned Soul shields on ALL players who have it available. Supposedly.
        // This is applied as IL (On hook) which draws right before Inferno Ring.
        internal static void DrawProfanedSoulShields(int whoAmI)
        {
            if (whoAmI < 0 || whoAmI >= Main.maxPlayers)
                return;

            var player = Main.player[whoAmI];
            if (player is null)
                return;

            if (player.outOfRange || player.dead)
                return;

            CalamityPlayer modPlayer = player.Calamity();

            if (modPlayer.drawnAnyShieldThisFrame)
                return;

            if (modPlayer.drawingParameters.ProfanedShieldCharge <= 0.0f)
                return;

            // Scale the shield is drawn at.
            // The "i" parameter is to make different player's shields not be perfectly synced.
            int i = player.whoAmI;
            float scale = 0.15f + 0.03f * (0.5f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 0.5f + i * 0.2f));
            float visualShieldStrength = modPlayer.drawingParameters.ProfanedShieldCharge;
            Color shieldColor = modPlayer.drawingParameters.ProfanedShieldColor;

            // The scale used for the noise overlay polygons also grows and shrinks
            // This is intentionally out of sync with the shield, and intentionally desynced per player
            // Don't put this anywhere less than 0.25f or higher than 1f. The higher it is, the denser / more zoomed out the noise overlay is.
            float noiseScale = MathHelper.Lerp(0.4f, 0.8f, MathF.Sin(Main.GlobalTimeWrappedHourly * 0.3f) * 0.5f + 0.5f);

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
            finalShieldOpacity *= CalamityClientConfig.Instance.EnergyShieldOpacity;

            shieldEffect.Parameters["shieldOpacity"].SetValue(finalShieldOpacity);
            shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

            Color primaryEdgeColor = new Color(230, 199, 102) * 0.8f;
            Color secondaryEdgeColor = new Color(249, 231, 217) * 0.8f;
            Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, primaryEdgeColor, secondaryEdgeColor);

            // Define shader parameters for shield color
            shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
            shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.Transform);
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

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                Main.spriteBatch.Draw(shieldTexture, pos, shieldFrame, shieldColor * 0.5f, player.fullRotation, origin, shieldScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(shieldTexture, pos, shieldFrame, secondaryEdgeColor * 0.5f, player.fullRotation, origin, shieldScale * 0.95f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(shieldTexture, pos, shieldFrame, shieldColor * 0.5f, player.fullRotation, origin, shieldScale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(shieldTexture, pos, shieldFrame, secondaryEdgeColor * 0.5f, player.fullRotation, origin, shieldScale * 0.95f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
            }
            modPlayer.drawnAnyShieldThisFrame = true;
        }
    }
}
