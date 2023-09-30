using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class SkyfinNuke : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SkyfinBombers";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 720;
            Projectile.alpha = 0;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            const float turnSpeed = 20f;
            float speedMult = Projectile.Calamity().stealthStrike ? 9f : 12f;
            const float homingRange = 300f;
            if (!Projectile.Calamity().stealthStrike) //normal attack
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] > 30f) //0.5 seconds
                {
                    NPC target = Projectile.Center.ClosestNPCAt(homingRange);
                    // Ignore targets above the nuke
                    if (target != null)
                    {
                        if (target.Bottom.Y < Projectile.Top.Y)
                        {
                            target = null;
                        }
                    }
                    if (target != null)
                    {
                        Vector2 distNorm = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                        Projectile.velocity = (Projectile.velocity * (turnSpeed - 1f) + distNorm * speedMult) / turnSpeed;
                    }
                }
            }
            else
            {
                //More range
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, homingRange, speedMult, turnSpeed);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.extraUpdates = 0;
            //Dust
            for (int i = 0; i < 30;i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                Dust.NewDust(Projectile.Center, 1, 1, (int)CalamityDusts.SulfurousSeaAcid, dspeed.X, dspeed.Y, 0, default, 1.1f);
            }
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();

            int cloudAmt = Main.rand.Next(2, 5);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int c = 0; c < cloudAmt; c++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(50f, 10f, 50f, 0.01f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SkyBomberGas>(), (int)(Projectile.damage * 0.4), Projectile.knockBack * 0.4f, Projectile.owner);
                }
                if (Projectile.Calamity().stealthStrike)
                {
                    int explode = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BileExplosion>(), (int)(Projectile.damage * 0.4), Projectile.knockBack * 0.4f, Projectile.owner, 1f);
                    Main.projectile[explode].usesLocalNPCImmunity = true;
                    Main.projectile[explode].localNPCHitCooldown = 30;
                }
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Bottom);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
    }
}
