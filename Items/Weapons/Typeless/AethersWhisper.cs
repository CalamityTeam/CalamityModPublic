using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Hybrid;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class AethersWhisper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aether's Whisper");
            Tooltip.SetDefault("Inflicts long-lasting shadowflame and splits on tile hits\n" +
                "Projectiles gain damage as they travel\n" +
                "Right click to change from magic to ranged damage\n" +
                "Right click consumes no mana");
        }

        public override void SetDefaults()
        {
            Item.damage = 504;
            Item.knockBack = 5.5f;
            Item.useTime = Item.useAnimation = 24;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<AetherBeam>();
            Item.mana = 30;
            Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;

            Item.width = 134;
            Item.height = 44;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = Mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.DamageType = DamageClass.Ranged;
                // item.magic = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
            }
            else
            {
                // item.ranged = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
                Item.DamageType = DamageClass.Magic;
            }
            return base.CanUseItem(player);
        }

        public override bool OnPickup(Player player)
        {
            // item.ranged = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
            Item.DamageType = DamageClass.Magic;
            return true;
        }

        public override void UpdateInventory(Player player)
        {
            // Reset to magic if not using an item to prevent sorting bugs.
            if (player.itemAnimation <= 0)
            {
                // item.ranged = false /* tModPorter - this is redundant, for more info see https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide#damage-classes */ ;
                Item.DamageType = DamageClass.Magic;
            }
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse == 2)
                mult *= 0f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // This is coded weirdly because it defaults to using magic boosts without this.
            int dmg = player.GetWeaponDamage(player.ActiveItem());
            float ai0 = player.altFunctionUse == 2 ? 1f : 0f;
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, dmg, knockBack, player.whoAmI, ai0, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<PlasmaRod>()).AddIngredient(ModContent.ItemType<Lazhar>()).AddIngredient(ModContent.ItemType<SpectreRifle>()).AddIngredient(ModContent.ItemType<TwistingNether>(), 3).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
