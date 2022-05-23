using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MiniGuardianAttack : ModProjectile
    {
        private int ai = 3;
        private void updateDamage(int type)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            float baseDamage = (modPlayer.profanedCrystal && !modPlayer.profanedCrystalBuffs) ? 0f : (75f +
                        (modPlayer.profanedCrystalBuffs ? 420f : 0f));
            Projectile.damage = baseDamage == 0 ? 0 : (int)player.GetDamage<SummonDamageClass>().ApplyTo(baseDamage * 0.7f);
            ai = type;
            if (baseDamage >= 420f)
            {
                Projectile.localNPCHitCooldown = 6;
            }
            else if (baseDamage == 0)
            {
                Projectile.localNPCHitCooldown = 69;
            }
            else
            {
                Projectile.localNPCHitCooldown = 9;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ai);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
             ai = reader.ReadInt32();
        }

        private void AI(int type, float num535, float num536, Player player)
        {
            updateDamage(type);
            switch (ai)
            {
                case 1: //offensive bab (profaned soul artifact)
                case 2: //Empowered bab WEEEEEEEEEE (profaned soul crystal)
                    if (Projectile.ai[1] <= -1f)
                    {
                        Projectile.ai[1] = 17f;
                    }
                    if (Projectile.ai[1] > 0f)
                    {
                        Projectile.ai[1] -= type;
                    }
                    if (Projectile.ai[1] == 0f)
                    {
                        float num550 = 24f; //12
                        Vector2 vector43 = Projectile.Center;
                        float num551 = num535 - vector43.X;
                        float num552 = num536 - vector43.Y;
                        float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                        if (num553 < 100f)
                        {
                            num550 = 28f; //14
                        }

                        if (type == 2)
                            num550 *= 2f;
                        else if (player.Calamity().gDefense)
                            num550 *= 0.95f;

                        num553 = num550 / num553;
                        num551 *= num553;
                        num552 *= num553;
                        Projectile.velocity.X = (Projectile.velocity.X * (type == 1 ? 12f : 20f) + num551) / (type == 1 ? 13f : 21f);
                        Projectile.velocity.Y = (Projectile.velocity.Y * (type == 1 ? 12f : 20f) + num552) / (type == 1 ? 13f : 21f);
                    }
                    else
                    {
                        if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 10f)
                        {
                            Projectile.velocity *= 1.05f;
                        }
                    }
                    break;
                case 3: //bored bab - (idle)
                    float num16 = 0.5f;
                    Projectile.tileCollide = false;
                    int num17 = 100;
                    Vector2 vector3 = Projectile.Center;
                    float num18 = player.Center.X - vector3.X;
                    float num19 = player.Center.Y - vector3.Y;
                    num19 += (float)Main.rand.Next(-10, 21);
                    num18 += (float)Main.rand.Next(-10, 21);
                    num18 += (float)(60 * -(float)Main.player[Projectile.owner].direction);
                    num19 -= 60f;
                    float num20 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
                    float num21 = 18f;

                    if (num20 < (float)num17 && player.velocity.Y == 0f &&
                        Projectile.position.Y + (float)Projectile.height <= player.position.Y + (float)player.height &&
                        !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        Projectile.ai[0] = 0f;
                        if (Projectile.velocity.Y < -6f)
                        {
                            Projectile.velocity.Y = -6f;
                        }
                    }
                    if (num20 > 2000f)
                    {
                        Projectile.position = player.position;
                        Projectile.netUpdate = true;
                    }
                    if (num20 < 50f)
                    {
                        if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                        {
                            Projectile.velocity *= 0.90f;
                        }
                        num16 = 0.01f;
                    }
                    else
                    {
                        if (num20 < 100f)
                        {
                            num16 = 0.1f;
                        }
                        if (num20 > 300f)
                        {
                            num16 = 1f;
                        }
                        num20 = num21 / num20;
                        num18 *= num20;
                        num19 *= num20;
                    }

                    if (Projectile.velocity.X < num18)
                    {
                        Projectile.velocity.X += num16;
                        if (num16 > 0.05f && Projectile.velocity.X < 0f)
                        {
                            Projectile.velocity.X += num16;
                        }
                    }
                    if (Projectile.velocity.X > num18)
                    {
                        Projectile.velocity.X -= num16;
                        if (num16 > 0.05f && Projectile.velocity.X > 0f)
                        {
                            Projectile.velocity.X -= num16;
                        }
                    }
                    if (Projectile.velocity.Y < num19)
                    {
                        Projectile.velocity.Y += num16;
                        if (num16 > 0.05f && Projectile.velocity.Y < 0f)
                        {
                            Projectile.velocity.Y += num16 * 2f;
                        }
                    }
                    if (Projectile.velocity.Y > num19)
                    {
                        Projectile.velocity.Y -= num16;
                        if (num16 > 0.05f && Projectile.velocity.Y > 0f)
                        {
                            Projectile.velocity.Y -= num16 * 2f;
                        }
                    }
                    break;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Offensive Guardian"); // *swears at u*
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.tileCollide = false;
            Projectile.width = 60;
            Projectile.height = 88;
            Projectile.minionSlots = 0f;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.timeLeft *= 5;
            Projectile.usesLocalNPCImmunity = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.player[Projectile.owner].Calamity().profanedCrystalBuffs && !Main.player[Projectile.owner].Calamity().endoCooper)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool? CanCutTiles()
        {
            if (Projectile.damage == 0)
                return false;
            return null;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.gOffense = false;
            }
            if (modPlayer.gOffense)
            {
                Projectile.timeLeft = 2;
            }
            if (!modPlayer.pArtifact || (!modPlayer.profanedCrystal && !modPlayer.WearingPostMLSummonerSet))
            {
                modPlayer.gOffense = false;
                Projectile.active = false;
                return;
            }
            Projectile.MinionAntiClump();
            float num535 = Projectile.position.X;
            float num536 = Projectile.position.Y;
            float num537 = 3000f;
            bool flag19 = false;
            NPC ownerMinionAttackTargetNPC2 = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(Projectile, false))
            {
                float num539 = ownerMinionAttackTargetNPC2.Center.X;
                float num540 = ownerMinionAttackTargetNPC2.Center.Y;
                float num541 = Math.Abs(Projectile.Center.X - num539) + Math.Abs(Projectile.Center.Y - num540);
                if (num541 < num537)
                {
                    num537 = num541;
                    num535 = num539;
                    num536 = num540;
                    flag19 = true;
                }
            }
            if (!flag19)
            {
                int num3;
                for (int num542 = 0; num542 < Main.maxNPCs; num542 = num3 + 1)
                {
                    if (Main.npc[num542].CanBeChasedBy(Projectile, false))
                    {
                        float num543 = Main.npc[num542].Center.X;
                        float num544 = Main.npc[num542].Center.Y;
                        float num545 = Math.Abs(Projectile.Center.X - num543) + Math.Abs(Projectile.Center.Y - num544);
                        if (num545 < num537)
                        {
                            num537 = num545;
                            num535 = num543;
                            num536 = num544;
                            flag19 = true;
                        }
                    }
                    num3 = num542;
                }
            }
            if (!flag19 || Projectile.damage == 0)
            {
                AI(3, num535, num536, player);
            }
            else
            {
                {
                    if (player.Calamity().profanedCrystalBuffs)
                        AI(2, num535, num536, player);
                    else
                        AI(1, num535, num536, player);
                }
            }
            if (Projectile.velocity.X > 0.25f)
            {
                Projectile.direction = -1;
            }
            else if (Projectile.velocity.X < -0.25f)
            {
                Projectile.direction = 1;
            }

            if (Math.Abs(Projectile.velocity.X) > 0.2f)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame % 2 == 0)
                    Projectile.netUpdate = true;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.player[Projectile.owner].Calamity().angelicAlliance)
                target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Main.player[Projectile.owner].Calamity().angelicAlliance)
                target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);
        }
    }
}
