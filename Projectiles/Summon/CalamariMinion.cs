using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class CalamariMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamari");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.owner < 0 || Projectile.owner >= Main.maxPlayers)
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            // Spawn dust on frame 1
            if (Projectile.localAI[1] == 0f)
            {
                int dustAmt = 36;
                for (int i = 0; i < dustAmt; i++)
                {
                    Vector2 spawnPos = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    spawnPos = spawnPos.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 velocity = spawnPos - Projectile.Center;
                    int idx = Dust.NewDust(spawnPos + velocity, 0, 0, 109, velocity.X * 1.5f, velocity.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].noLight = true;
                    Main.dust[idx].velocity = velocity;
                }
                Projectile.localAI[1] += 1f;
            }

            // Update frames
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
            {
                Projectile.frame = 0;
            }

            // Update buffs and player bools
            bool isMinion = Projectile.type == ModContent.ProjectileType<CalamariMinion>();
            player.AddBuff(ModContent.BuffType<Calamari>(), 3600);
            if (isMinion)
            {
                if (player.dead)
                {
                    modPlayer.calamari = false;
                }
                if (modPlayer.calamari)
                {
                    Projectile.timeLeft = 2;
                }
            }

            // Randomly make noise
            if (Main.rand.NextBool(600))
            {
                SoundEngine.PlaySound(SoundID.Zombie35, Projectile.position);
            }

            if (Projectile.ai[0] == 2f)
            {
                Projectile.ai[1] -= 1f;
                if (Projectile.ai[1] > 3f)
                {
                    SoundEngine.PlaySound(SoundID.Zombie34, Projectile.position);
                    int num = Dust.NewDust(Projectile.Center, 0, 0, 109, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1.4f);
                    Main.dust[num].scale = 0.5f + (float)Main.rand.NextDouble() * 0.3f;
                    Main.dust[num].velocity /= 2.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].noLight = true;
                }
                if (Projectile.ai[1] != 0f)
                {
                    return;
                }
                Projectile.ai[1] = 15f;
                Projectile.ai[0] = 0f;
                Projectile.velocity /= 5f;
                Projectile.velocity.Y = 0f;
                Projectile.extraUpdates = 0;
                Projectile.numUpdates = 0;
                Projectile.netUpdate = true;
                Projectile.extraUpdates = 0;
                Projectile.numUpdates = 0;
            }
            if (Projectile.extraUpdates > 1)
            {
                Projectile.extraUpdates = 0;
            }
            if (Projectile.numUpdates > 1)
            {
                Projectile.numUpdates = 0;
            }
            if (Projectile.localAI[0] > 0f)
            {
                Projectile.localAI[0] -= 1f;
            }

            // Prevent clumping
            Projectile.MinionAntiClump();

            // Find a target
            Vector2 shootPosition = Projectile.position;
            float range = 3500f;
            bool foundTarget = false;
            Vector2 center = player.Center;
            Vector2 half = new Vector2(0.5f);
            half.Y = 0f;
            int targetIndex = -1;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    Vector2 npcPos = npc.position + npc.Size * half;
                    float npcDist = Vector2.Distance(npcPos, center);
                    if (!foundTarget && npcDist < range && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                    {
                        shootPosition = npcPos;
                        foundTarget = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }
            if (!foundTarget)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        Vector2 npcPos = npc.position + npc.Size * half;
                        float npcDist = Vector2.Distance(npcPos, center);
                        if (!foundTarget && npcDist < range && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            range = npcDist;
                            shootPosition = npcPos;
                            foundTarget = true;
                            targetIndex = k;
                        }
                    }
                }
            }

            // Return to the player when far away
            int returnDist = 3500;
            if (foundTarget)
            {
                returnDist = 4000;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > returnDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }

            // Position towards the target
            if (foundTarget && Projectile.ai[0] == 0f)
            {
                Vector2 shootDir = shootPosition - Projectile.Center;
                shootDir.Normalize();
                shootDir = shootPosition - Vector2.UnitY * 80f;
                int tileY = (int)shootDir.Y / 16;
                if (tileY < 0)
                {
                    tileY = 0;
                }
                Tile tile = Main.tile[(int)shootDir.X / 16, tileY];
                if (tile != null && tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                {
                    shootDir += Vector2.UnitY * 16f;
                    tile = Main.tile[(int)shootDir.X / 16, (int)shootDir.Y / 16];
                    if (tile != null && tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType])
                    {
                        shootDir += Vector2.UnitY * 16f;
                    }
                }
                shootDir -= Projectile.Center;
                float distance = shootDir.Length();
                shootDir.Normalize();
                if (distance > 300f && distance <= 1600f && Projectile.localAI[0] == 0f)
                {
                    Projectile.ai[0] = 2f;
                    Projectile.ai[1] = (int)(distance / 10f);
                    Projectile.extraUpdates = (int)(Projectile.ai[1] * 2f) + 1;
                    Projectile.velocity = shootDir * 20f;
                    Projectile.localAI[0] = 60f;
                    return;
                }
                if (distance > 200f)
                {
                    float scaleFactor2 = 20f;
                    shootDir *= scaleFactor2;
                    Projectile.velocity.X = (Projectile.velocity.X * 30f + shootDir.X) / 31f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 30f + shootDir.Y) / 31f;
                }
                if (distance > 70f && distance < 130f)
                {
                    float scaleFactor3 = 18f;
                    if (distance < 100f)
                    {
                        scaleFactor3 = -7.5f;
                    }
                    shootDir *= scaleFactor3;
                    Projectile.velocity = (Projectile.velocity * 15f + shootDir) / 16f;
                    if (Math.Abs(shootDir.X) > Math.Abs(shootDir.Y))
                    {
                        Projectile.velocity.X = (Projectile.velocity.X * 6f + shootDir.X) / 7f;
                    }
                }
                else
                {
                    Projectile.velocity *= 0.99f;
                }
            }

            // Idle Behavior
            else
            {
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, Main.player[Projectile.owner].Center, 1, 1))
                {
                    Projectile.ai[0] = 1f;
                }
                float returnSpd = 12f; //6
                if (Projectile.ai[0] == 1f)
                {
                    returnSpd = 18f; //15
                }
                Vector2 returnPos = player.Center - Projectile.Center + new Vector2(0f, -60f);
                float returnPosDist = returnPos.Length();
                if (returnPosDist > 200f && returnSpd < 13.5f)
                {
                    returnSpd = 13.5f; //9
                }
                if (returnPosDist < 800f && Projectile.ai[0] == 1f)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (returnPosDist > 4000f)
                {
                    Projectile.position.X = player.Center.X - (Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (Projectile.width / 2);
                }
                if (returnPosDist > 70f)
                {
                    returnPos.Normalize();
                    returnPos *= returnSpd;
                    Projectile.velocity = (Projectile.velocity * 20f + returnPos) / 21f;
                }
                else
                {
                    if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                    {
                        Projectile.velocity.X = -0.15f;
                        Projectile.velocity.Y = -0.05f;
                    }
                    Projectile.velocity *= 1.01f;
                }
            }

            // Update rotation
            Projectile.rotation = Projectile.velocity.X * 0.025f;

            // Update shoot cooldown
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += 1f;
                if (Main.rand.Next(2) != 0)
                {
                    Projectile.ai[1] += 1f;
                }
            }
            if (Projectile.ai[1] > 20f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            // Shoot projectiles
            if (Projectile.ai[0] == 0f)
            {
                float inkShootSpeed = 15f;
                int projID = ModContent.ProjectileType<CalamariInk>();
                if (foundTarget)
                {
                    if (Math.Abs((shootPosition - Projectile.Center).ToRotation() - MathHelper.PiOver2) > MathHelper.PiOver4)
                    {
                        Projectile.velocity += (shootPosition - Projectile.Center - Vector2.UnitY * 100f).SafeNormalize(Vector2.Zero) * 2f;
                        return;
                    }
                    if ((shootPosition - Projectile.Center).Length() <= 600f && Projectile.ai[1] == 0f)
                    {
                        Projectile.ai[1] += 1f;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            SoundEngine.PlaySound(SoundID.Item111, Projectile.position);
                            Vector2 inkShootVelocity = Projectile.SafeDirectionTo(shootPosition, Vector2.UnitY) * inkShootSpeed;
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Vector2.UnitY * 20f, inkShootVelocity, projID, Projectile.damage, 0f, Main.myPlayer, targetIndex, 0f);
                            if (Main.projectile.IndexInRange(p))
                                Main.projectile[p].originalDamage = Projectile.originalDamage;
                            Projectile.netUpdate = true;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int num214 = texture2D13.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
