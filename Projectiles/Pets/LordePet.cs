using CalamityMod.CalPlayer;
using CalamityMod.NPCs.Other;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Pets
{
    public class LordePet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Pets";
        public override string Texture => "CalamityMod/NPCs/Other/THELORDE";
        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = Projectile.height = 67;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            AIType = ProjectileID.BabySkeletronHead;
            Projectile.scale = 0.3f;
            DrawOriginOffsetX -= 150;
            DrawOriginOffsetY -= 150;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.lordePet = false;
            }
            if (modPlayer.lordePet)
            {
                Projectile.timeLeft = 2;
            }
            Lighting.AddLight(Projectile.Center, Main.DiscoColor.ToVector3() * 2);
            if (Main.rand.NextBool(1200))
            {
                SoundEngine.PlaySound(THELORDE.DeathSound with { PitchVariance = 2, MaxInstances = 5 }, Projectile.Center);
            }
        }

        public override bool PreDraw(ref Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frameUsed = texture.Frame(2, 7, 0, 1);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), frameUsed, drawColor, Projectile.rotation, new Vector2(texture.Width / 4f, texture.Height / 14f), Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
