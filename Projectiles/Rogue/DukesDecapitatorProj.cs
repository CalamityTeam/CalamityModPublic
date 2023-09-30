using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class DukesDecapitatorProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        float rotationAmount = 1.5f;
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DukesDecapitator";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            CalamityPlayer modPlayer = Main.player[Projectile.owner].Calamity();
            if (Projectile.velocity.X != 0 || Projectile.velocity.Y != 0)
            {
                Projectile.velocity *= 0.99f;
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] == 5f)
                Projectile.tileCollide = true;

            if ((Projectile.ai[0] % 15f) == 0f && rotationAmount > 0)
            {
                rotationAmount -= 0.05f;
                if (Projectile.Calamity().stealthStrike && Projectile.owner == Main.myPlayer)
                {
                    float velocityX = Main.rand.NextFloat(-0.8f, 0.8f);
                    float velocityY = Main.rand.NextFloat(-0.8f, -0.8f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, velocityX, velocityY, ModContent.ProjectileType<DukesDecapitatorBubble>(), (int)(Projectile.damage * 0.8), Projectile.knockBack, Projectile.owner);
                }
            }
            if (rotationAmount <= 0f)
                Projectile.Kill();

            Projectile.rotation += rotationAmount;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = Vector2.Zero;
            rotationAmount -= 0.05f;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 49, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 0, new Color(255, 255, 255), 0.75f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
