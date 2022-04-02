using CalamityMod.Buffs.DamageOverTime;
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
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.aiStyle = 2;
            projectile.timeLeft = 600;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<MythrilKnife>());
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!projectile.Calamity().stealthStrike)
                return;

            target.AddBuff(BuffID.CursedInferno, 300);
            target.AddBuff(BuffID.Venom, 300);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300);
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (!projectile.Calamity().stealthStrike)
                return;

            target.AddBuff(BuffID.CursedInferno, 300);
            target.AddBuff(BuffID.Venom, 300);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300);
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
        }
    }
}
