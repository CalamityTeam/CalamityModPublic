using System;
using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
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

        public static float offset = 0f;
        
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(CalamityKeybinds.BlazingCoreHotKey);
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
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.blazingCore = true;
        }

        public static void HandleStars(Player player)
        {
            float divisor = 3f;
            int totalFlameProjectiles = 45;
            int chains = 4;
            float interval = totalFlameProjectiles / chains * divisor;
            double patternInterval = Math.Floor(player.Calamity().blazingCoreSuccessfulParry / interval);

            if (patternInterval % 2 == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, player.Center);
                double radians = MathHelper.TwoPi / chains;
                double angleA = radians * 0.5;
                double angleB = MathHelper.ToRadians(90f) - angleA;
                float velocityX = (float)(2f * Math.Sin(angleA) / Math.Sin(angleB));
                Vector2 spinningPoint = new Vector2(velocityX, -2f);
                for (int i = 0; i < chains; i++)
                {
                    Vector2 vector2 = spinningPoint.RotatedBy(radians * i + MathHelper.ToRadians(offset));

                    int projectileType = ModContent.ProjectileType<BlazingStarThatDoesNotHeal>();
                    int dmgAmt = 90;
                    if (!player.Calamity().blazingCoreEmpoweredParry && Main.rand.NextBool(4))
                    {
                        projectileType = ModContent.ProjectileType<BlazingStarHeal>();
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        spinningPoint *= 1.5f;
                        dmgAmt = (int)player.GetTotalDamage<GenericDamageClass>().ApplyTo(dmgAmt);
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
        
        public static void HandleIncomingHit(Player player, ref Player.HurtInfo hurtInfo)
        {
            Main.NewText(player.Calamity().blazingCoreParry >= 18);

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
                player.RemoveAllGrapplingHooks();
            }
        }
    }
}
