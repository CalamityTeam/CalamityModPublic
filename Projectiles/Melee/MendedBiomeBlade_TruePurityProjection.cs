using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class TruePurityProjection : ModProjectile, ILocalizedModType //The boring plain one. With cool homing now
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public NPC target;
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/Melee/BrokenBiomeBlade_PurityProjection";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            AIType = ProjectileID.LightBeam;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = TrueBiomeBlade.DefaultAttunement_BeamTime;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {

            if (target == null)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ProjectileType<PurityProjectionSigil>() && proj.owner == Owner.whoAmI)
                    {
                        target = Main.npc[(int)proj.ai[0]];
                        break;
                    }
                }
            }
            else if ((Projectile.Center - target.Center).Length() >= (Projectile.Center + Projectile.velocity - target.Center).Length() && CalamityUtils.AngleBetween(Projectile.velocity, target.Center - Projectile.Center) < TrueBiomeBlade.DefaultAttunement_HomingAngle) //Home in
            {
                Projectile.timeLeft = 30; //Remain alive
                float angularTurnSpeed = MathHelper.ToRadians(MathHelper.Lerp(12.5f, 2.5f, MathHelper.Clamp(Projectile.Distance(target.Center) / 10f, 0f, 1f)));
                float idealDirection = Projectile.AngleTo(target.Center);
                float updatedDirection = Projectile.velocity.ToRotation().AngleTowards(idealDirection, angularTurnSpeed);
                Projectile.velocity = updatedDirection.ToRotationVector2() * Projectile.velocity.Length();
            }

            if (Projectile.timeLeft < TrueBiomeBlade.DefaultAttunement_BeamTime - 5f)
                Projectile.tileCollide = true;

            Lighting.AddLight(Projectile.Center, 0.75f, 1f, 0.24f);
            int dustParticle = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 75, 0f, 0f, 100, default, 0.9f);
            Main.dust[dustParticle].noGravity = true;
            Main.dust[dustParticle].velocity *= 0.5f;
            Main.dust[dustParticle].velocity += Projectile.velocity * 0.1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > TrueBiomeBlade.DefaultAttunement_BeamTime - 5f)
                return false;

            DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item43, Projectile.Center);
            for (int i = 0; i <= 15; i++)
            {
                Vector2 displace = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * (-0.5f + (i / 15f)) * 88f;
                int dustParticle = Dust.NewDust(Projectile.Center + displace, Projectile.width, Projectile.height, 75, 0f, 0f, 100, default, 2f);
                Main.dust[dustParticle].noGravity = true;
                Main.dust[dustParticle].velocity = Projectile.oldVelocity;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int debuffTime = 90;
            target.AddBuff(BuffType<ArmorCrunch>(), debuffTime);
        }
    }
}
