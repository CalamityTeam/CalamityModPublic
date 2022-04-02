using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class HermitCrabMinion : ModProjectile
    {
        private int playerStill = 0;
        private bool fly = false;
        private bool spawnDust = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hermit Crab");
            Main.projFrames[projectile.type] = 9;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 36;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (spawnDust)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num501 = 20;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 33, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
                spawnDust = false;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            bool flag64 = projectile.type == ModContent.ProjectileType<HermitCrabMinion>();
            player.AddBuff(ModContent.BuffType<HermitCrab>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.hCrab = false;
                }
                if (modPlayer.hCrab)
                {
                    projectile.timeLeft = 2;
                }
            }
            Vector2 vector46 = projectile.position;
            if (!fly)
            {
                Vector2 center2 = projectile.Center;
                Vector2 vector48 = player.Center - center2;
                float playerDistance = vector48.Length();
                if (projectile.velocity.Y == 0 && (HoleBelow() || (playerDistance > 205f && projectile.position.X == projectile.oldPosition.X)))
                {
                    projectile.velocity.Y = -10f;
                }
                projectile.velocity.Y += 0.6f;
                if (projectile.velocity.X != 0f)
                {
                    projectile.frameCounter++;
                }
                else
                {
                    projectile.frame = 0;
                }
                if (projectile.frameCounter > 4)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 5)
                {
                    projectile.frame = 1;
                }
                float num633 = 600f;
                bool chaseNPC = false;
                float npcPositionX = 0f;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float num646 = Vector2.Distance(npc.Center, projectile.Center);
                        if (!chaseNPC && num646 < num633)
                        {
                            num633 = num646;
                            vector46 = npc.Center;
                            npcPositionX = npc.position.X;
                            chaseNPC = true;
                        }
                    }
                }
                if (!chaseNPC)
                {
                    for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                    {
                        NPC npcTarget = Main.npc[num645];
                        if (npcTarget.CanBeChasedBy(projectile, false))
                        {
                            float num646 = Vector2.Distance(npcTarget.Center, projectile.Center);
                            if (!chaseNPC && num646 < num633)
                            {
                                num633 = num646;
                                vector46 = npcTarget.Center;
                                npcPositionX = npcTarget.position.X;
                                chaseNPC = true;
                            }
                        }
                    }
                }
                if (chaseNPC)
                {
                    if (npcPositionX - projectile.position.X > 0f)
                    {
                        switch (Main.rand.Next(1, 2))
                        {

                            case 1:
                                projectile.velocity.X += 0.10f;
                                break;

                            case 2:
                                projectile.velocity.X += 0.15f;
                                break;
                        }

                        if (projectile.velocity.X > 9f)
                        {
                            projectile.velocity.X = 9f;
                        }
                    }
                    else
                    {
                        switch (Main.rand.Next(1, 2))
                        {

                            case 1:
                                projectile.velocity.X -= 0.10f;
                                break;

                            case 2:
                                projectile.velocity.X -= 0.15f;
                                break;
                        }

                        if (projectile.velocity.X < -9f)
                        {
                            projectile.velocity.X = -9f;
                        }
                    }
                    if (playerDistance > 1000f)
                    {
                        fly = true;
                        chaseNPC = false;
                        projectile.velocity.X = 0f;
                        projectile.velocity.Y = 0f;
                        projectile.tileCollide = false;
                    }
                }
                else
                {
                    if (playerDistance > 600f)
                    {
                        fly = true;
                        projectile.velocity.X = 0f;
                        projectile.velocity.Y = 0f;
                        projectile.tileCollide = false;
                    }
                    if (playerDistance > 200f)
                    {
                        if (player.position.X - projectile.position.X > 0f)
                        {
                            switch (Main.rand.Next(1, 3))
                            {
                                case 1:
                                    projectile.velocity.X += 0.05f;
                                    break;

                                case 2:
                                    projectile.velocity.X += 0.10f;
                                    break;

                                case 3:
                                    projectile.velocity.X += 0.15f;
                                    break;
                            }

                            if (projectile.velocity.X > 9f)
                            {
                                projectile.velocity.X = 9f;
                            }
                        }
                        else
                        {
                            switch (Main.rand.Next(1, 3))
                            {
                                case 1:
                                    projectile.velocity.X -= 0.05f;
                                    break;

                                case 2:
                                    projectile.velocity.X -= 0.10f;
                                    break;

                                case 3:
                                    projectile.velocity.X -= 0.15f;
                                    break;
                            }

                            if (projectile.velocity.X < -9f)
                            {
                                projectile.velocity.X = -9f;
                            }
                        }
                    }
                    if (playerDistance < 200f)
                    {
                        if (projectile.velocity.X != 0f)
                        {
                            if (projectile.velocity.X > 0.5f)
                            {
                                switch (Main.rand.Next(1, 3))
                                {
                                    case 1:
                                        projectile.velocity.X -= 0.05f;
                                        break;

                                    case 2:
                                        projectile.velocity.X -= 0.10f;
                                        break;

                                    case 3:
                                        projectile.velocity.X -= 0.15f;
                                        break;
                                }
                            }
                            else if (projectile.velocity.X < -0.5f)
                            {
                                switch (Main.rand.Next(1, 3))
                                {
                                    case 1:
                                        projectile.velocity.X += 0.05f;
                                        break;

                                    case 2:
                                        projectile.velocity.X += 0.10f;
                                        break;

                                    case 3:
                                        projectile.velocity.X += 0.15f;
                                        break;
                                }
                            }
                            else if (projectile.velocity.X < 0.5f && projectile.velocity.X > -0.5f)
                            {
                                projectile.velocity.X = 0f;
                            }
                        }
                    }
                }
            }
            else if (fly)
            {
                Vector2 center2 = projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(0f, 0f);
                float playerDistance = vector48.Length();
                vector48.Normalize();
                vector48 *= 8f;
                projectile.velocity = (projectile.velocity * 40f + vector48) / 41f;

                projectile.rotation = projectile.velocity.X * 0.03f;
                projectile.frameCounter++;
                if (projectile.frameCounter > 3)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame >= 9)
                {
                    projectile.frame = 5;
                }
                if (playerDistance > 2000f)
                {
                    projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (playerDistance < 100f)
                {
                    if (player.velocity.Y == 0f)
                    {
                        ++playerStill;
                    }
                    else
                    {
                        playerStill = 0;
                    }
                    if (playerStill > 30 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {
                        fly = false;
                        projectile.tileCollide = true;
                        projectile.rotation = 0;
                    }
                }
            }
            if (projectile.velocity.X > 0.25f)
            {
                projectile.spriteDirection = 1;
            }
            else if (projectile.velocity.X < -0.25f)
            {
                projectile.spriteDirection = -1;
            }
        }

        private bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(projectile.Center.X / 16f) - tileWidth;
            if (projectile.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((projectile.position.Y + projectile.height) / 16f);
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

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
