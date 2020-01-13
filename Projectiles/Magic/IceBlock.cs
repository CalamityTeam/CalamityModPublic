using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Magic
{
    public class IceBlock : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Barrage");
        }

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 58;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.timeLeft = 140;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            if (projectile.alpha > 20)
            {
                projectile.alpha -= 12;
            }
            if(projectile.alpha < 20)
            {
                projectile.alpha = 20;
            }
            switch(projectile.ai[0])
            {
                case 1: projectile.rotation = (MathHelper.Pi * 0.5f);
                        break;
                case 2: projectile.rotation = MathHelper.Pi;
                        break;
                case 3: projectile.rotation = (MathHelper.Pi * 1.5f);
                        break;
                default: break;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                int num297 = Main.rand.NextBool(2) ? 68 : 67;
                if (Main.rand.NextBool(4))
                {
                    num297 = 80;
                }
                Vector2 direction = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, num297, direction.X, direction.Y, 50, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
            Main.PlaySound(SoundID.NPCHit5, projectile.Center);
            for (int i = 0; i< 8; i++)
            {
                Vector2 projdir = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                Vector2 projpos = projectile.Center + new Vector2(Main.rand.NextFloat(-50f, 50f), Main.rand.NextFloat(-50f, 50f));
                Projectile.NewProjectile(projpos, projdir, ModContent.ProjectileType<IceBlockIcicle>(), (int)(projectile.damage * 0.2f), 4f, projectile.owner, Main.rand.Next(0, 2), 0f);
            }
        }
    }
}
