using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Summon
{
    public class GastricBelcherVomit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 18;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 3;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            //Rotation and gravity
            Projectile.rotation += 0.6f * Projectile.direction;
            Projectile.velocity.Y += 0.27f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Venom, 120);

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            //Dust effect
            int splash = 0;
            while (splash < 4)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 233, -Projectile.velocity.X * 0.15f, -Projectile.velocity.Y * 0.10f, 150, default, 0.9f);
                splash += 1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture;
            switch (Projectile.ai[0])
            {
                case 1f: texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/GastricBelcherVomit2").Value;
                    break;
                case 2f: texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/GastricBelcherVomit3").Value;
                    break;
                default: texture = ModContent.Request<Texture2D>(Texture).Value;
                    break;
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
