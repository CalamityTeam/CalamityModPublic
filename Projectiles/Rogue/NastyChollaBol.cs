using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class NastyChollaBol : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/NastyCholla";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nasty Cholla");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 200;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 157, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            //Sticky Behaviour
            projectile.StickyProjAI(15);
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
            projectile.ModifyHitNPCSticky(20, false);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        //So you can stick a bol up the Guide's ass
        public override bool? CanHitNPC(NPC target)
        {
            if (target.townNPC)
            {
                return true;
            }
            return null;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[projectile.owner];
            int needleAmt = Main.rand.Next(2, 4);
            if (projectile.owner == Main.myPlayer)
            {
                for (int n = 0; n < needleAmt; n++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    int shard = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<NastyChollaNeedle>(), (int)((NastyCholla.BaseDamage/4) * player.RogueDamage()), 0f, projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
