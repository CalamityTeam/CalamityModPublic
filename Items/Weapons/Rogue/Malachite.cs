using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Malachite : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Malachite");
            Tooltip.SetDefault("Throws a stream of kunai that stick to enemies and explode\n" +
                "Right click to throw a single kunai that pierces, after piercing an enemy it emits a massive explosion on the next enemy hit\n" +
                "Stealth strikes fire three kunai that home in, stick to enemies, and explode");
            SacrificeTotal = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.damage = 55;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 58;
            Item.shoot = ModContent.ProjectileType<MalachiteProj>();
            Item.shootSpeed = 10f;
            Item.DamageType = RogueDamageClass.Instance;

            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                Item.UseSound = SoundID.Item109;
                Item.shoot = ModContent.ProjectileType<MalachiteStealth>();
            }
            else if (player.altFunctionUse == 2)
            {
                Item.UseSound = SoundID.Item109;
                Item.shoot = ModContent.ProjectileType<MalachiteBolt>();
            }
            else
            {
                Item.UseSound = SoundID.Item1;
                Item.shoot = ModContent.ProjectileType<MalachiteProj>();
            }
            return base.CanUseItem(player);
        }

        public override float UseSpeedMultiplier(Player player)
        {
            if (player.Calamity().StealthStrikeAvailable() || player.altFunctionUse == 2)
                return 1f;
            return 2f;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float dmgMult = 1f;
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (float i = -6.5f; i <= 6.5f; i += 6.5f)
                {
                    Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.ToRadians(i));
                    int stealth = Projectile.NewProjectile(source, position, perturbedSpeed, type, (int)(damage * 1.3), knockback, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }
                return false;
            }
            else if (player.altFunctionUse == 2)
            {
                dmgMult = 1.75f;
            }
            else
            {
                dmgMult = 1f;
            }
            Projectile.NewProjectile(source, position, velocity, type, (int)(damage * dmgMult), knockback, player.whoAmI);
            return false;
        }
    }
}
