using Terraria;
using Terraria.ID;
using CalamityMod.Projectiles.BaseProjectiles;
namespace CalamityMod.Projectiles.Melee.Spears
{
    public class YateveoBloomSpear : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yateveo Bloom");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.aiStyle = 19;
            projectile.melee = true;
            projectile.timeLeft = 90;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.Calamity().trueMelee = true;
        }

        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(3))
            {
                int dustType = Main.rand.Next(5);
                switch (dustType)
                {
                    case 0:
                        dustType = 2;
                        break;
                    case 1:
                        dustType = 44;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        dustType = 136;
                        break;
                    default:
                        break;
                }
                int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, dustType, 0f, 0f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 180);
            target.AddBuff(BuffID.Venom, 90);
        }
    }
}
