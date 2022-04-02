using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class RadiantStar2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/RadiantStar";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 270 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            if (projectile.ai[0] == 1f)
            {
                float num472 = projectile.Center.X;
                float num473 = projectile.Center.Y;
                float num474 = projectile.Calamity().stealthStrike ? 1800f : 600f;
                float homingSpeed = 0.25f;
                for (int num475 = 0; num475 < Main.maxNPCs; num475++)
                {
                    if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                    {
                        float npcCenterX = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
                        float npcCenterY = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
                        float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - npcCenterX) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - npcCenterY);
                        if (num478 < num474)
                        {
                            if (Main.npc[num475].position.X < num472)
                            {
                                Main.npc[num475].velocity.X += homingSpeed;
                            }
                            else
                            {
                                Main.npc[num475].velocity.X -= homingSpeed;
                            }
                            if (Main.npc[num475].position.Y < num473)
                            {
                                Main.npc[num475].velocity.Y += homingSpeed;
                            }
                            else
                            {
                                Main.npc[num475].velocity.Y -= homingSpeed;
                            }
                        }
                    }
                }
            }
            else if (projectile.timeLeft < 270)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, projectile.Calamity().stealthStrike ? 900f : 450f, 12f, 20f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
