using CalamityMod.Items.Ammo;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 180;
            Item.noMelee = true;
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/IceBarrageCast");

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

        public override bool CanUseItem(Player player)
        {
            return CalamityGlobalItem.HasEnoughAmmo(player, Item, 2);
        }

        public override bool ConsumeAmmo(Player player)
        {
            return false;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            vector2.X = Main.mouseX + Main.screenPosition.X;
            vector2.Y = Main.mouseY + Main.screenPosition.Y;
            Projectile.NewProjectile(vector2, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 0f);

            CalamityGlobalItem.ConsumeAdditionalAmmo(player, Item, 2);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.BlizzardStaff).AddIngredient(ItemID.IceRod).AddIngredient(ModContent.ItemType<IcicleStaff>()).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8).AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 40).AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 18).AddTile(TileID.IceMachine).Register();
        }

        public override void UseStyle(Player player)
        {
            player.itemLocation.X -= 8f * player.direction;
            player.itemRotation = player.direction * MathHelper.ToRadians(-45f);
        }
    }
}
