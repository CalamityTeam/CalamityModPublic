using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class JawsProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaper Tooth");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0f)
            {
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
                if (projectile.Calamity().stealthStrike)
                {
                    int dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        33,
                        101,
                        111,
                        180
                    });

                    int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, projectile.velocity.X, projectile.velocity.Y, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }
            }
            //Sticky Behaviour
            projectile.StickyProjAI(15);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(6, false);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
            target.AddBuff(BuffID.Venom, 120);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 120);
            if (projectile.Calamity().stealthStrike)
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<JawsShockwave>(), (int)(100f * player.RogueDamage()), 10f, projectile.owner, 0, 0);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            Player player = Main.player[projectile.owner];
            target.AddBuff(BuffID.Venom, 120);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 120);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 120);
            if (projectile.Calamity().stealthStrike)
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 120);
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<JawsShockwave>(), (int)(100f * player.RogueDamage()), 10f, projectile.owner, 0, 0);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            projectile.Kill();
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    33,
                    101,
                    111,
                    180
                });

                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, 0, 0, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
