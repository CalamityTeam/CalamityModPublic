using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class NastyChollaBol : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/NastyCholla";

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -2;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 157, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            //Sticky Behaviour
            Projectile.StickyProjAI(15);
            if (Projectile.ai[0] != 1f)
            {
                Projectile.StickToTiles(true, false);
                Projectile.localAI[1] += 1f;
                if (Projectile.localAI[1] > 10f)
                {
                    Projectile.localAI[1] = 10f;
                    if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                    {
                        Projectile.velocity.X *= 0.97f;
                        if (Math.Abs(Projectile.velocity.X) < 0.01f)
                        {
                            Projectile.velocity.X = 0f;
                            Projectile.netUpdate = true;
                        }
                    }
                    Projectile.velocity.Y += 0.2f;
                }
                Projectile.rotation += Projectile.velocity.X * 0.1f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => Projectile.ModifyHitNPCSticky(3);
        public override bool? CanDamage() => Projectile.ai[0] == 1f ? false : base.CanDamage();

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        //So you can stick a bol up the Guide's ass
        public override bool? CanHitNPC(NPC target)
        {
            if (target.townNPC)
            {
                return true;
            }
            return null;
        }

        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            int needleAmt = Main.rand.Next(2, 4);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int n = 0; n < needleAmt; n++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    // Since damage is reset on stick, this needs new damage calculations or stored damage in some way.
                    int damage = Projectile.Calamity().stealthStrike ? Projectile.originalDamage / 2 : (int)player.GetTotalDamage<RogueDamageClass>().ApplyTo(0.5f * NastyCholla.BaseDamage);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<NastyChollaNeedle>(), damage, 0f, Projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
