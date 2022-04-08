using CalamityMod.Buffs.Mounts;
using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts.Minecarts
{
    public class DoGCartMount : ModMount
    {
        public const int SegmentCount = 18;
        public override void SetStaticDefaults()
        {
            MountData.Minecart = true;
            MountData.delegations = new();
            MountData.delegations.MinecartDust = CreateSparkDust;
            MountID.Sets.Cart[ModContent.MountType<DoGCartMount>()] = true;

            MountData.spawnDust = 173;
            MountData.buff = ModContent.BuffType<DoGCartBuff>();

            // Movement fields.
            MountData.flightTimeMax = 0;
            MountData.fallDamage = 1f;

            // Mechanical Cart in vanilla uses 20f and 0.1f respectively for these fields.
            MountData.runSpeed = 22f;
            MountData.acceleration = 0.16f;

            MountData.jumpHeight = 16;
            MountData.jumpSpeed = 5.2f;
            MountData.blockExtraJumps = true;
            MountData.heightBoost = 12;

            // Drawing fields.
            MountData.playerYOffsets = new int[] { 6, 6, 6 };
            MountData.xOffset = 2;
            MountData.yOffset = 12;
            MountData.bodyFrame = 3;

            // Animation fields.
            MountData.totalFrames = 3;
            MountData.standingFrameCount = 1;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = 3;
            MountData.runningFrameDelay = 12;
            MountData.runningFrameStart = 0;
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 0;
            MountData.inAirFrameCount = 0;
            MountData.inAirFrameDelay = 0;
            MountData.inAirFrameStart = 0;
            MountData.idleFrameCount = 1;
            MountData.idleFrameDelay = 10;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = false;
            if (Main.netMode != NetmodeID.Server)
            {
                MountData.textureWidth = 74;
                MountData.textureHeight = 114;
            }
        }

        public static void CreateSparkDust(Vector2 dustPosition)
        {
            Vector2 offsetDirection = DelegateMethods.Minecart.rotation.ToRotationVector2();
            offsetDirection *= new Vector2(Main.rand.NextBool().ToDirectionInt(), 1f) * 13f;
            dustPosition += offsetDirection;

            Dust spark = Dust.NewDustPerfect(dustPosition, 234);
            spark.velocity = Main.rand.NextVector2Circular(4f, 4f);
            spark.velocity.X *= Main.rand.NextFloat(0.25f, 1f);
            spark.velocity.Y -= Main.rand.NextFloat(1.25f, 3f);
            spark.position.Y -= 4f;
            spark.scale = Main.rand.NextFloat(0.9f, 1.4f);
            spark.noGravity = true;

            if (Main.rand.NextBool(8))
                spark.scale *= 1.5f;
            if (Main.rand.NextBool(8))
                spark.scale *= 0.667f;

            if (Main.rand.NextBool(3))
                spark.scale *= 0.65f;
            else
                spark.noGravity = false;
        }

        public static float CalculateIdealWormRotation(Player player)
        {
            int direction = (player.velocity.SafeNormalize(Vector2.UnitX * player.direction).X > 0f).ToDirectionInt();
            if (player.velocity.X == 0f)
                direction = player.direction;
            return (direction == 1 ? 0f : MathHelper.Pi) - DelegateMethods.Minecart.rotation * 0.1f;
        }

        public static Vector2 CalculateSegmentWaveOffset(int index, Player player)
        {
            Vector2 baseDirection = (CalculateIdealWormRotation(player) - MathHelper.PiOver2 + DelegateMethods.Minecart.rotation).ToRotationVector2();
            Vector2 waveOffset = baseDirection * (float)Math.Sin(MathHelper.Pi * index / player.Calamity().DoGCartSegments.Length + Main.GameUpdateCount / 14f) * 5f;
            waveOffset *= Utils.GetLerpValue(10f, 4f, player.velocity.Length(), true);

            return waveOffset;
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            if (drawPlayer.Calamity().DoGCartSegments is null || drawPlayer.Calamity().DoGCartSegments[0] is null)
                return true;

            // Account for stupid rotational issues.
            List<Vector2> rotationAdjustedPositions = new List<Vector2>()
            {
                drawPlayer.Calamity().DoGCartSegments[0].Center
            };

            for (int i = 1; i < drawPlayer.Calamity().DoGCartSegments.Length; i++)
            {
                rotationAdjustedPositions.Add(drawPlayer.Center - (drawPlayer.Center - drawPlayer.Calamity().DoGCartSegments[i].Center).RotatedBy(-drawPlayer.Calamity().SmoothenedMinecartRotation));
            }

            for (int i = 0; i < SegmentCount; i++)
            {
                DoGCartSegment segment = drawPlayer.Calamity().DoGCartSegments[i];
                if (segment is null)
                    break;

                Texture2D segmentTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/DoGCartBody").Value;
                if (i == SegmentCount - 1)
                    segmentTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/DoGCartTail").Value;
                Vector2 segmentDrawPosition = rotationAdjustedPositions[i] + CalculateSegmentWaveOffset(i, drawPlayer);

                Color segmentColor = Lighting.GetColor((int)segmentDrawPosition.X / 16, (int)segmentDrawPosition.Y / 16);
                Vector2 origin = segmentTexture.Size() * 0.5f;
                float segmentRotation = segment.Rotation - DelegateMethods.Minecart.rotation;
                DrawData segmentDrawData = new DrawData(segmentTexture, segmentDrawPosition - Main.screenPosition, null, segmentColor, segmentRotation, origin, Vector2.One, SpriteEffects.None, 0)
                {
                    shader = Mount.currentShader
                };

                playerDrawData.Add(segmentDrawData);
            }
            return true;
        }
    }
}
