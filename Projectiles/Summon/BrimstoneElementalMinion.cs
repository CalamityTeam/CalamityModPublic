using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BrimstoneElementalMinion : ModProjectile
    {
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Elemental");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 126;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            bool flag64 = Projectile.type == ModContent.ProjectileType<BrimstoneElementalMinion>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.brimstoneWaifu && !modPlayer.allWaifus)
            {
                Projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.bWaifu = false;
                }
                if (modPlayer.bWaifu)
                {
                    Projectile.timeLeft = 2;
                }
            }
            dust--;
            if (dust >= 0)
            {
                int num501 = 50;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 9)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            float num = (float)Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight(Projectile.Center, 1.25f * num, 0f * num, 0.5f * num);
            if (Math.Abs(Projectile.velocity.X) > 0.2f)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }
            float num633 = 700f;
            float num636 = 400f; //150
            Projectile.MinionAntiClump();
            Vector2 targetCenter = Projectile.position;
            bool flag25 = false;
            if (Projectile.ai[0] != 1f)
            {
                Projectile.tileCollide = false;
            }
            if (Projectile.tileCollide && WorldGen.SolidTile(Framing.GetTileSafely((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16)))
            {
                Projectile.tileCollide = false;
            }
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float num646 = Vector2.Distance(npc.Center, Projectile.Center);
                    if ((!flag25 && num646 < num633) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                    {
                        targetCenter = npc.Center;
                        flag25 = true;
                    }
                }
            }
            if (!flag25)
            {
                for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float num646 = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if ((!flag25 && num646 < num633) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, nPC2.position, nPC2.width, nPC2.height))
                        {
                            num633 = num646;
                            targetCenter = nPC2.Center;
                            flag25 = true;
                        }
                    }
                }
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > 1200f)
            {
                Projectile.ai[0] = 1f;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
            bool flag26 = false;
            if (!flag26)
            {
                flag26 = Projectile.ai[0] == 1f;
            }
            float num650 = 5f; //6
            if (flag26)
            {
                num650 = 12f; //15
            }
            Vector2 center2 = Projectile.Center;
            Vector2 vector48 = player.Center - center2 + new Vector2(-500f, -60f); //-60
            float num651 = vector48.Length();
            if (num651 > 200f && num650 < 6.5f) //200 and 8
            {
                num650 = 6.5f; //8
            }
            if (num651 < num636 && flag26 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (num651 > 2000f)
            {
                Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.height / 2);
                Projectile.netUpdate = true;
            }
            if (num651 > 70f)
            {
                vector48.Normalize();
                vector48 *= num650;
                Projectile.velocity = (Projectile.velocity * 40f + vector48) / 41f;
            }
            else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
            {
                Projectile.velocity.X = -0.18f;
                Projectile.velocity.Y = -0.08f;
            }
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (Projectile.ai[1] > 160f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            // Prevent firing immediately
            if (Projectile.localAI[0] < 120f)
                Projectile.localAI[0] += 1f;

            if (Projectile.ai[0] == 0f)
            {
                float fireballShootSpeed = 14f;
                int num658 = ModContent.ProjectileType<BrimstoneFireballMinion>();
                if (flag25 && Projectile.ai[1] == 0f && Projectile.localAI[0] >= 120f)
                {
                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, targetCenter, 0, 0))
                    {
                        Vector2 fireballshootVelocity = Projectile.SafeDirectionTo(targetCenter) * fireballShootSpeed;
                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireballshootVelocity, num658, Projectile.damage, 0f, Projectile.owner, 0f, 0f);
                        if (Main.projectile.IndexInRange(p))
                            Main.projectile[p].originalDamage = Projectile.originalDamage;
                        Projectile.netUpdate = true;
                    }
                }
            }
        }
    }
}
