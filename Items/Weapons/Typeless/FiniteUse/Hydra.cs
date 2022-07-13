using CalamityMod.CalPlayer;
using CalamityMod.Items.Ammo.FiniteUse;
using CalamityMod.Projectiles.Typeless.FiniteUse;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Typeless.FiniteUse
{
    public class Hydra : ModItem
    {
        public static readonly SoundStyle FireSound = new("CalamityMod/Sounds/Item/Hydra");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydra");
            Tooltip.SetDefault("Uses Explosive Shotgun Shells\n" +
                "Can be used once per boss battle");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.width = 66;
            Item.height = 30;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = FireSound;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<ExplosiveShotgunShell>();
            Item.useAmmo = ModContent.ItemType<ExplosiveShells>();
            if (CalamityPlayer.areThereAnyDamnBosses)
                Item.Calamity().timesUsed = 1;
        }

        public override bool OnPickup(Player player)
        {
            if (CalamityPlayer.areThereAnyDamnBosses)
                Item.Calamity().timesUsed = 1;

            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return Item.Calamity().timesUsed < 1;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override void UpdateInventory(Player player)
        {
            if (!CalamityPlayer.areThereAnyDamnBosses)
                Item.Calamity().timesUsed = 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int index = 0; index < 8; ++index)
            {
                float SpeedX = velocity.X + Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = velocity.Y + Main.rand.Next(-40, 41) * 0.05f;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }

            if (CalamityPlayer.areThereAnyDamnBosses)
            {
                player.HeldItem.Calamity().timesUsed++;
                for (int i = 0; i < Main.InventorySlotsTotal; i++)
                {
                    if (player.inventory[i].type == Item.type && player.inventory[i] != player.HeldItem)
                        player.inventory[i].Calamity().timesUsed++;
                }
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Shotgun).
                AddIngredient(ItemID.IllegalGunParts).
                AddRecipeGroup("IronBar", 20).
                AddIngredient(ItemID.Ectoplasm, 20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
