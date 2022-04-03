using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicEnergySpiral : ModProjectile
    {
        private bool justSpawned = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Energy");
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 78;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 10f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, (float)Main.DiscoR / 255f, (float)Main.DiscoG / 255f, (float)Main.DiscoB / 255f);
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
                Projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                Projectile.damage = damage2;
            }
            bool flag64 = Projectile.type == ModContent.ProjectileType<CosmicEnergySpiral>();
            player.AddBuff(ModContent.BuffType<CosmicEnergy>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.cEnergy = false;
                }
                if (modPlayer.cEnergy)
                {
                    Projectile.timeLeft = 2;
                }
            }
            float num633 = 1400f; //700
            float num634 = 1600f; //800
            float num635 = 2400f; //1200
            float num636 = 800f;
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            Vector2 vector46 = Projectile.position;
            bool flag25 = false;
            int target = 0;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float num646 = Vector2.Distance(npc.Center, Projectile.Center);
                    if (!flag25 && num646 < num633)
                    {
                        vector46 = npc.Center;
                        flag25 = true;
                        target = npc.whoAmI;
                    }
                }
            }
            else
            {
                for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float num646 = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if (!flag25 && num646 < num633)
                        {
                            num633 = num646;
                            vector46 = nPC2.Center;
                            flag25 = true;
                            target = num645;
                        }
                    }
                }
            }
            float num647 = num634;
            if (flag25)
            {
                num647 = num635;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > num647)
            {
                Projectile.ai[1] = 1f;
                Projectile.netUpdate = true;
            }
            if (flag25 && Projectile.ai[1] == 0f)
            {
                Vector2 vector47 = vector46 - Projectile.Center;
                float num648 = vector47.Length();
                vector47.Normalize();
                if (num648 > 200f)
                {
                    float scaleFactor2 = 6f; //6
                    vector47 *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 4f; //4
                    vector47 *= -num649;
                    Projectile.velocity = (Projectile.velocity * 40f + vector47) / 41f;
                }
            }
            else
            {
                bool flag26 = false;
                if (!flag26)
                {
                    flag26 = Projectile.ai[1] == 1f;
                }
                float num650 = 6f;
                if (flag26)
                {
                    num650 = 15f;
                }
                Vector2 center2 = Projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(0f, -60f);
                float num651 = vector48.Length();
                if (num651 > 200f && num650 < 8f)
                {
                    num650 = 8f;
                }
                if (num651 < num636 && flag26 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
                if (num651 > 2000f) //2000
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
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            float num395 = (float)Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            Projectile.scale = num395 + 0.95f;
            if (justSpawned)
            {
                justSpawned = false;
                Projectile.ai[0] = 100f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[0] != 0f)
                {
                    Projectile.ai[0] -= 1f;
                    return;
                }
                float num396 = Projectile.position.X;
                float num397 = Projectile.position.Y;
                float num398 = 1200f;
                bool flag11 = false;
                for (int num399 = 0; num399 < Main.maxNPCs; num399++)
                {
                    if (Main.npc[num399].CanBeChasedBy(Projectile, false))
                    {
                        float num400 = Main.npc[num399].position.X + (float)(Main.npc[num399].width / 2);
                        float num401 = Main.npc[num399].position.Y + (float)(Main.npc[num399].height / 2);
                        float num402 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num400) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num401);
                        if (num402 < num398)
                        {
                            num398 = num402;
                            num396 = num400;
                            num397 = num401;
                            flag11 = true;
                        }
                    }
                }
                if (flag11)
                {
                    SoundEngine.PlaySound(SoundID.Item105, (int)Projectile.position.X, (int)Projectile.position.Y);
                    int blastAmt = Main.rand.Next(5, 8);
                    for (int b = 0; b < blastAmt; b++)
                    {
                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                        Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<CosmicBlast>(), (int)(Projectile.damage * 0.5), 2f, Projectile.owner, (float)target, 0f);
                    }
                    float speed = 15f;
                    float num404 = num396 - Projectile.Center.X;
                    float num405 = num397 - Projectile.Center.Y;
                    float num406 = (float)Math.Sqrt((double)(num404 * num404 + num405 * num405));
                    num406 = speed / num406;
                    num404 *= num406;
                    num405 *= num406;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, num404, num405, ModContent.ProjectileType<CosmicBlastBig>(), Projectile.damage, 3f, Projectile.owner, (float)target, 0f);
                    Projectile.ai[0] = 100f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 255);
        }

        public override bool? CanDamage() => false;
    }
}
