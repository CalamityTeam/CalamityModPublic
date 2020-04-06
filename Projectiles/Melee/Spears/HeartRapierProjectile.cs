using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class HeartRapierProjectile : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rapier");
        }

        public override void SetDefaults()
        {
            projectile.width = 44;  //The width of the .png file in pixels divided by 2.
            projectile.aiStyle = 19;
            projectile.melee = true;  //Dictates whether this is a melee-class weapon.
            projectile.timeLeft = 90;
            projectile.height = 44;  //The height of the .png file in pixels divided by 2.
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.Calamity().trueMelee = true;
        }
        public override float InitialSpeed => 3f;
        public override float ReelbackSpeed => 1.1f;
        public override float ForwardSpeed => 0.95f;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy || !target.canGhostHeal)
                return;
            Player player = Main.player[projectile.owner];
            player.statLife += 5;
            player.HealEffect(5);
        }
    }
}
