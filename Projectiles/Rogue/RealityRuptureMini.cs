using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class RealityRuptureMini : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Rogue/RealityRuptureMini";
        public static readonly SoundStyle Hitsound = new("CalamityMod/Sounds/Item/WulfrumKnifeTileHit2") { PitchVariance = 0.3f, Volume = 0.5f };

        public int framesInAir = 0;
        public int SparkChance = 7;

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 800;
            AIType = 0;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            framesInAir++;
            if (framesInAir < 120)
            {
                Lighting.AddLight(Projectile.Center + Projectile.velocity * 0.6f, 0.6f, 0.2f, 0.5f);
            }

            if (Main.rand.NextBool(SparkChance))
            {
                Vector2 SparkVelocity1 = Projectile.velocity.RotatedBy(-3, default) * 0.1f - Projectile.velocity / 2f;
                Vector2 SparkPosition1 = Projectile.velocity.RotatedBy(-0.8, default);
                SparkParticle spark = new SparkParticle(Projectile.Center + SparkPosition1, SparkVelocity1, false, Main.rand.Next(6, 8), Main.rand.NextFloat(0.6f, 0.8f), Color.Plum);
                GeneralParticleHandler.SpawnParticle(spark);

            }
            if (Main.rand.NextBool(SparkChance))
            {
                Vector2 SparkVelocity2 = Projectile.velocity.RotatedBy(3, default) * 0.1f - Projectile.velocity / 2f;
                Vector2 SparkPosition2 = Projectile.velocity.RotatedBy(0.8, default);
                SparkParticle spark2 = new SparkParticle(Projectile.Center + SparkPosition2, SparkVelocity2, false, Main.rand.Next(6, 8), Main.rand.NextFloat(0.6f, 0.8f), Color.Plum);
                GeneralParticleHandler.SpawnParticle(spark2);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            Vector2 center = Projectile.Center;
            float maxDistance = 350f;
            bool homeIn = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);
                    bool canHit = Projectile.Calamity().stealthStrike || Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);

                    if (Vector2.Distance(Main.npc[i].Center, Projectile.Center) < (maxDistance + extraDistance) && canHit)
                    {
                        center = Main.npc[i].Center;
                        homeIn = true;
                        break;
                    }
                }
            }

            if (homeIn)
            {
                SparkChance = 14;
                Projectile.extraUpdates = 4;
                Vector2 moveDirection = Projectile.SafeDirectionTo(center, Vector2.UnitY);
                Projectile.velocity = (Projectile.velocity * 20f + moveDirection * 12f) / (21f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i <= 2; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 272, Projectile.oldVelocity.X * Main.rand.NextFloat(1.1f, 1.3f), Projectile.oldVelocity.Y * Main.rand.NextFloat(1.1f, 1.3f));
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(Hitsound, Projectile.position);
        }

        //public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Ichor, 120);
    }
}
