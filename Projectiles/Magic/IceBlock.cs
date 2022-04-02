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
            projectile.coldDamage = true;
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
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type == ModContent.ProjectileType<IceBarrageMain>() && proj.owner == Main.myPlayer)
                {
                    Vector2 pos1 = new Vector2(proj.Center.X, proj.Center.Y - (proj.height * 0.5f) - 44f);
                    Vector2 pos2 = new Vector2(proj.Center.X + (proj.width * 0.5f) + 48f, proj.Center.Y);
                    Vector2 pos3 = new Vector2(proj.Center.X, proj.Center.Y + (proj.height * 0.5f) + 44f);
                    Vector2 pos4 = new Vector2(proj.Center.X - (proj.width * 0.5f) - 49f, proj.Center.Y);
                    switch (projectile.ai[0])
                    {
                        case 0: projectile.Center = pos1;
                                break;
                        case 1: projectile.Center = pos2;
                                break;
                        case 2: projectile.Center = pos3;
                                break;
                        case 3: projectile.Center = pos4;
                                break;
                        default: break;
                    }
                }
            }
            switch (projectile.ai[0])
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
                int dustType = Main.rand.NextBool(2) ? 68 : 67;
                if (Main.rand.NextBool(4))
                {
                    dustType = 80;
                }
                Vector2 direction = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, direction.X, direction.Y, 50, default, 1.5f);
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
