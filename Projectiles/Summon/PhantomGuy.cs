using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class PhantomGuy : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/Polterghast/PhantomFuckYou";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public int shootTimeCounter = 0;
        bool canShoot = false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.localAI[0] == 0f)
            {
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((num227 - (num226 / 2 - 1)) * MathHelper.TwoPi / num226) + Projectile.Center;
                    Vector2 vector7 = vector6 - Projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 180, vector7.X * 1.75f, vector7.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
                Projectile.localAI[0] += 1f;
            }
            bool flag64 = Projectile.type == ModContent.ProjectileType<PhantomGuy>();
            player.AddBuff(ModContent.BuffType<Phantom>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.pGuy = false;
                }
                if (modPlayer.pGuy)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.MinionAntiClump();
            float num633 = 3000f;
            float num634 = 3500f;
            float num635 = 4000f;
            float num636 = 600f;
            Vector2 vector46 = Projectile.position;
            bool flag25 = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float num646 = Vector2.Distance(npc.Center, Projectile.Center);
                    if (num646 < num633)
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
                    float scaleFactor2 = 18f;
                    vector47 *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 12f;
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
                float num650 = 12f;
                if (flag26)
                {
                    num650 = 30f;
                }
                Vector2 center2 = Projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(0f, -120f);
                float num651 = vector48.Length();
                if (num651 > 200f && num650 < 16f)
                {
                    num650 = 16f;
                }
                if (num651 < num636 && flag26)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (num651 > 3500f)
                {
                    Projectile.position.X = player.Center.X - (Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (Projectile.height / 2);
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
            if (flag25)
            {
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(vector46), 0.1f);
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += Main.rand.Next(1, 3);
            }
            if (Projectile.ai[1] > 75f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                if (flag25 && Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        canShoot = true;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (canShoot)
            {
                shootTimeCounter++;
                
                if (shootTimeCounter % 20 == 0 && shootTimeCounter <= 60)
                {
                    SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                    float randomRadius = Main.rand.Next(10, 14);
                    Vector2 randomVelocity = Main.rand.NextVector2CircularEdge(randomRadius, randomRadius);
                    Projectile.velocity -= randomVelocity*0.22f; //funny recoil
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, randomVelocity.X, randomVelocity.Y, ModContent.ProjectileType<GhostFire>(), Projectile.damage, 0f, Main.myPlayer, 0f, 0f);
                    Projectile.netUpdate = true;
                } 
                if (shootTimeCounter > 200)
                {
                    canShoot = false;
                    shootTimeCounter = 0;
                    Projectile.netUpdate = true;
                }
                
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 250, 250, Projectile.alpha);
        }

        public override bool? CanDamage() => false;
    }
}
