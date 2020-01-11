using CalamityMod.Items.Ammo;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
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
            Tooltip.SetDefault("Casts a deadly and powerful ice spell in the location of the cursor \n" +
                               "Consumes 2 Blood Runes every time its used \n" +
                               "Oh dear, you are dead!");
        }

        public override void SetDefaults()
        {
            item.width = 60;
            item.height = 60;
            item.Calamity().postMoonLordRarity = 21;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.magic = true;
            item.mana = 530;
            item.noMelee = true;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/IceBarrageCast");

            item.damage = 9000;
            item.knockBack = 6f;
            item.useTime = 300;
            item.useAnimation = 300;
            item.reuseDelay = 60;
            item.shoot = ModContent.ProjectileType<IceBarrageMain>();
            item.shootSpeed = 2f;
            item.useAmmo = ModContent.ItemType<BloodRune>();
        }

        public override bool CanUseItem(Player player)
        {
            return CalamityGlobalItem.HasEnoughAmmo(player, item, 2);
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

            CalamityGlobalItem.ConsumeAdditionalAmmo(player, item, 2);

            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BlizzardStaff);
            recipe.AddIngredient(ItemID.IceRod);
            recipe.AddIngredient(ModContent.ItemType<IcicleStaff>());
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 23);
            recipe.AddIngredient(ModContent.ItemType<CryoBar>(), 18);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void UseStyle(Player player)
        {
            player.itemLocation.X -= 8f * player.direction;
            player.itemRotation = player.direction * MathHelper.ToRadians(-45f);
        }
    }
}
