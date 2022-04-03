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
            Main.projFrames[Projectile.type] = 9;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 36;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (spawnDust)
            {
                Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
                int num501 = 20;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 33, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
                spawnDust = false;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                Projectile.damage = damage2;
            }
            bool flag64 = Projectile.type == ModContent.ProjectileType<HermitCrabMinion>();
            player.AddBuff(ModContent.BuffType<HermitCrab>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.hCrab = false;
                }
                if (modPlayer.hCrab)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Vector2 vector46 = Projectile.position;
            if (!fly)
            {
                Vector2 center2 = Projectile.Center;
                Vector2 vector48 = player.Center - center2;
                float playerDistance = vector48.Length();
                if (Projectile.velocity.Y == 0 && (HoleBelow() || (playerDistance > 205f && Projectile.position.X == Projectile.oldPosition.X)))
                {
                    Projectile.velocity.Y = -10f;
                }
                Projectile.velocity.Y += 0.6f;
                if (Projectile.velocity.X != 0f)
                {
                    Projectile.frameCounter++;
                }
                else
                {
                    Projectile.frame = 0;
                }
                if (Projectile.frameCounter > 4)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 1;
                }
                float num633 = 600f;
                bool chaseNPC = false;
                float npcPositionX = 0f;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float num646 = Vector2.Distance(npc.Center, Projectile.Center);
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
                        if (npcTarget.CanBeChasedBy(Projectile, false))
                        {
                            float num646 = Vector2.Distance(npcTarget.Center, Projectile.Center);
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
                    if (npcPositionX - Projectile.position.X > 0f)
                    {
                        switch (Main.rand.Next(1, 2))
                        {

                            case 1:
                                Projectile.velocity.X += 0.10f;
                                break;

                            case 2:
                                Projectile.velocity.X += 0.15f;
                                break;
                        }

                        if (Projectile.velocity.X > 9f)
                        {
                            Projectile.velocity.X = 9f;
                        }
                    }
                    else
                    {
                        switch (Main.rand.Next(1, 2))
                        {

                            case 1:
                                Projectile.velocity.X -= 0.10f;
                                break;

                            case 2:
                                Projectile.velocity.X -= 0.15f;
                                break;
                        }

                        if (Projectile.velocity.X < -9f)
                        {
                            Projectile.velocity.X = -9f;
                        }
                    }
                    if (playerDistance > 1000f)
                    {
                        fly = true;
                        chaseNPC = false;
                        Projectile.velocity.X = 0f;
                        Projectile.velocity.Y = 0f;
                        Projectile.tileCollide = false;
                    }
                }
                else
                {
                    if (playerDistance > 600f)
                    {
                        fly = true;
                        Projectile.velocity.X = 0f;
                        Projectile.velocity.Y = 0f;
                        Projectile.tileCollide = false;
                    }
                    if (playerDistance > 200f)
                    {
                        if (player.position.X - Projectile.position.X > 0f)
                        {
                            switch (Main.rand.Next(1, 3))
                            {
                                case 1:
                                    Projectile.velocity.X += 0.05f;
                                    break;

                                case 2:
                                    Projectile.velocity.X += 0.10f;
                                    break;

                                case 3:
                                    Projectile.velocity.X += 0.15f;
                                    break;
                            }

                            if (Projectile.velocity.X > 9f)
                            {
                                Projectile.velocity.X = 9f;
                            }
                        }
                        else
                        {
                            switch (Main.rand.Next(1, 3))
                            {
                                case 1:
                                    Projectile.velocity.X -= 0.05f;
                                    break;

                                case 2:
                                    Projectile.velocity.X -= 0.10f;
                                    break;

                                case 3:
                                    Projectile.velocity.X -= 0.15f;
                                    break;
                            }

                            if (Projectile.velocity.X < -9f)
                            {
                                Projectile.velocity.X = -9f;
                            }
                        }
                    }
                    if (playerDistance < 200f)
                    {
                        if (Projectile.velocity.X != 0f)
                        {
                            if (Projectile.velocity.X > 0.5f)
                            {
                                switch (Main.rand.Next(1, 3))
                                {
                                    case 1:
                                        Projectile.velocity.X -= 0.05f;
                                        break;

                                    case 2:
                                        Projectile.velocity.X -= 0.10f;
                                        break;

                                    case 3:
                                        Projectile.velocity.X -= 0.15f;
                                        break;
                                }
                            }
                            else if (Projectile.velocity.X < -0.5f)
                            {
                                switch (Main.rand.Next(1, 3))
                                {
                                    case 1:
                                        Projectile.velocity.X += 0.05f;
                                        break;

                                    case 2:
                                        Projectile.velocity.X += 0.10f;
                                        break;

                                    case 3:
                                        Projectile.velocity.X += 0.15f;
                                        break;
                                }
                            }
                            else if (Projectile.velocity.X < 0.5f && Projectile.velocity.X > -0.5f)
                            {
                                Projectile.velocity.X = 0f;
                            }
                        }
                    }
                }
            }
            else if (fly)
            {
                Vector2 center2 = Projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(0f, 0f);
                float playerDistance = vector48.Length();
                vector48.Normalize();
                vector48 *= 8f;
                Projectile.velocity = (Projectile.velocity * 40f + vector48) / 41f;

                Projectile.rotation = Projectile.velocity.X * 0.03f;
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 3)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 9)
                {
                    Projectile.frame = 5;
                }
                if (playerDistance > 2000f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
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
                    if (playerStill > 30 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        fly = false;
                        Projectile.tileCollide = true;
                        Projectile.rotation = 0;
                    }
                }
            }
            if (Projectile.velocity.X > 0.25f)
            {
                Projectile.spriteDirection = 1;
            }
            else if (Projectile.velocity.X < -0.25f)
            {
                Projectile.spriteDirection = -1;
            }
        }

        private bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(Projectile.Center.X / 16f) - tileWidth;
            if (Projectile.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((Projectile.position.Y + Projectile.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile)
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
