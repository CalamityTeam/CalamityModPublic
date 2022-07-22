using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using System;
using CalamityMod.Particles;
using ReLogic.Content;

namespace CalamityMod.Items.Accessories
{
    public class RoverDrive : ModItem
    {
        public static readonly SoundStyle ShieldHurtSound = new("CalamityMod/Sounds/Custom/RoverDriveHit") { PitchVariance = 0.6f, Volume = 0.6f, MaxInstances = 0 };
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Custom/RoverDriveActivate") { Volume = 0.85f };
        public static readonly SoundStyle BreakSound = new("CalamityMod/Sounds/Custom/RoverDriveBreak") { Volume = 0.75f };

        public static int ProtectionMatrixDurabilityMax = 50;
        public static int ProtectionMatrixRechargeTime = 60 * 10;
        public static int ProtectionMatrixDefenseBoost = 10;

        public static Asset<Texture2D> NoiseTex;


        public override void Load()
        {
            On.Terraria.Main.DrawInfernoRings += DrawRoverDriveShields;
        }
        public override void Unload()
        {
            On.Terraria.Main.DrawInfernoRings -= DrawRoverDriveShields;
        }


        private void DrawRoverDriveShields(On.Terraria.Main.orig_DrawInfernoRings orig, Main self)
        {
            bool playerFound = false;

            for (int i = 0; i < 255; i++)
            {
                if (!Main.player[i].active || Main.player[i].outOfRange || Main.player[i].dead)
                    continue;

                RoverDrivePlayer modPlayer = Main.player[i].GetModPlayer<RoverDrivePlayer>();
                if (!modPlayer.VisibleShield ||  modPlayer.ProtectionMatrixDurability <= 0)
                    continue;

                float scale = 0.15f + 0.03f * (0.5f + 0.5f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f + i * 0.2f));

                if (playerFound == false)
                {
                    float noiseScale = MathHelper.Lerp(0.4f, 0.8f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.3f) * 0.5f + 0.5f);

                    Effect shieldEffect = Filters.Scene["RoverDriveShield"].GetShader().Shader;
                    shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.24f);
                    shieldEffect.Parameters["blowUpPower"].SetValue(2.5f);
                    shieldEffect.Parameters["blowUpSize"].SetValue(0.5f);
                    shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);
                    //shieldEffect.Parameters["resolution"].SetValue(resolution);

                    float baseShieldOpacity = 0.9f + 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f);
                    shieldEffect.Parameters["shieldOpacity"].SetValue(baseShieldOpacity * (0.5f + 0.5f * (float)Math.Pow(Main.LocalPlayer.GetModPlayer<RoverDrivePlayer>().ProtectionMatrixDurability / (float)ProtectionMatrixDurabilityMax, 0.5f)));
                    shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

                    Color blueTint = new Color(51, 102, 255);
                    Color cyanTint = new Color(71, 202, 255);
                    Color wulfGreen = new Color(194, 255, 67) * 0.8f;
                    Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, blueTint, cyanTint, wulfGreen);
                    shieldEffect.Parameters["shieldColor"].SetValue(blueTint.ToVector3());
                    shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());


                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);

                }



                playerFound = true;
                Player myPlayer = Main.player[i];
                if (NoiseTex == null)
                    NoiseTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/TechyNoise");

                Texture2D tex = NoiseTex.Value;
                Vector2 pos = myPlayer.MountedCenter + myPlayer.gfxOffY * Vector2.UnitY - Main.screenPosition;

                Main.spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);
            }

            if (playerFound)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }

            orig(self);
        }


        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Rover Drive");
            Tooltip.SetDefault($"Activates a protective matrix that can absorb {ProtectionMatrixDurabilityMax} damage and grants {ProtectionMatrixDefenseBoost} defense\n" +
            $"However, the systems are fickle and the shield will need {ProtectionMatrixRechargeTime / 60} seconds to charge up fully\n" +
            "Getting hit during the shield recharge period will reset it back to zero\n" +
                "Can also be scrapped at an extractinator");

            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;

            //Needed for extractination
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.useAnimation = 10;
            Item.useTime = 2;
            Item.consumable = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            RoverDrivePlayer modPlayer = player.GetModPlayer<RoverDrivePlayer>();
            //modPlayer.roverDrive = true;

            modPlayer.RoverDriveOn = true;
            modPlayer.VisibleShield = !hideVisual;

            if (modPlayer.ProtectionMatrixDurability > 0)
                player.statDefense += ProtectionMatrixDefenseBoost;
        }

        //Scrappable for 3-6 wulfrum scrap or a 20% chance to get an energy core
        public override void ExtractinatorUse(ref int resultType, ref int resultStack)
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
        public bool VisibleShield;
        public int ProtectionMatrixDurability = 0;
        public int ProtectionMatrixCharge = 0;

        public override void ResetEffects()
        {
            //Turn this into armor health when we can
            if (RoverDriveOn)
                Player.statLifeMax2 += ProtectionMatrixDurability;

            else
                ProtectionMatrixDurability = 0;

            RoverDriveOn = false;
            VisibleShield = false;
        }

        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (RoverDriveOn)
            {
                if (ProtectionMatrixDurability > 0)
                {
                    ProtectionMatrixDurability -= (int)damage;
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
                        velocity.X += 5f * hitDirection;
                        GeneralParticleHandler.SpawnParticle(new TechyHoloysquareParticle(Player.Center, velocity, Main.rand.NextFloat(2.5f, 3f), Main.rand.NextBool() ? new Color(99, 255, 229) : new Color(25, 132, 247), 25));
                    }
                }

                if (Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveDurability.ID, out var cdDurability))
                {
                    cdDurability.timeLeft = ProtectionMatrixDurability;
                }


                //Reset recharge time.
                if (Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveRecharge.ID, out var cd))
                {
                    cd.timeLeft = RoverDrive.ProtectionMatrixRechargeTime;
                }
            }
        }

        public override void UpdateDead()
        {
            ProtectionMatrixDurability = 0;
        }


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
                {
                    Player.AddCooldown(WulfrumRoverDriveRecharge.ID, RoverDrive.ProtectionMatrixRechargeTime);
                }

                if (ProtectionMatrixDurability > 0 && !Player.Calamity().cooldowns.TryGetValue(WulfrumRoverDriveDurability.ID, out cd))
                {
                    CooldownInstance durabilityCooldown = Player.AddCooldown(WulfrumRoverDriveDurability.ID, RoverDrive.ProtectionMatrixDurabilityMax);
                    durabilityCooldown.timeLeft = ProtectionMatrixDurability;

                    SoundEngine.PlaySound(RoverDrive.ActivationSound, Player.Center);
                }

                if (ProtectionMatrixDurability > 0)
                {
                    Lighting.AddLight(Player.Center, Color.DeepSkyBlue.ToVector3() * 0.2f);
                }
            }
        }
    }
}
