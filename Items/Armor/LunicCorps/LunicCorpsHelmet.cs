using System;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.LunicCorps
{
    [AutoloadEquip(EquipType.Head)]
    public class LunicCorpsHelmet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";

        public static Asset<Texture2D> NoiseTex;
        // TODO -- Accurate shield sounds from Halo
        public static readonly SoundStyle ShieldHurtSound = new("CalamityMod/Sounds/Custom/RoverDriveHit") { PitchVariance = 0.6f, Volume = 0.6f, MaxInstances = 0 };
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/RoverDriveActivate") { Volume = 0.85f };
        public static readonly SoundStyle BreakSound = new("CalamityMod/Sounds/Custom/RoverDriveBreak") { Volume = 0.75f };

        public static int ShieldDurabilityMax = 50;

        // The following two values taken directly from Halo 3:
        // https://www.halopedia.org/Energy_shielding#Gameplay
        public static int ShieldRechargeDelay = CalamityUtils.SecondsToFrames(5);
        public static int TotalShieldRechargeTime = CalamityUtils.SecondsToFrames(2);

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.defense = 14;
            Item.rare = ItemRarityID.Cyan;
            Item.Calamity().donorItem = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LunicCorpsVest>() && legs.type == ModContent.ItemType<LunicCorpsBoots>();
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.lunicCorpsSet = true;
            player.setBonus = this.GetLocalizedValue("SetBonus");

            player.bulletDamage += 0.1f;
            player.specialistDamage += 0.1f;

            player.jumpSpeedBoost += 1f;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += 0.12f;
            player.nightVision = true;
            player.detectCreature = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.NightVisionHelmet).
                AddIngredient<AstralBar>(6).
                AddIngredient(ItemID.ChlorophyteBar, 6).
                AddIngredient(ItemID.Glass, 20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

        // Complex drawcode which draws Lunic Corps shields on ALL players who have it available. Supposedly.
        // This is applied as IL (On hook) which draws right before Inferno Ring.
        internal static void DrawHaloShields(On_Main.orig_DrawInfernoRings orig, Main mainObj)
        {
            // TODO -- Control flow analysis indicates that this hook is not stable (as it was copied from Rover Drive).
            // Lunic Corps shields will be drawn for each player with the Lunic Corps armor, yes.
            // But there is no guarantee that the shields will be in the right condition for each player.
            // Visibility is not net synced, for example.
            bool alreadyDrawnShieldForPlayer = false;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player is null || !player.active || player.outOfRange || player.dead)
                    continue;

                CalamityPlayer modPlayer = player.Calamity();

                // Do not render shield if it does not exist
                if (modPlayer.LunicCorpsShieldDurability <= 0 || modPlayer.drawnAnyShieldThisFrame)
                    continue;

                // Scale the shield is drawn at. The Lunic Corps shield sticks very close to the body to mimic Halo and occasionally pulses.
                // The "i" parameter is to make different player's shields not be perfectly synced.
                float baseScale = 0.11f;
                float maxExtraScale = 0.013f;
                float extraScalePulseInterpolant = MathF.Pow(12f, MathF.Sin(Main.GlobalTimeWrappedHourly * 1.6f + i) - 1);
                float scale = baseScale + maxExtraScale * extraScalePulseInterpolant;

                if (!alreadyDrawnShieldForPlayer)
                {
                    // Again, I believe there is no way this looks correct when two players have Lunic Corps armor equipped.
                    CalamityPlayer localModPlayer = Main.LocalPlayer.Calamity();
                    float shieldDurabilityRatio = localModPlayer.LunicCorpsShieldDurability / (float)ShieldDurabilityMax;
                    float visualShieldStrength = MathF.Pow(shieldDurabilityRatio, 0.5f);

                    // The scale used for the noise overlay polygons also grows and shrinks
                    // This is intentionally out of sync with the shield, and intentionally desynced per player
                    // Don't put this anywhere less than 0.25f or higher than 1f. The higher it is, the denser / more zoomed out the noise overlay is.
                    float noiseScale = MathHelper.Lerp(0.65f, 0.75f, 0.5f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 0.87f + i));

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

                    // Lunic Corps shields are not team specific
                    Color shieldColor = new Color(201, 180, 129);
                    Color primaryEdgeColor = new Color(232, 212, 175);
                    Color secondaryEdgeColor = new Color(237, 205, 145);
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
                NoiseTex ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/VoronoiShapes2");
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
