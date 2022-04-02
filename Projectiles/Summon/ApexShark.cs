using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class ApexShark : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/SandyWaifuShark";

        private int HitCooldown = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Mineral Shark");
            Main.projFrames[projectile.type] = 8;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (HitCooldown > 0)
                HitCooldown--;
            Player player = Main.player[projectile.owner];
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 85, vector7.X * 1.75f, vector7.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
                projectile.localAI[0]++;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 7)
            {
                projectile.frame = 0;
            }
            float num633 = 1200f;
            float num634 = 1500f;
            float num635 = 2200f;
            float num636 = 150f;
            bool flag64 = projectile.type == ModContent.ProjectileType<ApexShark>();

            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<ApexSharkBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.apexShark = false;
                }
                if (modPlayer.apexShark)
                {
                    projectile.timeLeft = 2;
                }
            }
            projectile.MinionAntiClump();
            bool flag24 = false;
            if (projectile.ai[0] == 2f)
            {
                projectile.ai[1] += 1f;
                projectile.extraUpdates = 1;
                if (projectile.ai[1] > 60f)
                {
                    projectile.ai[1] = 1f;
                    projectile.ai[0] = 0f;
                    projectile.extraUpdates = 0;
                    projectile.numUpdates = 0;
                    projectile.netUpdate = true;
                }
                else
                {
                    flag24 = true;
                }
            }
            if (flag24)
            {
                return;
            }
            Vector2 vector46 = projectile.position;
            bool flag25 = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float num646 = Vector2.Distance(npc.Center, projectile.Center);
                    if (!flag25 && num646 < num633)
                    {
                        num633 = num646;
                        vector46 = npc.Center;
                        flag25 = true;
                    }
                }
            }
            if (!flag25)
            {
                for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if (nPC2.CanBeChasedBy(projectile, false))
                    {
                        float num646 = Vector2.Distance(nPC2.Center, projectile.Center);
                        if (!flag25 && num646 < num633)
                        {
                            num633 = num646;
                            vector46 = nPC2.Center;
                            flag25 = true;
                        }
                    }
                }
            }
            float num647 = num634;
            if (flag25)
            {
                num647 = num635;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > num647)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }
            if (flag25 && projectile.ai[0] == 0f)
            {
                Vector2 vector47 = vector46 - projectile.Center;
                float num648 = vector47.Length();
                vector47.Normalize();
                if (num648 > 200f)
                {
                    float scaleFactor2 = 13f; //8
                    vector47 *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 6f;
                    vector47 *= -num649;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
            }
            else
            {
                bool flag26 = false;
                if (!flag26)
                {
                    flag26 = projectile.ai[0] == 1f;
                }
                float num650 = 10f;
                if (flag26)
                {
                    num650 = 21f;
                }
                Vector2 center2 = projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(0f, -60f);
                float num651 = vector48.Length();
                if (num651 > 200f && num650 < 8f)
                {
                    num650 = 1f;
                }
                if (num651 < num636 && flag26 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                if (num651 > 2000f)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (num651 > 70f)
                {
                    vector48.Normalize();
                    vector48 *= num650;
                    projectile.velocity = (projectile.velocity * 40f + vector48) / 41f;
                }
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
            }
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (projectile.ai[1] > 60f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            {
                if (projectile.ai[1] == 0f && flag25 && num633 < 500f)
                {
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner)
                    {
                        projectile.ai[0] = 2f;
                        Vector2 value20 = vector46 - projectile.Center;
                        value20.Normalize();
                        projectile.velocity = value20 * 13f; //8
                        projectile.netUpdate = true;
                    }
                }
            }

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 70, 0.5f);
            projectile.velocity = new Vector2(0f, 5f).RotatedBy(projectile.velocity.ToRotation() + 1.5f);
            int sand = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<SandExplosion>(), (int)(projectile.damage * 0.7f), (int)(projectile.knockBack * 0.7f), projectile.owner, 0f, 0f);
            Main.projectile[sand].Center = projectile.Center;
            projectile.netUpdate = true;
            HitCooldown = 20;

        }

        public override bool? CanHitNPC(NPC target)
        {
            if (HitCooldown == 0)
            {
                return null;
            }
            return false;
        }
    }
}
