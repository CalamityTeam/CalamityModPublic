using System;
using System.IO;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("WulfrumBow")]
    public class WulfrumBlunderbuss : ModItem
    {
        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Item/WulfrumBlunderbussFire") { PitchVariance = 0.1f };

        public static float MinSpreadDistance = 460f; 
        public static float MaxSpreadDistance = 60f;
        public static float MinSpread = 0.2f;
        public static float MaxSpread = 0.6f;
        public static int BulletCount = 6;

        public int storedScrap = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Blunderbuss");
            Tooltip.SetDefault("Consumes ");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 23;
            Item.height = 8;
            Item.useTime = 55;
            Item.useAnimation = 55;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = ShootSound;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;
        }

        public override bool CanUseItem(Player player)
        {
            return storedScrap > 0 || player.HasItem(ModContent.ItemType<WulfrumShard>()) || player.HasItem(ItemID.SilverCoin);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().GeneralScreenShakePower < 3f)
                player.Calamity().GeneralScreenShakePower = 3f;

            float aimLenght = (Main.MouseWorld - player.MountedCenter).Length();
            float spreadDistance = Math.Clamp(aimLenght - MaxSpreadDistance, 0, MinSpreadDistance - MaxSpreadDistance) / (MinSpreadDistance - MaxSpreadDistance);
            float spread = MathHelper.Lerp(MaxSpread, MinSpread, spreadDistance);

            Vector2 nuzzleDir = velocity.SafeNormalize(Vector2.Zero);


            for (int i = 0; i < BulletCount; i++)
            {
                Vector2 direction = nuzzleDir.RotatedByRandom(spread);
                Vector2 nuzzlePos = player.MountedCenter + direction * 15f;

                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(Item, Item.useAmmo), nuzzlePos, direction * Item.shootSpeed, type, damage, (int)Item.knockBack, player.whoAmI, 0, 0);
            }

            return false;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;
            Vector2 itemSize = new Vector2(46, 16);
            Vector2 itemOrigin = new Vector2(-13, 3);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);

            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.direction = Math.Sign((player.Calamity().mouseWorld - player.Center).X);

            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() + MathHelper.PiOver2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WulfrumShard>(10).
                AddTile(TileID.Anvils).
                Register();
        }

        #region saving the durability
        public override ModItem Clone(Item item)
        {
            ModItem clone = base.Clone(item);
            if (clone is WulfrumBlunderbuss a && item.ModItem is WulfrumBlunderbuss a2)
            {
                a.storedScrap = a2.storedScrap;
            }
            return clone;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["ammoStored"] = storedScrap;
        }

        public override void LoadData(TagCompound tag)
        {
            storedScrap = tag.GetInt("ammoStored");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(storedScrap);
        }

        public override void NetReceive(BinaryReader reader)
        {
            storedScrap = reader.ReadInt32();
        }
        #endregion
    }
}
