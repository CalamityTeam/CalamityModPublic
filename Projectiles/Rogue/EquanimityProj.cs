using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EquanimityProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Equanimity";

        private bool recall = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Equanimity");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 36;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 15;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if ((Main.player[projectile.owner].position - projectile.position).Length() > 600f)
            {
                recall = true;
                projectile.tileCollide = false;
            }

            projectile.rotation += 0.4f * projectile.direction;

            if (recall)
            {
                if (Main.rand.Next(0, 10) == 0)
                {
                    Vector2 shardVelocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                    shardVelocity.Normalize();
                    shardVelocity *= 5f;
                    Projectile.NewProjectile(projectile.Center, shardVelocity, ModContent.ProjectileType<EquanimityDarkShard>(), (int)(projectile.damage * 0.15f), 0f, projectile.owner);
                    if (projectile.Calamity().stealthStrike)
                    {
                        Projectile.NewProjectile(projectile.Center, -shardVelocity, ModContent.ProjectileType<EquanimityLightShard>(), (int)(projectile.damage * 0.15f), 0f, projectile.owner);
                    }
                }

                Vector2 posDiff = Main.player[projectile.owner].position - projectile.position;
                if (posDiff.Length() > 30f)
                {
                    posDiff.Normalize();
                    projectile.velocity = posDiff * 30f;
                }
                else
                {
                    projectile.timeLeft = 0;
                    Kill(projectile.timeLeft);
                }
                return;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
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
                    Projectile.NewProjectile(projectile.Center, shardVelocity, ModContent.ProjectileType<EquanimityLightShard>(), (int)(projectile.damage * 0.1f), 0f, projectile.owner);
                    if (projectile.Calamity().stealthStrike)
                    {
                        Projectile.NewProjectile(projectile.Center, -shardVelocity, ModContent.ProjectileType<EquanimityDarkShard>(), (int)(projectile.damage * 0.1f), 0f, projectile.owner);
                    }
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
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
                    Projectile.NewProjectile(projectile.Center, shardVelocity, ModContent.ProjectileType<EquanimityLightShard>(), (int)(projectile.damage * 0.1f), 0f, projectile.owner);
                    if (projectile.Calamity().stealthStrike)
                    {
                        Projectile.NewProjectile(projectile.Center, -shardVelocity, ModContent.ProjectileType<EquanimityDarkShard>(), (int)(projectile.damage * 0.1f), 0f, projectile.owner);
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 3);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (recall)
            {
                return false;
            }
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            recall = true;
            projectile.tileCollide = false;
            return false;
        }
    }
}
