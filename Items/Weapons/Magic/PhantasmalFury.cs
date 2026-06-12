using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PhantasmalFury : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 60;
            Item.damage = 190;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 120;
            Item.useTime = 3;
            Item.useAnimation = 45;
            Item.reuseDelay = 75;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 20f;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/PhantasmalFuryShoot") with { Volume = 0.6f, PitchVariance = 0.15f };
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PhantasmalFuryProj>();
            Item.shootSpeed = 6f;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position + velocity * 13, velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.8f, 1.1f), ModContent.ProjectileType<PhantasmalFuryProj>(), damage, 0, player.whoAmI);
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(source, position + velocity * 13, velocity.RotatedByRandom(0.4f), ModContent.ProjectileType<Phantom>(), damage / 2, 0, player.whoAmI);
            }
            for (int i = 0; i < 2; i++)
            {
                Dust chargefull = Dust.NewDustPerfect(position + velocity * 13, DustID.RainbowMk2);
                chargefull.velocity = velocity.RotatedByRandom(0.25f) * Main.rand.NextFloat(1f, 4);
                chargefull.scale = Main.rand.NextFloat(0.5f, 0.9f);
                chargefull.noGravity = true;
                chargefull.color = Color.Lerp(Color.White, Color.Aqua, 0.3f);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreStaff).
                AddIngredient<RuinousSoul>(2).
                AddIngredient<DarkPlasma>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
