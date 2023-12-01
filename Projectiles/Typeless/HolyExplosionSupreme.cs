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

        public override void OnKill(int timeLeft)
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
                for (int i = 0; i < 5; i++)
                {
                    int holyDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[holyDust].velocity *= 3f;
                    if (Projectile.ai[0] == 1)
                        Main.dust[holyDust].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                    if (Main.rand.NextBool())
                    {
                        Main.dust[holyDust].scale = 0.5f;
                        Main.dust[holyDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 10; j++)
                {
                    int holyDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 3f);
                    Main.dust[holyDust2].noGravity = true;
                    Main.dust[holyDust2].velocity *= 5f;
                    if (Projectile.ai[0] == 1)
                        Main.dust[holyDust2].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                    holyDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                    Main.dust[holyDust2].velocity *= 2f;
                    if (Projectile.ai[0] == 1)
                        Main.dust[holyDust2].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
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
                for (int i = 0; i < 8; i++)
                {
                    int holyDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 181, 0f, 0f, 100, default, Main.rand.NextFloat(1.2f, 1.8f));
                    Main.dust[holyDust].velocity *= 3f;
                    Main.dust[holyDust].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                    if (Main.rand.NextBool())
                    {
                        Main.dust[holyDust].scale = 0.5f;
                        Main.dust[holyDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 12; j++)
                {
                    int holyDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 272, 0f, 0f, 100, default, 0.8f);
                    Main.dust[holyDust2].noGravity = true;
                    Main.dust[holyDust2].velocity *= 5f;
                    Main.dust[holyDust2].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                    holyDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 226, 0f, 0f, 100, default, 0.6f);
                    Main.dust[holyDust2].velocity *= 2f;
                    Main.dust[holyDust2].shader = GameShaders.Armor.GetSecondaryShader(Owner.cShield, Owner);
                }
            }
        }
    }
}
