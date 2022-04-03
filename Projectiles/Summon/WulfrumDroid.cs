using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class WulfrumDroid : ModProjectile
    {
        public float dust = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Droid");
            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 32;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (dust == 0f)
            {
                Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 source = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = source - Projectile.Center;
                    int green = Dust.NewDust(source + dustVel, 0, 0, 61, dustVel.X * 1.1f, dustVel.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[green].noGravity = true;
                    Main.dust[green].noLight = true;
                    Main.dust[green].velocity = dustVel;
                }
                dust += 1f;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                Projectile.damage = damage2;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            bool correctMinion = Projectile.type == ModContent.ProjectileType<WulfrumDroid>();
            player.AddBuff(ModContent.BuffType<WulfrumDroidBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.wDroid = false;
                }
                if (modPlayer.wDroid)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.MinionAntiClump();
            Vector2 targetVector = Projectile.position;
            float range = 450f;
            bool foundTarget = false;
            int targetIndex = -1;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy((object) this, false))
                {
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if ((!foundTarget && targetDist < range) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                    {
                        range = targetDist;
                        targetVector = npc.Center;
                        foundTarget = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }
            if (!foundTarget)
            {
                for (int npcIndex = 0; npcIndex < Main.npc.Length; ++npcIndex)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.CanBeChasedBy((object) this, false))
                    {
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                        if ((!foundTarget && targetDist < range) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            range = targetDist;
                            targetVector = npc.Center;
                            foundTarget = true;
                            targetIndex = npcIndex;
                        }
                    }
                }
            }
            float separationAnxietyDist = 500f;
            if (foundTarget)
                separationAnxietyDist = 1000f;
            if (Vector2.Distance(player.Center, Projectile.Center) > separationAnxietyDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 1f)
                Projectile.tileCollide = false;
            if (foundTarget && Projectile.ai[0] == 0f)
            {
                Vector2 targetPos = targetVector - Projectile.Center;
                float targetDist = targetPos.Length();
                targetPos.Normalize();
                if (targetDist > 200f)
                {
                    float speed = 6f;
                    Vector2 goToTarget = targetPos * speed;
                    Projectile.velocity.X = (Projectile.velocity.X * 40f + goToTarget.X) / 41f;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 40f + goToTarget.Y) / 41f;
                }
                else if (Projectile.velocity.Y > -1f)
                    Projectile.velocity.Y -= 0.1f;
            }
            else
            {
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
                    Projectile.ai[0] = 1f;
                float returnSpeed = 6f;
                if (Projectile.ai[0] == 1f)
                    returnSpeed = 15f;
                Vector2 playerVec = player.Center - Projectile.Center + new Vector2(0f, -60f);
                float playerDist = playerVec.Length();
                if (playerDist > 200f && returnSpeed < 9f)
                    returnSpeed = 9f;
                if (playerDist < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (playerDist > 2000f)
                {
                    Projectile.position.X = player.Center.X - (float) (Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float) (Projectile.width / 2);
                    Projectile.netUpdate = true;
                }
                else if (playerDist > 70f)
                {
                    playerVec.Normalize();
                    Projectile.velocity = (Projectile.velocity * 20f + playerVec * returnSpeed) / 21f;
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
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            if (Projectile.velocity.X > 0f)
                Projectile.spriteDirection = Projectile.direction = 1;
            else if (Projectile.velocity.X < 0f)
                Projectile.spriteDirection = Projectile.direction = -1;
            if (Projectile.ai[1] > 0f)
                Projectile.ai[1] += (float) Main.rand.Next(1, 4);
            if (Projectile.ai[1] > 90f)
            {
                Projectile.ai[1] = 0.0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[1] != 0f)
                return;
            ++Projectile.ai[1];
            if (Main.myPlayer != Projectile.owner)
                return;
            if (!foundTarget)
                return;
            Vector2 velocity = targetVector - Projectile.Center;
            velocity.Normalize();
            velocity *= 10f;
            int bolt = Projectile.NewProjectile(Projectile.Center, velocity, ModContent.ProjectileType<WulfrumBoltMinion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Main.projectile[bolt].netUpdate = true;
            Projectile.netUpdate = true;
        }

        public override bool CanDamage() => false;
    }
}
