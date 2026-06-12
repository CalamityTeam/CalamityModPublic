using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class IceBarrage : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public static readonly SoundStyle CastSound = new("CalamityMod/Sounds/Item/IceBarrageCast");
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Frozen, BuffID.Frostburn2];
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 300;
            Item.noMelee = true;
            Item.UseSound = CastSound;

            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<CosmicPurple>();
            Item.Calamity().donorItem = true;

            Item.damage = 2250;
            Item.knockBack = 6f;
            Item.useAnimation = Item.useTime = 300;
            Item.reuseDelay = 60;
            Item.useLimitPerAnimation = 1;
            Item.shoot = ModContent.ProjectileType<IceBarrageMain>();
            Item.shootSpeed = 2f;
            Item.useAmmo = ModContent.ItemType<BloodRune>();
        }

        public override bool CanUseItem(Player player) => CalamityGlobalItem.HasEnoughAmmo(player, Item, 2);

        public override bool CanConsumeAmmo(Item ammo, Player player) => false;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            realPlayerPos.X = Main.mouseX + Main.screenPosition.X;
            realPlayerPos.Y = Main.mouseY + Main.screenPosition.Y;
            Projectile.NewProjectile(source, realPlayerPos, Vector2.Zero, type, damage, knockback, player.whoAmI);

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
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
