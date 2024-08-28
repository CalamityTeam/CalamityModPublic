using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Gores.WaterDroplet
{
    public abstract class LiquidDropletGore : ModGore
    {
        public virtual bool lavaDroplet => false;

        public virtual Vector3 lavaColor => new Vector3(0, 0, 0);

        public override void SetStaticDefaults()
        {
            ChildSafety.SafeGore[Type] = true;
        }

        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.numFrames = 15;
            gore.behindTiles = true;
            gore.timeLeft = Gore.goreTime * 3;
        }

        public override bool Update(Gore gore)
        {
            gore.alpha = gore.position.Y < (Main.worldSurface * 16.0) + 8.0
                ? 0
                : 100;

            int frameDuration = 4;
            gore.frameCounter += 1;
            if (gore.frame <= 4)
            {
                int tileX = (int)(gore.position.X / 16f);
                int tileY = (int)(gore.position.Y / 16f) - 1;
                if (WorldGen.InWorld(tileX, tileY) && !Main.tile[tileX, tileY].HasTile)
                {
                    gore.active = false;
                }

                if (gore.frame == 0 || gore.frame == 1 || gore.frame == 2)
                {
                    frameDuration = 24 + Main.rand.Next(256);
                }

                if (gore.frame == 3)
                {
                    frameDuration = 24 + Main.rand.Next(96);
                }

                if (gore.frameCounter >= frameDuration)
                {
                    gore.frameCounter = 0;
                    gore.frame += 1;
                    if (gore.frame == 5)
                    {
                        int droplet = Gore.NewGore(new EntitySource_Misc("0"), gore.position, gore.velocity, gore.type);
                        Main.gore[droplet].frame = 9;
                        Main.gore[droplet].velocity *= 0f;
                    }
                }
            }
            else if (gore.frame <= 6)
            {
                frameDuration = 8;
                if (gore.frameCounter >= frameDuration)
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
                frameDuration = 6;
                gore.velocity.Y += 0.2f;
                if (gore.velocity.Y < 0.5f)
                {
                    gore.velocity.Y = 0.5f;
                }

                if (gore.velocity.Y > 12f)
                {
                    gore.velocity.Y = 12f;
                }

                if (gore.frameCounter >= frameDuration)
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
                if (gore.frameCounter >= frameDuration)
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

            if (lavaDroplet)
            {
                float num24 = 1f;
                float num25 = 1f;
                float num26 = 1f;
                float num27 = 0.6f;
                num27 *= gore.frame switch
                {
                    0 => 0.1f,
                    1 => 0.2f,
                    2 => 0.3f,
                    3 => 0.4f,
                    4 => 0.5f,
                    5 => 0.4f,
                    6 => 0.2f,
                    7 => 0.5f,
                    8 => 0.5f,
                    9 => 0.5f,
                    10 => 0.5f,
                    11 => 0.4f,
                    12 => 0.3f,
                    13 => 0.2f,
                    14 => 0.1f,
                    _ => 0.0f,
                };
                num24 = (lavaColor.X / 4) * num27; //R
                num25 = (lavaColor.Y / 4) * num27; //G
                num26 = (lavaColor.Z / 4) * num27; //B
                Lighting.AddLight(gore.position + new Vector2(8f, 8f), num24, num25, num26);
            }

            Vector2 oldVelocity = gore.velocity;
            gore.velocity = Collision.TileCollision(gore.position, gore.velocity, 16, 14);
            if (gore.velocity != oldVelocity)
            {
                if (gore.frame < 10)
                {
                    gore.frame = 10;
                    gore.frameCounter = 0;
                    if (!lavaDroplet)
                    {
                        SoundEngine.PlaySound(SoundID.Drip, gore.position + new Vector2(8, 8));
                    }
                }
            }
            else if (Collision.WetCollision(gore.position + gore.velocity, 16, 14))
            {
                if (gore.frame < 10)
                {
                    gore.frame = 10;
                    gore.frameCounter = 0;
                    if (!lavaDroplet)
                    {
                        SoundEngine.PlaySound(SoundID.Drip, gore.position + new Vector2(8, 8));
                    }
                }

                int tileX = (int)(gore.position.X + 8f) / 16;
                int tileY = (int)(gore.position.Y + 14f) / 16;
                if (Main.tile[tileX, tileY] != null && Main.tile[tileX, tileY].LiquidAmount > 0)
                {
                    gore.velocity *= 0f;
                    gore.position.Y = (tileY * 16) - (Main.tile[tileX, tileY].LiquidAmount / 16);
                }
            }

            gore.position += gore.velocity;
            return false;
        }

        public override Color? GetAlpha(Gore gore, Color lightColor)
        {
            if (lavaDroplet)
            {
                return new Color(255, 255, 255, 200);
            }
            return null;
        }
    }
}
