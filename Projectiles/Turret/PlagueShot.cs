using Microsoft.Xna.Framework;
using CalamityMod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.DraedonsArsenal;

namespace CalamityMod.Projectiles.Turret
{
    public class PlagueShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/Ranged/GoliathRocket";
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 110;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.hide = true;
        }

        public override bool PreAI()
        {
            // If projectile knockback is set to 0 in the tile entity file, projectile hits players instead
            // This is used to check if the projectile came from the hostile version of the tile entity
            if (Projectile.knockBack == 0f)
                Projectile.hostile = true;
            else Projectile.friendly = true;
            return true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.localAI[0] == 0f)
            {
                // play a sound frame 1.
                SoundEngine.PlaySound(SoundID.Item61 with { Volume = 0.3f }, Projectile.position);
            }
            else Projectile.hide = false; //hide projectile for frame 1

            Projectile.localAI[0]++;
            if (Projectile.friendly)
                CalamityUtils.HomeInOnNPC(Projectile, false, 180f, 12f, 0f);
            Projectile.velocity = ((Projectile.oldVelocity*7f) + Projectile.velocity) / 8; //inertia
            DrawParticles();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 60);

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 60);
            if (Projectile.hostile && Main.netMode == NetmodeID.MultiplayerClient) //hostile version pierces through players in multiplayer
                return;
            Projectile.Kill();
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White;
        }
        public void DrawParticles()
        {
            if (Projectile.localAI[0] < 3f)
                return;
            Vector2 bloodSpawnPosition = Projectile.Center + (Vector2.UnitY * 13f).RotatedBy(Projectile.rotation);
            Vector2 splatterDirection = -(Projectile.Center - bloodSpawnPosition).SafeNormalize(Vector2.UnitY);
            int bloodLifetime = Main.rand.Next(8, 13);
            float bloodScale = Main.rand.NextFloat(0.6f, 0.9f);
            Color bloodColor = Color.Lerp(Color.Green, Color.DarkGreen, Main.rand.NextFloat());
            bloodColor = Color.Lerp(bloodColor, new Color(55, 125, 11), Main.rand.NextFloat(0.65f));

            if (Main.rand.NextBool(20))
                bloodScale *= 1.7f;

            Vector2 bloodVelocity = splatterDirection.RotatedByRandom(0.81f) * Main.rand.NextFloat(3f, 6f);
            bloodVelocity.Y -= 0.5f;
            BloodParticle blood = new BloodParticle(bloodSpawnPosition, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
            GeneralParticleHandler.SpawnParticle(blood);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(TeslaCannon.FireSound with { Volume = 0.18f }, Projectile.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0.4f, ModContent.ProjectileType<PlagueExplosionGas>(), (int)(Projectile.damage * 0.25f), Projectile.knockBack * 0.16f, Main.myPlayer);
            }
        }
    }
}
