using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class SpiritOriginBullseye : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];
        public NPC Target => Main.npc[(int)Projectile.ai[0]];
        public bool FadingOut
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
        }
        public Vector2 BullseyeOffsetFromCenter;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = (int)(2 * DaawnlightSpiritOrigin.RegularEnemyBullseyeRadius);
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.Opacity = 0f;
            Projectile.penetrate = -1;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.WriteVector2(BullseyeOffsetFromCenter);

        public override void ReceiveExtraAI(BinaryReader reader) => BullseyeOffsetFromCenter = reader.ReadVector2();

        public override void AI()
        {
            if (!Main.npc.IndexInRange((int)Projectile.ai[0]) || !Owner.Calamity().spiritOrigin || !Target.active || Target.life <= 0 || Target.dontTakeDamage)
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.timeLeft < 45)
                FadingOut = true;

            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + (FadingOut ? -0.08f : 0.04f), 0f, 1f);
            Projectile.scale = Projectile.Opacity;
            if (FadingOut && Projectile.Opacity <= 0f)
                Projectile.Kill();

            if (BullseyeOffsetFromCenter == Vector2.Zero)
            {
                BullseyeOffsetFromCenter = Main.rand.NextVector2CircularEdge(Target.width, Target.height) * Main.rand.NextFloat(0.925f, 1f) * 0.54f;
                if (BullseyeOffsetFromCenter.Y > 0f)
                    BullseyeOffsetFromCenter.Y *= -1f;
                Projectile.netUpdate = true;
            }
            else
                Projectile.Center = Target.Center + BullseyeOffsetFromCenter;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.myPlayer != Projectile.owner)
                return false;

            float scale = 2f - Projectile.scale;
            float rotation = MathHelper.TwoPi * Projectile.scale;
            Vector2 drawPosition = Target.Center + BullseyeOffsetFromCenter - Main.screenPosition;
            if (FadingOut)
            {
                scale = Projectile.scale;
                rotation = 0f;
            }

            Texture2D bullseyeTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/DaawnlightSpiritOriginRegularBullseye").Value;
            Rectangle frame = bullseyeTexture.Frame();
            if (Target.IsABoss())
            {
                bullseyeTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/DaawnlightSpiritOriginBossBullseye").Value;
                frame = bullseyeTexture.Frame(1, 4, 0, (int)(Main.GlobalTimeWrappedHourly * 7f) % 4);
                rotation = 0f;
                drawPosition.Y -= 17;
                drawPosition.X -= 1;
            }

            Main.EntitySpriteDraw(bullseyeTexture, drawPosition, frame, Color.White * Projectile.Opacity, rotation, frame.Size() * 0.5f, scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
