using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Eradicator : ModItem
    {
        public static float Speed = 10.5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eradicator");
            Tooltip.SetDefault("Throws a disk that fires lasers at nearby enemies\n" +
            "Stealth strikes stick to enemies and unleash a barrage of lasers in all directions");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 58;
            Item.damage = 563;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.shoot = ModContent.ProjectileType<EradicatorProjectile>();
            Item.shootSpeed = Speed;
            Item.DamageType = RogueDamageClass.Instance;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * 0.5f);

            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && proj.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[proj].timeLeft += EradicatorProjectile.StealthExtraLifetime;
                Main.projectile[proj].Calamity().stealthStrike = true;
            }
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/EradicatorGlow").Value);
        }
    }
}
