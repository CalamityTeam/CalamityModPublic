using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SoulPiercerBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 9f)
            {
                Vector2 projPos = Projectile.position;
                projPos -= Projectile.velocity;
                Projectile.alpha = 255;
                int godSlay = Dust.NewDust(projPos, 1, 1, 173, 0f, 0f, 0, default, 0.5f);
                Main.dust[godSlay].position = projPos;
                Main.dust[godSlay].scale = Main.rand.Next(70, 110) * 0.014f;
                Main.dust[godSlay].velocity *= 0.2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);

            var source = Projectile.GetSource_FromThis();
            for (int x = 0; x < 5; x++)
            {
                if (Projectile.owner == Main.myPlayer && Projectile.numHits == 0)
                    CalamityUtils.ProjectileBarrage(source, Projectile.Center, target.Center, true, -500f, 500f, 0f, 500f, 10f, ModContent.ProjectileType<SoulPiercerBolt>(), Projectile.damage, 0f, Projectile.owner, false, 0f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 180);
    }
}
