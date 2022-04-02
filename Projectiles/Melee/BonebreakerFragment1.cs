using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Melee
{
    public class BonebreakerFragment1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shrapnel");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.aiStyle = 24;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.alpha = 50;
            projectile.timeLeft = 600;
            projectile.melee = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Changes the texture of the projectile
            if (projectile.ai[0] == 1f)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Melee/BonebreakerFragment2");
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            if (projectile.ai[0] == 2f)
            {
                Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Melee/BonebreakerFragment2");
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 60);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 60);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 60);
        }
    }
}
