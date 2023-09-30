using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicShivBall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        // TODO -- Please for the love of god refactor this at some point. It is ancient.
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public NPC target = null;
        public const float maxDistanceToTarget = 540f;
        public bool initialized = false;
        public float startingVelocityY = 0f;
        public float randomAngleDelta = 0f;
        public const float explosionDamageMultiplier = 1.5f;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 220;
        }
        public override void AI()
        {
            if (!initialized)
            {
                target = Projectile.Center.ClosestNPCAt(maxDistanceToTarget);
                startingVelocityY = Projectile.velocity.Y;
                randomAngleDelta = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                initialized = true;
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 3; i++)
                {
                    int dustID = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 173, (float)(Projectile.direction * 2), 0f, 115, Color.White, 1.3f);
                    Main.dust[dustID].noGravity = true;
                    Main.dust[dustID].velocity *= 0f;
                }
            }
            if (Projectile.localAI[0] % 30 == 0) // every 0.5 seconds
            {
                target = Projectile.Center.ClosestNPCAt(maxDistanceToTarget);
            }
            if (target != null)
            {
                float inertia = 35f;
                float homingSpeed = 33.5f;
                Vector2 idealVelocity = Projectile.SafeDirectionTo(target.Center, Vector2.UnitX) * homingSpeed;
                Projectile.velocity = (Projectile.velocity * (inertia - 1f) + idealVelocity) / inertia;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // for spawning the side beams
            for (int i = 0; i < 2; i++)
            {
                int directionSign = Main.rand.NextBool().ToDirectionInt();
                Vector2 spawnPos = new Vector2(target.Center.X + directionSign * 650, Projectile.Center.Y + Main.rand.Next(-500, 501));
                Vector2 velocity = Vector2.Normalize(target.Center - spawnPos) * 30f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos.X, spawnPos.Y, velocity.X, velocity.Y, ModContent.ProjectileType<CosmicShivBlade>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack * 0.1f, Projectile.owner);
            }
            int starMax = Main.rand.Next(6, 11); // 6 to 10 stars
            for (int i = -starMax / 2; i < starMax / 2; i++)
            {
                int ySpawnAdditive = Main.rand.Next(-40, 41);
                Vector2 toSpawn = target.Center - new Vector2(0f, 800f + ySpawnAdditive).RotatedBy(MathHelper.ToRadians(i * 11f / starMax));
                Vector2 toTarget = Vector2.Normalize(target.Center - toSpawn) * 35f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), toSpawn, toTarget, ModContent.ProjectileType<GalaxyStar>(), Projectile.damage, Projectile.knockBack * 0.5f, Projectile.owner);
            }
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 60);
        }
        // pretty much entirely from the Oracle circular damage code

        private void CircularDamage(float radius)
        {
            if (Projectile.owner != Main.myPlayer)
                return;
            Player owner = Main.player[Projectile.owner];

            for (int i = 0; i < Main.npc.Length; ++i)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.dontTakeDamage || target.friendly)
                    continue;

                // Shock any valid target within range. Check all four corners of their hitbox.
                float d1 = Vector2.Distance(Projectile.Center, target.Hitbox.TopLeft());
                float d2 = Vector2.Distance(Projectile.Center, target.Hitbox.TopRight());
                float d3 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomLeft());
                float d4 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomRight());
                float dist = MathHelper.Min(d1, d2);
                dist = MathHelper.Min(dist, d3);
                dist = MathHelper.Min(dist, d4);

                if (dist <= radius)
                {
                    int damage = (int)(Projectile.damage * explosionDamageMultiplier);
                    bool crit = Main.rand.Next(100) <= owner.GetCritChance<MeleeDamageClass>() + 4;
                    target.StrikeNPC(target.CalculateHitInfo(damage, 0, crit, 0));

                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, i, damage, 0f, 0f, crit ? 1 : 0, 0, 0);
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            // mostly from AstralCrystal kill code
            float rand2PI = Main.rand.NextFloat(MathHelper.TwoPi);
            int petalCount = 5;
            float speed = 12f;
            float scale = Main.rand.NextFloat(1f, 1.35f);
            for (float k = 0f; k < MathHelper.TwoPi; k += 0.05f)
            {
                Vector2 velocity = k.ToRotationVector2() * (2f + (float)(Math.Sin((double)(rand2PI + k * (float)petalCount)) + 1.0) * speed) * Main.rand.NextFloat(0.95f, 1.05f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 173, new Vector2?(velocity), 0, default, scale);
                dust.customData = 0.025f;
            }
            CircularDamage(80f);
        }
    }
}
