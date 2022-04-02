using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class GodsParanoiaProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/GodsParanoia";

        public int kunaiStabbing = 12;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God's Paranoia");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 15;
            projectile.height = 15;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.35f, 0f, 0.25f);
            if (Main.rand.NextBool(2))
            {
                Dust flame = Dust.NewDustDirect(projectile.position, 1, 1, Main.rand.NextBool(3) ? 56 : 242, 0f, 0f, 0, default, 0.5f);
                flame.alpha = projectile.alpha;
                flame.velocity = Vector2.Zero;
                flame.noGravity = true;
            }

            projectile.StickyProjAI(50);

            if (projectile.ai[0] == 1f)
            {
                kunaiStabbing++;
                if (kunaiStabbing >= 20)
                {
                    kunaiStabbing = 0;
                    float startOffsetX = Main.rand.NextFloat(100f, 200f) * (Main.rand.NextBool() ? -1f : 1f);
                    float startOffsetY = Main.rand.NextFloat(100f, 200f) * (Main.rand.NextBool() ? -1f : 1f);
                    Vector2 startPos = new Vector2(projectile.position.X + startOffsetX, projectile.position.Y + startOffsetY);
                    float dx = projectile.position.X - startPos.X;
                    float dy = projectile.position.Y - startPos.Y;

                    // Add some randomness / inaccuracy
                    dx += Main.rand.NextFloat(-5f, 5f);
                    dy += Main.rand.NextFloat(-5f, 5f);
                    float speed = Main.rand.NextFloat(20f, 25f);
                    float dist = (float)Math.Sqrt((double)(dx * dx + dy * dy));
                    dist = speed / dist;
                    dx *= dist;
                    dy *= dist;
                    Vector2 kunaiSp = new Vector2(dx, dy);
                    float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                    if (projectile.owner == Main.myPlayer)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int idx = Projectile.NewProjectile(startPos, kunaiSp, ModContent.ProjectileType<GodsParanoiaDart>(), projectile.damage / 2, projectile.knockBack / 2f, projectile.owner, 0f, 0f);
                            Main.projectile[idx].rotation = angle;
                        }
                    }
                }
            }
            else
            {
                projectile.rotation += 0.2f * (float)projectile.direction;
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 300f, projectile.Calamity().stealthStrike ? 12f : 7f, 20f);
            }

            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.killSpikyBalls == true)
            {
                projectile.active = false;
                projectile.netUpdate = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(10, true);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }
    }
}
