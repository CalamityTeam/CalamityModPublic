using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class StickyBol : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/StickySpikyBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sticky Bol");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            Color color = new Color(0, 80, 255, 100);
            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(projectile.position + Vector2.One * 6f, projectile.width - 12, projectile.height - 12, 4, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 175, color, 1.2f);
            }
            //Sticky Behaviour
            projectile.StickyProjAI(10);
            if (projectile.ai[0] != 1f)
            {
                projectile.StickToTiles(true, false);
                projectile.localAI[1] += 1f;
                if (projectile.localAI[1] > 10f)
                {
                    projectile.localAI[1] = 10f;
                    if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                    {
                        projectile.velocity.X *= 0.97f;
                        if (Math.Abs(projectile.velocity.X) < 0.01f)
                        {
                            projectile.velocity.X = 0f;
                            projectile.netUpdate = true;
                        }
                    }
                    projectile.velocity.Y += 0.2f;
                }
                projectile.rotation += projectile.velocity.X * 0.1f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(5, true);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 120);
        }
    }
}
