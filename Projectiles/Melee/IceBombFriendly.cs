using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class IceBombFriendly : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Boss/IceBomb";
        public ref float Timer => ref Projectile.ai[0];
        public const float FormTime = 60f;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.scale = 0.5f;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[1]];

            // Rotate around the hit enemy
            if (!npc.active)
                Projectile.Kill();
            else
            {
                Vector2 centerOffset = new Vector2((npc.height < npc.width ? npc.height : npc.width) * 0.7f, 0f);
                // Enforce a minimum offset so that it doesn't look weird on small enemies
                centerOffset.X = MathHelper.Max(centerOffset.X, 45f);
                Projectile.Center = npc.Center + centerOffset.RotatedBy((MathHelper.Pi / 30f * Timer) + (MathHelper.PiOver2 * Projectile.ai[2]));
            }

            // Constantly pulse in size and transparency
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.scale += 0.01f;
                Projectile.alpha -= 50;
                if (Projectile.alpha <= 0)
                {
                    Projectile.localAI[0] = 1f;
                    Projectile.alpha = 0;
                }
            }
            else
            {
                Projectile.scale -= 0.01f;
                Projectile.alpha += 50;
                if (Projectile.alpha >= 255)
                {
                    Projectile.localAI[0] = 0f;
                    Projectile.alpha = 255;
                }
            }

            // Once out for long enough, materialize
            Timer++;
            if (Timer == FormTime)
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust icyDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceRod, 0f, 0f, 100, default, 2f);
                    icyDust.velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        icyDust.scale = 0.5f;
                        icyDust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 14; j++)
                {
                    Dust icyDust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceRod, 0f, 0f, 100, default, 3f);
                    icyDust2.noGravity = true;
                    icyDust2.velocity *= 5f;
                    icyDust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceRod, 0f, 0f, 100, default, 2f);
                    icyDust2.velocity *= 2f;
                }

                Projectile.scale = 1f;
                Projectile.ExpandHitboxBy((int)(30f * Projectile.scale));
                SoundEngine.PlaySound(SoundID.Item30, Projectile.Center);
            }
        }

        public override Color? GetAlpha(Color lightColor) => Main.dayTime ? new Color(50, 50, 255, Projectile.alpha) : new Color(255, 255, 255, Projectile.alpha);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 shardVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(2.25f, 4.5f);
                    int iceShard = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shardVelocity, ModContent.ProjectileType<FrostShardFriendly>(), (int)(Projectile.damage * 0.5), 0f, Projectile.owner, 0f, 0f);
                    if (iceShard.WithinBounds(Main.maxProjectiles))
                        Main.projectile[iceShard].DamageType = DamageClass.Melee;
                }
            }

            for (int k = 0; k < 3; k++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.IceRod, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Frozen, 30);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
