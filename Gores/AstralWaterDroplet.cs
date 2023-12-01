using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Terraria.DataStructures;

namespace CalamityMod.Gores
{
    public class AstralWaterDroplet : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.numFrames = 15;
            gore.behindTiles = true;
            gore.timeLeft = Gore.goreTime * 3;
        }

        public override bool Update(Gore gore)
        {
            if ((double)gore.position.Y < Main.worldSurface * 16.0 + 8.0)
            {
                gore.alpha = 0;
            }
            else
            {
                gore.alpha = 100;
            }
            int goreFrameDelay = 4;
            gore.frameCounter++;
            if (gore.frame <= 4)
            {
                int goreXPos = (int)(gore.position.X / 16f);
                int goreYPos = (int)(gore.position.Y / 16f) - 1;
                if (WorldGen.InWorld(goreXPos, goreYPos, 0) && !Main.tile[goreXPos, goreYPos].HasTile)
                {
                    gore.active = false;
                }
                if (gore.frame == 0)
                {
                    goreFrameDelay = 24 + Main.rand.Next(256);
                }
                if (gore.frame == 1)
                {
                    goreFrameDelay = 24 + Main.rand.Next(256);
                }
                if (gore.frame == 2)
                {
                    goreFrameDelay = 24 + Main.rand.Next(256);
                }
                if (gore.frame == 3)
                {
                    goreFrameDelay = 24 + Main.rand.Next(96);
                }
                if (gore.frame == 5)
                {
                    goreFrameDelay = 16 + Main.rand.Next(64);
                }
                if ((int)gore.frameCounter >= goreFrameDelay)
                {
                    gore.frameCounter = 0;
                    gore.frame += 1;
                    if (gore.frame == 5 && Main.netMode != NetmodeID.Server)
                    {
                        int astralWater = Gore.NewGore(new EntitySource_Misc("0"), gore.position, gore.velocity, gore.type, 1f);
                        Main.gore[astralWater].frame = 9;
                        Main.gore[astralWater].velocity *= 0f;
                    }
                }
            }
            else if (gore.frame <= 6)
            {
                goreFrameDelay = 8;
                if ((int)gore.frameCounter >= goreFrameDelay)
                {
                    gore.frameCounter = 0;
                    gore.frame += 1;
                    if (gore.frame == 7)
                    {
                        gore.active = false;
                    }
                }
            }
            else if (gore.frame <= 9)
            {
                goreFrameDelay = 6;
                gore.velocity.Y += 0.2f;
                if ((double)gore.velocity.Y < 0.5)
                {
                    gore.velocity.Y = 0.5f;
                }
                if (gore.velocity.Y > 12f)
                {
                    gore.velocity.Y = 12f;
                }
                if ((int)gore.frameCounter >= goreFrameDelay)
                {
                    gore.frameCounter = 0;
                    gore.frame += 1;
                }
                if (gore.frame > 9)
                {
                    gore.frame = 7;
                }
            }
            else
            {
                gore.velocity.Y += 0.1f;
                if ((int)gore.frameCounter >= goreFrameDelay)
                {
                    gore.frameCounter = 0;
                    gore.frame += 1;
                }
                gore.velocity *= 0f;
                if (gore.frame > 14)
                {
                    gore.active = false;
                }
            }

            Vector2 goreVelCheck = gore.velocity;
            gore.velocity = Collision.TileCollision(gore.position, gore.velocity, 16, 14, false, false, 1);
            if (gore.velocity != goreVelCheck)
            {
                if (gore.frame < 10)
                {
                    gore.frame = 10;
                    gore.frameCounter = 0;
                    if (gore.type != 716 && gore.type != 717 && gore.type != 943)
                    {
                        SoundEngine.PlaySound(SoundID.Drip, gore.position + Vector2.One * 8);
                    }
                }
            }
            else if (Collision.WetCollision(gore.position + gore.velocity, 16, 14))
            {
                if (gore.frame < 10)
                {
                    gore.frame = 10;
                    gore.frameCounter = 0;
                    if (gore.type != 716 && gore.type != 717 && gore.type != 943)
                    {
                        SoundEngine.PlaySound(SoundID.Drip, gore.position + Vector2.One * 8);
                    }
                    ((WaterShaderData)Filters.Scene["WaterDistortion"].GetShader()).QueueRipple(gore.position + new Vector2(8f, 8f), 1f, RippleShape.Square, 0f);
                }
                int goreTileX = (int)(gore.position.X + 8f) / 16;
                int goreTileY = (int)(gore.position.Y + 14f) / 16;
                if (Main.tile[goreTileX, goreTileY] != null && Main.tile[goreTileX, goreTileY].LiquidAmount > 0)
                {
                    gore.velocity *= 0f;
                    gore.position.Y = (float)(goreTileY * 16 - (int)(Main.tile[goreTileX, goreTileY].LiquidAmount / 16));
                }
            }

            return true;
        }
    }
}
