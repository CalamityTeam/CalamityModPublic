using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Graphics.Metaballs;

namespace CalamityMod.Projectiles.Typeless
{
    public class VoidFieldGenerator : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public bool start = true;
        public StreamGougeMetaball.CosmicParticle VoidAura;

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft *= 5;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (!modPlayer.voidField)
            {
                Projectile.active = false;
                return;
            }
            if (player.dead)
                modPlayer.voidField = false;
            if (modPlayer.voidField)
                Projectile.timeLeft = 2;

            if (Projectile.ai[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            }

            Vector2 vector = player.Center - Projectile.Center;

            Projectile.Center = player.Center + new Vector2(300, 0).RotatedBy(Projectile.ai[1] + Projectile.ai[0] * MathHelper.PiOver2);

            Projectile.ai[1] += 0.01f;

            Projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;

            for (int k = 0; k < Main.projectile.Length; k++)
            {
                var proj = Main.projectile[k];
                if (proj.active && proj.owner == Projectile.owner && proj.arrow && !proj.Calamity().nihilicArrow && proj.friendly && Vector2.Distance(proj.Center, Projectile.Center) < 65)
                {
                    Main.projectile[k].damage = (int)(proj.damage * 1.75f);
                    proj.extraUpdates += 1;
                    Main.projectile[k].Calamity().nihilicArrow = true;
                    SoundEngine.PlaySound(SoundID.Item104 with { Volume = SoundID.Item104.Volume * 0.75f }, Projectile.Center);

                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 dustpos = Vector2.UnitX * (float)-(float)proj.width / 2f;
                        dustpos += -Vector2.UnitY.RotatedBy((double)((float)i * 3.14159274f / 6f), default) * new Vector2(8f, 16f);
                        dustpos = dustpos.RotatedBy((double)(proj.rotation - 1.57079637f), default);
                        int dust = Dust.NewDust(proj.Center, 0, 0, 27, 0f, 0f, 100, Color.HotPink, 1f);
                        Main.dust[dust].scale = 1.1f;
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].position = proj.Center + dustpos;
                        Main.dust[dust].velocity = proj.velocity * 0.1f;
                        Main.dust[dust].velocity = Vector2.Normalize(proj.Center - proj.velocity * 3f - Main.dust[dust].position) * 1.25f;
                    }
                }
            }

            if (VoidAura == null)
            {
                VoidAura = VoidGeneratorMetaball.SpawnParticle(Projectile.Center, Vector2.Zero, 120f);
            }
            else
            {
                VoidAura.Center = Projectile.Center;
                VoidAura.Size = 120f;
            }
        }
        public override bool? CanCutTiles() => false;
    }
}
