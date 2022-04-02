using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class AuroradicalStar : ModProjectile
    {
        public int[] dustTypes = new int[]
        {
            ModContent.DustType<AstralBlue>(),
            ModContent.DustType<AstralOrange>()
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auroradical Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.alpha = 100;
            projectile.penetrate = 1;
            projectile.timeLeft = 485;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            //Rotation
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;

            //Lighting
            Lighting.AddLight(projectile.Center, 0.3f, 0.5f, 0.1f);

            //sound effects
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 9);
                }
            }

            //Change the scale size a little bit to make it pulse in and out
            float scaleAmt = (float)Main.mouseTextColor / 200f - 0.35f;
            scaleAmt *= 0.2f;
            projectile.scale = scaleAmt + 0.95f;

            //Spawn dust
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 15f)
            {
                int astral = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.Next(dustTypes), 0f, 0f, 100, default, 0.8f);
                Main.dust[astral].noGravity = true;
                Main.dust[astral].velocity *= 0f;
            }

            //Home in
            float maxDistance = 500f;
            int targetIndex = -1;
            int i;
            for (i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(projectile, false))
                {
                    float extraDistance = (Main.npc[i].width / 2) + (Main.npc[i].height / 2);

                    if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance))
                    {
                        targetIndex = i;
                        break;
                    }
                }
            }
            if (targetIndex != -1)
            {
                Vector2 targetVec = Main.npc[i].Center - projectile.Center;
                if (targetVec.Length() < 60f)
                {
                    projectile.Kill();
                    return;
                }
                if (projectile.ai[0] >= 120f)
                {
                    projectile.ai[1] += 1f;
                    if (projectile.ai[1] < 120f)
                    {
                        float speedMult = 25f;
                        targetVec.Normalize();
                        targetVec *= speedMult;
                        projectile.velocity = (projectile.velocity * 15f + targetVec) / 16f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= speedMult;
                    }
                    else if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 18f)
                    {
                        projectile.velocity.X *= 1.01f;
                        projectile.velocity.Y *= 1.01f;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
            OnHitEffect(target.Center, target.width);
        }

        private void OnHitEffect(Vector2 targetPos, int width)
        {
            if (projectile.Calamity().stealthStrike && Main.myPlayer == projectile.owner)
            {
                Vector2 pos = new Vector2(targetPos.X + width * 0.5f + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                Vector2 velocity = (targetPos - pos) / 40f;
                int dmg = projectile.damage / 2;
                int comet = Projectile.NewProjectile(pos, velocity, ModContent.ProjectileType<CometQuasherMeteor>(), dmg, projectile.knockBack, projectile.owner);
                if (comet.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[comet].Calamity().forceRogue = true;
                    Main.projectile[comet].Calamity().lineColor = Main.rand.Next(3);
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
            OnHitEffect(target.Center, target.width);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 9, 1f, 0f);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 96);
            projectile.localNPCHitCooldown = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.Damage();
            for (int d = 0; d < 2; d++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.Next(dustTypes), 0f, 0f, 50, default, 1f);
            }
            for (int d = 0; d < 20; d++)
            {
                int astral = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.Next(dustTypes), 0f, 0f, 0, default, 1.5f);
                Main.dust[astral].noGravity = true;
                Main.dust[astral].velocity *= 3f;
                astral = Dust.NewDust(projectile.position, projectile.width, projectile.height, 173, 0f, 0f, 50, default, 1f);
                Main.dust[astral].velocity *= 2f;
                Main.dust[astral].noGravity = true;
            }
            for (int g = 0; g < 3; g++)
            {
                Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.05f, projectile.velocity.Y * 0.05f), Main.rand.Next(16, 18), 1f);
            }
        }
    }
}
