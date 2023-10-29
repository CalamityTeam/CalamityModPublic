using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Rogue
{
    public class DesecratedWaterProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/DesecratedWater";

        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.timeLeft = 300;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.position);
            Vector2 vector2 = new Vector2(20f, 20f);
            for (int index = 0; index < 10; ++index)
                Dust.NewDust(Projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 191, 0.0f, 0.0f);
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 179, 0.0f, 0.0f, 100, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity = dust1.velocity * 5f;
                int index3 = Dust.NewDust(Projectile.Center - vector2 / 2f, (int) vector2.X, (int) vector2.Y, 179, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust2 = Main.dust[index3];
                dust2.velocity = dust2.velocity * 3f;
            }
            int numProj = (Projectile.Calamity().stealthStrike ? 12 : 5);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int j = 0; j < numProj; j++)
                {
                    Vector2 bubbleVel = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    bubbleVel.Normalize();
                    bubbleVel *= (float)Main.rand.Next(10, 201) * 0.01f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, bubbleVel.X, bubbleVel.Y, ModContent.ProjectileType<DesecratedBubble>(), (int)(Projectile.damage * 0.6), Projectile.knockBack * 0.6f, Projectile.owner, (Projectile.Calamity().stealthStrike ? 1f : 0f), 1f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
