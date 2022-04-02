using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DormantBrimseeker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dormant Brimseeker");
            Tooltip.SetDefault("You could've sworn that they turned even scarier when you looked at their reflections in a mirror\n" +
                               "Summons a brimseeker to keep you company\n" +
                               "Firing another brimseeker when all minion slots are filled summons a brimstone aura\n" +
                               "The aura empowers your brimseeker summons and produces damaging fireballs\n" +
                               "Only one aura can persist at a time");
        }

        public override void SetDefaults()
        {
            item.damage = 42;
            item.mana = 10;
            item.width = item.height = 32;
            item.useTime = item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DormantBrimseekerSummoner>();
            item.shootSpeed = 10f;
            item.summon = true;

            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float totalSlots = 0f;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].minion && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
                {
                    totalSlots += Main.projectile[i].minionSlots;
                }
            }
            if (totalSlots < player.maxMinions)
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            else if (player.ownedProjectileCounts[ModContent.ProjectileType<DormantBrimseekerAura>()] <= 0f)
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<DormantBrimseekerAura>(), damage * 2, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
