
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Rogue
{
    public class MetalShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Metal Shard");
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 12;
            projectile.height = 12;
            projectile.Calamity().rogue = true;
            projectile.penetrate = 10;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            //Rotation
            if (projectile.ai[0] == 0f)
                projectile.rotation += 0.1f;
            //Gravity
            projectile.velocity.Y += 0.1f;
            if (projectile.velocity.Y > 16f)
                projectile.velocity.Y = 16f;
            //Sticky Behaviour
            projectile.StickyProjAI(15);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(8 , true);
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
            Texture2D texture;
            if (projectile.localAI[1] == 0f)
                projectile.localAI[1] = Main.rand.Next(1, 4);
            switch (projectile.localAI[1])
            {

                case 2f: texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/MetalShard2");
                         break;
                case 3f: texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/MetalShard3");
                         break;
                default: texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/MetalShard");
                         break;
            }
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDust(projectile.Center, projectile.width, projectile.height, DustID.Lead, 0f, 0f, 0, default, 1f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Main.PlaySound(SoundID.Dig, projectile.Center);
            return true;
        }
    }
}
