using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstralBiomeNPCs
{
    public class Mantis : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mantis");
            Main.npcFrameCount[npc.type] = 14;
            if (!Main.dedServ)
                glowmask = mod.GetTexture("NPCs/AstralBiomeNPCs/MantisGlow");
        }

        public override void SetDefaults()
        {
            npc.damage = 85;
            npc.width = 60;
            npc.height = 58;
            npc.aiStyle = -1;
            npc.defense = 16;
            npc.lifeMax = 510;
            npc.knockBackResist = 0.1f;
            npc.value = 450f;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyDeath");
        }

        public override void AI()
        {
            npc.TargetClosest(false);

            Player target = Main.player[npc.target];

            if (npc.ai[0] == 0f)
            {
                float acceleration = 0.045f;
                float maxSpeed = 6.8f;
                if (npc.Center.X > target.Center.X)
                {
                    npc.velocity.X -= acceleration;
                    if (npc.velocity.X > 0)
                    {
                        npc.velocity.X -= acceleration;
                    }
                    if (npc.velocity.X < -maxSpeed)
                    {
                        npc.velocity.X = -maxSpeed;
                    }
                }
                else
                {
                    npc.velocity.X += acceleration;
                    if (npc.velocity.X < 0)
                    {
                        npc.velocity.X += acceleration;
                    }
                    if (npc.velocity.X > maxSpeed)
                    {
                        npc.velocity.X = maxSpeed;
                    }
                }

                //if need to jump
                if (npc.velocity.Y == 0 && (HoleBelow() || (npc.collideX && npc.position.X == npc.oldPosition.X)))
                {
                    npc.velocity.Y = -5f;
                }

                //check if we can shoot at target.
                Vector2 vector = npc.Center - target.Center;
                if (Math.Abs(vector.Y) < 64 && Math.Abs(vector.X) < 540 && Collision.CanHit(npc.position, npc.width, npc.height, target.position, target.width, target.height))
                {
                    npc.ai[1]++;
                    if (npc.ai[1] > 120)
                    {
                        //fire projectile
                        npc.ai[0] = 1f;
                        npc.ai[1] = npc.ai[2] = 0f;
                        npc.frame.Y = 400; 
                        npc.frameCounter = 0;
                    }
                }
                else
                {
                    npc.ai[1] -= 0.5f;
                }

                if (npc.justHit)
                {
                    npc.ai[1] -= 60f;
                }


                if (npc.ai[1] < 0) npc.ai[1] = 0f;
            }
            else
            {
                npc.ai[2]++;
                npc.velocity.X *= 0.95f;
                if (npc.ai[2] == 25f)
                {
                    Vector2 vector = Main.player[npc.target].Center - npc.Center;
                    vector.Normalize();
                    Projectile.NewProjectile(npc.Center + (npc.Center.X < target.Center.X ? -14f : 14f) * Vector2.UnitX, vector * 7f, mod.ProjectileType("MantisRing"), 76, 0.5f);
                }
            }

            npc.direction = npc.Center.X > target.Center.X ? 0 : 1;
            npc.spriteDirection = npc.direction;
        }

        private bool HoleBelow()
        {
            //width of npc in tiles
            int tileWidth = 4;
            int tileX = (int)(npc.Center.X / 16f) - tileWidth;
            if (npc.velocity.X > 0) //if moving right
            {
                tileX += tileWidth;
            }
            int tileY = (int)((npc.position.Y + npc.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].active())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[0] == 0f)
            {
                if (npc.velocity.Y != 0)
                {
                    npc.frame.Y = frameHeight * 13;
                    npc.frameCounter = 20;
                }
                else
                {
                    npc.frameCounter += 0.8f + Math.Abs(npc.velocity.X) * 0.5f;
                    if (npc.frameCounter > 10.0)
                    {
                        npc.frameCounter = 0;
                        npc.frame.Y += frameHeight;
                        if (npc.frame.Y > frameHeight * 5)
                        {
                            npc.frame.Y = 0;
                        }
                    }
                }
            }
            else
            {
                npc.frameCounter++;
                if (npc.frameCounter > 4)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y >= frameHeight * 13)
                    {
                        npc.frame.Y = 0;
                        npc.frameCounter = 0;
                        npc.ai[0] = 0f;
                    }
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/AstralEnemyHit" + Main.rand.Next(3)), npc.Center);

            CalamityGlobalNPC.DoHitDust(npc, hitDirection, mod.DustType("AstralOrange"), 1f, 4, 24);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //draw glowmask
            spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition - new Vector2(0, 8), npc.frame, Color.White * 0.6f, npc.rotation, new Vector2(70, 40), 1f, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>().ZoneAstral && !spawnInfo.player.ZoneRockLayerHeight)
            {
                return 0.16f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Stardust"), Main.rand.Next(2, 4));
            if (Main.expertMode)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Stardust"));
            }
        }
    }
}
