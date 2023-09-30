using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class LeonidStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        private bool hasHit = false;
        private bool initialized = false;

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 3;
        }

        public override void AI()
        {
            if (!initialized)
            {
                Projectile.rotation += Main.rand.NextFloat();
                initialized = true;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 120f)
            {
                Projectile.scale *= 0.98f;
                Projectile.ExpandHitboxBy(Projectile.scale);
                if (Projectile.scale <= 0.05f)
                    Projectile.Kill();
            }
            Projectile.rotation += Projectile.direction * 0.05f;
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(9))
                {
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
            hasHit = true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
            hasHit = true;
        }

        public override bool? CanDamage() => !hasHit ? null : false;

        public override Color? GetAlpha(Color lightColor) => CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 2.5f);

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    ModContent.DustType<AstralOrange>(),
                    ModContent.DustType<AstralBlue>()
                });

                int dust = Dust.NewDust(Projectile.Center, 1, 1, dustType, Projectile.velocity.X, Projectile.velocity.Y, 0, CalamityUtils.ColorSwap(LeonidProgenitor.blueColor, LeonidProgenitor.purpleColor, 1f), 1.5f);
                Main.dust[dust].noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int num480 = 0; num480 < 3; num480++)
                {
                    Vector2 velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    velocity.SafeNormalize(default);
                    velocity *= Main.rand.Next(1, 6) * 0.01f;
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, velocity, Main.rand.Next(16, 18), 1f);
                }
            }
        }
    }
}
