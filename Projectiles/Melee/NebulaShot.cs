using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Melee
{
    public class NebulaShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item12, Projectile.position);
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 15;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Lighting.AddLight(Projectile.Center, 0.4f, 0.2f, 0.4f);
            for (int i = 0; i < 5; i++)
            {
                Dust dustSpawn = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? 56 : 242, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f)];
                dustSpawn.velocity = Vector2.Zero;
                dustSpawn.position -= Projectile.velocity / 5f * (float)i;
                dustSpawn.noGravity = true;
                dustSpawn.scale = 0.8f;
                dustSpawn.noLight = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 90);
        }
    }
}
