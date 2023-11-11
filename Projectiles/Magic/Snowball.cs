using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class Snowball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.coldDamage = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.coldDamage = true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn2, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Projectile.alpha);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            //crystal bullet shards
            for (int i = 0; i < 10; ++i)
            {
                int snow = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 68, 0f, 0f, 0, new Color(), 1f);
                Main.dust[snow].noGravity = true;
                Main.dust[snow].velocity *= 2f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int j = 0; j < 6; ++j)
                {
                    float SpeedX = -Projectile.velocity.X * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    float SpeedY = -Projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + SpeedX, Projectile.Center.Y + SpeedY, SpeedX, SpeedY, ProjectileID.CrystalShard, Projectile.damage / 2, 0f, Projectile.owner);
                    Main.projectile[p].DamageType = DamageClass.Magic;
                }
            }
            //insert ice shattering dust here
        }
    }
}
