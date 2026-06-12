using System;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class WhitewaterAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref float time => ref Projectile.ai[0];
        public float fade = 0;
        public float areaScale = 1;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile.rotation += Main.rand.NextFloat(0.09f, 0.23f);
            areaScale = (Math.Abs((float)Math.Sin(time * 0.175f / MathHelper.Pi)) * 0.05f) + 1;
            for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
            {
                Player player = Main.player[playerIndex];
                float targetDist = Vector2.Distance(player.Center, Projectile.Center);

                if (targetDist < 200f * areaScale && player.Calamity().whitewaterHeal == 0)
                {
                    player.Calamity().whitewaterHeal = (player.whoAmI == Projectile.owner ? 300 : 600);
                }
            }

            if (Projectile.timeLeft < 30)
            {
                fade = MathHelper.Lerp(fade, 0, 0.12f);
                areaScale = MathHelper.Lerp(areaScale, 0, 0.12f);
            }
            if (time < 30)
                fade = MathHelper.Lerp(fade, 1, 0.12f);
            else if (Projectile.timeLeft > 30)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(200f * areaScale, 200f * areaScale), DustID.RainbowTorch);
                    dust.scale = Main.rand.NextFloat(0.3f, 0.7f);
                    dust.velocity = Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1f);
                    dust.color = Color.LightBlue;
                    dust.noGravity = true;
                }
            }
            time++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Owner = Main.player[Projectile.owner];
            target.AddBuff(BuffID.Wet, 300);
            target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 300);

            Vector2 launchVel = Utils.DirectionTo(Projectile.Center, target.Center);
            target.MoveNPC(launchVel, 9, true, Owner);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Particles/HighResFoggyCircleHardEdge").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("CalamityMod/Particles/SoftRoundExplosion").Value;
            Color drawColor2 = Color.LightBlue;
            float rotMult = CalamityClientConfig.Instance.Photosensitivity ? 0f : 1f;

            float opacityMult = CalamityClientConfig.Instance.Photosensitivity ? 0.33f : 1f;

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, drawColor2 with { A = 0 } * opacityMult, 0, tex.Size() / 2f, 0.2f * fade * areaScale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(tex2, Projectile.Center - Main.screenPosition, null, drawColor2 with { A = 0 } * 0.3f * opacityMult, Projectile.rotation * rotMult, tex2.Size() / 2f, 0.2f * fade * areaScale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(tex2, Projectile.Center - Main.screenPosition, null, drawColor2 with { A = 0 } * 0.3f * opacityMult, -Projectile.rotation * rotMult, tex2.Size() / 2f, 0.2f * fade * areaScale, SpriteEffects.None, 0);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 200 * areaScale, targetHitbox);
    }
}
