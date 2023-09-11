using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MarksmanBow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 36;
            Item.height = 110;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.JestersArrow;
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;

            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 20;

        public override Vector2? HoldoutOffset() => new Vector2(-4, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Convert wooden arrows to Jester's Arrows
            if (CalamityUtils.CheckWoodenAmmo(type, player))
                type = ProjectileID.JestersArrow;

            for (int i = 0; i < 3; i++)
            {
                int randomExtraUpdates = Main.rand.Next(3);
                float SpeedX = velocity.X + Main.rand.NextFloat(-10f, 10f) * 0.05f;
                float SpeedY = velocity.Y + Main.rand.NextFloat(-10f, 10f) * 0.05f;
                int arrow = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI);
                Main.projectile[arrow].noDropItem = true;
                Main.projectile[arrow].extraUpdates += randomExtraUpdates; //0 to 2 extra updates
                if (type == ProjectileID.JestersArrow)
                {
                    Main.projectile[arrow].localNPCHitCooldown = 10 * (randomExtraUpdates + 1);
                    Main.projectile[arrow].usesLocalNPCImmunity = true;
                    Main.projectile[arrow].tileCollide = false;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Ectoplasm, 31).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
