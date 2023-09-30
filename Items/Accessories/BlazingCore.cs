using System;
using System.Collections.Generic;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BlazingCore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        private static float offset = 0f;
        
        public static readonly SoundStyle ParryActivateSound = new("CalamityMod/Sounds/Item/BlazingCoreParryActivate") { Volume = 0.7f};
        public static readonly SoundStyle ParrySuccessSound = new("CalamityMod/Sounds/Item/BlazingCoreParry") { Volume = 0.6f};
        
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(CalamityKeybinds.AccessoryParryHotKey);
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 46;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().blazingCore = true;
        }

        public static void HandleStars(Player player)
        {
            bool empowered = player.Calamity().blazingCoreEmpoweredParry;
            float divisor = 3f;
            int totalFlameProjectiles = 45;
            int chains = 3;
            float interval = totalFlameProjectiles / chains * divisor;
            double patternInterval = Math.Floor(player.Calamity().blazingCoreSuccessfulParry / interval);
            

            if (player.Calamity().blazingCoreSuccessfulParry % 4 == 0) //play sound every 4 frames
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, player.Center);
            }
            
            if (patternInterval % 2 == 0)
            {
                double radians = MathHelper.TwoPi / chains;
                double angleA = radians * 0.5;
                double angleB = MathHelper.ToRadians(90f) - angleA;
                float velocityX = (float)(2f * Math.Sin(angleA) / Math.Sin(angleB));
                Vector2 spinningPoint = new Vector2(velocityX, -2f);
                for (int i = 0; i < chains; i++)
                {
                    int projectileType = ModContent.ProjectileType<BlazingStarThatDoesNotHeal>();
                    int dmgAmt = 90;
                    if (!empowered && Main.rand.NextBool(5))
                    {
                        projectileType = ModContent.ProjectileType<BlazingStarHeal>();
                    }
                    
                    Vector2 vector2 = spinningPoint.RotatedBy(radians * i + MathHelper.ToRadians(offset));
                    //not sure if this is correct, will see in testing
                    if (!Main.dedServ)
                    {
                        spinningPoint *= 1.5f;
                        dmgAmt = (int)player.GetBestClassDamage().ApplyTo(dmgAmt);
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, vector2, projectileType, dmgAmt, 0f, Main.myPlayer);

                    }
                }
                offset += 10f;
            }
            
            player.Calamity().blazingCoreSuccessfulParry--;
            if (player.Calamity().blazingCoreSuccessfulParry <= 0)
            {
                offset = 0f;
                player.Calamity().blazingCoreEmpoweredParry = false;
            }
        }

        public static void HandleParryCountdown(Player player)
        {
            player.Calamity().blazingCoreParry--;

            if (player.Calamity().blazingCoreParry > 0)
            {
                player.controlJump = false;
                player.controlDown = false;
                player.controlLeft = false;
                player.controlRight = false;
                player.controlUp = false;
                player.controlUseItem = false;
                player.controlUseTile = false;
                player.controlThrow = false;
                player.gravDir = 1f;
                player.velocity = Vector2.Zero;
                player.velocity.Y = -0.1f; //if player velocity is 0, the flight meter gets reset
                player.RemoveAllGrapplingHooks();
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    int theDust = Dust.NewDust(player.position, player.width, player.height, 205, 0f, 0f, 100, new Color(255, 255, 255), 2f);
                    Main.dust[theDust].noGravity = true;
                }
            }
        }
    }
}
