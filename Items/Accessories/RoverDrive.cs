using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RoverDrive : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static readonly SoundStyle ShieldHurtSound = new("CalamityMod/Sounds/Custom/RoverDriveHit") { PitchVariance = 0.6f, Volume = 0.6f, MaxInstances = 0 };
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/RoverDriveActivate") { Volume = 0.85f };
        public static readonly SoundStyle BreakSound = new("CalamityMod/Sounds/Custom/RoverDriveBreak") { Volume = 0.75f };

        public static int ProtectionMatrixDurabilityMax = 40;
        public static int ProtectionMatrixRechargeTime = 60 * 10;
        public static int ProtectionMatrixDefenseBoost = 10;

        public static Asset<Texture2D> NoiseTex;

        // What in the actual fuck is this
        public override void Load() => Terraria.On_Main.DrawInfernoRings += DrawRoverDriveShields;

        private void DrawRoverDriveShields(Terraria.On_Main.orig_DrawInfernoRings orig, Main self)
        {
            bool playerFound = false;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (!Main.player[i].active || Main.player[i].outOfRange || Main.player[i].dead)
                    continue;

                RoverDrivePlayer modPlayer = Main.player[i].GetModPlayer<RoverDrivePlayer>();
                bool forcedVisibility = !modPlayer.ShieldVisibility.HasValue ? false : modPlayer.ShieldVisibility.Value;

                // Skip if not forced, if it has a value (aka false since if it was true forced visibility would be true
                if (!forcedVisibility && (modPlayer.ShieldVisibility.HasValue || (modPlayer.ProtectionMatrixDurability <= 0)))
                    continue;

                float scale = 0.15f + 0.03f * (0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f + i * 0.2f));

                if (!playerFound)
                {
                    float shieldStrentgh = forcedVisibility ? 1f : (float)Math.Pow(Main.LocalPlayer.GetModPlayer<RoverDrivePlayer>().ProtectionMatrixDurability / (float)ProtectionMatrixDurabilityMax, 0.5f);
                    float noiseScale = MathHelper.Lerp(0.4f, 0.8f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.3f) * 0.5f + 0.5f);

                    Effect shieldEffect = Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
                    shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.24f);
                    shieldEffect.Parameters["blowUpPower"].SetValue(2.5f);
                    shieldEffect.Parameters["blowUpSize"].SetValue(0.5f);
                    shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

                    float baseShieldOpacity = 0.9f + 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f);
                    shieldEffect.Parameters["shieldOpacity"].SetValue(baseShieldOpacity * (0.5f + 0.5f * shieldStrentgh));
                    shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                    Color edgeColor;
                    Color shieldColor;

                    if (Main.netMode != NetmodeID.SinglePlayer && Main.player[i].team != 0)
                    {
                        switch (Main.player[i].team)
                        {
                            case 1:
                                shieldColor = new Color(178, 24, 31);
                                edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, Color.Tomato, Color.Crimson, shieldColor);
                                break;

                            case 2:
                                shieldColor = new Color(194, 255, 67) * 0.7f;
                                edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, Color.Chartreuse, Color.YellowGreen, new Color(194, 255, 67));
                                break;

                            case 3:
                                shieldColor = new Color(64, 207, 200);
                                edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, Color.MediumSpringGreen, Color.DeepSkyBlue, new Color(64, 207, 200));
                                break;

                            case 4:
                                shieldColor = new Color(176, 156, 45);
                                edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, Color.Gold, Color.Coral, Color.LightGoldenrodYellow);
                                break;

                            default:
                                shieldColor = new Color(173, 111, 221);
                                edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, Color.DeepPink, Color.MediumOrchid, Color.MediumPurple);
                                break;
                        }
                        
                    }
                    else
                    {
                        Color blueTint = new Color(51, 102, 255);
                        Color cyanTint = new Color(71, 202, 255);
                        Color wulfGreen = new Color(194, 255, 67) * 0.8f;
                        edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, blueTint, cyanTint, wulfGreen);
                        shieldColor = blueTint;
                    }

                    shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
                    shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);

                }

                playerFound = true;
                if (NoiseTex == null)
                    NoiseTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise");

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
            RoverDrivePlayer modPlayer = player.GetModPlayer<RoverDrivePlayer>();

            modPlayer.RoverDriveOn = true;
            modPlayer.ShieldVisibility = hideVisual ? false : null;

            if (modPlayer.ProtectionMatrixDurability > 0)
                player.statDefense += ProtectionMatrixDefenseBoost;
        }

        public override void UpdateVanity(Player player) => player.GetModPlayer<RoverDrivePlayer>().ShieldVisibility = true;

        // Scrappable for 3-6 wulfrum scrap or a 20% chance to get an energy core
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
    }

    public class RoverDrivePlayer : ModPlayer
    {
        public bool RoverDriveOn;
        public bool? ShieldVisibility; // Null for default, false for never, true for always
        public int ProtectionMatrixDurability = 0;

        public override void ResetEffects()
        {
            // Turn this into armor health when we can
            if (RoverDriveOn)
                Player.statLifeMax2 += ProtectionMatrixDurability;
            else
                ProtectionMatrixDurability = 0;

            RoverDriveOn = false;
            ShieldVisibility = false;
        }

        public override void PostHurt(Player.HurtInfo info)
        {
            if (RoverDriveOn)
            {
                if (ProtectionMatrixDurability > 0)
                {
                    ProtectionMatrixDurability -= info.Damage;
                    if (ProtectionMatrixDurability <= 0)
                    {
                        ProtectionMatrixDurability = 0;
                        SoundEngine.PlaySound(RoverDrive.BreakSound, Player.Center);
                        Player.Calamity().GeneralScreenShakePower += 2f;
                    }

                    int numParticles = Main.rand.Next(2, 6);
                    for (int i = 0; i < numParticles; i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(3, 14);
                        velocity.X += 5f * info.HitDirection;
                        GeneralParticleHandler.SpawnParticle(new TechyHoloysquareParticle(Player.Center, velocity, Main.rand.NextFloat(2.5f, 3f), Main.rand.NextBool() ? new Color(99, 255, 229) : new Color(25, 132, 247), 25));
                    }
                }

                if (Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveDurability.ID, out var cdDurability))
                    cdDurability.timeLeft = ProtectionMatrixDurability;

                // Reset recharge time
                if (Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveRecharge.ID, out var cd))
                    cd.timeLeft = RoverDrive.ProtectionMatrixRechargeTime;
            }
        }

        public override void UpdateDead() => ProtectionMatrixDurability = 0;

        public override void PostUpdateMiscEffects()
        {
            if (!RoverDriveOn)
            {
                if (Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveDurability.ID, out var cdDurability) && !RoverDriveOn)
                    cdDurability.timeLeft = 0;

                if (Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveRecharge.ID, out var cdRecharge) && !RoverDriveOn)
                    cdRecharge.timeLeft = 0;
            }
            
            else
            {
                if (ProtectionMatrixDurability == 0 && !Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveRecharge.ID, out var cd))
                    Player.AddCooldown(WulfrumRoverDriveRecharge.ID, RoverDrive.ProtectionMatrixRechargeTime);

                if (ProtectionMatrixDurability > 0 && !Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveDurability.ID, out cd))
                {
                    CooldownInstance durabilityCooldown = Player.AddCooldown(WulfrumRoverDriveDurability.ID, RoverDrive.ProtectionMatrixDurabilityMax);
                    durabilityCooldown.timeLeft = ProtectionMatrixDurability;

                    SoundEngine.PlaySound(RoverDrive.ActivationSound, Player.Center);
                }

                if (ProtectionMatrixDurability > 0)
                    Lighting.AddLight(Player.Center, Color.DeepSkyBlue.ToVector3() * 0.2f);
            }
        }
    }
}
