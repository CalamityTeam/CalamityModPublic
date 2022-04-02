using CalamityMod.CalPlayer;
using CalamityMod.Items.Ammo.FiniteUse;
using CalamityMod.Projectiles.Typeless.FiniteUse;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless.FiniteUse
{
    public class Hydra : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydra");
            Tooltip.SetDefault("Uses Explosive Shotgun Shells\n" +
                "Can be used once per boss battle");
        }

        public override void SetDefaults()
        {
            item.damage = 120;
            item.width = 66;
            item.height = 30;
            item.useTime = 33;
            item.useAnimation = 33;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 10f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/Hydra");
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<ExplosiveShotgunShell>();
            item.useAmmo = ModContent.ItemType<ExplosiveShells>();
            if (CalamityPlayer.areThereAnyDamnBosses)
                item.Calamity().timesUsed = 1;
        }

        public override bool OnPickup(Player player)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
                item.Calamity().timesUsed = 1;

            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return item.Calamity().timesUsed < 1;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override void UpdateInventory(Player player)
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
                item.Calamity().timesUsed = 0;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 8; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-40, 41) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);
            }

            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                player.HeldItem.Calamity().timesUsed++;
                for (int i = 0; i < Main.maxInventory; i++)
                {
                    if (player.inventory[i].type == item.type && player.inventory[i] != player.HeldItem)
                        player.inventory[i].Calamity().timesUsed++;
                }
            }

            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Shotgun);
            recipe.AddIngredient(ItemID.IllegalGunParts);
            recipe.AddIngredient(ItemID.IronBar, 20);
            recipe.anyIronBar = true;
            recipe.AddIngredient(ItemID.Ectoplasm, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
