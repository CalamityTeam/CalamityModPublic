using System;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts
{
    public class LemonNadeExplodeDust : ModDust
    {
        public override string Texture => "CalamityMod/Particles/LemonNadeExplode";
        Color RandomColor
        {
            get
            {
                switch (Main.rand.Next(4))
                {
                    case 0: return new Color(54, 54, 54);
                    case 1: return new Color(76, 36, 36);
                    case 2: return new Color(132, 72, 56);
                    case 3: return new Color(50, 45, 37);
                }
                return Color.White;
            }
        }
        public override void OnSpawn(Dust dust)
        {
            if (dust.color == Color.Transparent)
                dust.color = RandomColor;
            dust.dataAsFloat = 0.75f;
            dust.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
        }
        public override bool MidUpdate(Dust dust)
        {
            return true;
        }

        public override bool Update(Dust dust)
        {
            dust.dataAsFloat *= 0.96f;
            dust.position += dust.velocity;
            dust.velocity *= 0.925f;
            dust.rotation += MathF.Sin(dust.dustIndex * 0.1f) * 0.2f * dust.dataAsFloat;
            if (dust.dataAsFloat <= 0.01f)
                dust.active = false;
            return false;
        }

        static Asset<Texture2D> tex;
        public override bool PreDraw(Dust dust)
        {
            tex ??= ModContent.Request<Texture2D>("CalamityMod/Particles/LemonNadeExplode");
            Main.EntitySpriteDraw(
                tex.Value,
                dust.position - Main.screenPosition,
                null,
                dust.color * dust.dataAsFloat,
                dust.rotation,
                tex.Size() * 0.5f,
                dust.scale * 0.1f * (0.75f + 0.25f * dust.dataAsFloat),
                SpriteEffects.None,
                0
            );
            return false;
        }


    }
}
