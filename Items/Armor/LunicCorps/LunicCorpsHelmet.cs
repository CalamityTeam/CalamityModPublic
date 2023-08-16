using System;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables;
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

        // TODO -- Accurate shield sounds from Halo
        public static readonly SoundStyle ShieldHurtSound = new("CalamityMod/Sounds/Custom/RoverDriveHit") { PitchVariance = 0.6f, Volume = 0.6f, MaxInstances = 0 };
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/RoverDriveActivate") { Volume = 0.85f };
        public static readonly SoundStyle BreakSound = new("CalamityMod/Sounds/Custom/RoverDriveBreak") { Volume = 0.75f };

        public static int ShieldDurabilityMax = 70;

        // The following two values taken directly from Halo 3:
        // https://www.halopedia.org/Energy_shielding#Gameplay
        public static int ShieldRechargeDelay = CalamityUtils.SecondsToFrames(5);
        public static int TotalShieldRechargeTime = CalamityUtils.SecondsToFrames(2);

        public static Asset<Texture2D> NoiseTex;

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

                CalamityPlayer modPlayer = Main.player[i].GetModPlayer<CalamityPlayer>();

                // Do not render shield if it does not exist
                if (modPlayer.LunicCorpsShieldDurability <= 0 || modPlayer.drawnAnyShieldThisFrame)
                    continue;

                // Tweaked shield visibility math from Rover Drive. Unsure if it is fitting of Halo
                float scale = 0.11f + 0.022f * (0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f + i * 0.2f));

                if (!alreadyDrawnShieldForPlayer)
                {
                    // Again, I believe there is no way this looks correct when two players have Lunic Corps armor equipped.
                    CalamityPlayer localModPlayer = Main.LocalPlayer.Calamity();
                    float shieldDurabilityRatio = localModPlayer.LunicCorpsShieldDurability / (float)ShieldDurabilityMax;
                    float shieldStrentgh = MathF.Pow(shieldDurabilityRatio, 0.5f);

                    // Noise scale also grows and shrinks, although out of sync with the shield
                    // Seems unedited from Rover Drive
                    float noiseScale = MathHelper.Lerp(0.4f, 0.8f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.3f) * 0.5f + 0.5f);

                    // Define shader parameters
                    Effect shieldEffect = Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
                    shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.24f);
                    shieldEffect.Parameters["blowUpPower"].SetValue(2.5f);
                    shieldEffect.Parameters["blowUpSize"].SetValue(0.5f);
                    shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

                    // Shield opacity multiplier slightly changes, this is independent of current shield strength
                    float baseShieldOpacity = 0.9f + 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f);
                    shieldEffect.Parameters["shieldOpacity"].SetValue(baseShieldOpacity * (0.5f + 0.5f * shieldStrentgh));
                    shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                    // Lunic Corps shields are not team specific
                    // Apparently they still use Wulfrum Green in them somewhere
                    Color blueTint = new Color(51, 102, 255);
                    Color cyanTint = new Color(71, 202, 255);
                    Color wulfGreen = new Color(194, 255, 67) * 0.8f;
                    Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, blueTint, cyanTint, wulfGreen);
                    Color shieldColor = blueTint;

                    // Define shader parameters for shield color
                    shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
                    shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

                    // GOD I LOVE END BEGIN CAN THIS GAME PLEASE BE SWALLOWED BY THE FIRES OF HELL THANKS
                    // yes I copy pasted that comment, I hate end begin that much
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);
                }

                alreadyDrawnShieldForPlayer = true;

                // Draw shield noise? Why is this unconditional??
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
    }
}
