using Microsoft.Xna.Framework;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class BloodGeyser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Geyser");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.penetrate = 1;
            projectile.aiStyle = 1;
            aiType = 1;
        }

        public override void AI()
        {
            if (projectile.timeLeft < 180)
                projectile.tileCollide = true;

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;

            int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 5, 0f, 0f, 100, default, 0.5f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 120);
        }
    }
}
