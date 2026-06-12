using System;
using System.Net.Http.Headers;
using CalamityMod.Dusts;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    [PierceResistExceptionAttribute]
    public class DuststormCloud : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";

        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.penetrate = -1;
        }

        Color color = Color.White;

        public static Color RandomColor
        {
            get
            {
                switch (Main.rand.Next(4))
                {
                    case 0: return Color.SandyBrown;
                    case 1: return new Color(164, 118, 78);
                    case 2: return  new Color(210, 183, 126);
                    case 3: return new Color(151, 130, 103);
                }
                return Color.White;
            }
        }
        public override void AI()
        {
            if (color == Color.White)
            {
                color = RandomColor;
            }
            Projectile.velocity *= 0.97f;
            Projectile.rotation += 0.05f;
            Projectile.Opacity = (1- MathF.Pow(1-Projectile.timeLeft / 180f,2)) * 0.5f;
            if (Projectile.timeLeft % 5 == 0)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(32,32), ModContent.DustType<LemonNadeExplodeDust>(),newColor: RandomColor, Scale: 0.75f).customData = Projectile.Opacity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = color;
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center-Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, TextureAssets.Projectile[Type].Size() * 0.5f, 0.08f * Projectile.scale, 0);
            return false;
        }
    }
}
