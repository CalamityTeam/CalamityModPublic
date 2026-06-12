using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ChronomancersScythe : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 45;
            Item.height = 45;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.damage = 159;
            Item.knockBack = 4f;
            Item.useAnimation = 25;
            Item.useTime = 5;
            Item.autoReuse = false;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item71;
            Item.mana = 30;

            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<ChronomancersScytheSwing>();
            Item.shootSpeed = 24f;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                int p = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ChronomancersScytheHoldout>(), damage, knockback, Main.myPlayer, ai2: player.direction);
                float rot = -MathHelper.PiOver2 + MathHelper.PiOver4;
                Main.projectile[p].rotation = player.direction * rot;
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2f)
            {
                Item.reuseDelay = 10;
                Item.channel = false;
                Item.useTurn = false;
            }
            else
            {
                Item.reuseDelay = 0;
                Item.channel = true;
                Item.useTurn = true;
            }
            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.IceSickle).
                AddIngredient(ItemID.FastClock).
                AddIngredient(ItemID.LunarBar, 10).
                AddIngredient<EssenceofEleum>(6).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
