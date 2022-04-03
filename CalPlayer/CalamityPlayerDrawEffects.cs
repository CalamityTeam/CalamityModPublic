using CalamityMod.Dusts;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AdultEidolonWyrm;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        #region Profaned Moonlight Colors
        public static readonly List<Color> MoonlightDyeDayColors = new List<Color>()
        {
            new Color(255, 163, 56),
            new Color(235, 30, 19),
            new Color(242, 48, 187),
        };
        public static readonly List<Color> MoonlightDyeNightColors = new List<Color>()
        {
            new Color(24, 134, 198),
            new Color(130, 40, 150),
            new Color(40, 64, 150),
        };

        public static void DetermineMoonlightDyeColors(out Color drawColor, Color dayColor, Color nightColor)
        {
            int totalTime = Main.dayTime ? 54000 : 32400;
            float transitionTime = 5400;
            float interval = Utils.InverseLerp(0f, transitionTime, (float)Main.time, true) + Utils.InverseLerp(totalTime - transitionTime, totalTime, (float)Main.time, true);
            if (Main.dayTime)
            {
                // Dusk.
                if (Main.time >= totalTime - transitionTime)
                    drawColor = Color.Lerp(dayColor, nightColor, Utils.InverseLerp(totalTime - transitionTime, totalTime, (float)Main.time, true));
                // Dawn.
                else if (Main.time <= transitionTime)
                    drawColor = Color.Lerp(nightColor, dayColor, interval);
                else
                    drawColor = dayColor;
            }
            else drawColor = nightColor;
        }

        public static Color GetCurrentMoonlightDyeColor(float angleOffset = 0f)
        {
            float interval = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.6f + angleOffset) * 0.5f + 0.5f;
            interval = MathHelper.Clamp(interval, 0f, 0.995f);
            Color dayColorToUse = CalamityUtils.MulticolorLerp(interval, MoonlightDyeDayColors.ToArray());
            Color nightColorToUse = CalamityUtils.MulticolorLerp(interval, MoonlightDyeNightColors.ToArray());
            DetermineMoonlightDyeColors(out Color drawColor, dayColorToUse, nightColorToUse);
            return drawColor;
        }
        #endregion

        #region Draw Layers

        public static readonly PlayerLayer MiscEffectsBack = new PlayerLayer("CalamityMod", "MiscEffectsBack", PlayerLayer.MiscEffectsBack, drawInfo =>
        {
            if (drawInfo.shadow != 0f)
                return;

            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();

            modPlayer.ProvidenceBurnEffectDrawer.DrawSet(drawPlayer.Bottom - Vector2.UnitY * 10f);
            modPlayer.ProvidenceBurnEffectDrawer.SpawnAreaCompactness = 18f;
            modPlayer.ProvidenceBurnEffectDrawer.RelativePower = 0.4f;

            if (modPlayer.sirenIce)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/IceShield");
                int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y);
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Color.White, 0f, texture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);
            }
            if (modPlayer.amidiasBlessing)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/AmidiasBubble");
                int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y); //4
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Color.White, 0f, texture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);
            }
        });

        public static readonly PlayerLayer Skin = new PlayerLayer("CalamityMod", "Skin", PlayerLayer.Skin, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (drawInfo.shadow != 0f || drawPlayer.dead)
            {
                return;
            }
        });

        public static readonly PlayerLayer GemTechGems = new PlayerLayer("CalamityMod", "GemTechGems", PlayerLayer.Skin, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (drawInfo.shadow != 0f || drawPlayer.dead || !modPlayer.GemTechSet || drawPlayer.Calamity().andromedaState != AndromedaPlayerState.Inactive)
                return;

            for (int i = 5; i >= 0; i--)
            {
                Texture2D gemTexture;
                float pulseFactor = 1.8f;
                GemTechArmorGemType gemType;
                switch (i)
                {
                    case 0:
                    default:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GemTechArmor/RedGem");
                        gemType = GemTechArmorGemType.Rogue;
                        break;
                    case 1:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GemTechArmor/YellowGem");
                        gemType = GemTechArmorGemType.Melee;
                        break;
                    case 2:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GemTechArmor/GreenGem");
                        gemType = GemTechArmorGemType.Ranged;
                        break;
                    case 3:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GemTechArmor/BlueGem");
                        gemType = GemTechArmorGemType.Summoner;
                        break;
                    case 4:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GemTechArmor/PurpleGem");
                        gemType = GemTechArmorGemType.Magic;
                        break;
                    case 5:
                        gemTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GemTechArmor/PinkGem");
                        gemType = GemTechArmorGemType.Base;
                        pulseFactor = 2.5f;
                        break;
                }

                // Don't bother doing anything else if the gem is inactive.
                if (!modPlayer.GemTechState.GemIsActive(gemType))
                    continue;

                float drawOffsetAngle = modPlayer.GemTechState.CalculateGemOffsetAngle(gemType);
                float gemOpacity = MathHelper.Lerp(0.85f, 1.05f, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 2.3f) * 0.5f + 0.5f);

                // Incorporate a sinusoidal pulse into the pulse factor.
                pulseFactor *= (float)Math.Cos(Main.GlobalTimeWrappedHourly * 0.61f + drawOffsetAngle);

                // Somewhat unorthodox way of creating an illusion of gems being drawn behind the player via orbiting.
                // Instead of actually messing with a Z position it simply fades away when the sine of the draw
                // offset angle is in the low negatives.
                gemOpacity *= Utils.InverseLerp(-0.75f, -0.51f, (float)Math.Sin(drawOffsetAngle), true);

                Vector2 baseDrawPosition = modPlayer.GemTechState.CalculateGemPosition(gemType) - Main.screenPosition;

                // Draw back afterimages.
                for (int j = 0; j < 5; j++)
                {
                    Color backAfterimageColor = Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.47f + j / 5f) % 1f, 1f, 0.67f);
                    backAfterimageColor = Color.Lerp(backAfterimageColor, Color.White, 0.64f) * gemOpacity * 0.24f;
                    Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * j / 5f).ToRotationVector2() * pulseFactor;
                    DrawData gemBackDrawData = new DrawData(gemTexture, drawPosition, null, backAfterimageColor, 0f, gemTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                    Main.playerDrawData.Add(gemBackDrawData);
                }

                // Draw the main gem.
                Color baseGemColor = Main.hslToRgb(Main.GlobalTimeWrappedHourly * 0.51f % 1f, 1f, 0.67f);
                baseGemColor = Color.Lerp(baseGemColor, Color.White, 0.56f);
                baseGemColor.A = 105;
                baseGemColor *= gemOpacity;
                DrawData gemDrawData = new DrawData(gemTexture, baseDrawPosition, null, baseGemColor, 0f, gemTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
                Main.playerDrawData.Add(gemDrawData);
            }
        });

        public static readonly PlayerLayer MiscEffectsFront = new PlayerLayer("CalamityMod", "MiscEffectsFront", PlayerLayer.MiscEffectsFront, drawInfo =>
        {
            if (drawInfo.shadow != 0f)
                return;
            Player drawPlayer = drawInfo.drawPlayer;
            Item currentlyHeldItem = drawPlayer.ActiveItem();

            // Go through the old positions for the player.
            for (int i = drawPlayer.Calamity().OldPositions.Length - 1; i > 0; i--)
            {
                if (drawPlayer.Calamity().OldPositions[i - 1] == Vector2.Zero)
                    drawPlayer.Calamity().OldPositions[i - 1] = drawPlayer.position;
                drawPlayer.Calamity().OldPositions[i] = drawPlayer.Calamity().OldPositions[i - 1];
            }
            drawPlayer.Calamity().OldPositions[0] = drawPlayer.position;

            // Kamei trail/afterimage effect.
            if (drawPlayer.Calamity().kamiBoost)
            {
                List<DrawData> existingDrawData = Main.playerDrawData;
                for (int i = 0; i < drawPlayer.Calamity().OldPositions.Length; i++)
                {
                    float completionRatio = i / (float)drawPlayer.Calamity().OldPositions.Length;
                    float scale = MathHelper.Lerp(1f, 0.5f, completionRatio);
                    float opacity = MathHelper.Lerp(0.25f, 0.08f, completionRatio);
                    List<DrawData> afterimages = new List<DrawData>();
                    for (int j = 0; j < existingDrawData.Count; j++)
                    {
                        var drawData = existingDrawData[j];
                        drawData.position = existingDrawData[j].position - drawPlayer.position + drawPlayer.oldPosition;
                        drawData.color = Color.Cyan * opacity;
                        drawData.color.G = (byte)(drawData.color.G * 1.6);
                        drawData.color.B = (byte)(drawData.color.B * 1.2);
                        drawData.scale = new Vector2(scale);
                        afterimages.Add(drawData);
                    }
                    Main.playerDrawData.InsertRange(0, afterimages);
                }
            }

            // Cobalt set trail/afterimage effect.
            if (drawPlayer.Calamity().CobaltSet)
            {
                List<DrawData> existingDrawData = Main.playerDrawData;
                for (int i = 0; i < drawPlayer.Calamity().OldPositions.Length; i++)
                {
                    float completionRatio = i / (float)drawPlayer.Calamity().OldPositions.Length;
                    float scale = MathHelper.Lerp(1f, 0.5f, completionRatio);
                    float opacity = MathHelper.Lerp(0.23f, 0.07f, completionRatio) * CobaltArmorSetChange.CalculateMovementSpeedInterpolant(drawPlayer);
                    List<DrawData> afterimages = new List<DrawData>();
                    for (int j = 0; j < existingDrawData.Count; j++)
                    {
                        var drawData = existingDrawData[j];
                        drawData.position = existingDrawData[j].position - drawPlayer.position + drawPlayer.oldPosition;
                        drawData.color = Color.Cyan * opacity;
                        drawData.color.G = (byte)(drawData.color.G * 0.87);
                        drawData.color.B = (byte)(drawData.color.B * 1.24);
                        drawData.scale = new Vector2(scale);
                        afterimages.Add(drawData);
                    }
                    Main.playerDrawData.InsertRange(0, afterimages);
                }
            }

            if (!drawPlayer.frozen &&
                currentlyHeldItem.type > ItemID.None &&
                !drawPlayer.dead &&
                !currentlyHeldItem.noUseGraphic &&
                (!drawPlayer.wet || !currentlyHeldItem.noWet))
            {
                SpriteEffects drawEffects;

                if (drawPlayer.direction == 1)
                    drawEffects = SpriteEffects.None;
                else drawEffects = SpriteEffects.FlipHorizontally;

                if (drawPlayer.gravDir != 1f)
                    drawEffects |= SpriteEffects.FlipVertically;

                if ((drawPlayer.itemAnimation > 0 && currentlyHeldItem.useStyle != 0) || (currentlyHeldItem.holdStyle > 0 && !drawPlayer.pulley))
                {
                    // Staffs.
                    if (currentlyHeldItem.type == ModContent.ItemType<DeathhailStaff>() ||
                        currentlyHeldItem.type == ModContent.ItemType<Vesuvius>() ||
                        currentlyHeldItem.type == ModContent.ItemType<SoulPiercer>() ||
                        currentlyHeldItem.type == ModContent.ItemType<FatesReveal>() ||
                        (currentlyHeldItem.type == ModContent.ItemType<PrismaticBreaker>() && currentlyHeldItem.useStyle == ItemUseStyleID.Shoot))
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/DeathhailStaffGlow");
                        if (currentlyHeldItem.type == ModContent.ItemType<Vesuvius>())
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/VesuviusGlow");
                        else if (currentlyHeldItem.type == ModContent.ItemType<SoulPiercer>())
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/SoulPiercerGlow");
                        else if (currentlyHeldItem.type == ModContent.ItemType<FatesReveal>())
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/FatesRevealGlow");
                        else if (currentlyHeldItem.type == ModContent.ItemType<PrismaticBreaker>())
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/PrismaticBreakerGlow");

                        float rotation = drawPlayer.itemRotation + MathHelper.PiOver4 * (float)drawPlayer.direction;
                        int xOffset = 0;
                        Vector2 origin = new Vector2(0f, Main.itemTexture[currentlyHeldItem.type].Height);

                        if (drawPlayer.gravDir == -1f)
                        {
                            if (drawPlayer.direction == -1)
                            {
                                rotation += MathHelper.PiOver2;
                                origin = new Vector2(Main.itemTexture[currentlyHeldItem.type].Width, 0f);
                                xOffset -= Main.itemTexture[currentlyHeldItem.type].Width;
                            }
                            else
                            {
                                rotation -= MathHelper.PiOver2;
                                origin = Vector2.Zero;
                            }
                        }
                        else if (drawPlayer.direction == -1)
                        {
                            origin = new Vector2(Main.itemTexture[currentlyHeldItem.type].Width, (float)Main.itemTexture[currentlyHeldItem.type].Height);
                            xOffset -= Main.itemTexture[currentlyHeldItem.type].Width;
                        }

                        DrawData data = new DrawData(texture,
                            new Vector2((int)(drawPlayer.itemLocation.X - Main.screenPosition.X + origin.X + xOffset), (int)(drawPlayer.itemLocation.Y - Main.screenPosition.Y)),
                            Main.itemTexture[currentlyHeldItem.type].Bounds,
                            Color.White,
                            rotation,
                            origin,
                            currentlyHeldItem.scale,
                            drawEffects,
                            0);

                        Main.playerDrawData.Add(data);
                    }

                    // Bow and Book.
                    else if (currentlyHeldItem.type == ModContent.ItemType<Deathwind>() ||
                             currentlyHeldItem.type == ModContent.ItemType<Apotheosis>() ||
                             currentlyHeldItem.type == ModContent.ItemType<CleansingBlaze>() ||
                             currentlyHeldItem.type == ModContent.ItemType<SubsumingVortex>() ||
                             currentlyHeldItem.type == ModContent.ItemType<AuroraBlazer>() ||
                             currentlyHeldItem.type == ModContent.ItemType<Auralis>())
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/DeathwindGlow");
                        int offsetX = 10;
                        if (currentlyHeldItem.type == ModContent.ItemType<Apotheosis>())
                        {
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/ApotheosisGlow");
                            offsetX = 6;
                        }
                        else if (currentlyHeldItem.type == ModContent.ItemType<CleansingBlaze>())
                        {
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/CleansingBlazeGlow");
                            offsetX = 37;
                        }
                        else if (currentlyHeldItem.type == ModContent.ItemType<SubsumingVortex>())
                        {
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/SubsumingVortexGlow");
                            offsetX = 9;
                        }
                        else if (currentlyHeldItem.type == ModContent.ItemType<AuroraBlazer>())
                        {
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/AuroraBlazerGlow");
                            offsetX = 44;
                        }
                        else if (currentlyHeldItem.type == ModContent.ItemType<Auralis>())
                        {
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/AuralisGlow");
                            offsetX = 62;
                        }

                        Vector2 center = Main.itemTexture[currentlyHeldItem.type].Size() * 0.5f;
                        int originOffsetX = (int)center.X - offsetX;

                        Vector2 origin = new Vector2(-originOffsetX, (float)(Main.itemTexture[currentlyHeldItem.type].Height / 2));
                        if (drawPlayer.direction == -1)
                            origin = new Vector2((float)(Main.itemTexture[currentlyHeldItem.type].Width + originOffsetX), (float)(Main.itemTexture[currentlyHeldItem.type].Height / 2));

                        DrawData data = new DrawData(texture,
                            new Vector2((int)(drawPlayer.itemLocation.X - Main.screenPosition.X + center.X), (int)(drawPlayer.itemLocation.Y - Main.screenPosition.Y + center.Y)),
                            Main.itemTexture[currentlyHeldItem.type].Bounds,
                            Color.White,
                            drawPlayer.itemRotation,
                            origin,
                            currentlyHeldItem.scale,
                            drawEffects,
                            0);

                        Main.playerDrawData.Add(data);
                    }

                    // Sword.
                    else if (currentlyHeldItem.type == ModContent.ItemType<Excelsus>() ||
                             currentlyHeldItem.type == ModContent.ItemType<EssenceFlayer>() ||
                             currentlyHeldItem.type == ModContent.ItemType<TheEnforcer>() ||
                             currentlyHeldItem.type == ModContent.ItemType<TerrorBlade>() ||
                             currentlyHeldItem.type == ModContent.ItemType<EtherealSubjugator>() ||
                             (currentlyHeldItem.type == ModContent.ItemType<PrismaticBreaker>() && currentlyHeldItem.useStyle == ItemUseStyleID.Swing))
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/ExcelsusGlow");
                        if (currentlyHeldItem.type == ModContent.ItemType<EssenceFlayer>())
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/EssenceFlayerGlow");
                        else if (currentlyHeldItem.type == ModContent.ItemType<TheEnforcer>())
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/TheEnforcerGlow");
                        else if (currentlyHeldItem.type == ModContent.ItemType<TerrorBlade>())
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/TerrorBladeGlow");
                        else if (currentlyHeldItem.type == ModContent.ItemType<EtherealSubjugator>())
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Summon/EtherealSubjugatorGlow");
                        else if (currentlyHeldItem.type == ModContent.ItemType<PrismaticBreaker>())
                            texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/PrismaticBreakerGlow");

                        float yOffset = drawPlayer.gravDir == -1f ? 0f : (float)Main.itemTexture[currentlyHeldItem.type].Height;

                        DrawData data = new DrawData(texture,
                            new Vector2((int)(drawPlayer.itemLocation.X - Main.screenPosition.X), (int)(drawPlayer.itemLocation.Y - Main.screenPosition.Y)),
                            Main.itemTexture[currentlyHeldItem.type].Bounds,
                            Color.White,
                            drawPlayer.itemRotation,
                            new Vector2((float)Main.itemTexture[currentlyHeldItem.type].Width * 0.5f - (float)Main.itemTexture[currentlyHeldItem.type].Width * 0.5f * drawPlayer.direction, yOffset) + Vector2.Zero,
                            currentlyHeldItem.scale,
                            drawEffects,
                            0);

                        Main.playerDrawData.Add(data);
                    }
                }
            }
        });

        public static readonly PlayerLayer clAfterAll = new PlayerLayer("Calamity", "clAfterAll", PlayerLayer.MiscEffectsFront, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (drawPlayer.mount != null && (modPlayer.fab || modPlayer.crysthamyr || modPlayer.onyxExcavator))
            {
                drawPlayer.mount.Draw(Main.playerDrawData, 3, drawPlayer, drawInfo.position, drawInfo.mountColor, drawInfo.spriteEffects, drawInfo.shadow);
            }
        });

        public static readonly PlayerLayer Tail = new PlayerLayer("CalamityMod", "Tail", PlayerLayer.BackAcc, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (drawInfo.shadow != 0f || drawPlayer.dead)
                return;

            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Armor/FathomSwarmerArmor_Tail");

            Rectangle frame = texture.Frame(1, 4, 0, modPlayer.tailFrame);
            if (modPlayer.fathomSwarmerTail)
            {
                int dyeShader = drawPlayer.dye?[2].dye ?? 0;
                int frameSizeY = texture.Height / 4;
                int centerX = (int)(drawInfo.position.X + drawPlayer.width / 2f);
                int centerY = (int)(drawInfo.position.Y + drawPlayer.height / 2f);
                int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X - (3 * drawPlayer.direction));
                int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y);
                DrawData tailDrawData = new DrawData(texture, new Vector2(drawX, drawY), frame, drawInfo.pantsColor, 0f, new Vector2(texture.Width / 2f, frameSizeY / 2f), 1f, drawInfo.spriteEffects, 0);
                tailDrawData.shader = dyeShader;
                Main.playerDrawData.Add(tailDrawData);
            }
        });

        public static readonly PlayerLayer DrawRancorBookManually = new PlayerLayer("CalamityMod", "RancorBook", PlayerLayer.Arms, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (drawInfo.shadow != 0f || drawPlayer.dead)
                return;

            int bookType = ModContent.ProjectileType<RancorHoldout>();
            Texture2D bookTexture = Main.projectileTexture[bookType];
            Projectile book = Main.projectile[drawPlayer.heldProj];
            Rectangle frame = bookTexture.Frame(1, Main.projFrames[bookType], 0, book.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = drawPlayer.Center + Vector2.UnitX * drawPlayer.direction * 8f - Main.screenPosition;
            Color drawColor = book.GetAlpha(Color.White);
            SpriteEffects direction = book.spriteDirection == 1f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            DrawData bookDrawData = new DrawData(bookTexture, drawPosition, frame, drawColor, book.rotation, origin, book.scale, direction, 0);
            Main.playerDrawData.Add(bookDrawData);
        });

        //I feel like having an Interface folder might be good.
        public interface IExtendedHat
        {
            /// <summary>
            /// The texture of the extension
            /// </summary>
            string ExtensionTexture { get;  }
            /// <summary>
            /// Unless you are using custom drawing, mount offsets are taken into account automatically.
            /// </summary>
            Vector2 ExtensionSpriteOffset(PlayerDrawInfo drawInfo);
            /// <summary>
            ///Return true to make the extension get drawn automatically from the texture and offsets provided. Return false if you want to draw it yourself
            /// </summary>
            bool PreDrawExtension(PlayerDrawInfo drawInfo);
        }

        public static readonly PlayerLayer HatExtension = new PlayerLayer("CalamityMod", "HatExtension", PlayerLayer.Head, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0f || drawPlayer.dead)
                return;

            int headItemType = drawPlayer.armor[0].type;
            if (drawPlayer.armor[10].type > ItemID.None)
                headItemType = drawPlayer.armor[10].type;

            if (ModContent.GetModItem(headItemType) is IExtendedHat ExtendedHatDrawer)
            {
                if (ExtendedHatDrawer.PreDrawExtension(drawInfo))
                {
                    int dyeShader = drawPlayer.dye?[0].dye ?? 0;

                    //Remember to use drawInfo.position and not drawPlayer.position, or else it will not display properly in the player selection screen.
                    Vector2 origin = drawInfo.headOrigin;
                    Vector2 headDrawPosition = drawInfo.position.Floor() + origin - Main.screenPosition;

                    //Account for the hellspawns known as mounts
                    if (drawPlayer.mount.Active)
                        headDrawPosition.Y += drawPlayer.mount.HeightBoost;

                    headDrawPosition += ExtendedHatDrawer.ExtensionSpriteOffset(drawInfo);

                    Texture2D extraPieceTexture = ModContent.Request<Texture2D>(ExtendedHatDrawer.ExtensionTexture);
                    Rectangle frame = extraPieceTexture.Frame(1, 20, 0, drawPlayer.bodyFrame.Y / drawPlayer.bodyFrame.Height);
                    DrawData pieceDrawData = new DrawData(extraPieceTexture, headDrawPosition, frame, drawInfo.upperArmorColor, drawPlayer.fullRotation, origin, 1f, drawInfo.spriteEffects, 0);
                    pieceDrawData.shader = dyeShader;
                    Main.playerDrawData.Add(pieceDrawData);
                }
            }
        });

        public static readonly PlayerLayer ForbiddenCircletSign = new PlayerLayer("CalamityMod", "ForbiddenSigil", PlayerLayer.BackAcc, drawInfo =>
        {
            DrawData drawData = new DrawData();
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (drawInfo.shadow != 0f || drawPlayer.dead || !modPlayer.forbiddenCirclet)
                return;

            SpriteEffects spriteEffects;
            if (drawPlayer.direction == 1)
                spriteEffects = SpriteEffects.None;
            else spriteEffects = SpriteEffects.FlipHorizontally;

            if (drawPlayer.gravDir != 1f)
                spriteEffects |= SpriteEffects.FlipVertically;

            int dyeShader = 0;
            if (drawPlayer.dye[1] != null)
                dyeShader = drawPlayer.dye[1].dye;

            Color baseColor = drawPlayer.GetImmuneAlphaPure(Lighting.GetColor((int)(drawInfo.position.X + drawPlayer.width * 0.5f) / 16, (int)(drawInfo.position.Y + drawPlayer.height * 0.5f) / 16, Color.White), drawInfo.shadow);
            Color color = Color.Lerp(baseColor, Color.White, 0.7f);
            Texture2D texture = Main.extraTexture[ExtrasID.ForbiddenSign];
            Texture2D glowmask = Main.glowMaskTexture[GlowMaskID.ForbiddenSign];
            float offsetY = (float)Math.Sin(drawPlayer.miscCounter / 300f * MathHelper.TwoPi) * 6f;
            float sinusoidalTime = (float)Math.Cos(drawPlayer.miscCounter / 75f * MathHelper.TwoPi);
            Color afterimageColor = new Color(80, 70, 40, 0) * (sinusoidalTime * 0.5f + 0.5f) * 0.8f;
            float gravCheckOffset = drawPlayer.gravDir != 1f ? -20f : 20f;

            Vector2 position = new Vector2(drawInfo.position.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2, drawInfo.position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f) + drawPlayer.bodyPosition;
            position += new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2) + new Vector2(-drawPlayer.direction * 10, offsetY - gravCheckOffset);
            position -= Main.screenPosition;

            // Draw the original sign-
            drawData = new DrawData(texture, position, null, color, drawPlayer.bodyRotation, texture.Size() * 0.5f, 1f, spriteEffects, 0)
            {
                shader = dyeShader
            };
            Main.playerDrawData.Add(drawData);

            // And 4 semi-transparent copies.
            for (float i = 0f; i < 4f; i++)
            {
                float angle = MathHelper.TwoPi / 4f * i;
                drawData = new DrawData(glowmask, position + angle.ToRotationVector2() * sinusoidalTime * 4f, null, afterimageColor, drawPlayer.bodyRotation, texture.Size() * 0.5f, 1f, spriteEffects, 0);
                Main.playerDrawData.Add(drawData);
            }
        });

        public static readonly PlayerLayer ColdDivinityOverlay = new PlayerLayer("CalamityMod", "ColdDivinity", PlayerLayer.Skin, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (modPlayer.coldDivinity)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ColdDivinityBody");
                int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y);
                SpriteEffects spriteEffects = drawPlayer.direction != -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, new Color(53, Main.DiscoG, 255) * 0.5f, 0f, texture.Size() * 0.5f, 1.15f, spriteEffects, 0);
                Main.playerDrawData.Add(data);
            }
        });

        public static readonly PlayerLayer ConcentratedVoidAura = new PlayerLayer("CalamityMod", "VoidAura", PlayerLayer.Skin, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (modPlayer.voidAura || modPlayer.voidAuraDamage)
            {
                Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VoidConcentrationAura");
                Vector2 drawPos = drawPlayer.Center - Main.screenPosition + new Vector2(0f, drawPlayer.gfxOffY);
                drawPos.Y -= 9;
                SpriteEffects spriteEffects = drawPlayer.direction != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None; //intentionally inverse due to giving more space for the player model without faffing about with the specific positioning
                Rectangle frame = tex.Frame(1, 4, 0, modPlayer.voidFrame);
                Vector2 origin = new Vector2(tex.Width / 2f, tex.Height / 2f / 4f);
                float scale = 1.75f;

                DrawData data = new DrawData(tex, drawPos, frame, Color.White * 0.4f, 0f, origin, scale, spriteEffects, 0);
                Main.playerDrawData.Add(data);
            }
        });

        public static readonly PlayerLayer RoverDriveShield = new PlayerLayer("CalamityMod", "RoverDrive", PlayerLayer.Skin, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (modPlayer.roverDriveTimer < 616 && modPlayer.roverDrive && !drawPlayer.dead)
            {
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/RoverAccShield");
                Vector2 drawPos = drawPlayer.Center - Main.screenPosition + new Vector2(0f, drawPlayer.gfxOffY);
                Rectangle frame = texture.Frame(1, 11, 0, modPlayer.roverFrame);
                Color color = Color.White * 0.625f;
                Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f / 11f);
                float scale = 1f + (float)Math.Cos(Main.GlobalTimeWrappedHourly) * 0.1f;
                SpriteEffects spriteEffects = drawPlayer.direction != -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                DrawData data = new DrawData(texture, drawPos, frame, color, 0f, origin, scale, spriteEffects, 0);
                Main.playerDrawData.Add(data);
            }
        });

        public static readonly PlayerLayer StratusSphereDrawing = new PlayerLayer("CalamityMod", "StratusSphereDrawing", PlayerLayer.HeldProjFront, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.inventory[drawPlayer.selectedItem].type == ModContent.ItemType<StratusSphere>())
            {
                SpriteEffects effect;
                if (drawPlayer.direction == 1)
                {
                    effect = SpriteEffects.None;
                }
                else
                {
                    effect = SpriteEffects.FlipHorizontally;
                }
                if (drawPlayer.gravDir != 1f)
                    effect |= SpriteEffects.FlipVertically;
                Vector2 itemDrawPosition = drawPlayer.Center;
                Texture2D drawTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/StratusSphereHold");
                Rectangle rectangle = drawTexture.Frame(1, 4, 0, (int)(2 * Math.Sin(drawPlayer.miscCounter / 20f * MathHelper.TwoPi) + 2));
                Vector2 drawOffset = new Vector2(rectangle.Width / 2 * drawPlayer.direction, 0f);
                Vector2 origin = rectangle.Size() / 2f;
                Main.playerDrawData.Add(new DrawData(drawTexture,
                                                     (itemDrawPosition - Main.screenPosition + drawOffset).Floor(),
                                                     new Rectangle?(rectangle),
                                                     Color.White,
                                                     drawPlayer.itemRotation,
                                                     origin,
                                                     drawPlayer.inventory[drawPlayer.selectedItem].scale,
                                                     effect,
                                                     0));
                drawTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/StratusSphereHoldGlow");
                Main.playerDrawData.Add(new DrawData(drawTexture,
                                                     (itemDrawPosition - Main.screenPosition + drawOffset).Floor(),
                                                     new Rectangle?(rectangle),
                                                     Color.White,
                                                     drawPlayer.itemRotation,
                                                     origin,
                                                     drawPlayer.inventory[drawPlayer.selectedItem].scale,
                                                     effect,
                                                     0));
            }
        });

        public static readonly PlayerLayer ProfanedMoonlightDyeEffects = new PlayerLayer("CalamityMod", "ProfanedMoonlight", PlayerLayer.Body, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            int totalMoonlightDyes = drawPlayer.dye.Count(dyeItem => dyeItem.type == ModContent.ItemType<ProfanedMoonlightDye>());
            if (totalMoonlightDyes <= 0)
                return;
            CalamityUtils.DrawAuroras(drawPlayer, 5 + (int)MathHelper.Clamp(totalMoonlightDyes, 0f, 4f) * 2, MathHelper.Clamp(totalMoonlightDyes / 3f, 0f, 1f), GetCurrentMoonlightDyeColor());
        });

        public static readonly PlayerLayer AuralisAuroraEffects = new PlayerLayer("CalamityMod", "AuralisAurora", PlayerLayer.Body, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (modPlayer.auralisAuroraCounter < 300 || modPlayer.auralisAuroraCooldown > 0)
                return;
            CalamityUtils.DrawAuroras(drawPlayer, 7, 0.4f, CalamityUtils.ColorSwap(Auralis.blueColor, Auralis.greenColor, 3f));
        });

        public static readonly PlayerLayer AngelicAllianceAurora = new PlayerLayer("CalamityMod", "AngelicAllianceAurora", PlayerLayer.Body, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (!modPlayer.divineBless)
                return;
            CalamityUtils.DrawAuroras(drawPlayer, 7, 0.4f, CalamityUtils.ColorSwap(new Color(255, 163, 56), new Color(242, 48, 187), 3f));
        });

        public static readonly PlayerLayer IbanDevRobot = new PlayerLayer("CalamityMod", "IbanDevRobot", PlayerLayer.Body, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawPlayer.Calamity().andromedaState == AndromedaPlayerState.Inactive)
                return;
            Main.playerDrawData.Clear();
            int robot = -1;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == ModContent.ProjectileType<GiantIbanRobotOfDoom>() &&
                    Main.projectile[i].owner == drawPlayer.whoAmI)
                {
                    robot = i;
                    break;
                }
            }
            if (robot == -1)
            {
                drawPlayer.Calamity().andromedaState = AndromedaPlayerState.Inactive;
                return;
            }

            GiantIbanRobotOfDoom robotEntityInstance = (GiantIbanRobotOfDoom)Main.projectile[robot].modProjectile;
            switch (drawPlayer.Calamity().andromedaState)
            {
                case AndromedaPlayerState.SpecialAttack:
                    Texture2D dashTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/AndromedaBolt");
                    Rectangle frame = dashTexture.Frame(1, 4, 0, robotEntityInstance.RightIconCooldown / 4 % 4);

                    DrawData drawData = new DrawData(dashTexture,
                                     drawPlayer.Center + new Vector2(0f, -8f) - Main.screenPosition,
                                     frame,
                                     Color.White,
                                     Main.projectile[robot].rotation,
                                     drawPlayer.Size / 2,
                                     1f,
                                     Main.projectile[robot].spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                                     1);
                    drawData.shader = drawPlayer.cBody;

                    Main.playerDrawData.Add(drawData);
                    break;
                case AndromedaPlayerState.LargeRobot:
                    Texture2D robotTexture = ModContent.Request<Texture2D>(robotEntityInstance.Texture);
                    frame = new Rectangle(robotEntityInstance.FrameX * robotTexture.Width / 3, robotEntityInstance.FrameY * robotTexture.Height / 7, robotTexture.Width / 3, robotTexture.Height / 7);

                    drawData = new DrawData(ModContent.Request<Texture2D>(Main.projectile[robot].modProjectile.Texture),
                                     Main.projectile[robot].Center + Vector2.UnitY * 6f - Main.screenPosition,
                                     frame,
                                     Color.White,
                                     Main.projectile[robot].rotation,
                                     Main.projectile[robot].Size / 2,
                                     1f,
                                     Main.projectile[robot].spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                                     1);
                    drawData.shader = drawPlayer.cBody;

                    Main.playerDrawData.Add(drawData);
                    break;
                case AndromedaPlayerState.SmallRobot:
                    robotTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AndromedaSmall");
                    frame = new Rectangle(0, robotEntityInstance.CurrentFrame * 54, robotTexture.Width, robotTexture.Height / 21);
                    drawData = new DrawData(robotTexture,
                                     drawPlayer.Center + new Vector2(drawPlayer.direction == 1 ? -24 : -10, -8f) - Main.screenPosition,
                                     frame,
                                     Color.White,
                                     0f,
                                     drawPlayer.Size / 2,
                                     1f,
                                     Main.projectile[robot].spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                                     1);
                    drawData.shader = drawPlayer.cBody;

                    Main.playerDrawData.Add(drawData);
                    break;
            }
        });

        public static readonly PlayerLayer DyeInvisibilityFix = new PlayerLayer("CalamityMod", "DyeInvisibilityFix", PlayerLayer.Arms, drawInfo =>
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            if (!drawPlayer.invis ||
                drawPlayer.itemAnimation > 0)
            {
                return;
            }
            for (int i = 0; i < Main.playerDrawData.Count; i++)
            {
                var copy = Main.playerDrawData[i];
                copy.shader = 0; // There's no other easy solution here to my knowledge since DrawData is a value type.
                Main.playerDrawData[i] = copy;
            }
        });

        #endregion

        #region Static Methods
        public static void AddPlayerLayer(List<PlayerLayer> list, PlayerLayer layer, PlayerLayer parent, bool first)
        {
            int insertAt = -1;
            for (int m = 0; m < list.Count; m++)
            {
                PlayerLayer currentLayer = list[m];
                if (currentLayer.Name.Equals(parent.Name))
                {
                    insertAt = m;
                    break;
                }
            }
            if (insertAt == -1)
                list.Add(layer);
            else list.Insert(first ? insertAt : insertAt + 1, layer);
        }
        #endregion

        #region Draw Hooks

        public override void ModifyDrawLayers(List<PlayerLayer> list)
        {
            MiscEffectsBack.visible = true;
            list.Insert(0, MiscEffectsBack);

            Skin.visible = true;
            list.Insert(list.IndexOf(PlayerLayer.Skin) + 1, Skin);

            MiscEffectsFront.visible = true;
            list.Add(MiscEffectsFront);

            // Remove shoe drawing effects if special legs are meant to be drawn.
            if (CalamityLists.legOverrideList.Contains(Player.legs))
                list[list.IndexOf(PlayerLayer.ShoeAcc)].visible = false;

            if (Player.Calamity().fab || Player.Calamity().crysthamyr || Player.Calamity().onyxExcavator)
                AddPlayerLayer(list, clAfterAll, list[list.Count - 1], false);

            if (Player.heldProj != -1 && Main.projectile[Player.heldProj].active && Main.projectile[Player.heldProj].type == ModContent.ProjectileType<RancorHoldout>())
                list.Insert(list.IndexOf(PlayerLayer.Arms), DrawRancorBookManually);

            if (Player.Calamity().fathomSwarmerTail)
            {
                int skinIndex = list.IndexOf(PlayerLayer.Skin);
                list.Insert(skinIndex - 1, Tail);
            }

            if (Player.Calamity().forbiddenCirclet)
            {
                int drawTheStupidSign = list.IndexOf(PlayerLayer.Skin);
                list.Insert(drawTheStupidSign, ForbiddenCircletSign);
            }

            list.Add(HatExtension);
            list.Add(ColdDivinityOverlay);
            list.Add(ConcentratedVoidAura);
            list.Add(RoverDriveShield);
            list.Add(StratusSphereDrawing);
            list.Add(ProfanedMoonlightDyeEffects);
            list.Add(AuralisAuroraEffects);
            list.Add(AngelicAllianceAurora);
            list.Add(IbanDevRobot);
            list.Add(DyeInvisibilityFix);
            list.Add(GemTechGems);
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            CalamityPlayer calamityPlayer = Player.Calamity();
            if (Main.myPlayer == Player.whoAmI && !Main.gameMenu && CalamityConfig.Instance.SpeedrunTimer)
            {
                string formatStr = @"hh\:mm\:ss\.ff";
                string formatStrDays = @"d\:hh\:mm\:ss\.ff";
                TimeSpan totalTime = CalamityMod.SpeedrunTimer.Elapsed.Add(calamityPlayer.previousSessionTotal);
                string text = totalTime.ToString(totalTime.Days > 0 ? formatStrDays : formatStr);
                float scale = 2f;
                float xOffset = CalamityConfig.Instance.SpeedrunTimerPosX;
                float yOffset = CalamityConfig.Instance.SpeedrunTimerPosY;

                Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, text, Main.screenWidth / 2f - xOffset, yOffset, Color.White, Color.Black, default, scale);

                if (calamityPlayer.lastSplitType > -1)
                {
                    TimeSpan split = calamityPlayer.lastSplit;
                    text = split.ToString(split.Days > 0 ? formatStrDays : formatStr);
                    scale = 1f;
                    yOffset += 44f;

                    Texture2D texture = null;
                    switch (calamityPlayer.lastSplitType)
                    {
                        // King Slime
                        case 1:
                            texture = Main.npcHeadBossTexture[7];
                            break;

                        case 2:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<DesertScourgeHead>()]];
                            break;

                        // Eye of Cthulhu
                        case 3:
                            texture = Main.npcHeadBossTexture[1];
                            break;

                        case 4:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<CrabulonIdle>()]];
                            break;

                        // Eater of Worlds
                        case 5:
                            texture = Main.npcHeadBossTexture[2];
                            break;

                        // Brain of Cthulhu
                        case 6:
                            texture = Main.npcHeadBossTexture[23];
                            break;

                        case 7:
                            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/HiveMind/HiveMindP2_Head_Boss");
                            break;

                        case 8:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<PerforatorHive>()]];
                            break;

                        // Queen Bee
                        case 9:
                            texture = Main.npcHeadBossTexture[14];
                            break;

                        // Skeletron
                        case 10:
                            texture = Main.npcHeadBossTexture[19];
                            break;

                        case 11:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<SlimeGodCore>()]];
                            break;

                        // Wall of Flesh
                        case 12:
                            texture = Main.npcHeadBossTexture[22];
                            break;

                        case 13:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Cryogen>()]];
                            break;

                        // The Twins
                        case 14:
                            texture = Main.npcHeadBossTexture[21];
                            break;

                        case 15:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AquaticScourgeHead>()]];
                            break;

                        // The Destroyer
                        case 16:
                            texture = Main.npcHeadBossTexture[25];
                            break;

                        case 17:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<BrimstoneElemental>()]];
                            break;

                        // Skeletron Prime
                        case 18:
                            texture = Main.npcHeadBossTexture[18];
                            break;

                        case 19:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<CalamitasRun3>()]];
                            break;

                        // Plantera
                        case 20:
                            texture = Main.npcHeadBossTexture[12];
                            break;

                        case 21:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Leviathan>()]];
                            break;

                        case 22:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AstrumAureus>()]];
                            break;

                        // Golem
                        case 23:
                            texture = Main.npcHeadBossTexture[5];
                            break;

                        case 24:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<PlaguebringerGoliath>()]];
                            break;

                        // Duke Fishron
                        case 25:
                            texture = Main.npcHeadBossTexture[4];
                            break;

                        case 26:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<RavagerBody>()]];
                            break;

                        // Lunatic Cultist
                        case 27:
                            texture = Main.npcHeadBossTexture[31];
                            break;

                        case 28:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AstrumDeusHeadSpectral>()]];
                            break;

                        // Moon Lord
                        case 29:
                            texture = Main.npcHeadBossTexture[8];
                            break;

                        case 30:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<ProfanedGuardianBoss>()]];
                            break;

                        case 31:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Bumblefuck>()]];
                            break;

                        case 32:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Providence>()]];
                            break;

                        case 33:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<CeaselessVoid>()]];
                            break;

                        case 34:
                            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/StormWeaver/StormWeaverHeadNaked_Head_Boss");
                            break;

                        case 35:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Signus>()]];
                            break;

                        case 36:
                            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Polterghast/Necroplasm_Head_Boss");
                            break;

                        case 37:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<OldDuke>()]];
                            break;

                        case 38:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<DevourerofGodsHead>()]];
                            break;

                        case 39:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<Yharon>()]];
                            break;

                        case 40:
                            texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/HoodlessHeadIcon");
                            break;

                        case 41:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<AresBody>()]];
                            break;

                        case 42:
                            texture = Main.npcHeadBossTexture[NPCID.Sets.BossHeadTextures[ModContent.NPCType<EidolonWyrmHeadHuge>()]];
                            break;

                        default:
                            break;
                    }

                    xOffset -= 58f;

                    if (texture != null)
                        Main.spriteBatch.Draw(texture, new Vector2(Main.screenWidth / 2f - xOffset - texture.Width - 4f, yOffset), null, Color.White, 0f, default, 1f, SpriteEffects.None, 0f);

                    Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, text, Main.screenWidth / 2f - xOffset, yOffset, Color.White, Color.Black, default, scale);
                }
            }

            // Dust modifications while high.
            if (calamityPlayer.trippy)
            {
                if (Main.myPlayer == Player.whoAmI)
                {
                    Rectangle screenArea = new Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 50, Main.screenWidth + 1000, Main.screenHeight + 100);
                    int dustDrawn = 0;
                    float maxShroomDust = Main.maxDustToDraw / 2;
                    for (int i = 0; i < Main.maxDustToDraw; i++)
                    {
                        Dust dust = Main.dust[i];
                        if (dust.active)
                        {
                            // Only draw dust near the screen, for performance reasons.
                            if (new Rectangle((int)dust.position.X, (int)dust.position.Y, 4, 4).Intersects(screenArea))
                            {
                                dust.color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
                                for (int j = 0; j < 4; j++)
                                {
                                    Vector2 dustDrawPosition = dust.position;
                                    Vector2 dustCenter = dustDrawPosition + new Vector2(4f);

                                    float distanceX = Math.Abs(dustCenter.X - Player.Center.X);
                                    float distanceY = Math.Abs(dustCenter.Y - Player.Center.Y);
                                    if (j == 0 || j == 2)
                                        dustDrawPosition.X = Player.Center.X + distanceX;
                                    else dustDrawPosition.X = Player.Center.X - distanceX;

                                    dustDrawPosition.X -= 4f;

                                    if (j == 0 || j == 1)
                                        dustDrawPosition.Y = Player.Center.Y + distanceY;

                                    else dustDrawPosition.Y = Player.Center.Y - distanceY;

                                    dustDrawPosition.Y -= 4f;
                                    Main.spriteBatch.Draw(Main.dustTexture, dustDrawPosition - Main.screenPosition, dust.frame, dust.color, dust.rotation, new Vector2(4f), dust.scale, SpriteEffects.None, 0f);
                                    dustDrawn++;
                                }

                                // Break if too many dust clones have been drawn
                                if (dustDrawn > maxShroomDust)
                                    break;
                            }
                        }
                    }
                }
            }

            bool noRogueStealth = calamityPlayer.rogueStealth == 0f || Player.townNPCs > 2f || !CalamityConfig.Instance.StealthInvisbility;
            if (calamityPlayer.rogueStealth > 0f && calamityPlayer.rogueStealthMax > 0f && Player.townNPCs < 3f && CalamityConfig.Instance.StealthInvisbility)
            {
                // A translucent orchid color, the rogue class color
                float colorValue = calamityPlayer.rogueStealth / calamityPlayer.rogueStealthMax * 0.9f; //0 to 0.9
                r *= 1f - (colorValue * 0.89f); //255 to 50
                g *= 1f - colorValue; //255 to 25
                b *= 1f - (colorValue * 0.89f); //255 to 50
                a *= 1f - colorValue; //255 to 25
                Player.armorEffectDrawOutlines = false;
                Player.armorEffectDrawShadow = false;
                Player.armorEffectDrawShadowSubtle = false;
            }

            Texture2D heart3 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Heart3");
            Texture2D heart4 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Heart4");
            Texture2D heart5 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Heart5");
            Texture2D heart6 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Heart6");
            Texture2D heartOriginal = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/HeartOriginal"); // Life fruit
            Texture2D heartOriginal2 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/HeartOriginal2"); // Life crystal

            int totalFruit =
                (calamityPlayer.mFruit ? 1 : 0) +
                (calamityPlayer.bOrange ? 1 : 0) +
                (calamityPlayer.eBerry ? 1 : 0) +
                (calamityPlayer.dFruit ? 1 : 0);

            switch (totalFruit)
            {
                default:
                    Main.heart2Texture = heartOriginal;
                    break;
                case 4:
                    Main.heart2Texture = heart6;
                    break;
                case 3:
                    Main.heart2Texture = heart5;
                    break;
                case 2:
                    Main.heart2Texture = heart4;
                    break;
                case 1:
                    Main.heart2Texture = heart3;
                    break;
            }

            Main.heartTexture = heartOriginal2;
            if (calamityPlayer.revivify)
            {
                if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 91, Player.velocity.X * 0.2f, Player.velocity.Y * 0.2f, 100, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
            }
            if (calamityPlayer.tRegen)
            {
                if (Main.rand.NextBool(10) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 107, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.Y -= 0.35f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.025f;
                    g *= 0.15f;
                    b *= 0.035f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.IBoots)
            {
                if (!Player.StandingStill() && !Player.mount.Active)
                {
                    if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 229, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        Main.playerDrawDust.Add(dust);
                    }
                    if (noRogueStealth)
                    {
                        r *= 0.05f;
                        g *= 0.05f;
                        b *= 0.05f;
                        fullBright = true;
                    }
                }
            }
            if (calamityPlayer.elysianFire)
            {
                if (!Player.StandingStill() && !Player.mount.Active)
                {
                    if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 246, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        Main.playerDrawDust.Add(dust);
                    }
                    if (noRogueStealth)
                    {
                        r *= 0.75f;
                        g *= 0.55f;
                        b *= 0f;
                        fullBright = true;
                    }
                }
            }
            if (calamityPlayer.dsSetBonus)
            {
                if (!Player.StandingStill() && !Player.mount.Active)
                {
                    if (Main.rand.NextBool(2) && drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 27, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        Main.playerDrawDust.Add(dust);
                    }
                    if (noRogueStealth)
                    {
                        r *= 0.15f;
                        g *= 0.025f;
                        b *= 0.1f;
                        fullBright = true;
                    }
                }
            }
            if (calamityPlayer.auricSet)
            {
                if (!Player.StandingStill() && !Player.mount.Active)
                {
                    if (drawInfo.shadow == 0f)
                    {
                        int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, Main.rand.NextBool(2) ? 57 : 244, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.5f;
                        Main.playerDrawDust.Add(dust);
                    }
                    if (noRogueStealth)
                    {
                        r *= 0.15f;
                        g *= 0.025f;
                        b *= 0.1f;
                        fullBright = true;
                    }
                }
            }
            if (calamityPlayer.bFlames || calamityPlayer.aFlames || calamityPlayer.rageModeActive)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, ModContent.DustType<BrimstoneFlame>(), Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.shadowflame)
            {
                if (Main.rand.Next(5) < 4 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 27, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.95f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
                    Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
            }
            if (calamityPlayer.sulphurPoison)
            {
                if (Main.rand.Next(5) < 4 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 46, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 1.95f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.X = Main.dust[dust].velocity.X * 0.75f;
                    Main.dust[dust].velocity.Y = Main.dust[dust].velocity.Y - 1f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                }
                if (noRogueStealth)
                {
                    r *= 0.65f;
                    b *= 0.75f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.adrenalineModeActive)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 206, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.01f;
                    g *= 0.15f;
                    b *= 0.1f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.gsInferno)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 173, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.astralInfection)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dustType = Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, dustType, Player.velocity.X * 0.2f, Player.velocity.Y * 0.2f, 100, default, 0.7f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].color = new Color(255, 255, 255, 0);
                    Main.playerDrawDust.Add(dust);
                }
            }
            if (calamityPlayer.hFlames || calamityPlayer.hInferno || calamityPlayer.banishingFire)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, ModContent.DustType<HolyFireDust>(), Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.25f;
                    b *= 0.1f;
                    fullBright = true;
                }
            }
            else if (calamityPlayer.eGravity || calamityPlayer.dragonFire)
            {
                if (Main.rand.NextBool(calamityPlayer.dragonFire ? 6 : 12) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, (int)CalamityDusts.ProfanedFire, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
            }
            if (calamityPlayer.pFlames)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 89, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.2f;
                    Main.dust[dust].velocity.Y -= 0.15f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.07f;
                    g *= 0.15f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.nightwither)
            {
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 176, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.25f;
                    b *= 0.1f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.vaporfied)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    246,
                    242,
                    229,
                    226,
                    247,
                    187,
                    234
                });
                if (Main.rand.NextBool(4) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, dustType, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        Main.dust[dust].noGravity = false;
                        Main.dust[dust].scale *= 0.5f;
                    }
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.25f;
                    g *= 0.25f;
                    b *= 0.1f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.eFreeze || calamityPlayer.gState || calamityPlayer.cDepth || calamityPlayer.eutrophication)
            {
                if (noRogueStealth)
                {
                    r *= 0f;
                    g *= 0.05f;
                    b *= 0.3f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.draedonsHeart && !calamityPlayer.shadeRegen && !calamityPlayer.cFreeze && Player.StandingStill() && Player.itemAnimation == 0)
            {
                if (noRogueStealth)
                {
                    r *= 0f;
                    g *= 0.5f;
                    b *= 0f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.bBlood)
            {
                if (Main.rand.NextBool(6) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 5, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.15f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.mushy)
            {
                if (Main.rand.NextBool(6) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 56, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.5f;
                    Main.dust[dust].velocity.Y -= 0.1f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.15f;
                    g *= 0.01f;
                    b *= 0.01f;
                    fullBright = true;
                }
            }
            if (calamityPlayer.bloodfinBoost)
            {
                if (Main.rand.NextBool(6) && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f), Player.width + 4, Player.height + 4, 5, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                if (noRogueStealth)
                {
                    r *= 0.5f;
                    g *= 0f;
                    b *= 0f;
                    fullBright = true;
                }
            }
            if ((calamityPlayer.cadence || calamityPlayer.ladHearts > 0) && !Player.loveStruck)
            {
                if (Main.rand.NextBool(5) && drawInfo.shadow == 0f)
                {
                    Vector2 velocity = Main.rand.NextVector2Unit();
                    velocity.X *= 0.66f;
                    velocity *= Main.rand.NextFloat(1f, 2f);

                    int heart = Gore.NewGore(drawInfo.position + new Vector2(Main.rand.Next(Player.width + 1), Main.rand.Next(Player.height + 1)), velocity, 331, Main.rand.NextFloat(0.4f, 1.2f));
                    Main.gore[heart].sticky = false;
                    Main.gore[heart].velocity *= 0.4f;
                    Main.gore[heart].velocity.Y -= 0.6f;
                    Main.playerDrawGore.Add(heart);
                }
            }
        }
        #endregion

        #region Tanks/Backpacks
        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player drawPlayer = drawInfo.drawPlayer;
            Item item = drawPlayer.ActiveItem();

            if (!drawPlayer.frozen &&
                (item.IsAir || item.type > ItemID.None) &&
                !drawPlayer.dead &&
                (!drawPlayer.wet || !item.noWet) &&
                (drawPlayer.wings == 0 || drawPlayer.velocity.Y == 0f))
            {
                //Make sure the lists are in the same order
                List<int> tankItems = new List<int>()
                {
                    ModContent.ItemType<FlurrystormCannon>(),
                    ModContent.ItemType<MepheticSprayer>(),
                    ModContent.ItemType<BrimstoneFlameblaster>(),
                    ModContent.ItemType<BrimstoneFlamesprayer>(),
                    ModContent.ItemType<SparkSpreader>(),
                    ModContent.ItemType<HalleysInferno>(),
                    ModContent.ItemType<CleansingBlaze>(),
                    ModContent.ItemType<ElementalEruption>(),
                    ModContent.ItemType<TheEmpyrean>(),
                    ModContent.ItemType<Meowthrower>(),
                    ModContent.ItemType<OverloadedBlaster>(),
                    ModContent.ItemType<TerraFlameburster>(),
                    ModContent.ItemType<Photoviscerator>(),
                    ModContent.ItemType<Shadethrower>(),
                    ModContent.ItemType<BloodBoiler>(),
                    ModContent.ItemType<PristineFury>(),
                    ModContent.ItemType<AuroraBlazer>()
                };
                List<Texture2D> tankTextures = new List<Texture2D>()
                {
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_FlurrystormCannon"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_BlightSpewer"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_BrimstoneFlameblaster"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_HavocsBreath"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_SparkSpreader"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_HalleysInferno"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_CleansingBlaze"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_ElementalEruption"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_TheEmpyrean"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_Meowthrower"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_OverloadedBlaster"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_TerraFlameburster"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_Photoviscerator"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_Shadethrower"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_BloodBoiler"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_PristineFury"),
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Tanks/Backpack_AuroraBlazer")
                };
                if (tankItems.Contains(item.type) || drawPlayer.Calamity().plaguebringerCarapace)
                {
                    Texture2D thingToDraw = null;
                    if (tankItems.Contains(item.type))
                    {
                        for (int i = 0; i < tankItems.Count; i++)
                        {
                            if (item.type == tankItems[i])
                            {
                                thingToDraw = tankTextures[i];
                                break;
                            }
                        }
                    }
                    else if (drawPlayer.Calamity().plaguebringerCarapace)
                        thingToDraw = ModContent.Request<Texture2D>("CalamityMod/Items/Armor/PlaguebringerCarapace_Back");

                    if (thingToDraw is null)
                        return;

                    SpriteEffects spriteEffects = Player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    float num25 = -4f;
                    float num24 = -8f;
                    DrawData howDoIDrawThings = new DrawData(thingToDraw,
                        new Vector2((int)(drawPlayer.position.X - Main.screenPosition.X + (drawPlayer.width / 2) - (9 * drawPlayer.direction)) + num25 * drawPlayer.direction, (int)(drawPlayer.position.Y - Main.screenPosition.Y + (drawPlayer.height / 2) + 2f * drawPlayer.gravDir + num24 * drawPlayer.gravDir)),
                        new Rectangle(0, 0, thingToDraw.Width, thingToDraw.Height),
                        drawInfo.middleArmorColor,
                        drawPlayer.bodyRotation,
                        new Vector2(thingToDraw.Width / 2, thingToDraw.Height / 2),
                        1f,
                        spriteEffects,
                        0);
                    howDoIDrawThings.shader = 0;
                    Main.playerDrawData.Add(howDoIDrawThings);
                }
            }
        }
        #endregion
    }
}
