using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Projectiles.Typeless
{
    public class HolyExplosionSupreme : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static readonly SoundStyle Impact = new("CalamityMod/Sounds/NPCKilled/DevourerSegmentBreak1") { Volume = 0.3f };
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[0] < 2)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = 60;
                Projectile.height = 60;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Projectile.ai[0] == 1)
                        Main.dust[num622].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                    if (Main.rand.NextBool())
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    if (Projectile.ai[0] == 1)
                        Main.dust[num624].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                    num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                    if (Projectile.ai[0] == 1)
                        Main.dust[num624].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                }
            }
            else
            {
                SoundEngine.PlaySound(Impact with { PitchVariance = 0.3f }, Projectile.position);
                SoundEngine.PlaySound(SoundID.Item62 with { Volume = 0.5f, PitchVariance = 0.3f }, Projectile.position);
                Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
                Projectile.width = 60;
                Projectile.height = 60;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                for (int num621 = 0; num621 < 8; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 181, 0f, 0f, 100, default, Main.rand.NextFloat(1.2f, 1.8f));
                    Main.dust[num622].velocity *= 3f;
                    Main.dust[num622].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                    if (Main.rand.NextBool())
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 12; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 272, 0f, 0f, 100, default, 0.8f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    Main.dust[num624].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                    num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 226, 0f, 0f, 100, default, 0.6f);
                    Main.dust[num624].velocity *= 2f;
                    Main.dust[num624].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                }
            }
        }
    }
}
