using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class EquanimityProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Equanimity";

        private bool recall = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if ((Main.player[Projectile.owner].position - Projectile.position).Length() > 600f)
            {
                recall = true;
                Projectile.tileCollide = false;
            }

            Projectile.rotation += 0.4f * Projectile.direction;

            if (recall)
            {
                if (Main.rand.Next(0, 10) == 0)
                {
                    Vector2 shardVelocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                    shardVelocity.Normalize();
                    shardVelocity *= 5f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shardVelocity, ModContent.ProjectileType<EquanimityDarkShard>(), (int)(Projectile.damage * 0.8f), 0f, Projectile.owner);
                    if (Projectile.Calamity().stealthStrike)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -shardVelocity, ModContent.ProjectileType<EquanimityLightShard>(), (int)(Projectile.damage * 0.8f), 0f, Projectile.owner);
                    }
                }

                Vector2 posDiff = Main.player[Projectile.owner].position - Projectile.position;
                if (posDiff.Length() > 30f)
                {
                    posDiff.Normalize();
                    Projectile.velocity = posDiff * 30f;
                }
                else
                {
                    Projectile.timeLeft = 0;
                    OnKill(Projectile.timeLeft);
                }
                return;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Confused, 30);

            if (!recall)
            {
                int shardCount = Main.rand.Next(1, 3);
                for (int i = 0; i <= shardCount; i++)
                {
                    Vector2 shardVelocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                    shardVelocity.Normalize();
                    shardVelocity *= 5f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shardVelocity, ModContent.ProjectileType<EquanimityLightShard>(),  (int)(Projectile.damage * 0.8f), 0f, Projectile.owner);
                    if (Projectile.Calamity().stealthStrike)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -shardVelocity, ModContent.ProjectileType<EquanimityDarkShard>(),  (int)(Projectile.damage * 0.8f), 0f, Projectile.owner);
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Confused, 30);

            if (!recall)
            {
                int shardCount = Main.rand.Next(1, 3);
                for (int i = 0; i <= shardCount; i++)
                {
                    Vector2 shardVelocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                    shardVelocity.Normalize();
                    shardVelocity *= 5f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shardVelocity, ModContent.ProjectileType<EquanimityLightShard>(),  (int)(Projectile.damage * 0.8f), 0f, Projectile.owner);
                    if (Projectile.Calamity().stealthStrike)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -shardVelocity, ModContent.ProjectileType<EquanimityDarkShard>(),  (int)(Projectile.damage * 0.8f), 0f, Projectile.owner);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 3);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (recall)
            {
                return false;
            }
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            recall = true;
            Projectile.tileCollide = false;
            return false;
        }
    }
}
