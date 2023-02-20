using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class FeatherKnifeProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/FeatherKnife";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Knife");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.timeLeft = 600;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 35 == 0)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X / 20, 2), ModContent.ProjectileType<StickyFeatherAero>(), (int)(Projectile.damage * 0.4), Projectile.knockBack, Projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].DamageType = RogueDamageClass.Instance;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, new Vector2(Projectile.velocity.X / 20, 2), ModContent.ProjectileType<StickyFeatherAero>(), (int)(Projectile.damage * 0.69), Projectile.knockBack, Projectile.owner);
            if (proj.WithinBounds(Main.maxProjectiles))
                Main.projectile[proj].DamageType = RogueDamageClass.Instance;
        }
    }
}
