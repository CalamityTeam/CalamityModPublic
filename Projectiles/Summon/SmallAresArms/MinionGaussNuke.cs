using CalamityMod.Items.Weapons.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class MinionGaussNuke : ModProjectile
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Gauss Nuke");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 76;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            // Handle rotation.
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Home towards targets.
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(960f, false);
            if (potentialTarget != null)
                Projectile.velocity = Projectile.SuperhomeTowardsTarget(potentialTarget, 23f, 10f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(TeslaCannon.FireSound, Projectile.Center);
            if (Main.myPlayer == Projectile.owner)
            {
                int boom = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MinionGaussBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                if (Main.projectile.IndexInRange(boom))
                {
                    Main.projectile[boom].ai[1] = 720f;
                    Main.projectile[boom].originalDamage = Projectile.originalDamage;
                }
            }
        }
    }
}
