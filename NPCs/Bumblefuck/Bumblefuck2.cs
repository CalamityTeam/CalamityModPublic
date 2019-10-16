using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.NPCs
{
    public class Bumblefuck2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bumblebirb");
            Main.npcFrameCount[npc.type] = 10;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 1f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 110;
            npc.width = 80;
            npc.height = 80;
            npc.scale = 0.66f;
            npc.defense = 20;
			npc.LifeMaxNERD(25000, 30000, 35000, 60000, 65000);
            npc.knockBackResist = 0f;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[ModContent.BuffType<ExoFreeze>()] = false;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.canGhostHeal = false;
            npc.HitSound = SoundID.NPCHit51;
            npc.DeathSound = SoundID.NPCDeath46;
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];
            Vector2 vector = npc.Center;
            npc.damage = npc.defDamage;
            if (Vector2.Distance(player.Center, vector) > 5600f)
            {
                if (npc.timeLeft > 5)
                {
                    npc.timeLeft = 5;
                }
            }
            npc.noTileCollide = false;
            npc.noGravity = true;
            npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.05f) / 10f;
            if (npc.ai[0] == 0f || npc.ai[0] == 1f)
            {
                int num;
                for (int num1376 = 0; num1376 < 200; num1376 = num + 1)
                {
                    if (num1376 != npc.whoAmI && Main.npc[num1376].active && Main.npc[num1376].type == npc.type)
                    {
                        Vector2 value42 = Main.npc[num1376].Center - npc.Center;
                        if (value42.Length() < (float)(npc.width + npc.height))
                        {
                            value42.Normalize();
                            value42 *= -0.1f;
                            npc.velocity += value42;
                            NPC nPC6 = Main.npc[num1376];
                            nPC6.velocity -= value42;
                        }
                    }
                    num = num1376;
                }
            }
            if (npc.target < 0 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(true);
                Vector2 vector240 = Main.player[npc.target].Center - npc.Center;
                if (Main.player[npc.target].dead || vector240.Length() > 5600f)
                {
                    npc.ai[0] = -1f;
                }
            }
            else
            {
                Vector2 vector241 = Main.player[npc.target].Center - npc.Center;
                if (npc.ai[0] > 1f && vector241.Length() > 3600f)
                {
                    npc.ai[0] = 1f;
                }
            }
            if (npc.ai[0] == -1f)
            {
                Vector2 value43 = new Vector2(0f, -8f);
                npc.velocity = (npc.velocity * 21f + value43) / 10f;
                npc.noTileCollide = true;
                npc.dontTakeDamage = true;
                return;
            }
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                npc.spriteDirection = npc.direction;
                if (npc.collideX)
                {
                    npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);
                    if (npc.velocity.X > 4f)
                    {
                        npc.velocity.X = 4f;
                    }
                    if (npc.velocity.X < -4f)
                    {
                        npc.velocity.X = -4f;
                    }
                }
                if (npc.collideY)
                {
                    npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);
                    if (npc.velocity.Y > 4f)
                    {
                        npc.velocity.Y = 4f;
                    }
                    if (npc.velocity.Y < -4f)
                    {
                        npc.velocity.Y = -4f;
                    }
                }
                Vector2 value44 = Main.player[npc.target].Center - npc.Center;
                if (value44.Length() > 2800f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                }
                else if (value44.Length() > 400f)
                {
                    float scaleFactor20 = 9f + value44.Length() / 100f + npc.ai[1] / 15f;
                    float num1377 = 30f;
                    value44.Normalize();
                    value44 *= scaleFactor20;
                    npc.velocity = (npc.velocity * (num1377 - 1f) + value44) / num1377;
                }
                else if (npc.velocity.Length() > 2f)
                {
                    npc.velocity *= 0.95f;
                }
                else if (npc.velocity.Length() < 1f)
                {
                    npc.velocity *= 1.05f;
                }
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 90f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 2f;
                }
            }
            else
            {
                if (npc.ai[0] == 1f)
                {
                    npc.collideX = false;
                    npc.collideY = false;
                    npc.noTileCollide = true;
                    if (npc.target < 0 || !Main.player[npc.target].active || Main.player[npc.target].dead)
                    {
                        npc.TargetClosest(true);
                    }
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.04f) / 10f;
                    Vector2 value45 = Main.player[npc.target].Center - npc.Center;
                    if (value45.Length() < 800f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                    npc.ai[2] += 0.0166666675f;
                    float scaleFactor21 = 12f + npc.ai[2] + value45.Length() / 150f;
                    float num1378 = 25f;
                    value45.Normalize();
                    value45 *= scaleFactor21;
                    npc.velocity = (npc.velocity * (num1378 - 1f) + value45) / num1378;
                    return;
                }
                if (npc.ai[0] == 2f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * 7f + npc.velocity.X * 0.05f) / 8f;
                    npc.noTileCollide = true;
                    Vector2 vector242 = Main.player[npc.target].Center - npc.Center;
                    vector242.Y -= 8f;
                    float scaleFactor22 = 18f;
                    float num1379 = 8f;
                    vector242.Normalize();
                    vector242 *= scaleFactor22;
                    npc.velocity = (npc.velocity * (num1379 - 1f) + vector242) / num1379;
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 10f)
                    {
                        npc.velocity = vector242;
                        if (npc.velocity.X < 0f)
                        {
                            npc.direction = -1;
                        }
                        else
                        {
                            npc.direction = 1;
                        }
                        npc.ai[0] = 2.1f;
                        npc.ai[1] = 0f;
                    }
                }
                else if (npc.ai[0] == 2.1f)
                {
                    npc.damage = (int)((double)npc.defDamage * 1.5);
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.velocity *= 1.01f;
                    npc.noTileCollide = true;
                    npc.ai[1] += 1f;
                    int num1380 = 30;
                    if (npc.ai[1] > (float)num1380)
                    {
                        if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                        {
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            return;
                        }
                        if (npc.ai[1] > (float)(num1380 * 2))
                        {
                            npc.ai[0] = 1f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                    }
                }
            }
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += (double)(npc.velocity.Length() / 4f);
            npc.frameCounter += 1.0;
            if (npc.ai[0] < 4f)
            {
                if (npc.frameCounter >= 6.0)
                {
                    npc.frameCounter = 0.0;
                    npc.frame.Y = npc.frame.Y + frameHeight;
                }
                if (npc.frame.Y / frameHeight > 4)
                {
                    npc.frame.Y = 0;
                }
            }
            else
            {
                if (npc.frameCounter >= 6.0)
                {
                    npc.frameCounter = 0.0;
                    npc.frame.Y = npc.frame.Y + frameHeight;
                }
                if (npc.frame.Y / frameHeight > 9)
                {
                    npc.frame.Y = frameHeight * 5;
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
