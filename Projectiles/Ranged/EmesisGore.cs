using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Ranged
{
    public class EmesisGore : ModProjectile
    {
        public int HurtCounter = 0;
        public const int HurtTimeIncrement = 10;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotten Gore");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HurtCounter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HurtCounter = reader.ReadInt32();
        }
        public override void AI()
        {
            Projectile.StickyProjAI(15);

            // Override the default DOT used in the method above.
            if (Projectile.ai[0] == 1f)
            {
                Projectile.localAI[0] = 5f;
                Projectile.velocity = Vector2.Zero;
                HurtCounter++;
                if (HurtCounter % HurtTimeIncrement == 0)
                {
                    Main.npc[(int)Projectile.ai[1]].HitEffect(0, 50.0);
                }
            }
            else
            {
                Projectile.rotation += (Projectile.velocity.X > 0).ToDirectionInt() * MathHelper.ToRadians(8f);
            }
            if (Projectile.timeLeft % 12 == 11)
            {
                for (int i = 0; i < (Projectile.ai[0] == 1f ? 3 : 1); i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, 10, 10, 27);
                    dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi);
                    dust.noGravity = true;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Projectile.ModifyHitNPCSticky(8, true);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 60);
        }
    }
}
