using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class MythrilKnifeProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/MythrilKnife";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Knife");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.timeLeft = 600;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override void AI()
        {
            if (Projectile.Calamity().stealthStrike)
            {
                if (Main.rand.NextBool(7))
                {
                    int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 171, 0.0f, 0.0f, 100, new Color(), 1f);
                    Main.dust[index].noGravity = true;
                    Main.dust[index].fadeIn = 1.5f;
                    Main.dust[index].velocity *= 0.25f;
                }
                if (Main.rand.NextBool(5))
                {
                    int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 46, 0.0f, 0.0f, 100, new Color(), 1f);
                    Main.dust[index].noGravity = true;
                    Main.dust[index].fadeIn = 1.5f;
                    Main.dust[index].velocity *= 0.25f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!Projectile.Calamity().stealthStrike)
                return;

            target.AddBuff(BuffID.Poisoned, 480);
            target.AddBuff(BuffID.Venom, 480);
            target.AddBuff(ModContent.BuffType<Irradiated>(), 480);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (!Projectile.Calamity().stealthStrike)
                return;

            target.AddBuff(BuffID.Poisoned, 480);
            target.AddBuff(BuffID.Venom, 480);
            target.AddBuff(ModContent.BuffType<Irradiated>(), 480);
        }
    }
}
