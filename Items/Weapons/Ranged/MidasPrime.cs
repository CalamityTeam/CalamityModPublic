using System;
using System.IO;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static CalamityMod.CalamityUtils;
using CalamityMod.Projectiles.Ranged;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MidasPrime : ModItem
    {
        public static float MaxDownwardsAngle4Coin = MathHelper.PiOver4;

        public static int ShotCoin = 0;
        public static float SilverRicochetDamageMult = 1.5f;
        public static float GoldRicochetDamageMult = 1.75f;
        public static int RipeningTime = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Midas Prime");
            Tooltip.SetDefault("Struck enemies drop extra coins\n" +
                                "Right click to a coin in the air. Hitting the coin with a bullet redirects the shot into the nearest enemy\n" +
                               "If you have multiple coins up in the air, bullets will first redirect towards other coins up to a maximum of 4\n" +
                               "Coin ricochets will increase the damage of the bullet, provided the coins have been in the air for long enough" +
                               "Coin throws consume gold and silver coins");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 52;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 23;
            Item.height = 8;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = Item.sellPrice(0, 7, 20, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = DeadeyeRevolver.ShootSound;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MidasBlast>();
            Item.useAmmo = AmmoID.Bullet;
            Item.shootSpeed = 14f;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;
        
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.altFunctionUse == 2 ? false : true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                //Using player.CanBuyItem() breaks if the player has too many platinum coins
                long coinCount = Utils.CoinsCount(out bool overflow, player.inventory);
                return overflow || coinCount >= 100;
            }
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                long coinCount = Utils.CoinsCount(out bool overflow, player.inventory);

                if (overflow || coinCount > 10000)
                {
                    player.BuyItem(10000);
                    ShotCoin = 1;
                }

                else
                {
                    player.BuyItem(100);
                    ShotCoin = 0;
                }

                //Clear prev coins
                int ownedCoins = 0;
                int oldestCoinIndex = -1;
                int oldestCoinTime = 10000;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];

                    if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<MidasCoin>())
                    {
                        ownedCoins++;
                        if (proj.timeLeft < oldestCoinTime)
                        {
                            oldestCoinIndex = proj.whoAmI;
                            oldestCoinTime = proj.timeLeft;
                        }
                    }
                }

                if (ownedCoins >= 4)
                    Main.projectile[oldestCoinIndex].Kill();

            }

            return base.UseItem(player);
        }

        public override void UseAnimation(Player player)
        {
            Item.UseSound = DeadeyeRevolver.ShootSound; 
            if (player.altFunctionUse == 2)
                Item.UseSound = DeadeyeRevolver.BlingSound;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 5f;
            return 1f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position -= Vector2.UnitY * 13f;

            //Override every projectile
            type = ModContent.ProjectileType<MidasBlast>();

            if (player.altFunctionUse == 2)
            {
                damage = 0;
                type = ModContent.ProjectileType<MidasCoin>();

                //Ok the velocity is flipped because the in world coordinates have 0 at the top, so to do the typical trigo stuff we flip it, you get me.
                float shootAngle = (player.Calamity().mouseWorld - player.MountedCenter).ToRotation() * -1;

                if (shootAngle > -MathHelper.Pi + MaxDownwardsAngle4Coin && shootAngle < -MathHelper.PiOver2)
                    shootAngle = -MathHelper.Pi + MaxDownwardsAngle4Coin;

                else if (shootAngle < -MaxDownwardsAngle4Coin && shootAngle >= -MathHelper.PiOver2)
                    shootAngle = -MaxDownwardsAngle4Coin;

                velocity = (shootAngle * -1).ToRotationVector2() * 1.3f - Vector2.UnitY * 1.12f + player.velocity / 4f;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ShotCoin);
                return false;
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;
            Vector2 itemSize = new Vector2(50, 24);
            Vector2 itemOrigin = new Vector2(-17, 3);
            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                rotation += -0.45f * (float)Math.Pow((0.4f - animProgress) / 0.4f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
    }

    public class MidasPrimeItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public bool magnetMode = false;

        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            if (player.HeldItem.type == ModContent.ItemType<MidasPrime>() && magnetMode && item.timeSinceItemSpawned > 60)
                grabRange *= 8;
        }

        public override bool GrabStyle(Item item, Player player)
        {
            if (player.HeldItem.type == ModContent.ItemType<MidasPrime>() && magnetMode && item.timeSinceItemSpawned > 60)
            {
                //This is just Player.PullItemPickup() but not private

                Vector2 towardsPlayer = player.Center - item.Center;
                float movementSpeed = towardsPlayer.Length();
                movementSpeed = 12 / movementSpeed;
                towardsPlayer *= movementSpeed;
                item.velocity = (item.velocity * 4f + towardsPlayer) / 5f;
                return false;
            }

            return true;
        }
    }

}
