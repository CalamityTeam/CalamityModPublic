using CalamityMod.Dusts;
using CalamityMod.Items.SummonItems.Invasion;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class BrokenWaterFilterSpewer : ModProjectile
    {
        public const int TimeLeft = 180;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.timeLeft = TimeLeft;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.velocity *= 0.98f;
            int totalTearCount = projectile.wet ? 5 : 3;
            totalTearCount++;
            if (projectile.ai[0]++ >= TimeLeft / totalTearCount)
            {
                Item.NewItem(projectile.Center, ModContent.ItemType<CausticTear>());
                projectile.ai[0] = 0;
            }
            projectile.rotation = projectile.rotation.AngleTowards(MathHelper.PiOver4, 0.04f);
            Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.SulfurousSeaAcid);
            dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1.5f, 3f);
            dust.noGravity = true;
            dust.scale = 1.3f;
        }
        public override bool CanDamage() => false;
    }
}
