using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Rogue
{
    public class PenumbraBomb : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Penumbra";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Penumbra Bomb");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 150;
            projectile.extraUpdates = 2;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            projectile.ai[0]++;
            //Hitbox Fixing
            if (projectile.direction == 1)
            {
                drawOffsetX = -4;
                drawOriginOffsetX = -5;
            }
            else
            {
                drawOffsetX = -11;
                drawOriginOffsetX = 5;
            }

            //Alpha
            if (projectile.alpha > 10)
                projectile.alpha -= 7;
            else
                projectile.alpha = 10;
            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = (projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi)) + (MathHelper.ToRadians(180)*projectile.direction);

            //Dust
            float dfreq = projectile.Calamity().stealthStrike ? 4f : 2f;
            if (projectile.ai[0] == dfreq)
            {
                Vector2 dustspeed = projectile.velocity * Main.rand.NextFloat(0.5f,0.8f);
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, dustspeed.X, dustspeed.Y, 0, new Color(38, 30, 43), 1.4f);
                Main.dust[d].velocity = dustspeed;
                if (projectile.Calamity().stealthStrike)
                {
                    Vector2 dustspeed2 = new Vector2 (Main.rand.NextFloat(-3f,3f),Main.rand.NextFloat(-3f,3f));
                    int d2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, dustspeed2.X, dustspeed2.Y, 0, new Color(38, 30, 43), 1.3f);
                    Main.dust[d2].velocity = dustspeed2;
                }
                projectile.ai[0] = 0f;
            }

        }

        public override void Kill(int timeLeft)
        {
            //Dark soul projectiles
            int ad = projectile.Calamity().stealthStrike ? 40 : 60;
            float dmgMult = projectile.Calamity().stealthStrike ? 0.08f : 0.15f;
            int randrot = Main.rand.Next(-30,31);
            for (int i = 0; i < 360; i += ad)
            {
                Vector2 SoulSpeed = new Vector2(13f, 13f).RotatedBy(MathHelper.ToRadians(i + randrot));
                Projectile.NewProjectile(projectile.Center, SoulSpeed, ModContent.ProjectileType<PenumbraSoul>(), (int)(projectile.damage * dmgMult), 3f, projectile.owner, 0f, 0f);
            }
            //Dust
            int maxDust = projectile.Calamity().stealthStrike ? 100 : 70;
            for (int i = 0; i < maxDust; i++)
            {
                Vector2 dustspeed = new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-6f, 6f));
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, dustspeed.X, dustspeed.Y, 0, new Color(38, 30, 43), 1.6f);
            }
            Main.PlaySound(SoundID.Item14, projectile.Center);
            projectile.width = 110;
            projectile.height = 110;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.damage /= 2;
            projectile.Damage();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Blackout, 300);
        }
    }
}
