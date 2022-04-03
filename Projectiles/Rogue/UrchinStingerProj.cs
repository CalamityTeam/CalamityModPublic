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
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.aiStyle = 2;
            Projectile.timeLeft = 600;
            aiType = ProjectileID.ThrowingKnife;
            Projectile.Calamity().rogue = true;
        }

        public override bool PreAI()
        {
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.StickyProjAI(15);
                Projectile.localAI[1]++;
                if (Projectile.localAI[1] <= 20f && Projectile.ai[0] != 1f)
                {
                    Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
                    Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * Projectile.direction;
                }
                if (Projectile.localAI[1] > 20f && Projectile.ai[0] != 1f)
                {
                    Projectile.velocity.Y += 0.4f;
                    Projectile.velocity.X *= 0.97f;
                    if (Projectile.velocity.Y > 16f)
                        Projectile.velocity.Y = 16f;
                    Projectile.rotation += 0.2f * Projectile.direction;
                }
                if (Projectile.localAI[0] % 40 == 0 && Projectile.ai[0] == 1f)
                {
                    Vector2 projspeed = new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f));
                    int proj = Projectile.NewProjectile(Projectile.Center, projspeed, ModContent.ProjectileType<SulphuricAcidBubbleFriendly>(), (int)(projdmg * 0.5f), 1f, Projectile.owner);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].Calamity().forceRogue = true;
                }
                return false;
            }
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.Calamity().stealthStrike)
            {
                projdmg = Projectile.damage;
                Projectile.ModifyHitNPCSticky(4, false);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.Calamity().stealthStrike)
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
            Texture2D tex = Main.projectileTexture[Projectile.type];
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.rand.NextBool(2))
            {
                Item.NewItem((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height, ModContent.ItemType<UrchinStinger>());
            }
        }
    }
}
