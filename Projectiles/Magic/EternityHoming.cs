using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class EternityHoming : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eternity");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 120;
            projectile.alpha = 255;
            projectile.magic = true;
        }

        public override void AI()
        {
            NPC target = null;
            float distance = 4400f;
            for (int index = 0; index < Main.npc.Length; index++)
            {
                if (Main.npc[index].CanBeChasedBy(null, false))
                {
                    if (Main.npc[index].boss && Vector2.Distance(projectile.Center, Main.npc[index].Center) <= 4400f)
                    {
                        target = Main.npc[index];
                        break;
                    }
                    if (Vector2.Distance(projectile.Center, Main.npc[index].Center) < distance)
                    {
                        distance = Vector2.Distance(projectile.Center, Main.npc[index].Center);
                        target = Main.npc[index];
                    }
                }
            }
            if (target != null)
                projectile.velocity = (projectile.velocity * 7f + projectile.SafeDirectionTo(target.Center) * 10f) / 8f;

            projectile.ai[0] += 0.18f;
            float angle = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            float pulse = (float)Math.Sin(projectile.ai[0]);
            float radius = 10f;
            Vector2 offset = angle.ToRotationVector2() * pulse * radius;
            Dust dust = Dust.NewDustPerfect(projectile.Center + offset, Eternity.DustID, Vector2.Zero, 0, Eternity.BlueColor);
            dust.noGravity = true;

            dust = Dust.NewDustPerfect(projectile.Center - offset, Eternity.DustID, Vector2.Zero, 0, Eternity.BlueColor);
            dust.noGravity = true;
        }
    }
}