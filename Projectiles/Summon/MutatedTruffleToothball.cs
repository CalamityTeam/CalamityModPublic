using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MutatedTruffleToothball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public ref float TargetShotID => ref Projectile.ai[0];

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 120;
            Projectile.width = Projectile.height = 40;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            // Deacceleration.
            Projectile.velocity *= Utils.Remap(Projectile.timeLeft, 120f, 0f, 1f, 0.9f);

            Projectile.rotation += Projectile.velocity.Length() * .02f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.SourceDamage *= 1.25f;

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.TwoPi).SafeNormalize(Vector2.Zero) * (MutatedTruffle.ToothballSpikeSpeed - 15f), ModContent.ProjectileType<MutatedTruffleToothballSpike>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner, TargetShotID);
            }

            if (!Main.dedServ)
            {
                int dustAmount = 15;
                for (int dustIndex = 0; dustIndex < 15; dustIndex++)
                {
                    float angle = MathHelper.TwoPi / dustAmount * dustIndex;
                    Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(1f, 3f);
                    Dust.NewDustPerfect(Projectile.Center, 7, velocity);

                    Dust deathDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 7);
                    deathDust.noGravity = true;
                }

                SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            }

            Projectile.netUpdate = true;
            Projectile.netSpam = 0;
        }
    }
}
