using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class GildedProboscisProj : BaseSpearProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<GildedProboscis>();
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.timeLeft = 90;
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
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 60, Projectile.direction * 2, 0f, 150, default, 1f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.canGhostHeal || Main.player[Projectile.owner].moonLeech)
                return;

            Player player = Main.player[Projectile.owner];
            player.statLife += 1;
            player.HealEffect(1);
        }
    }
}
