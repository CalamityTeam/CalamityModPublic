using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Astral
{
    public class AstralScytheProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Ring");
            Main.projFrames[projectile.type] = 3;
        }
    	
        public override void SetDefaults()
        {
            projectile.width = 72;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.melee = true;
            projectile.timeLeft = 300;
			projectile.penetrate = 8;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            projectile.velocity *= 1.03f;

            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame > 2)
                {
                    projectile.frame = 0;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item1, projectile.Center);

            for (int i = 0; i < 60; i++)
            {
                float angle = MathHelper.TwoPi * Main.rand.NextFloat(0f, 1f);
                Vector2 angleVec = angle.ToRotationVector2();
                float distance = Main.rand.NextFloat(14f, 36f);
                Vector2 off = angleVec * distance;
                off.Y *= ((float)projectile.height / projectile.width);
                Vector2 pos = projectile.Center + off;
                Dust d = Dust.NewDustPerfect(pos, mod.DustType("AstralBlue"), angleVec * Main.rand.NextFloat(2f, 4f));
                d.customData = true;
            }
        }
    }
}