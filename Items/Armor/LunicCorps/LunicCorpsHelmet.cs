using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
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
        public static readonly SoundStyle ShieldHurtSound = new("CalamityMod/Sounds/Custom/RoverDriveHit") { PitchVariance = 0.6f, Volume = 0.6f, MaxInstances = 0 };
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/RoverDriveActivate") { Volume = 0.85f };
        public static readonly SoundStyle BreakSound = new("CalamityMod/Sounds/Custom/RoverDriveBreak") { Volume = 0.75f };

        public const int MasterChefShieldDefenseBoostMax = 20;
        public const int MasterChefShieldDurabilityMax = 70;
        public const int MasterChefShieldRechargeTime_Seconds = 10;
        public const int MasterChefShieldRechargeTime = 60 * MasterChefShieldRechargeTime_Seconds;

        public static Asset<Texture2D> NoiseTex;

        // Still not sure why the fuck this is necessary
        public override void Load() => Terraria.On_Main.DrawInfernoRings += DrawMasterChefShields;

        private void DrawMasterChefShields(Terraria.On_Main.orig_DrawInfernoRings orig, Main self)
        {
            bool playerFound = false;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (!Main.player[i].active || Main.player[i].outOfRange || Main.player[i].dead)
                    continue;

                CalamityPlayer modPlayer = Main.player[i].GetModPlayer<CalamityPlayer>();

                // Skip if not forced, if it has a value (aka false since if it was true forced visibility would be true
                if (modPlayer.masterChefShieldDurability <= 0)
                    continue;

                float scale = 0.11f + 0.022f * (0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f + i * 0.2f));

                if (!playerFound)
                {
                    float shieldStrentgh = (float)Math.Pow(Main.LocalPlayer.GetModPlayer<CalamityPlayer>().masterChefShieldDurability / (float)MasterChefShieldDurabilityMax, 0.5f);
                    float noiseScale = MathHelper.Lerp(0.4f, 0.8f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.3f) * 0.5f + 0.5f);

                    Effect shieldEffect = Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
                    shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.24f);
                    shieldEffect.Parameters["blowUpPower"].SetValue(2.5f);
                    shieldEffect.Parameters["blowUpSize"].SetValue(0.5f);
                    shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

                    float baseShieldOpacity = 0.9f + 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f);
                    shieldEffect.Parameters["shieldOpacity"].SetValue(baseShieldOpacity * (0.5f + 0.5f * shieldStrentgh));
                    shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                    Color blueTint = new Color(51, 102, 255);
                    Color cyanTint = new Color(71, 202, 255);
                    Color wulfGreen = new Color(194, 255, 67) * 0.8f;
                    Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, blueTint, cyanTint, wulfGreen);
                    Color shieldColor = blueTint;

                    shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
                    shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);

                }

                playerFound = true;
                if (NoiseTex == null)
                    NoiseTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/VoronoiShapes2");

                Player myPlayer = Main.player[i];
                Vector2 pos = myPlayer.MountedCenter + myPlayer.gfxOffY * Vector2.UnitY - Main.screenPosition;

                Texture2D tex = NoiseTex.Value;
                Main.spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);
            }

            if (playerFound)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }

            orig(self);
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
