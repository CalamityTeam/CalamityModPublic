using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Summon
{
    public class SiriusBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;

            Projectile.penetrate = -1;
            Projectile.extraUpdates = 220; // Random number to make it go fast.
            Projectile.localNPCHitCooldown = 110; // Random number so it doesn't multi-hit.
            Projectile.timeLeft = 1000; // Random number so it doesn't die too fast.

            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            for (int d = 0; d < 4; d++)
            {
                Vector2 projPos = Projectile.position;
                projPos -= Projectile.velocity * (d * 0.25f);
                Projectile.alpha = 255;
                int trailDust = Dust.NewDust(projPos, 1, 1, 20, 0f, 0f, 0, default, 1f);
                Main.dust[trailDust].position = projPos;
                Main.dust[trailDust].scale = Main.rand.Next(70, 110) * 0.013f;
                Main.dust[trailDust].velocity *= 0.2f;
                Main.dust[trailDust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 180);
            float x4 = Main.rgbToHsl(new Color(103, 203, Main.DiscoB)).X;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SiriusExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, x4, Projectile.whoAmI);
        }
    }
}
