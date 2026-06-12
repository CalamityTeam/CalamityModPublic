using System;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class FrigidflashBolt : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/FrigidflashUse");
        public static readonly SoundStyle ProjDeathSound = new("CalamityMod/Sounds/Item/FrigidflashDeath");
        public static readonly SoundStyle ChargeSound = new("CalamityMod/Sounds/Item/FrigidflashCharge");

        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3, BuffID.Frostburn2];
        }
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 42;
            Item.damage = 95;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FrigidflashBoltProjectile>();
            Item.shootSpeed = 9f;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override float UseSpeedMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 0.25f;

            return 1f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) // Right Click
            {
                Projectile bigMagic = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 5);
                bigMagic.extraUpdates = 4;
                bigMagic.timeLeft = 370;
                bigMagic.penetrate = -1;
            }
            else // Left Click
            {
                Projectile smallMagic = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback / 2, player.whoAmI);
            }
            return false;
        }
        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 4f;
            Vector2 itemSize = new Vector2(38, 42);
            Vector2 itemOrigin = new Vector2(-24, 4);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);

            base.UseStyle(player, heldItemFrame);
        }
        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FrostBolt>().
                AddIngredient<FlareBolt>().
                AddIngredient<EssenceofHavoc>(5).
                AddIngredient<EssenceofEleum>(5).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
