using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class RadiantStarKnife : ModProjectile
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
            projectile.penetrate = 4;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            if (projectile.ai[0] == 1f)
            {
                float num472 = projectile.Center.X;
                float num473 = projectile.Center.Y;
                float num474 = projectile.Calamity().stealthStrike ? 1800f : 600f;
                float homingSpeed = 0.25f;
                for (int num475 = 0; num475 < Main.npc.Length; num475++)
                {
                    NPC npc = Main.npc[num475];
                    if (npc.CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1) && !npc.boss)
                    {
                        float num476 = npc.position.X + (float)(npc.width / 2);
                        float num477 = npc.position.Y + (float)(npc.height / 2);
                        float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
                        if (num478 < num474)
                        {
                            if (npc.position.X < num472)
                            {
                                npc.velocity.X += homingSpeed;
                            }
                            else
                            {
                                npc.velocity.X -= homingSpeed;
                            }
                            if (npc.position.Y < num473)
                            {
                                npc.velocity.Y += homingSpeed;
                            }
                            else
                            {
                                npc.velocity.Y -= homingSpeed;
                            }
                        }
                    }
                }
            }
            projectile.ai[1] += 1f;
            if (projectile.ai[1] == 25f)
            {
                int numProj = projectile.Calamity().stealthStrike ? 5 : 3;
                float rotation = MathHelper.ToRadians(50);
                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                        while (speed.X == 0f && speed.Y == 0f)
                        {
                            speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                        }
                        speed.Normalize();
                        speed *= (float)Main.rand.Next(30, 61) * 0.1f * 2.5f;
                        int stabber2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, ModContent.ProjectileType<RadiantStar2>(), (int)(projectile.damage * 0.5f), projectile.knockBack, projectile.owner,
                            projectile.ai[0] == 1f ? 1f : 0f, 0f);
                        Main.projectile[stabber2].Calamity().stealthStrike = projectile.Calamity().stealthStrike;
                    }
                    Main.PlaySound(SoundID.Item14, projectile.position);
                    int boomer = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<RadiantExplosion>(), (int)(projectile.damage * 0.75f), projectile.knockBack, projectile.owner);
                    if (projectile.Calamity().stealthStrike && boomer.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[boomer].Calamity().stealthStrike = true;
                        Main.projectile[boomer].height = 300;
                        Main.projectile[boomer].width = 300;
                    }
                    Main.projectile[boomer].Center = projectile.Center;
                    projectile.active = false;
                }
            }
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
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<AstralBlue>(), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
