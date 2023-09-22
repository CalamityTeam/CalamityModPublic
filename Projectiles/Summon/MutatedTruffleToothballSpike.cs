using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MutatedTruffleToothballSpike : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public ref float TargetShotID => ref Projectile.ai[0];
        public ref float TimerForHitbox => ref Projectile.ai[1];
        public NPC TargetShot => Main.npc[(int)TargetShotID];

        public const int TimeForHitbox = 15;

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 600;
            Projectile.width = Projectile.height = 26;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            TimerForHitbox++;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (TargetShot is not null && TargetShot.active && TimerForHitbox >= TimeForHitbox)
            {
                float inertia = 25f;
                Projectile.velocity = (Projectile.velocity * inertia + Projectile.SafeDirectionTo(TargetShot.Center) * Utils.Remap(Projectile.timeLeft, 600f, 540f, MutatedTruffle.ToothballSpikeSpeed - 15f, MutatedTruffle.ToothballSpikeSpeed)) / (inertia + 1f);
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }
        }

        public override bool? CanDamage() => (TimerForHitbox >= TimeForHitbox && Projectile.getRect().Intersects(TargetShot.getRect())) ? null : false;
    }
}
