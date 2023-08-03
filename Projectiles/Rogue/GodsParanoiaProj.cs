using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class GodsParanoiaProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/GodsParanoia";

        public int kunaiStabbing = 12;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.35f, 0f, 0.25f);
            if (Main.rand.NextBool(2))
            {
                Dust flame = Dust.NewDustDirect(Projectile.position, 1, 1, Main.rand.NextBool(3) ? 56 : 242, 0f, 0f, 0, default, 0.5f);
                flame.alpha = Projectile.alpha;
                flame.velocity = Vector2.Zero;
                flame.noGravity = true;
            }

            Projectile.StickyProjAI(50);

            if (Projectile.ai[0] == 1f)
            {
                kunaiStabbing++;
                if (kunaiStabbing >= 20)
                {
                    kunaiStabbing = 0;
                    float startOffsetX = Main.rand.NextFloat(100f, 200f) * (Main.rand.NextBool() ? -1f : 1f);
                    float startOffsetY = Main.rand.NextFloat(100f, 200f) * (Main.rand.NextBool() ? -1f : 1f);
                    Vector2 startPos = new Vector2(Projectile.position.X + startOffsetX, Projectile.position.Y + startOffsetY);
                    float dx = Projectile.position.X - startPos.X;
                    float dy = Projectile.position.Y - startPos.Y;

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
                    if (Projectile.owner == Main.myPlayer)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            int idx = Projectile.NewProjectile(Projectile.GetSource_FromThis(), startPos, kunaiSp, ModContent.ProjectileType<GodsParanoiaDart>(), Projectile.damage / 2, Projectile.knockBack / 2f, Projectile.owner, 0f, 0f);
                            Main.projectile[idx].rotation = angle;
                        }
                    }
                }
            }
            else
            {
                Projectile.rotation += 0.2f * (float)Projectile.direction;
                CalamityUtils.HomeInOnNPC(Projectile, true, 300f, Projectile.Calamity().stealthStrike ? 12f : 7f, 20f);
            }

            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.killSpikyBalls == true)
            {
                Projectile.active = false;
                Projectile.netUpdate = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => Projectile.ModifyHitNPCSticky(10);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
    }
}
