using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class BonebreakerProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -2;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.ai[0] == 0f)
            {
                Projectile.localAI[1] += 1f;
                if (Projectile.localAI[1] >= 45f)
                {
                    Projectile.velocity.X *= 0.98f;
                    Projectile.velocity.Y += 0.35f;
                }
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            //Sticky Behaviour
            Projectile.StickyProjAI(15);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => Projectile.ModifyHitNPCSticky(6);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 3; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 78, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 0, default, 0.8f);
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int s = 0; s < 2; s++)
                {
                    Vector2 velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (velocity.X == 0f && velocity.Y == 0f)
                    {
                        velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    velocity.Normalize();
                    velocity *= (float)Main.rand.Next(70, 101) * 0.1f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<BonebreakerFragment1>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack * 0.5f, Projectile.owner, Main.rand.Next(0,4), 0f);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.BoneJavelin, 180);
            target.AddBuff(BuffID.Venom, 90);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.BoneJavelin, 180);
            target.AddBuff(BuffID.Venom, 90);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1f)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.ai[0] != 1f;
    }
}
