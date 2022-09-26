using CalamityMod.Projectiles.BaseProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class GildedProboscisProj : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Proboscis");
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;  //The width of the .png file in pixels divided by 2.
            Projectile.aiStyle = ProjAIStyleID.Spear;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.timeLeft = 90;
            Projectile.height = 40;  //The height of the .png file in pixels divided by 2.
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }

        // These numbers sure are common, huh? yeah, they are
        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 2.4f;
        public override float ForwardSpeed => 0.95f;
        public override void ExtraBehavior()
        {
            if (Main.rand.NextBool(4))
            {
                int num = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 60, Projectile.direction * 2, 0f, 150, default, 1f);
                Main.dust[num].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!target.canGhostHeal || Main.player[Projectile.owner].moonLeech)
                return;

            Player player = Main.player[Projectile.owner];
            player.statLife += 1;
            player.HealEffect(1);
        }
    }
}
