using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Mistlestorm : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 54;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 48;
            Item.height = 48;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.Item39;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PineNeedleFriendly;
            Item.shootSpeed = 24f;
        }

        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int projAmt = 2 + Main.rand.Next(3);
            for (int i = 0; i < projAmt; i++)
            {
                float randVelocityDampener = 0.025f * (float)i;
                velocity.X += (float)Main.rand.Next(-35, 36) * randVelocityDampener;
                velocity.Y += (float)Main.rand.Next(-35, 36) * randVelocityDampener;
                float projDistance = velocity.Length();
                projDistance = Item.shootSpeed / projDistance;
                velocity.X *= projDistance;
                velocity.Y *= projDistance;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (float)Main.rand.Next(0, 10 * (i + 1)), 0f);
                Projectile.NewProjectile(source, position, velocity, ProjectileID.Leaf, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Razorpine).
                AddIngredient(ItemID.LeafBlower).
                AddIngredient<UelibloomBar>(7).
                AddIngredient<DarkPlasma>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
