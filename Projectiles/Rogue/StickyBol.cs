using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class StickyBol : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/StickySpikyBall";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            Color color = new Color(0, 80, 255, 100);
            if (Main.rand.NextBool(12))
            {
                Dust.NewDust(Projectile.position + Vector2.One * 6f, Projectile.width - 12, Projectile.height - 12, 4, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 175, color, 1.2f);
            }
            //Sticky Behaviour
            Projectile.StickyProjAI(10);
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => Projectile.ModifyHitNPCSticky(5);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Slimed, 120);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Slimed, 120);
    }
}
