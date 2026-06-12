using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class NullFlash : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public ref float time => ref Projectile.ai[0];
        public Color baseColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (time == 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            float rate = (Main.GlobalTimeWrappedHourly * 5);
            List<Color> eColors = new List<Color>()
                {
                    Color.Turquoise,
                    Color.Orchid
                };
            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            if (!Main.zenithWorld)
                baseColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

            if (Projectile.ai[1] == 5 && !Main.zenithWorld)
                baseColor = Color.White;

            Projectile.velocity *= 0.95f;
            time++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player Owner = Main.player[Projectile.owner];
            if (Projectile.numHits == 0)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/NullImpact");
                SoundEngine.PlaySound(fire with { Volume = 0.5f, Pitch = 0.5f }, Projectile.Center);
            }

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, Owner.Calamity().mouseWorld);
            target.MoveNPC(launchVel, 20, true, Owner);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 1)
                return false;
            Vector2 placement = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 50;
            Asset<Texture2D> tex = ModContent.Request<Texture2D>("CalamityMod/Particles/VerticalSmear");
            for (int i = 0; i < 3; i++)
            {
                Vector2 scale = new Vector2(1 - (time * 0.025f) + i * 0.1f, 1 + (time * 0.025f) - i * 0.1f) * Projectile.scale * (0.7f - (i * 0.05f));
                Main.EntitySpriteDraw(tex.Value, placement - Main.screenPosition, null, baseColor with { A = 0 } * 0.5f, Projectile.rotation, tex.Size() * 0.5f, scale, SpriteEffects.None);
            }
            return false;
        }
    }
}
