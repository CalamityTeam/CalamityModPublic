using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Rogue
{
    public class HypothermiaChunk : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Chunk");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 80;
            Projectile.extraUpdates = 4;
            Projectile.ignoreWater = true;
            Projectile.Calamity().rogue = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            Lighting.AddLight(Projectile.Center, 0.1f, 0f, 0.5f);
            if (Projectile.ai[0] < 0.2f)
            {
                Projectile.ai[0] += 0.1f;
            }
            else
            {
                Projectile.tileCollide = true;
            }

            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 191, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 0, default, 0.8f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 3; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 191, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 0, default, 0.8f);
            }
            if (Projectile.owner == Main.myPlayer)
            {
                int numSplits = Main.rand.NextBool() ? 4 : 3;
                int type = ModContent.ProjectileType<HypothermiaShard>();
                int shardDamage = (int)(Projectile.damage * 0.5f);
                float shardKB = Projectile.knockBack * 0.75f;

                for (int i = 0; i < numSplits; ++i)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    int texID = Main.rand.Next(4);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, type, shardDamage, shardKB, Main.myPlayer, texID, 1f);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
