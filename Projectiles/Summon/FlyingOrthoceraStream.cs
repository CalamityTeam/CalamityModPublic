using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class FlyingOrthoceraStream : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.MaxUpdates = 3;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 4f)
            {
                Projectile.tileCollide = true;
            }
            Projectile.scale -= 0.002f;
            if (Projectile.scale <= 0f)
            {
                Projectile.Kill();
            }
            if (Projectile.ai[0] <= 3f)
            {
                Projectile.ai[0] += 1f;
                return;
            }
            Projectile.velocity.Y += 0.03f;
            for (int i = 0; i < 3; i++)
            {
                Vector2 positionDelta = Projectile.velocity / 3f * i;
                int spawnDelta = 14;
                int dustIdx = Dust.NewDust(new Vector2(Projectile.position.X + spawnDelta, Projectile.position.Y + spawnDelta), Projectile.width - spawnDelta * 2, Projectile.height - spawnDelta * 2, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[dustIdx];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.5f;
                dust.position -= positionDelta;
            }
            if (Main.rand.NextBool(8))
            {
                int spawnDelta = 16;
                int dustIdx = Dust.NewDust(new Vector2(Projectile.position.X + spawnDelta, Projectile.position.Y + spawnDelta), Projectile.width - spawnDelta * 2, Projectile.height - spawnDelta * 2, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 0.5f);
                Main.dust[dustIdx].velocity *= 0.25f;
                Main.dust[dustIdx].velocity += Projectile.velocity * 0.5f;
            }
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 180);
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
