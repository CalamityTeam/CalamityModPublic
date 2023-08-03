using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class LionfishProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Lionfish";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.timeLeft = CalamityUtils.SecondsToFrames(20f);
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
                Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
                Projectile.ai[1] += 1f;
                if (Projectile.ai[1] >= 45f)
                {
                    float horizontalMult = 0.98f;
                    float fallSpeed = 0.35f;
                    Projectile.velocity.X *= horizontalMult;
                    Projectile.velocity.Y += fallSpeed;
                }
                if (Projectile.Calamity().stealthStrike)
                {
                    if (Projectile.timeLeft % 8 == 0 && Projectile.owner == Main.myPlayer)
                    {
                        Vector2 velocity = Projectile.DirectionFrom(Main.player[Projectile.owner].Center);
                        velocity *= Main.rand.NextFloat(4.5f, 6.5f);
                        velocity = velocity.RotatedBy((Main.rand.NextDouble() - 0.5) * Math.PI * 0.5, default);
                        int spike = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<UrchinSpikeFugu>(), (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.5f, Projectile.owner, -10f, 0f);
                        if (spike.WithinBounds(Main.maxProjectiles))
                            Main.projectile[spike].DamageType = RogueDamageClass.Instance;
                    }
                }
            }
            //Sticky Behaviour
            Projectile.StickyProjAI(15);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => Projectile.ModifyHitNPCSticky(6);
        public override bool? CanDamage() => Projectile.ai[0] == 1f ? false : base.CanDamage();

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
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.ExpandHitboxBy(72);
            for (int d = 0; d < 3; d++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
            }
            for (int d = 0; d < 30; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == NPCID.KingSlime || target.type == NPCID.WallofFlesh || target.type == NPCID.WallofFleshEye ||
                target.type == NPCID.SkeletronHead || target.type == NPCID.SkeletronHand)
            {
                target.buffImmune[BuffID.Venom] = false;
            }
            target.AddBuff(BuffID.Venom, 240);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Venom, 240);
    }
}
