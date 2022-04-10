using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class ProfanedEnergy : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/NormalNPCs/ImpiousImmolator";

        private float count = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Energy");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            if (count == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                for (int i = 0; i < 5; i++)
                {
                    int holy = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 2f);
                    Main.dust[holy].velocity *= 3f;
                    Main.dust[holy].position = Projectile.Center;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[holy].scale = 0.5f;
                        Main.dust[holy].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 10; j++)
                {
                    int fire = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 3f);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].velocity *= 5f;
                    Main.dust[fire].position = Projectile.Center;
                    fire = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 2f;
                    Main.dust[fire].position = Projectile.Center;
                }
                count += 1f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[0] != 0f)
                {
                    Projectile.ai[0] -= 1f;
                    return;
                }
                bool flag18 = false;
                float num506 = Projectile.Center.X;
                float num507 = Projectile.Center.Y;
                float num508 = 1000f;
                int target = 0;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float num539 = npc.Center.X;
                        float num540 = npc.Center.Y;
                        float num541 = Math.Abs(Projectile.Center.X - num539) + Math.Abs(Projectile.Center.Y - num540);
                        if (num541 < num508 && Collision.CanHit(Projectile.Center, Projectile.width, Projectile.height, npc.Center, npc.width, npc.height))
                        {
                            num506 = num539;
                            num507 = num540;
                            flag18 = true;
                            target = npc.whoAmI;
                        }
                    }
                }
                if (!flag18)
                {
                    for (int num512 = 0; num512 < Main.maxNPCs; num512++)
                    {
                        if (Main.npc[num512].CanBeChasedBy(Projectile, false))
                        {
                            float num513 = Main.npc[num512].position.X + (float)(Main.npc[num512].width / 2);
                            float num514 = Main.npc[num512].position.Y + (float)(Main.npc[num512].height / 2);
                            float num515 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num513) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num514);
                            if (num515 < num508 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[num512].position, Main.npc[num512].width, Main.npc[num512].height))
                            {
                                num508 = num515;
                                num506 = num513;
                                num507 = num514;
                                flag18 = true;
                                target = num512;
                            }
                        }
                    }
                }
                if (flag18)
                {
                    float num516 = num506;
                    float num517 = num507;
                    num506 -= Projectile.Center.X;
                    num507 -= Projectile.Center.Y;
                    if (num506 < 0f)
                    {
                        Projectile.spriteDirection = 1;
                    }
                    else
                    {
                        Projectile.spriteDirection = -1;
                    }
                    int projectileType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        ModContent.ProjectileType<FlameBlast>(),
                        ModContent.ProjectileType<FlameBurst>()
                    });
                    float speed = 25f;
                    Vector2 vector29 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float num404 = num516 - vector29.X;
                    float num405 = num517 - vector29.Y;
                    float num406 = (float)Math.Sqrt((double)(num404 * num404 + num405 * num405));
                    num406 = speed / num406;
                    num404 *= num406;
                    num405 *= num406;
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, num404, num405, projectileType, Projectile.damage, Projectile.knockBack, Projectile.owner, (float)target, 0f);
                    Projectile.ai[0] = 16f;
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
