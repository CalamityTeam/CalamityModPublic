using System;
using System.Collections.Generic;
using CalamityMod.NPCs;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SealedSingularityRock : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.Opacity = 0;
            Projectile.hide = true;
            Projectile.scale = 0.75f;
        }
        private static Texture2D OutlineTex
        {
            get
            {
                if (field == null)
                {
                    var texture = TextureAssets.Projectile[ProjectileID.DeerclopsRangedProjectile].Value;
                    field = new Texture2D(Main.graphics.GraphicsDevice, texture.Width, texture.Height);

                    var BaseArray = new Color[field.Width * field.Height];
                    var ColorArray = new Color[field.Width * field.Height];
                    texture.GetData(BaseArray);
                    for (var i = 0; i < BaseArray.Length; i++)
                    {
                        ColorArray[i] = new Color(255, 255, 255) * (((float)BaseArray[i].A) / 255f);
                    }
                    field.SetData(ColorArray);
                }
                return field;
            }
            set;
        } = null;

        public override bool PreDraw(ref Color lightColor)
        {
            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

                var tex = TextureAssets.Projectile[ProjectileID.DeerclopsRangedProjectile];
                var pos = Projectile.Center - Main.screenPosition;
                float rot = Projectile.rotation;
                var frame = tex.Frame(3, 4, 0, 1);
                float scale = Projectile.scale;
                float borderOp = MathF.Pow(Projectile.Opacity, 2);
                for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2)
                {
                    Main.spriteBatch.Draw(OutlineTex, pos + new Vector2(2, 0).RotatedBy(i), frame, SealedSingularityProjectile.BorderColor * borderOp, rot, frame.Size() * 0.5f, scale, SpriteEffects.None, 0);

                }
                Main.spriteBatch.Draw(tex.Value, pos, frame, Lighting.GetColor(Projectile.Center.ToTileCoordinates()) * Projectile.Opacity, rot, frame.Size() * 0.5f, scale, SpriteEffects.None, 0);
                Main.spriteBatch.End();
            }
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        Vector2? goaloffset;
        public override void AI()
        {
            var goal = Main.projectile[(int)Projectile.ai[0]];
            goaloffset ??= Projectile.Center - goal.Center;

            Projectile.Opacity += 0.1f;
            if (Projectile.Opacity >= 1f)
            {
                Projectile.velocity += Projectile.DirectionTo(goal.Center) * 0.4f;
            }
            Projectile.rotation += 0.1f;

            if (Projectile.Distance(goal.Center) < 16)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCHit/OtherworldlyHit") with { PitchVariance = 0.4f, Volume = 0.2f, MaxInstances = 1 }, Projectile.Center);
                Projectile.Kill();
            }
        }
    }
}
