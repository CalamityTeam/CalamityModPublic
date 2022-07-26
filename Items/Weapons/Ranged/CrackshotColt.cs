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
    public class CrackshotColt : ModItem
    {
        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Item/CrackshotColtShot") { PitchVariance = 0.1f };

        public static readonly SoundStyle BlingSound = new("CalamityMod/Sounds/Custom/Ultrabling") { PitchVariance = 0.5f };
        public static readonly SoundStyle BlingHitSound = new("CalamityMod/Sounds/Custom/UltrablingHit") { PitchVariance = 0.5f };

        public static float MaxDownwardsAngle4Coin = MathHelper.PiOver4;
        public static float RicochetDamageMult = 2f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crackshot Colt");
            Tooltip.SetDefault("Right click to throw a coin in the air. Hitting the coin with a bullet redirects the shot into the nearest enemy\n" +
                               "Coin throws consume copper coins");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 23;
            Item.height = 8;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = ShootSound with { Volume = 0.6f};
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CrackshotBlast>();
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
                return player.CanBuyItem(1); //Breaks if the player has > 999 plat. Ask tml people to fix that?
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.BuyItem(1);
            }

            return base.UseItem(player);
        }

        public override void UseAnimation(Player player)
        {
            Item.UseSound = ShootSound with { Volume = 0.6f};
            if (player.altFunctionUse == 2)
                Item.UseSound = BlingSound;
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.3f;
            return 1f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {

            //Override every projectile
            type = ModContent.ProjectileType<CrackshotBlast>();

            if (player.altFunctionUse == 2)
            {
                damage = 0;
                type = ModContent.ProjectileType<CrackshotCoin>() ;

                //Ok the velocity is flipped because the in world coordinates have 0 at the top, so to do the typical trigo stuff we flip it, you get me.
                float shootAngle = (player.Calamity().mouseWorld - player.MountedCenter).ToRotation() * -1;

                if (shootAngle > -MathHelper.Pi + MaxDownwardsAngle4Coin && shootAngle < -MathHelper.PiOver2)
                    shootAngle = -MathHelper.Pi + MaxDownwardsAngle4Coin;

                else if (shootAngle < -MaxDownwardsAngle4Coin && shootAngle >= -MathHelper.PiOver2)
                    shootAngle = -MaxDownwardsAngle4Coin;

                velocity = (shootAngle * -1).ToRotationVector2() * 1.3f - Vector2.UnitY * 1.12f + player.velocity / 4f;
            }
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;
            Vector2 itemSize = new Vector2(40, 20);
            Vector2 itemOrigin = new Vector2(-15, 1);

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

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<DesertFeather>()).
                AddRecipeGroup("AnyGoldBar", 3).
                AddIngredient(ItemID.CopperCoin, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
