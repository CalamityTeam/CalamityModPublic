using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class UrchinStingerProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/UrchinStinger";

        private int projdmg = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stinger");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 2;
            projectile.aiStyle = 2;
            projectile.timeLeft = 600;
            aiType = ProjectileID.ThrowingKnife;
            projectile.Calamity().rogue = true;
        }

        public override bool PreAI()
        {
            if (projectile.Calamity().stealthStrike)
            {
                projectile.StickyProjAI(15);
                projectile.localAI[1]++;
                if (projectile.localAI[1] <= 20f && projectile.ai[0] != 1f)
                {
                    projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
                    projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * projectile.direction;
                }
                if (projectile.localAI[1] > 20f && projectile.ai[0] != 1f)
                {
                    projectile.velocity.Y += 0.4f;
                    projectile.velocity.X *= 0.97f;
                    if (projectile.velocity.Y > 16f)
                        projectile.velocity.Y = 16f;
                    projectile.rotation += 0.2f * projectile.direction;
                }
                if (projectile.localAI[0] % 40 == 0 && projectile.ai[0] == 1f)
                {
                    Vector2 projspeed = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                    int proj = Projectile.NewProjectile(projectile.Center, projspeed, ModContent.ProjectileType<SulphuricAcidBubbleFriendly>(), (int)(projdmg * 0.5f), 1f, projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().forceRogue = true;
                }
                return false;
            }
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.Calamity().stealthStrike)
            {
                projdmg = projectile.damage;
                projectile.ModifyHitNPCSticky(4, false);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projectile.Calamity().stealthStrike)
            {
                if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
                {
                    targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
                }
                return null;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Venom, 180);
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
                Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, ModContent.ItemType<UrchinStinger>());
            }
        }
    }
}
