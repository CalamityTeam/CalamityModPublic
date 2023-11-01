using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class ForbiddenCircletEater : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        private int bounce = 3;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 50;
            }
            else
            {
                Projectile.extraUpdates = 0;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
            int dustType = 159;
            float slowXVel = Projectile.velocity.X / 3f;
            float slowYVel = Projectile.velocity.Y / 3f;
            int dustID = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[dustID];
            dust.position.X = Projectile.Center.X - slowXVel;
            dust.position.Y = Projectile.Center.Y - slowYVel;
            dust.velocity *= 0f;
            dust.scale = 0.5f;

            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + MathHelper.PiOver2;
            float projX = Projectile.position.X;
            float projY = Projectile.position.Y;
            float attackDistance = 100000f;
            bool canAttack = false;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 30f)
            {
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float npcX = npc.position.X + (float)(npc.width / 2);
                        float npcY = npc.position.Y + (float)(npc.height / 2);
                        float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                        if (npcDist < 640f && npcDist < attackDistance && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            attackDistance = npcDist;
                            projX = npcX;
                            projY = npcY;
                            canAttack = true;
                        }
                    }
                }
                if (!canAttack)
                {
                    for (int j = 0; j < Main.maxNPCs; j++)
                    {
                        NPC npc = Main.npc[j];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            float npcX = npc.position.X + (float)(npc.width / 2);
                            float npcY = npc.position.Y + (float)(npc.height / 2);
                            float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                            if (npcDist < 640f && npcDist < attackDistance && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                            {
                                attackDistance = npcDist;
                                projX = npcX;
                                projY = npcY;
                                canAttack = true;
                            }
                        }
                    }
                }
            }
            if (!canAttack)
            {
                projX = Projectile.position.X + (float)(Projectile.width / 2) + Projectile.velocity.X * 100f;
                projY = Projectile.position.Y + (float)(Projectile.height / 2) + Projectile.velocity.Y * 100f;
            }
            float projVel = 0.16f;
            Vector2 playerDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float playerX = projX - playerDirection.X;
            float playerY = projY - playerDirection.Y;
            float playerDist = (float)Math.Sqrt((double)(playerX * playerX + playerY * playerY));
            playerDist = 10f / playerDist;
            playerX *= playerDist;
            playerY *= playerDist;
            if (Projectile.velocity.X < playerX)
            {
                Projectile.velocity.X = Projectile.velocity.X + projVel;
                if (Projectile.velocity.X < 0f && playerX > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + projVel * 2f;
                }
            }
            else if (Projectile.velocity.X > playerX)
            {
                Projectile.velocity.X = Projectile.velocity.X - projVel;
                if (Projectile.velocity.X > 0f && playerX < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - projVel * 2f;
                }
            }
            if (Projectile.velocity.Y < playerY)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + projVel;
                if (Projectile.velocity.Y < 0f && playerY > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + projVel * 2f;
                }
            }
            else if (Projectile.velocity.Y > playerY)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - projVel;
                if (Projectile.velocity.Y > 0f && playerY < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - projVel * 2f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.ai[0] += 15f;
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override bool? CanDamage() => Projectile.ai[0] >= 30f;
    }
}
