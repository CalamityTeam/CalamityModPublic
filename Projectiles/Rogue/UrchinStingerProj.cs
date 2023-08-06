using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class UrchinStingerProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/UrchinStinger";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.timeLeft = 600;
            AIType = ProjectileID.ThrowingKnife;
            Projectile.DamageType = RogueDamageClass.Instance;
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
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, projspeed, ModContent.ProjectileType<SulphuricAcidBubbleFriendly>(), (int)(Projectile.damage * 0.5f), 1f, Projectile.owner, 2f);
                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].DamageType = RogueDamageClass.Instance;
                }
                return false;
            }
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.Calamity().stealthStrike)
                Projectile.ModifyHitNPCSticky(4);
        }

        public override bool? CanDamage() => Projectile.ai[0] == 1f ? false : base.CanDamage();

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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Poisoned, 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Poisoned, 180);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
