using CalamityMod.Items.Ammo;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Magic
{
    public class IceBarrage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Barrage");
            Tooltip.SetDefault("Oh dear, you are dead!\n" +
                "Casts a deadly and powerful ice spell in the location of the cursor\n" +
                "This ice spell locks itself to the position of nearby enemies\n" +
                "Consumes 2 Blood Runes every time it's used");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 180;
            Item.noMelee = true;
            Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/IceBarrageCast");

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.Calamity().donorItem = true;

            Item.damage = 2250;
            Item.knockBack = 6f;
            Item.useTime = 300;
            Item.useAnimation = 300;
            Item.reuseDelay = 60;
            Item.shoot = ModContent.ProjectileType<IceBarrageMain>();
            Item.shootSpeed = 2f;
            Item.useAmmo = ModContent.ItemType<BloodRune>();
        }

        public override bool CanUseItem(Player player) => CalamityGlobalItem.HasEnoughAmmo(player, Item, 2);

        public override bool CanConsumeAmmo(Item ammo, Player player) => false;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            vector2.X = Main.mouseX + Main.screenPosition.X;
            vector2.Y = Main.mouseY + Main.screenPosition.Y;
            Projectile.NewProjectile(source, vector2, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);

            CalamityGlobalItem.ConsumeAdditionalAmmo(player, Item, 2);

            return false;
        }

        public override void UseStyle(Player player, Rectangle rectangle)
        {
            player.itemLocation.X -= 8f * player.direction;
            player.itemRotation = player.direction * MathHelper.ToRadians(-45f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BlizzardStaff).
                AddIngredient(ItemID.IceRod).
                AddIngredient<IcicleStaff>().
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<EndothermicEnergy>(40).
                AddIngredient<VerstaltiteBar>(18).
                AddTile(TileID.IceMachine).
                Register();
        }
    }
}
