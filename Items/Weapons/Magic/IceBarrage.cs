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
            item.width = 60;
            item.height = 60;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.magic = true;
            item.mana = 180;
            item.noMelee = true;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/IceBarrageCast");

            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().donorItem = true;

            item.damage = 2250;
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
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 18);
            recipe.AddTile(TileID.LunarCraftingStation);
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
