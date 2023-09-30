using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlantationStaffThornball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public NPC Target => Projectile.Center.MinionHoming(PlantationStaff.EnemyDistanceDetection, Owner);

        public ref float IsSticked => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = 60;
            Projectile.width = Projectile.height = 30;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            if (IsSticked == 0f)
            {
                // Home towards the target, if there's one.
                if (Target is not null)
                    Projectile.velocity = (Projectile.velocity * 25f + Projectile.SafeDirectionTo(Target.Center) * PlantationStaff.ThornballSpeed) / 26f;

                Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.X);
            }
            else
            {
                // Making the projectile's velocity same as the target's means it stays with it and moves with it,
                // giving it an effect of attachment.
                if (Target is not null)
                    Projectile.velocity = Target.velocity;
                else
                    Projectile.Kill();
            }
        }

        #region AI Methods

        #endregion

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // If the projectile hits once after it's attached: die.
            if (IsSticked == 1f)
                Projectile.Kill();

            // On enemy hit, make it stick.
            IsSticked = 1f;

            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int dustIndex = 0; dustIndex < 10; dustIndex++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 40, newColor: Color.Pink);
        }
    }
}

