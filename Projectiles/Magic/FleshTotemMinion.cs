using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class FleshTotemMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        public int pulseTimer = 0;

        static Asset<Texture2D> totemEyeTexture;
        public Player Owner => Main.player[Projectile.owner];
        public bool visuals => Owner.Calamity().fleshTotemVisual; // Enables/disables visuals and sounds based on accessory visibility
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 26;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            bool isActive = Projectile.type == ModContent.ProjectileType<FleshTotemMinion>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            //If the player takes off the accessory, kill the projectile
            if (!modPlayer.fleshTotemMinion)
            {
                Projectile.active = false;
                return;
            }
            //Constantly ensure the projectile stays activem unless the player dies
            if (isActive)
            {
                if (player.dead)
                {
                    modPlayer.fleshTotemMinion = false;
                }
                if (modPlayer.fleshTotemMinion)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Lighting.AddLight(Projectile.Center, 0f, 0.25f, 1.5f);
            //The totem hovers above the player's head...
            Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 85f);
            //Unless gravity is flipped, in which case it's below the player and flipped
            if (player.gravDir == -1f)
            {
                Projectile.position.Y += 170f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            //Pulses happen every 4 seconds, divided by how much mana the totem has stored
            float pulseMax = Utils.Remap(modPlayer.fleshTotemManaStorage, 0, FleshTotem.manaStorageMax, FleshTotem.MaxDelay, FleshTotem.MinDelay);
            //Ensure the pulse timer can never go below 1 second
            if (pulseMax < 60)
            {
                pulseMax = 60;
            }
            //When the timer reaches max, pulse and restore 25 mana
            if (pulseTimer >= pulseMax)
            {
                if (visuals)
                {
                    SoundEngine.PlaySound(BrimstoneElemental.HellfireballSound with { Volume = 0.65f }, Projectile.Center);
                    for (int k = 0; k < 30; k++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch, new Vector2(9, 9).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 0.8f), 0, Color.LightSkyBlue, Main.rand.NextFloat(1.2f, 1.4f));
                        dust.noGravity = true;
                        dust.alpha = Main.rand.Next(70, 90 + 1);
                    }
                    if (modPlayer.fleshTotemManaStorage == 600)
                    {
                        int Dusts = 8;
                        float radians = MathHelper.TwoPi / Dusts;
                        Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                        for (int i = 0; i < Dusts; i++)
                        {
                            Vector2 dustVelocity = spinningPoint.RotatedBy(radians * i) * 12.5f;
                            GlowSparkParticle spark = new GlowSparkParticle(Projectile.Center, dustVelocity * 0.7f, false, 12, 0.009f, Color.Cyan, new Vector2(3.5f, 1.3f), true);
                            GeneralParticleHandler.SpawnParticle(spark);

                            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, dustVelocity.RotatedBy(MathHelper.ToRadians(22.5f)), 0, default, 0.9f);
                            dust.noGravity = true;
                            Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, dustVelocity.RotatedBy(MathHelper.ToRadians(22.5f)) * 0.4f, 0, default, 1.2f);
                            dust2.noGravity = true;
                        }
                    }
                }
                int manaGained = 25;
                player.statMana += manaGained;
                if (Main.myPlayer == player.whoAmI)
                    player.ManaEffect(manaGained);

                if (player.statMana > player.statManaMax2)
                    player.statMana = player.statManaMax2;
                pulseTimer = 0;
            }
            pulseTimer++;
        }
        //Set the player's mana storage to 0 when equipping the accessory
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.fleshTotemManaStorage = 0;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            totemEyeTexture ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/FleshTotemEyes");
            Texture2D totemEyes = totemEyeTexture.Value;
            Vector2 eyesDrawPosition = player.Center - Main.screenPosition + Vector2.UnitY * -69f;
            Vector2 drawPosition = player.Center - Main.screenPosition + Vector2.UnitY * -67f;
            Vector2 origin = texture.Size() * 0.5f;


            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor) * (visuals ? 1f : 0.5f), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            if (visuals)
            {
                float fade = Utils.GetLerpValue(0, modPlayer.fleshTotemManaStorage, FleshTotem.manaStorageMax, true);
                for (int i = 0; i < 10; i++)
                {
                    Main.spriteBatch.Draw(texture, drawPosition + Vector2.UnitY * -1.5f, null, Color.Cyan with { A = 0 } * fade, Projectile.rotation, origin, Projectile.scale * (Main.rand.NextFloat(0.0016f, 0.002f) * modPlayer.fleshTotemManaStorage), SpriteEffects.None, 0f);
                }
                for (int i = 0; i < 1; i++)
                {
                    int dustType = Main.rand.NextBool() ? 66 : 247;
                    float rotMulti = Main.rand.NextFloat(0.3f, 1f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Vector2.UnitY / 75f, dustType);
                    dust.scale = Main.rand.NextFloat(1.2f, 1.8f) * (modPlayer.fleshTotemManaStorage * 0.0009f) - rotMulti * 0.1f;
                    dust.noGravity = true;
                    dust.velocity = new Vector2(0, -2).RotatedByRandom(rotMulti * 0.3f) * (Main.rand.NextFloat(1f, 3.2f) - rotMulti) * (modPlayer.fleshTotemManaStorage * 0.0009f);
                    dust.alpha = 1;
                    dust.color = Color.Cyan;
                }
                Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            //When at max, draw glowing eyes on the totem
            if (modPlayer.fleshTotemManaStorage == 600)
                Main.EntitySpriteDraw(totemEyes, eyesDrawPosition + Vector2.UnitX * -2f, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool? CanDamage() => false;
    }
}
