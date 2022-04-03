using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;
namespace CalamityMod.Projectiles.Rogue
{
    public class Prismalline3 : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Prismalline";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismalline");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.Calamity().rogue = true;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 150 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

            if (Projectile.timeLeft < 150)
                CalamityGlobalProjectile.HomeInOnNPC(Projectile, !Projectile.tileCollide, 450f, 12f, 20f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 154, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
            if (Projectile.ai[0] == 1f)
            {
                int shardCount = Main.rand.Next(1,4);
                for (int s = 0; s < shardCount; s++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    int shard = Projectile.NewProjectile(Projectile.Center, velocity, ModContent.ProjectileType<AquashardSplit>(), Projectile.damage / 3, 0f, Projectile.owner);
                    if (shard.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[shard].Calamity().forceRogue = true;
                        Main.projectile[shard].penetrate = 1;
                    }
                }
            }
        }
    }
}
