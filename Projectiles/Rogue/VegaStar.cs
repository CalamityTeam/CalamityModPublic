
using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using rail;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class VegaStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public static int lifetime => 600;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = lifetime;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.localAI[0] = 20f;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.MaxUpdates = 2;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.direction * 0.05f;
            if (Projectile.FinalExtraUpdate())
            {
                var star = new BloomParticle(Projectile.Center, Vector2.Zero, Color.SkyBlue, 0.2f, 0.25f, 2, false);
                var star2 = new CustomSpark(Projectile.Center, Vector2.UnitX.RotatedBy(Projectile.rotation) * 0.1f, "CalamityMod/Particles/Sparkle", false, 2, 1f, Color.White, Vector2.One);
                GeneralParticleHandler.SpawnParticle(star);
                GeneralParticleHandler.SpawnParticle(star2);
            }
            if (Projectile.ai[0] == 0f)
            {
                if (Projectile.timeLeft < (lifetime - Projectile.ai[1]) && Projectile.localAI[0] >= 0)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= Projectile.localAI[0];
                    Projectile.localAI[0]--;
                    GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center, Projectile.velocity * 0.001f, false, 10, 1, Color.SkyBlue));
                }
                else if (Projectile.timeLeft >= (lifetime - Projectile.ai[1]))
                {
                    GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center, Projectile.velocity * 0.001f, false, 10, 1, Color.SkyBlue));
                }
            }
            else if (Projectile.ai[0] == 1f)
            {
                float minDist = 999f;
                int index = 0;
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float dist = (Projectile.Center - npc.Center).Length();
                        if (dist < minDist)
                        {
                            minDist = dist;
                            index = npc.whoAmI;
                        }
                    }
                }

                if (minDist < 999f)
                {
                    Vector2 velocityNew = Main.npc[index].Center - Projectile.Center;
                    float speed = 10f;
                    velocityNew.Normalize();
                    Projectile.velocity = velocityNew * speed;
                }
            }
            if (Projectile.ai[2] > 0 && Projectile.localAI[0] < 0)
            {
                if (Main.npc.IndexInRange((int)Projectile.ai[2]-1) && Main.npc[(int)Projectile.ai[2]-1].active)
                {
                    Projectile.velocity += Projectile.DirectionTo(Main.npc[(int)Projectile.ai[2] - 1].Center);
                    Projectile.velocity *= 0.95f;
                }
            }
            if (Projectile.soundDelay == 0 && Projectile.velocity.Length() > 0.1f)
            {
                Projectile.soundDelay = 60;
                SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.5f}, Projectile.position);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    109,
                    111,
                    132
                });

                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] == 0)
                return;
            target.AddBuff(ModContent.BuffType<Voidfrost>(), 120);
        }
    }
}
