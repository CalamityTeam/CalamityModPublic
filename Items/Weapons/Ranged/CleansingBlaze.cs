using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class CleansingBlaze : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 240;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 108;
            Item.height = 38;
            Item.useTime = 3;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item34;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EssenceFire>();
            Item.shootSpeed = 11f;
            Item.useAmmo = AmmoID.Gel;
            Item.consumeAmmoOnFirstShotOnly = true;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>(Texture + "Glow").Value);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 newPos = position + velocity.SafeNormalize(Vector2.UnitX) * 36f;
            for (int i = 0; i < 3; i++)
            {
                Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(5f));
                Projectile.NewProjectile(source, newPos, newVel, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20, 0);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CosmiliteBar>(12).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
