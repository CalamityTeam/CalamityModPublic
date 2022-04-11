using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class ApexShark : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Ranged/SandyWaifuShark";

        private int HitCooldown = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Mineral Shark");
            Main.projFrames[Projectile.type] = 8;
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
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (HitCooldown > 0)
                HitCooldown--;
            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] == 0f)
            {
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + Projectile.Center;
                    Vector2 vector7 = vector6 - Projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 85, vector7.X * 1.75f, vector7.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
                Projectile.localAI[0]++;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 0;
            }
            float num633 = 1200f;
            float num634 = 1500f;
            float num635 = 2200f;
            float num636 = 150f;
            bool flag64 = Projectile.type == ModContent.ProjectileType<ApexShark>();

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
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.MinionAntiClump();
            bool flag24 = false;
            if (Projectile.ai[0] == 2f)
            {
                Projectile.ai[1] += 1f;
                Projectile.extraUpdates = 1;
                if (Projectile.ai[1] > 60f)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.extraUpdates = 0;
                    Projectile.numUpdates = 0;
                    Projectile.netUpdate = true;
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
            Vector2 vector46 = Projectile.position;
            bool flag25 = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float num646 = Vector2.Distance(npc.Center, Projectile.Center);
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
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float num646 = Vector2.Distance(nPC2.Center, Projectile.Center);
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
            if (Vector2.Distance(player.Center, Projectile.Center) > num647)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (flag25 && Projectile.ai[0] == 0f)
            {
                Vector2 vector47 = vector46 - Projectile.Center;
                float num648 = vector47.Length();
                vector47.Normalize();
                if (num648 > 200f)
                {
                    float scaleFactor2 = 13f; //8
                    vector47 *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 6f;
                    vector47 *= -num649;
                    Projectile.velocity = (Projectile.velocity * 40f + vector47) / 41f;
                }
            }
            else
            {
                bool flag26 = false;
                if (!flag26)
                {
                    flag26 = Projectile.ai[0] == 1f;
                }
                float num650 = 10f;
                if (flag26)
                {
                    num650 = 21f;
                }
                Vector2 center2 = Projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(0f, -60f);
                float num651 = vector48.Length();
                if (num651 > 200f && num650 < 8f)
                {
                    num650 = 1f;
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
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (Projectile.ai[1] > 60f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                if (Projectile.ai[1] == 0f && flag25 && num633 < 500f)
                {
                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.ai[0] = 2f;
                        Vector2 value20 = vector46 - Projectile.Center;
                        value20.Normalize();
                        Projectile.velocity = value20 * 13f; //8
                        Projectile.netUpdate = true;
                    }
                }
            }

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.Item, (int)Projectile.Center.X, (int)Projectile.Center.Y, 70, 0.5f);
            Projectile.velocity = new Vector2(0f, 5f).RotatedBy(Projectile.velocity.ToRotation() + 1.5f);
            int sand = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SandExplosion>(), (int)(Projectile.damage * 0.7f), (int)(Projectile.knockBack * 0.7f), Projectile.owner, 0f, 0f);
            Main.projectile[sand].Center = Projectile.Center;
            Projectile.netUpdate = true;
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
