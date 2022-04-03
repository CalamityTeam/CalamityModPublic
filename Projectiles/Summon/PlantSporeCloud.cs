using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlantSporeCloud : ModProjectile
    {
        private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spore Cloud");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.alpha = 60;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Projectile.velocity.X *= 0.985f;
            Projectile.velocity.Y *= 0.985f;
            Projectile.rotation += Projectile.direction * 0.05f;

            if (counter % 5 == 0)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 107, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);

            counter++;
            if (counter > 300)
                Projectile.alpha += 5;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            if (Projectile.ai[0] == 1f)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/PlantSporeCloud2");
            if (Projectile.ai[0] == 2f)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/PlantSporeCloud3");

            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 180);
        }
    }
}
