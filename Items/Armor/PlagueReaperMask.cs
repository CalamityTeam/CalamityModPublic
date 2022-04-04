using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class PlagueReaperMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Reaper Mask");
            Tooltip.SetDefault("10% increased ranged damage and 8% increased ranged critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 9; //35
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<PlagueReaperVest>() && legs.type == ModContent.ItemType<PlagueReaperStriders>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            string hotkey = CalamityKeybinds.SetBonusHotKey.GetAssignedKeys().Aggregate((x, y) => x + ", " + y);
            player.setBonus = "25% reduced ammo usage and 5% increased flight time\n" +
                "Enemies receive 10% more damage from ranged projectiles when afflicted by the Plague\n" +
                "Getting hit causes plague cinders to rain from above\n" +
                "Press " + hotkey + " to blind yourself for 5 seconds but massively boost your ranged damage\n" +
                "This has a 25 second cooldown.";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.plagueReaper = true;
            player.ammoCost75 = true;


            bool hasPlagueBlackoutCD = modPlayer.cooldowns.TryGetValue(PlagueBlackout.ID, out CooldownInstance cd);
            if (hasPlagueBlackoutCD && cd.timeLeft > 1500)
            {
                player.blind = true;
                player.headcovered = true;
                player.blackout = true;
                player.GetDamage(DamageClass.Ranged) += 0.6f; //60% ranged dmg and 20% crit
                player.GetCritChance(DamageClass.Ranged) += 20;
            }

            if (player.whoAmI == Main.myPlayer)
            {
                if (player.immune)
                {
                    if (player.miscCounter % 10 == 0)
                    {
                        Projectile cinder = CalamityUtils.ProjectileRain(player.Center, 400f, 100f, 500f, 800f, 22f, ModContent.ProjectileType<TheSyringeCinder>(), (int)(40 * player.RangedDamage()), 4f, player.whoAmI);
                        if (cinder.whoAmI.WithinBounds(Main.maxProjectiles))
                            cinder.Calamity().forceTypeless = true;
                    }
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.1f;
            player.GetCritChance(DamageClass.Ranged) += 8;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.NecroHelmet).AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 15).AddIngredient(ItemID.Nanites, 11).AddTile(TileID.MythrilAnvil).Register();
            CreateRecipe(1).AddIngredient(ItemID.AncientNecroHelmet).AddIngredient(ModContent.ItemType<PlagueCellCluster>(), 15).AddIngredient(ItemID.Nanites, 11).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
